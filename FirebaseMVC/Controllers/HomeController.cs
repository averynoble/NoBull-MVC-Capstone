using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using NoBull.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using NoBull.Repositories;

namespace NoBull.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly ICommentRepository _commentRepository;

        public HomeController(IUserProfileRepository userProfileRepository, ICommentRepository commentRepository, IBlogRepository blogRepository)
        {
            _userProfileRepository = userProfileRepository;
            _blogRepository = blogRepository;
            _commentRepository = commentRepository;
        }

        public IActionResult Index()
        {
            
            var blogs = _blogRepository.GetAllWithComments();
            return View(blogs);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
