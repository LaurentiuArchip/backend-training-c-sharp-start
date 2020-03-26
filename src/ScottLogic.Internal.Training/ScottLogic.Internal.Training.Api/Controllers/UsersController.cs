using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ScottLogic.Internal.Training.Api.Controllers
{
    /// <summary>
    /// Contains all endpoints to access Users.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApiContext _context;

        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="context"> Instance of the database context.</param>
        public UsersController(ApiContext context)
        {
            _context = context;
        }

        //Get ------Just for Testing -- And for admin users
        /// <summary>
        /// Get the existing users.
        /// </summary>
        /// <returns>a List with the existing users.</returns>
        /// <response code="200">Returns the list of existing users.</response>
        /// <example>GET api/users</example>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Users.ToList());
        }

        // POST: api/Users
        /// <summary>
        /// Adds a user to the database.
        /// </summary>
        /// <param name="user">The user to add.</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">If the user is added to the database</response>
        /// <response code="400">If the user data posted is invalid</response>
        /// <response code="409">If the user is already present in the database</response>
        /// <example>GET api/users/AddUser</example>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] User user)
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
        /// <summary>
        /// Deletes a user from the database.
        /// </summary>
        /// <param name="userToDelete">The user to delete.</param>
        /// <returns>A confirmation message.</returns>
        /// <response code="200">If the user is removed from the database</response>
        /// <response code="400">If the user data posted is invalid</response>
        /// <response code="404">If the user is not found in database</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser([FromBody] string userToDelete)
        {
            if (userToDelete != null)
            {
                // Get the user Role from the access token
                var userIdentity = this.User.Identity as ClaimsIdentity;
                var userRole = UserRole.None;
                if (userIdentity != null && userIdentity.HasClaim(claim => claim.Type == ClaimTypes.Role))
                {
                    var userRolesString = userIdentity.Claims
                        .FirstOrDefault(claim => claim.Type == ClaimTypes.Role).Value;
                    if(userRolesString == "Admin")
                    {
                        userRole = UserRole.Admin;
                    }
                }

                // Get the user userName from the access token
                #nullable enable
                string? loggedInUsername = null;
                if (userIdentity != null && userIdentity.HasClaim(claim => claim.Type == "Username"))
                {
                    loggedInUsername = userIdentity.Claims.FirstOrDefault(claim => claim.Type == "Username").Value;
                }

                // Username to delete
                    // Admin can delete any user or The user can delete only their own data
                string? userNameToDelete = userRole == UserRole.Admin ? userToDelete : 
                    (loggedInUsername == userToDelete ? loggedInUsername : null);

                if(userNameToDelete != null)
                {
                    // Get the existing users from the database
                    var users = await _context.Users.ToArrayAsync();

                    if (users.Any(user => user.Username == userNameToDelete)) 
                    {
                        // Delete user
                        _context.Users.Remove(_context.Users.FirstOrDefault(user => user.Username == userNameToDelete));
                        _context.SaveChanges();
                        return Ok("User removed from the database");
                    }
                    return NotFound("User not found in database");
                }
                return Unauthorized("Unauthorized action");
            }
            // Invalid request
            return BadRequest("Invalid user data!");
        }
        public string[] EncryptPassword(string password)
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
