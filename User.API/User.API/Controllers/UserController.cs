using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.API.Data;

namespace User.API.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private UserContext _userContext;
        public UserController(UserContext userContext)
        {
            _userContext = userContext;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = _userContext.Users
                .AsNoTracking()
                .Include(u => u.Property)
                .SingleOrDefault(u => u.Id == UserIdentity.UserId);
            if (user == null)
                return NotFound();
            return Json(user);
        }

        [Route("")]
        [HttpPatch]
        public async Task<IActionResult> Patch()
        {
            return Json(new
            {
                message = "test info",
                user = await _userContext.Users.SingleOrDefaultAsync(u => u.Name == "no8")
            });
        }
    }
}
