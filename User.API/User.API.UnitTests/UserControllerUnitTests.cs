using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using User.API.Controllers;
using User.API.Data;
using System.Collections.Generic;
using System.Linq;

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

        private (UserController controller, UserContext userContext) GetUserController()
        {
            var context = GetUserContext();
            var loggerMoq = new Mock<ILogger<Controllers.UserController>>();
            var logger = loggerMoq.Object;
            return (controller: new UserController(context, logger), userContext: context );
        }

        [Fact]
        public async Task Get_ReturnRightUser_WithExpectedParameters()
        {
            //var controller = GetUserController();
            //var response = await controller.Get();
            (UserController controller, UserContext userContext) = GetUserController();
            var response = await controller.Get();

            var result = response.Should().BeOfType<JsonResult>().Subject;
            var appUser = result.Value.Should().BeAssignableTo<Models.AppUser>().Subject;
            appUser.Id.Should().Be(1);
            appUser.Name.Should().Be("no8");
        }

        [Fact]
        public async Task Patch_ReturnNewName_WithExpectedNewParameter()
        {
            (UserController controller, UserContext userContext) = GetUserController();
            var document = new JsonPatchDocument<Models.AppUser>();
            document.Replace(u => u.Name, "张三");
            var response = await controller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            // assert response
            var appuser = result.Value.Should().BeAssignableTo<Models.AppUser>().Subject;
            appuser.Name.Should().Be("张三");

            // assert name value in ef context
            var userModel = await userContext.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Should().NotBeNull();
            userModel.Name.Should().Be("张三");
        }

        [Fact]
        public async Task Patch_ReturnNewProperties_WithExpectedNewProperty()
        {
            (UserController controller, UserContext userContext) = GetUserController();
            var document = new JsonPatchDocument<Models.AppUser>();
            document.Replace(u => u.Properties, new List<Models.UserProperty> {
                new Models.UserProperty{ Key="fin_industry",Value="互联网",Text="互联网"}
            });
            var response = await controller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            // assert response
            var appuser = result.Value.Should().BeAssignableTo<Models.AppUser>().Subject;
            appuser.Properties.Count.Should().Be(1);
            appuser.Properties.First().Key.Should().Be("fin_industry");
            appuser.Properties.First().Value.Should().Be("互联网");

            // assert name value in ef context
            var userModel = await userContext.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Properties.Count.Should().Be(1);
            userModel.Properties.First().Key.Should().Be("fin_industry");
            userModel.Properties.First().Value.Should().Be("互联网");
        }

        [Fact]
        public async Task Patch_ReturnNewProperties_WithRemoveNewProperty()
        {
            (UserController controller, UserContext userContext) = GetUserController();
            var document = new JsonPatchDocument<Models.AppUser>();
            document.Replace(u => u.Properties, new List<Models.UserProperty> {
            });
            var response = await controller.Patch(document);
            var result = response.Should().BeOfType<JsonResult>().Subject;

            // assert response
            var appuser = result.Value.Should().BeAssignableTo<Models.AppUser>().Subject;
            appuser.Properties.Should().BeEmpty();

            // assert name value in ef context
            var userModel = await userContext.Users.SingleOrDefaultAsync(u => u.Id == 1);
            userModel.Properties.Should().BeEmpty();
        }
    }
}
