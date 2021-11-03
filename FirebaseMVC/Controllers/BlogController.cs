using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using NoBull.Repositories;
using NoBull.Models;
using NoBull.Utils;

namespace NoBull.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ICommentRepository _commentRepository;

        public BlogController(IBlogRepository blogRepository, IUserProfileRepository userProfileRepository, ICommentRepository commentRepository)
        {
            _blogRepository = blogRepository;
            _userProfileRepository = userProfileRepository;
            _commentRepository = commentRepository;
        }

        // GET: BlogController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MyBlog()
        {
            int userProfileId = GetCurrentUserProfileId();
            var blogs = _blogRepository.GetUserBlogsById(userProfileId);
            return View(blogs);
        }

        // GET: BlogController/Details/5
        public ActionResult Details(int id)
        {
            var blog = _blogRepository.GetBlogByIdWithComments(id);
            if (blog == null)
            {
                int userId = GetCurrentUserProfileId();
                blog = _blogRepository.GetUserBlogById(id, userId);
                if (blog == null)
                {
                    return NotFound();
                }
            }
            return View(blog);
        }

        // GET: BlogController/Create
        public ActionResult Create(int id)
        {
            Blog blog = new Blog()
            {
                Id = id,
            };

            return View(blog);
        }

        // POST: BlogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog)
        {
            try
            {
                blog.CreateDateTime = DateTime.Now;
                blog.UserProfileId = GetCurrentUserProfileId();
                _blogRepository.Add(blog);

                return RedirectToAction("MyBlog", "Blog", new { id = blog.Id });
            }
            catch (Exception ex)
            {
                return View(blog);
            }
        }

        // GET: BlogController/Edit/5
        public ActionResult Edit(int id)
        {
            var blog = _blogRepository.GetPublishedBlogById(id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: BlogController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Blog blog)
        {
            try
            {
                blog.UserProfileId = GetCurrentUserProfileId();
                _blogRepository.UpdateBlog(blog);
                return RedirectToAction("MyBlog");
            }
            catch (Exception ex)
            {
                return View(blog);
            }
        }

        // GET: BlogController/Delete/5
        public ActionResult Delete(int id)
        {
            var blog = _blogRepository.GetPublishedBlogById(id);
            return View(blog);
        }

        // POST: BlogController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Blog blog)
        {
            try
            {
                blog = _blogRepository.GetPublishedBlogById(id);
                if (blog.UserProfileId == GetCurrentUserProfileId())
                {
                    _blogRepository.DeleteBlog(id);
                    return RedirectToAction("MyBlog");
                }

                return StatusCode(403);
            }
            catch(Exception ex)
            {
                return View(blog);
            }
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
