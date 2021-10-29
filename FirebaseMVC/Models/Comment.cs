using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoBull.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreateDateTime { get; set; }
        public Blog Blog { get; set; }
        public int BlogId { get; set; }
        public int UserProfileId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}
