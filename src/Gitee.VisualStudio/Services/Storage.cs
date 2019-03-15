using Gitee.VisualStudio.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Composition;
using System.IO;
using Gitee.Api;
using System.Threading.Tasks;
using User = Gitee.VisualStudio.Shared.User;
using Gitee.VisualStudio.Helpers;

namespace Gitee.VisualStudio.Services
{
    [Export(typeof(IStorage))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Storage : IStorage
    {
        private static readonly string _path;
        private Shared.User _user;

        static Storage()
        {
            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".codecloud");
        }

        private bool _isChecked;
        private DateTime lastcheck = DateTime.MinValue;
        public bool IsLogined
        {
            get
            {
                lock (_path)
                {
                    if (DateTime.Now.Subtract(lastcheck).TotalSeconds > 5)
                    {
                        Task.Run(() => LoadUserAsync()).ContinueWith(task =>
                        {
                            if (task.IsCompleted)
                            {
                                lastcheck = DateTime.Now;
                            }
                            if (task.IsFaulted)
                            {
                                if (task.Exception != null && task.Exception.InnerException != null)
                                {
                                    OutputWindowHelper.ExceptionWriteLine($"Gitee4VS LoadUserAsync  IsFaulted {task.Exception.Message};{ task.Exception.InnerException.Message}.", task.Exception);
                                }
                                if (task.Exception != null && task.Exception.InnerException == null)
                                {
                                    OutputWindowHelper.ExceptionWriteLine($"Gitee4VS LoadUserAsync  IsFaulted {task.Exception.Message}.", task.Exception);
                                }
                            }
                        });
                    }
                }
                return _user != null && _user.Session != null;
            }
        }

        public string GetPassword()
        {
            var key = "git:https://gitee.com";

            using (var credential = new Credential())
            {
                credential.Target = key;
                return credential.Load()
                    ? credential.Password
                    : null;
            }
        }

        private (string access_token,DateTime exp,string refresh_token) GetAccessToken()
        {
            string _access_tokenken = null;
            string _refresh_token = null;
            DateTime exp = DateTime.MinValue;
            try
            {
                var key = "access_token:https://gitee.com";
            
                using (var credential = new Credential())
                {
                    if (credential.Load())
                    {
                        if (DateTime.TryParseExact(credential.Description, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out exp))
                        {
                            if (DateTime.Now < exp)
                            {
                                credential.Target = key;
                                _access_tokenken = credential.Password;
                            }
                        }
                    }
                }
                var keyref = "refresh_token:https://gitee.com";
                using (var credential = new Credential())
                {
                    credential.Target = keyref;
                    _refresh_token = credential.Load()
                        ? credential.Password
                        : null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("未能加载gitee的token信息，需要重新登录。");
            }
            return (_access_tokenken, exp, _refresh_token);
        }

        public async Task<Shared.User> GetUserAsync()
        {
            if (_user != null)
            {
                return _user;
            }
            await LoadUserAsync();
            return _user;
        }

        public void SaveUser(User user, string password)
        {
            SavePassword(user.Email, password);
            SaveToken(user);
            _user = user;
        }

  
        private void SavePassword(string email, string password)
        {
            var key = "git:https://gitee.com";
            using (var credential = new Credential(email, password, key))
            {
                credential.Save();
            }
        }

        private void SaveToken(Shared.User user)
        {
            var key = "access_token:https://gitee.com";
            using (var credential = new Credential(user.Username,  user.Session.Token.access_token, key))
            {
                credential.Description = user.Session.Token.Expires.ToString("yyyy-MM-dd HH:mm:ss");
                credential.Save();
            }
            var refkey = "refresh_token:https://gitee.com";
            using (var credential = new Credential(user.Username, user.Session.Token.refresh_token, refkey))
            {
                credential.Save();
            }
        }

        private async Task LoadUserAsync()
        {
            var token = GetAccessToken();
            if (string.IsNullOrEmpty(token.access_token) || DateTime.Now > token.exp)
            {
                if (!string.IsNullOrEmpty(token.refresh_token))
                {
                    try
                    {
                        _user.Session = await SDK.RefreshToken(token.refresh_token);
                    }
                    catch (Exception)
                    {
                        _user.Session = null;
                    }
                }
                if (_user.Session == null)
                {
                    var key = "git:https://gitee.com";
                    using (var credential = new Credential())
                    {
                        _user.Session = await SDK.LoginAsync(credential.Username, credential.Password);
                    }
                }
            }
            else
            {
                _user.Session = SDK.ContinueByToken(token.refresh_token, token.access_token, token.exp);
            }
        }

        public void Erase()
        {
            _user = null;
            EraseCredential("git:https://gitee.com");
            EraseCredential("token:https://gitee.com");
            File.Delete(_path);
        }

        private static void EraseCredential(string key)
        {
            using (var credential = new Credential())
            {
                credential.Target = key;
                credential.Delete();
            }
        }

        public string GetBaseRepositoryDirectory()
        {
            var user = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(user, "Dev", "Gitee");
        }
    }
}