using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoBull.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreateDateTime { get; set; }
        public DateTime PublishedDateTime { get; set; }
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
        public int CommentId { get; set; }
    }
}
