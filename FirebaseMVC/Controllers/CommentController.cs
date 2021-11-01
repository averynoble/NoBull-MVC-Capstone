using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Linq;
using System;
using NoBull.Repositories;
using NoBull.Models;
using NoBull.Utils;

namespace NoBull.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        public CommentController(ICommentRepository commentRepository, IBlogRepository blogRepository, IUserProfileRepository userProfileRepository)
        {
             _commentRepository = commentRepository;
            _blogRepository = blogRepository;
            _userProfileRepository = userProfileRepository;
        }
        // GET: CommentController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CommentController/Details/5
        public ActionResult Details(int id, int BlogId)
        {
            var blog = _blogRepository.GetPublishedBlogById(id);
            if (blog == null)
            {
                int userId = GetCurrentUserProfileId();
                blog = _blogRepository.GetUserBlogById(id, userId);
                List<Comment> comments = _commentRepository.GetCommentsByBlogId(BlogId);
                if (blog == null)
                {
                    return NotFound();
                }
            }

            return View(blog);
        }

        // GET: CommentController/Create
        public ActionResult Create(int id)
        {
            Comment comment = new Comment()
            {
                BlogId = id,
            };

            return View(comment);
        }

        // POST: CommentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment comment)
        {
            try
            {
                comment.CreateDateTime = DateTime.Now;
                comment.UserProfileId = GetCurrentUserProfileId();
                _commentRepository.Add(comment);

                return RedirectToAction("Details", "Blog", new { id = comment.BlogId });
            }
            catch (Exception ex)
            {
                return View(comment);
            }
        }

        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
