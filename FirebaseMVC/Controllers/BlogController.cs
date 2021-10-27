using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;
using NoBull.Repositories;
using NoBull.Models.ViewModels;
using NoBull.Models;
using NoBull.Utils;

namespace NoBull.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        public BlogController(IBlogRepository blogRepository, IUserProfileRepository userProfileRepository)
        {
            _blogRepository = blogRepository;
            _userProfileRepository = userProfileRepository;
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
            var blog = _blogRepository.GetPublishedBlogById(id);
            if (blog == null)
            {
                int userId = GetCurrentUserProfileId();
                blog = _blogRepository.GetUserBlogById(id, userId);
                if (blog == null)
                {
                    return NotFound();
                }
            }
            List<Comment> comments = _blogRepository.GetCommentsByBlogId(id);
            var vm = new BlogCommentsViewModel()
            {
                Blog = blog,
                Comments = comments,
            };

            return View(vm);
        }

        // GET: BlogController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
