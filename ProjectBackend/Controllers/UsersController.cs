using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EmployeeManagementAPI.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectBackend.Data.Context;
using ProjectBackend.DTOs;
using ProjectBackend.Models;

namespace ProjectBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BloggerContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public UsersController(BloggerContext context,
            IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser()
        {
            IFormFileCollection req = Request.Form.Files;
            var files = req;
            var userDtoString = Request.Form["UserDetails"];
            var userDtoObj = JsonConvert.DeserializeObject<UserDTO>(userDtoString);
            var uploads = Path.Combine(_env.WebRootPath, "UserImage");

            if (userDtoObj == null)
            {
                return BadRequest();
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    if (!Directory.Exists(uploads))
                    {
                        Directory.CreateDirectory(uploads);
                    }
                    var urls = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Split(";");
                    var filepath = Path.Combine(uploads, file.FileName);
                    using (var fileStream = new FileStream(filepath, FileMode.Create)) {
                        file.CopyTo(fileStream);
                    }
                        // userDtoObj.ProfileImageUrl = $"EmployeeImages/{file.FileName}";
                }   
                
            }

            var userObj = _mapper.Map<User>(userDtoObj);
            EncDescPassword.CreateHashPassword(userDtoObj.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userObj.PasswordHash = passwordHash;
            userObj.PasswordSalt = passwordSalt;
            userObj.CreatedTime = DateTime.Now;
            await _context.Users.AddAsync(userObj);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
