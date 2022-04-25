using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBackend.Data.Context;
using ProjectBackend.DTOs;
using ProjectBackend.Models;

namespace ProjectBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly BloggerContext _context;

        public CommentsController(BloggerContext context)
        {
            _context = context;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comment>>> GetComments()
        {
            return await _context.Comments.ToListAsync();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comment>> GetComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            return comment;
        }

        // GET: api/Comments/Blog/BlogId
        [HttpGet("Blog/{id}")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetCommentByBlogId(int blogId)
        {
            var commentList = await _context.Comments
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedTime)
                .ThenBy(c=>c.Id)
                .ToListAsync();

            if (commentList == null)
            {
                return NotFound();
            }

            return commentList;
        }


        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<Comment>> PutComment([FromBody] EditCommentDTO commentDtoObj)
        {
            if (commentDtoObj == null)
            {
                return BadRequest();
            }
            
            var commentId = commentDtoObj.Id;

            if (!CommentExists(commentId))
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(commentId);
            comment.Content = commentDtoObj.Content;
            _context.Entry(comment).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Status = 200,
                Message = "Comment Updated"
            });
        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id = comment.Id }, comment);
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
