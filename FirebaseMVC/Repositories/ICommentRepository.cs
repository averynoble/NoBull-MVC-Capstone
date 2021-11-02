using NoBull.Models;
using System.Collections.Generic;

namespace NoBull.Repositories
{
    public interface ICommentRepository
    {
        void Add(Comment comment);
        void DeleteComment(int id);
        Comment GetCommentById(int id);
        void UpdateComment(Comment comment);
        List<Comment> GetCommentsByBlogId(int BlogId);
    }
}