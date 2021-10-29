using NoBull.Models;
using System.Collections.Generic;

namespace NoBull.Repositories
{
    public interface IBlogRepository
    {
        List<Blog> GetAllPublishedBlogs();
        List<Blog> GetAllWithComments();
        List<Blog> GetUserBlogsById(int userProfileId);
        List<Comment> GetCommentsByBlogId(int blogId);
        Blog GetPublishedBlogById(int id);
        Blog GetUserBlogById(int id, int userProfileId);
        Blog GetBlogByIdWithComments(int id);
        void Add(Blog blog);
        void UpdateBlog(Blog blog);
        void DeleteBlog(int id);
    }
}