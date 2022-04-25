using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class BlogsController : ControllerBase
    {
        private readonly BloggerContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public BlogsController(BloggerContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        // GET: api/Blogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogs()
        {
            return await _context.Blogs.Include(b => b.User).OrderByDescending(b=> b.CreatedTime).ThenBy(b => b.Id).ToListAsync();
        }

        // GET: api/Blogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetBlog(int id)
        {
            var blog = await _context.Blogs.Include(b => b.User).Include(b => b.Comments).ThenInclude(c => c.User).AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

            if (blog == null)
            {
                return NotFound();
            }
            return blog;
        }

        // GET: api/Blogs/UserId
        [HttpGet("UserId/{id}")]
        public async Task<ActionResult<IEnumerable<Blog>>> GetBlogsByUser(int id)
        {
            var blogList = await _context.Blogs.Where(b => b.UserId == id).Include(b => b.User).ToListAsync();
            return blogList;
        }

        // PUT: api/Blogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutBlog()
        {
            IFormFileCollection req = Request.Form.Files;
            var files = req;
            var blogDtoString = Request.Form["BlogDetails"];
            var blogDtoObj = JsonConvert.DeserializeObject<BlogDTO>(blogDtoString);
            var uploads = Path.Combine(_env.WebRootPath, "BlogImage");

            if (blogDtoObj == null)
            {
                return BadRequest();
            }

            var blog = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(a => a.Id == blogDtoObj.Id);
            if (blog == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Blog not found!"
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
                    blogDtoObj.BlogImgUrl = $"BlogImage/{file.FileName}";
                }
            }


            var blogObj = _mapper.Map<Blog>(blogDtoObj);
            blogObj.UpdatedTime = DateTime.Now;
            blogObj.CreatedTime = blog.CreatedTime;

            _context.Entry(blogObj).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Message = "User is updated!"
            });
        }

        // POST: api/Blogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Blog>> PostBlog(Blog blog)
        {
            blog.CreatedTime = DateTime.Now;
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlog", new { id = blog.Id }, blog);
        }

        // DELETE: api/Blogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }
    }
}
