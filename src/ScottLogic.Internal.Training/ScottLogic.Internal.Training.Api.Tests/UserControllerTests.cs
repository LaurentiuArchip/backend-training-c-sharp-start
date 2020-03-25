using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScottLogic.Internal.Training.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ScottLogic.Internal.Training.Api.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetUsers_FromEmptyDb_ReturnsUnauthorized()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("usersDb").Options;
            var apiContext = new ApiContext(options);

            var controller = new UsersController(apiContext);

            var getZeroUsersResult = controller.Get();
            var objectResponse = getZeroUsersResult as ObjectResult;

            Assert.Equal(200, objectResponse.StatusCode);
            apiContext.Dispose();
        }

        [Fact]
        public async Task AddUser_GetUsers_ReturnsUnauthorized()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("usersDb").Options;
            var apiContext = new ApiContext(options);
            var controller = new UsersController(apiContext);

            var testUser1 = new User
            {
                Username = "Tom",
                Password = "password3"
            };

            var result = await controller.AddUser(testUser1);
            var getUsersResult = controller.Get();
            var objectResponse = getUsersResult as ObjectResult;
            var users = objectResponse.Value as List<User>;

            Assert.NotNull(objectResponse);
            Assert.Equal(200, objectResponse.StatusCode);
            Assert.Equal("Tom", users[0].Username);
            apiContext.Dispose();
        }
    }
}
