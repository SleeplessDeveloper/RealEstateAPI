using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstateAPI.Data;
using RealEstateAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        ApiDBCOntext _dbContext = new ApiDBCOntext();

        private IConfiguration _configuration;

        public UsersController(IConfiguration config)
        {
            _configuration = config;
        }

        [HttpPost("[action]")]
        public IActionResult Register([FromBody] User user)
        {
           var userExists = _dbContext.Users.FirstOrDefault(u=>u.Email == user.Email);
            if (userExists != null)
            {
                return BadRequest("Email is already registered");
            }
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPost("[action]")]
        public IActionResult Login([FromBody] User user)
        {
            var currentUser = _dbContext.Users.FirstOrDefault(u=> u.Email == user.Email && u.Password == user.Password);
            if (currentUser == null)
            {
                return NotFound();
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(jwt);
        }
    }
}
