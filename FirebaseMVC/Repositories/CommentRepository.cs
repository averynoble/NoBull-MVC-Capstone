using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using NoBull.Models;
using System.Linq;
using System;

namespace NoBull.Repositories
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public CommentRepository(IConfiguration config) : base(config) { }

        public List<Comment> GetCommentsByBlogId(int BlogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT c.Id, c.Content, c.CreateDateTime, c.BlogId, up.UserName
                            FROM Comment c
                                    LEFT JOIN Blog b on b.Id = c.BlogId
                                    LEFT JOIN UserProfile up on up.Id = c.UserProfileId
                            WHERE c.BlogId = @BlogId
                            ORDER BY c.CreateDateTime DESC";

                    cmd.Parameters.AddWithValue("@BlogId", BlogId);
                    var reader = cmd.ExecuteReader();

                    var comments = new List<Comment>(BlogId);
                    while (reader.Read())
                    {
                        comments.Add(NewCommentFromReader(reader));
                    }
                    reader.Close();
                    return comments;
                }
            }
        }

        public Comment GetCommentByBlogId(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT c.Content, c.CreateDateTime,c.BlogId, up.UserName
                                    FROM Comment c
                                        LEFT JOIN Blog b ON b.Id = c.BlogId
                                        LEFT JOIN UserProfile up ON up.Id = c.UserProfileId
                                    WHERE c.BlogId = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    Comment comment = null;

                    if (reader.Read())
                    {
                        comment = NewCommentFromReader(reader);
                    }
                    reader.Close();
                    return comment;
                }
            }
        }

        public void Add(Comment comment)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            INSERT INTO Comment (
                                        Content, CreateDateTime, UserProfileId, BlogId )
                            OUTPUT INSERTED.ID
                            VALUES (
                                    @Content, @CreateDateTime, @UserProfileId, @BlogId )";

                    cmd.Parameters.AddWithValue("@Content", comment.Content);
                    cmd.Parameters.AddWithValue("@CreateDateTime", comment.CreateDateTime);
                    cmd.Parameters.AddWithValue("@UserProfileId", comment.UserProfileId);
                    cmd.Parameters.AddWithValue("@BlogId", comment.BlogId);

                    comment.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void UpdateComment(Comment comment)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Comment
                            SET 
                                Content = @content
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", comment.Id);
                    cmd.Parameters.AddWithValue("@content", comment.Content);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteComment(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                DELETE FROM Comment
                                WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Comment NewCommentFromReader(SqlDataReader reader)
        {
            return new Comment()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                UserProfile = new UserProfile()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                },
                BlogId = reader.GetInt32(reader.GetOrdinal("BlogId"))
            };
        }
    }
}
