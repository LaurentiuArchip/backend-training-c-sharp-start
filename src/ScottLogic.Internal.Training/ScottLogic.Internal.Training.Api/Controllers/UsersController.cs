﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ScottLogic.Internal.Training.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiContext _context;

        public UsersController(ApiContext context)
        {
            _context = context;
        }
        
        //Get ------Just for Testing
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(_context.Users);
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            if (user.Username != null)
            {
                // Get existing users
                var users = await _context.Users.ToArrayAsync();
                if (users.Any(currentUser => currentUser.Username == user.Username))
                {
                    // User already exists
                    return Conflict("User already present in the database");
                }
                else
                {
                    // New user, added to the database
                    var saltPassword = EncryptPassword(user.Password);
                    user.Salt = saltPassword[0];
                    user.Password = saltPassword[1];
                    _context.Add(user);
                    _context.SaveChanges();
                    users = await _context.Users.ToArrayAsync();
                    return Ok("User added to the database");
                }
            }
            else
            {
                // Wrong user data
                return BadRequest("Invalid user data!");
            }
        }

        // DELETE: api/ApiWithActions/user
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] User user)
        {
            var users = await _context.Users.ToArrayAsync();
            if (user.Username != null)
            {
                if (users.Any(currentUser => currentUser.Username == user.Username))
                {
                    _context.Users.Remove(_context.Users.FirstOrDefault(u => u.Username == user.Username));
                    _context.SaveChanges();
                    return Ok("User removed from the database");
                }

                return NotFound("User not found in database");
            }
            // Invalid request
            return BadRequest();
        }
        private string[] EncryptPassword(string password)
        {
            // 1.Genrerate Salt value
            const int SALT_SIZE = 32;
            const int HASH_SIZE = 32;

            // Create a byte array to hold the random value.
            var salt = new byte[SALT_SIZE];
            using (var random = new RNGCryptoServiceProvider())
            {
                // Fill the array with a random value.
                random.GetNonZeroBytes(salt);
            }
            // 2. Genrerate Hash
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            var hashValue = pbkdf2.GetBytes(HASH_SIZE);

            return new string[] { Convert.ToBase64String(salt), Convert.ToBase64String(hashValue) };
        }
    }
}
