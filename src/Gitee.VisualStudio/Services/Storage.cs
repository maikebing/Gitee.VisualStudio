using Gitee.VisualStudio.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Composition;
using System.IO;
using Gitee.Api;
using System.Threading.Tasks;
using User = Gitee.VisualStudio.Shared.User;

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

        public bool IsLogined
        {
            get
            {
                lock (_path)
                {
                    if (!_isChecked)
                    {
                        LoadUserAsync();
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

        private string GetToken()
        {
            var key = "token:https://gitee.com";

            using (var credential = new Credential())
            {
                credential.Target = key;
                return credential.Load()
                    ? System.Text.Encoding.Default.GetString(Convert.FromBase64String(credential.Password))
                    : null;
            }
        }

        public Shared.User GetUser()
        {
            if (_user != null)
            {
                return _user;
            }
            LoadUserAsync().Wait();
            return _user;
        }

        public void SaveUser(User user, string password)
        {
            SavePassword(user.Email, password);
            SaveToken(user);
            SaveUserToLocal(user);
            _user = user;
        }

        private void SaveUserToLocal(Shared.User user)
        {
            var serializer = new JsonSerializer();
            if (File.Exists(_path))
            {
                JObject o = null;
                using (var reader = new JsonTextReader(new StreamReader(_path)))
                {
                    o = (JObject)serializer.Deserialize(reader);

                    o["User"] = JToken.FromObject(user.Detail);
                }
                using (var writer = new JsonTextWriter(new StreamWriter(_path)))
                {
                    writer.Formatting = Formatting.Indented;
                    o.WriteTo(writer);
                }
            }
            else
            {
                using (var writer = new JsonTextWriter(new StreamWriter(_path)))
                {
                    writer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, new { User = user });
                }
            }
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
            var key = "token:https://gitee.com";
            using (var credential = new Credential(user.Username, Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(user.Session.Token))), key))
            {
                credential.Save();
            }
        }

        private async Task LoadUserAsync()
        {
            if (File.Exists(_path))
            {
                JObject o = null;
                using (var reader = new JsonTextReader(new StreamReader(_path)))
                {
                    var serializer = new JsonSerializer();
                    o = (JObject)serializer.Deserialize(reader);
                    var token = o["User"];
                    if (token != null)
                    {
                        var t = token.ToObject<Gitee.Api.Dto.TokenDto>();
                        _user.Session = await Gitee.Api.SDK.RefreshToken(t);
                    }
                }
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