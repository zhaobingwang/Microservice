using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public class TestAuthCodeService : IAuthCodeService
    {
        /// <summary>
        /// 测试环境使用
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="authCode"></param>
        /// <returns></returns>
        public bool Validate(string phone, string authCode)
        {
            return true;
        }
    }
}
