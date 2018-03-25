using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace User.API.UnitTests
{
    public class UserControllerUnitTests
    {
        private Data.UserContext GetUserContext()
        {
            var options = new DbContextOptionsBuilder<Data.UserContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var userContext = new Data.UserContext(options);
            userContext.Users.Add(new Models.AppUser
            {
                Id = 1,
                Name = "no8"
            });
            userContext.SaveChanges();
            return userContext;
        }

        [Fact]
        public async Task Get_ReturnRightUser_WithExpectedParameters()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<Controllers.UserController>>();

            var logger = loggerMoq.Object;
            var controller = new Controllers.UserController(context, logger);

            var response = await controller.Get();

            Assert.IsType<JsonResult>(response);
        }
    }
}
