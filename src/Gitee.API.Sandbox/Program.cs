using Gitee.Api;
using Gitee.Api.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gitee.API.Sandbox
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //try
            //{
            //    var jtk = Newtonsoft.Json.Linq.JToken.Parse(System.IO.File.ReadAllText("json1.json"));
            //    var result = jtk.ToObject<List<Repo>>();
            //}
            //catch (Exception ex)
            //{
            //}

            Console.WriteLine("Hello World!");
            Task.Run(async () =>
            {
                try
                {
                    SDK.client_id = SdkConfig.client_id;
                    SDK.client_secret = SdkConfig.client_secret;
                    var client = await SDK.LoginAsync(SdkConfig.username, SdkConfig.password);
                    var user = await client.GetUserAsync();
                    var poject = await client.GetReposAsync(1, 20);
                    var ooox = await SDK.RefreshToken(client.Token.refresh_token);
                    var oooxxx = await ooox.GetUserAsync();
                    var o = SDK.ContinueByToken(ooox.Token.refresh_token, ooox.Token.access_token, ooox.Token.Expires);
                    var oxx = await o.GetUserAsync();
                    //   await client.CreateRepoAsync("test" + DateTime.Now.ToString("yyyyMMddHHmmss"), DateTime.Now.ToString(), true);
                    await client.CreateRepoAsync("testpublic1122211", "", false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }).Wait();
        }
    }
}