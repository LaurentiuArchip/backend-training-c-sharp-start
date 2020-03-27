using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScottLogic.Internal.Training.Api.Controllers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ScottLogic.Internal.Training.Api.Tests
{
    public class UserControllerTests
    {
        [Fact]
        public void GetUsers_FromEmptyDb_ReturnsOk()
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
        public async Task AddOneUser_GetOneUsers_ReturnsOk()
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

            await controller.AddUser(testUser1);
            var getUsersResult = controller.Get();
            var objectResponse = getUsersResult as ObjectResult;
            var users = objectResponse.Value as List<User>;

            Assert.Equal(200, objectResponse.StatusCode);
            Assert.Equal("Tom", users[0].Username);
            apiContext.Dispose();
        }

        [Fact]
        public void DeleteUser_FromEmptyDb_ReturnsNotFound()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
               .UseInMemoryDatabase("usersDb").Options;
            var apiContext = new ApiContext(options);

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, UserRole.Admin.ToString()),
                new Claim("Username", "Luke")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var controller = new UsersController(apiContext)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrincipal
                    }
                }
            };

            var getDeleteUserResponse = controller.DeleteUser("Tom");
            var result = getDeleteUserResponse.Result as ObjectResult;

            Assert.Equal(404, result.StatusCode);
            apiContext.Dispose();
        }

        [Fact]
        public async Task UserDeletesThemselves_FromDb_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
               .UseInMemoryDatabase("usersDb").Options;
            var apiContext = new ApiContext(options);

            var claims = new[]
            {
                new Claim("Username", "Tom")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var controller = new UsersController(apiContext)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrincipal
                    }
                }
            };

            var testUser1 = new User
            {
                Username = "Tom",
                Password = "password3"
            };

            await controller.AddUser(testUser1);

            var getDeleteUserResponse = controller.DeleteUser("Tom");
            var result = getDeleteUserResponse.Result as ObjectResult;

            Assert.Equal(200, result.StatusCode);
            apiContext.Dispose();
        }

        [Fact]
        public async Task AdminDeletesUser_FromDb_ReturnsOk()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
               .UseInMemoryDatabase("usersDb").Options;
            var apiContext = new ApiContext(options);

            var claims = new[]
            {
                new Claim(ClaimTypes.Role, UserRole.Admin.ToString()),
                new Claim("Username", "Luke")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var controller = new UsersController(apiContext)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrincipal
                    }
                }
            };

            var testUser1 = new User
            {
                Username = "Tom",
                Password = "password3"
            };

            await controller.AddUser(testUser1);

            var getDeleteUserResponse = controller.DeleteUser("Tom");
            var result = getDeleteUserResponse.Result as ObjectResult;

            Assert.Equal(200, result.StatusCode);
            apiContext.Dispose();
        }

        [Fact]
        public async Task UserDeletesDifferentUser_FromDb_ReturnsUnauthorized()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
               .UseInMemoryDatabase("usersDb").Options;
            var apiContext = new ApiContext(options);

            var claims = new[]
            {
                new Claim("Username", "Tim")
            };
            var identity = new ClaimsIdentity(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var controller = new UsersController(apiContext)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = claimsPrincipal
                    }
                }
            };

            var testUser1 = new User
            {
                Username = "Tom",
                Password = "password3"
            };

            await controller.AddUser(testUser1);

            var getDeleteUserResponse = controller.DeleteUser("Tom");
            var result = getDeleteUserResponse.Result as ObjectResult;

            Assert.Equal(401, result.StatusCode);
            apiContext.Dispose();
        }
    }
}
