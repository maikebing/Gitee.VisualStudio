using Gitee.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gitee.Api
{
    public class Session
    {
        public string BaseURL { get; private set; } = "https://gitee.com/api";
        public Client Client { get; set; } = null;
        public TokenDto Token { get; set; } = null;
    }
}