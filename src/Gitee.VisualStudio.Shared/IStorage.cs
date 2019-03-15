using System;
using System.Threading.Tasks;

namespace Gitee.VisualStudio.Shared
{
    public interface IStorage
    {
        bool IsLogined { get; }
        Task<User> GetUserAsync();

        string GetPassword();

        void SaveUser(User user, string password);
        void Erase();

        string GetBaseRepositoryDirectory();
    }
}
