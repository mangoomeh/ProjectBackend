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

            var users = await _context.Users.Include(u => u.Role).ToListAsync();
            var mapped = _mapper.Map<List<User>, List<UserDTO>>(users);
            return Ok(mapped);

        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }
            var mappedUser = _mapper.Map<UserDTO>(user);
            return mappedUser;
        }

        // PUT: api/Users/5/changePassword
        [HttpPut("{id}/changePassword")]
        public async Task<ActionResult<UserDTO>> ChangePassword(int id, [FromBody] ChangePasswordDTO changePasswordObj)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            if (!EncDescPassword.VerifyHashPassword(changePasswordObj.oldPassword, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest();
            }

            EncDescPassword.CreateHashPassword(changePasswordObj.newPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);
            user.PasswordHash = newPasswordHash;
            user.PasswordSalt = newPasswordSalt;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok();
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDTO>> PutUser(int id)
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

            var isUserExist = await _context.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (isUserExist == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "User not found!"
                });
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
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    userDtoObj.ProfileImgUrl = $"UserImage/{file.FileName}";
                } 
            }

            var userObj = _mapper.Map<User>(userDtoObj);
            userObj.CreatedTime = isUserExist.CreatedTime;
            userObj.PasswordHash = isUserExist.PasswordHash;
            userObj.PasswordSalt = isUserExist.PasswordSalt;
            userObj.Id = id;
            userObj.RoleId = isUserExist.RoleId;

            _context.Entry(userObj).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Message = "User is updated!"
            });
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
                    userDtoObj.ProfileImgUrl = $"UserImage/{file.FileName}";
                }   
                
            }

            var userObj = _mapper.Map<User>(userDtoObj);
            EncDescPassword.CreateHashPassword(userDtoObj.Password, out byte[] passwordHash, out byte[] passwordSalt);
            userObj.PasswordHash = passwordHash;
            userObj.PasswordSalt = passwordSalt;
            userObj.CreatedTime = DateTime.Now;
            await _context.Users.AddAsync(userObj);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Message = "User is Created!"
            });
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
