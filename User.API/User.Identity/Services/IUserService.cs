using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 检查手机号是否已注册，若有注册则注册一个用户
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>用户Id</returns>
        int CheckOrCreate(string phone);
    }
}
