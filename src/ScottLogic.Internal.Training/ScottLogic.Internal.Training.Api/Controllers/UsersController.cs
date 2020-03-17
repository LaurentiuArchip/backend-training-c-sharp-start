using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
                var users = await _context.Users
                    //.Include(u => u.Username)
                    .ToArrayAsync();
                // User already exists

                // New user, added to the database
                _context.Add(user);
                _context.SaveChanges();
                users = await _context.Users
                    //.Include(u => u.Username)
                    .ToArrayAsync();
                return Ok(users);
            }
            else
            {
                // Wrong user data
                return BadRequest();
            }
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
