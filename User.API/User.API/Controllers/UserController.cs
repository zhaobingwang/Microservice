﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using User.API.Data;
using Microsoft.AspNetCore.JsonPatch;

namespace User.API.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private UserContext _userContext;
        private ILogger<UserController> _logger;
        public UserController(UserContext userContext, ILogger<UserController> logger)
        {
            _userContext = userContext;
            _logger = logger;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userContext.Users
                .AsNoTracking()
                .Include(u => u.Properties)
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);
            if (user == null)
                throw new UserOperationException($"错误的用户上下文,Id:{UserIdentity.UserId}");
            return Json(user);
        }

        [Route("")]
        [HttpPatch]
        public async Task<IActionResult> Patch([FromBody]JsonPatchDocument<Models.AppUser> patch)
        {
            var user = await _userContext.Users
                .SingleOrDefaultAsync(u => u.Id == UserIdentity.UserId);
            patch.ApplyTo(user);

            foreach (var propetty in user.Properties)
            {
                _userContext.Entry(propetty).State = EntityState.Detached;
            }

            var originProperties = await _userContext.UserProperties.AsNoTracking().Where(u => u.AppUserId == UserIdentity.UserId).ToListAsync();
            var allProperties = originProperties.Union(user.Properties).Distinct();

            var removedProperties = originProperties.Except(user.Properties);
            var newProperties = allProperties.Except(originProperties);

            foreach (var propetty in removedProperties)
            {
                //_userContext.Entry(propetty).State = EntityState.Deleted;
                _userContext.Remove(propetty);
            }

            foreach (var property in newProperties)
            {
                //_userContext.Entry(property).State = EntityState.Added;
                _userContext.Add(property);
            }
            _userContext.Update(user);
            _userContext.SaveChanges();
            return Json(user);
        }

        /// <summary>
        /// 检查或者创建用户（当用户手机号不存在的时候创建用户）
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        [Route("check-or-create")]
        [HttpPost]
        public async Task<IActionResult> CheckOrCreate(string phone)
        {
            // TODO:做手机号码格式验证
            var user = _userContext.Users.SingleOrDefault(u => u.Phone == phone);
            if (user == null)
            {
                user = new Models.AppUser { Phone = phone };
                _userContext.Users.Add(user);
                await _userContext.SaveChangesAsync();
            }
            return Ok(user.Id);
        }
    }
}
