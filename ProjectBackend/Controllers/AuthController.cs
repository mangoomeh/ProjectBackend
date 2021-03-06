using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagementAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ProjectBackend.Data.Context;
using ProjectBackend.DTOs;
using ProjectBackend.Models;

namespace ProjectBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly BloggerContext _context;
        private readonly IConfiguration _config;

        public AuthController(BloggerContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private string CreateJwtToken(User user)
        {
            List<Claim> claimsList = new()
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Role", user.Role.RoleName),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("SecretKey").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claimsList,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginDTO>> Login([FromBody] LoginDTO loginDto)
        {
            if (loginDto == null)
            {
                return BadRequest();
            }

            User user = await _context.Users.Include(m => m.Role).FirstOrDefaultAsync(a => a.Email == loginDto.Email);
            
            if (user == null)
            {
                return NotFound();
            }

            if (!EncDescPassword.VerifyHashPassword(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest();
            }

            string token = CreateJwtToken(user);

            return Ok(new
            {
                Status = 200,
                Message = "Login Success!",
                Token = token
            });
        }
    }
}
