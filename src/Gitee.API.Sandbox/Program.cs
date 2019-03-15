﻿using Gitee.Api;
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
                    SDK.client_id = SdkConfig.client_id;
                    SDK.client_secret = SdkConfig.client_secret;
                    var client = await SDK.LoginAsync(SdkConfig.username,SdkConfig.password);
                    var user = await client.GetUserAsync();
                    var poject = await client.GetReposAsync(1,20);
                    var ooox = await SDK.RefreshToken(client.Token.refresh_token);
                    var oooxxx = await ooox.GetUserAsync();
                    var o = SDK.ContinueByToken(ooox.Token.refresh_token, ooox.Token.access_token, ooox.Token.Expires);
                    var oxx = await o.GetUserAsync();
                 //   await client.CreateRepoAsync("test" + DateTime.Now.ToString("yyyyMMddHHmmss"), DateTime.Now.ToString(), true);
                    await client.CreateRepoAsync("testpublic1122211" , "", false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }).Wait();

            
        }
    }
}
