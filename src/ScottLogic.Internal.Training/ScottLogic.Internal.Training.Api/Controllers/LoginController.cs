using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ScottLogic.Internal.Training.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApiContext _context;

        public LoginController(IConfiguration config, ApiContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody]User login)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(login);

            if (user != null)
            {
                var tokenString = GenerateJsonWebToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string GenerateJsonWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User AuthenticateUser(User login)
        {
            User user = null;
            
            //Validate the User Credentials  
                // Get the existing users from the database
            var existingUser = _context.Users.Where(u=> u.Username == login.Username).ToArray();
                // Check the password
            if (existingUser.Count() == 1 && CheckEncryptedPassword(existingUser[0], login.Password))
            {
                user = new User { Username = login.Username,
                                Password = login.Password
                };
            }
            return user;
        }

        private bool CheckEncryptedPassword(User user, string password)
        {
            byte[] salt = Convert.FromBase64String(user.Salt);
            const int HASH_SIZE = 32;
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            var hashValue = pbkdf2.GetBytes(HASH_SIZE);
            var hashValueString = Convert.ToBase64String(hashValue);
            if(hashValueString == user.Password)
            {
                return true;
            }
            return false;
        }
    }
}
