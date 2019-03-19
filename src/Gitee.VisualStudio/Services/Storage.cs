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
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

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
                var result = _user != null && _user.Session != null && _user.Detail != null && _user.Session.Token!=null;
                lock (_path)
                {
                    if (!result)
                    {
                        if (!_isChecked)
                        {
                            _isChecked = true;
                            Task.Run(() => LoadUserAsync()).ContinueWith(async task =>
                            {
                                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
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
                                _isChecked = false;
                            }, TaskScheduler.Default).Forget();
                        }
                    }
                }
                return result;
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
     

        private (string access_token,DateTime exp,string refresh_token,string username) GetAccessToken()
        {
            string _access_tokenken = null;
            string _refresh_token = null;
            string _username = null;
            DateTime exp = DateTime.MinValue;
            try
            {
                var key = "access_token:https://gitee.com";
            
                using (var credential = new Credential())
                {
                    credential.Target = key;
                    if (credential.Load())
                    {
                        if (DateTime.TryParseExact(credential.Description, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out exp))
                        {
                            if (DateTime.Now < exp)
                            {
                                _access_tokenken = credential.Password;
                            }
                        }
                        _username = credential.Username;
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
            return (_access_tokenken, exp, _refresh_token, _username);
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
            try
            {
                Gitee.Api.SDK.client_id = Gitee.Api.SdkConfig.client_id;
                Gitee.Api.SDK.client_secret = Gitee.Api.SdkConfig.client_secret;
                var token = GetAccessToken();
                _user = new User();
                if (string.IsNullOrEmpty(token.access_token) || DateTime.Now > token.exp)
                {
                    if (!string.IsNullOrEmpty(token.refresh_token))
                    {
                        try
                        {
                            _user.Session = await SDK.RefreshToken(token.refresh_token);
                            SaveToken(_user);
                        }
                        catch (Exception)
                        {
                            _user.Session = null;
                        }
                    }

                }
                else
                {
                    _user.Session = SDK.ContinueByToken(token.refresh_token, token.access_token, token.exp);
                }
                if (_user.Session != null)
                {
                    _user.Username = token.username;
                    if (_user.Detail == null)
                    {
                        _user.Detail = await _user.Session.GetUserAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _user.Session = null;
            }
            try
            {
                if (_user.Session == null)
                {
                    var key = "git:https://gitee.com";
                    using (var credential = new Credential())
                    {
                        credential.Target = key;
                        credential.Load();
                        _user.Username = credential.Username;
                        _user.Session = await SDK.LoginAsync(credential.Username, credential.Password);
                        SaveToken(_user);
                        _user.Detail = await _user.Session.GetUserAsync();
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("用户登录凭证无效，请重新登陆",ex);
            }
            finally
            {
                if (_user?.Session == null || _user?.Detail == null)
                {
                    _user = null;
                }
            }
          
        }

        public void Erase()
        {
            _user = null;
            EraseCredential("git:https://gitee.com");
            EraseCredential("access_token:https://gitee.com");
            EraseCredential("refresh_token:https://gitee.com");
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