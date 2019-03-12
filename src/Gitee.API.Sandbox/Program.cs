using Gitee.Api;
using System;
using System.Threading.Tasks;

namespace Gitee.API.Sandbox
{
    class Program
    {
        static    void  Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Task.Run(async () =>
            {
                try
                {
                    var client = await SDK.LoginAsync("gitee@vip.qq.com", "abc12345");
                    var user = await client.GetUserAsync();
                    var poject = await client.GetReposAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }).Wait();

            
        }
    }
}
