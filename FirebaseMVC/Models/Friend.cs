using System;
using Microsoft.Data.SqlClient.Server;

namespace NoBull.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public int FriendId { get; set; }
    }
}
