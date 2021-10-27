using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoBull.Models.ViewModels
{
    public class BlogCommentsViewModel
    {
        public Blog Blog { get; set; }
        public UserProfile UserProfile { get; set; }
        public int UserProfileId { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
