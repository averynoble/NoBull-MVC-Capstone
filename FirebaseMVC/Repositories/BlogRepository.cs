using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using NoBull.Models;
using NoBull.Utils;

namespace NoBull.Repositories
{
    public class BlogRepository : BaseRepository, IBlogRepository
    {
        public BlogRepository(IConfiguration config) : base(config) { }

        public List<Blog> GetAllPublishedBlogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT  b.Id, b.UserProfileId, b.Title, b.CreateDateTime,
                                        b.Content, up.Id, up.FirstName, up.LastName, up.UserName,
                                        up.Email, up.FirebaseUserId, up.CreateDateTime
                                FROM Blog b
                                     LEFT JOIN UserProfile up ON b.UserProfileId = up.Id
                                ORDER BY b.CreateDateTime DESC";
                    var reader = cmd.ExecuteReader();

                    var blogs = new List<Blog>();

                    while (reader.Read())
                    {
                        blogs.Add(NewBlogFromReader(reader));
                    }

                    reader.Close();

                    return blogs;
                }
            }
        }

        public List<Blog> GetAllWithComments()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT b.Id AS BlogId, b.Title, b.Content, b.UserProfileId AS BlogUserProfileId,
                                       b.CreateDateTime AS BlogDateCreated, up.Id, up.UserName, 
                                       c.Id, c.Content, c.UserProfileId, cup.UserName AS CommenterName
                            FROM Blog b
                                 LEFT JOIN UserProfile up ON b.UserProfileId = up.Id
                                 LEFT JOIN Comment c ON c.BlogId = b.id
                                 LEFT JOIN UserProfile cup ON cup.Id = c.UserProfileId
                            ORDER BY b.CreateDateTime DESC";

                    var reader = cmd.ExecuteReader();
                    var blogs = new List<Blog>();
                    while (reader.Read())
                    {
                        var blogId = reader.GetInt32(reader.GetOrdinal("BlogId"));

                        var existingBlog = blogs.FirstOrDefault(p => p.Id == blogId);
                        if (existingBlog == null)
                        {
                            existingBlog = new Blog()
                            {
                                Id = blogId,
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("BlogDateCreated")),
                                UserProfileId = reader.GetInt32(reader.GetOrdinal("BlogUserProfileId")),
                                UserProfile = new UserProfile()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("BlogUserProfileId")),
                                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                },

                                Comments = new List<Comment>()
                            };
                            blogs.Add(existingBlog);
                        }

                        if (DbUtils.IsNotDbNull(reader, "CommenterName"))
                        {
                            existingBlog.Comments.Add(new Comment()
                            {
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                UserProfile = new UserProfile()
                                {
                                    UserName = reader.GetString(reader.GetOrdinal("CommenterName"))
                                },
                            });
                        }
                    }
                    reader.Close();
                    return blogs;
                }
            }
        }

        public Blog GetBlogByIdWithComments(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                             SELECT b.Id AS BlogId, b.Title, b.Content, b.CreateDateTime AS BlogDateCreated,
                                    b.UserProfileId AS BlogUserProfileId, c.Id AS CommentId, c.CreateDateTime,
                                    c.UserProfileId AS CommentUserProfileId, c.Content AS CommentContent, 
                                    up.UserName AS BlogAuthor, cup.UserName

                             FROM Blog b
                                    LEFT JOIN UserProfile up ON b.UserProfileId = up.Id
                                    LEFT JOIN Comment c ON c.BlogId = b.Id
                                    LEFT JOIN UserProfile cup ON cup.Id = c.UserProfileId
                             WHERE b.Id = @Id";

                    cmd.Parameters.AddWithValue("Id", id);
                    var reader = cmd.ExecuteReader();

                    Blog blog = null;
                    while (reader.Read())
                    {
                        if (blog == null)
                        {
                            blog = new Blog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("BlogId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("BlogDateCreated")),
                                UserProfileId = reader.GetInt32(reader.GetOrdinal("BlogUserProfileId")),
                                UserProfile = new UserProfile()
                                {
                                    UserName = reader.GetString(reader.GetOrdinal("BlogAuthor")),
                                },
                                Comments = new List<Comment>()
                            };
                        }
                        if (DbUtils.IsNotDbNull(reader, "CommentId"))
                        {
                            blog.Comments.Add(new Comment()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CommentId")),
                                Content = reader.GetString(reader.GetOrdinal("CommentContent")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                BlogId = reader.GetInt32(reader.GetOrdinal("BlogId")),
                                UserProfileId = reader.GetInt32(reader.GetOrdinal("CommentUserProfileId")),
                                UserProfile = new UserProfile()
                                {
                                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                                }
                            });
                        }
                    }

                    reader.Close();
                    return blog;
                }
            }
        }

        public Blog GetPublishedBlogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT b.Id, b.UserProfileId, b.Title, b.CreateDateTime,
                                           b.Content, up.Id, up.Email, 
                                           up.FirstName, up.LastName, up.UserName, up.FirebaseUserId,
                                           up.CreateDateTime, c.Id, c.Content, c.CreateDateTime, c.BlogId,
                                           c.UserProfileId
                                    FROM Blog b
                                            LEFT JOIN UserProfile up ON b.UserProfileId = up.Id
                                            LEFT JOIN Comment c ON c.BlogId = b.Id
                                    WHERE b.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    Blog blog = null;

                    if (reader.Read())
                    {
                        blog = NewBlogFromReader(reader);
                    }

                    reader.Close();
                    return blog;
                }
            }
        }

        public Blog GetUserBlogById(int id, int userProfileId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT b.Id, b.Title, b.Content, b.CreateDateTime, b.UserProfileId, up.Id, 
                               up.Email, up.FirebaseUserId, up.UserName, up.FirstName,
                               up.LastName, up.CreateDateTime
                        FROM Blog b
                                LEFT JOIN UserProfile up ON b.UserProfileId = up.Id
                        WHERE b.Id = @id AND b.UserProfileId = @userProfileId";

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@userProfileId", userProfileId);
                    var reader = cmd.ExecuteReader();

                    Blog blog = null;

                    if (reader.Read())
                    {
                        blog = NewBlogFromReader(reader);
                    }

                    reader.Close();
                    return blog;
                }
            }
        }

        public List<Blog> GetUserBlogsById(int userProfileId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            SELECT b.Id, b.Title, b.Content, b.CreateDateTime,
                                   b.UserProfileId, up.FirstName, up.LastName, up.UserName,
                                   up.Email, up.FirebaseUserId, up.CreateDateTime
                            FROM Blog b
                                    LEFT JOIN UserProfile up ON b.UserProfileId = up.Id
                            WHERE b.UserProfileid = @userProfileId
                            
                            ORDER BY b.CreateDateTime DESC";

                    cmd.Parameters.AddWithValue("@userProfileId", userProfileId);
                    var reader = cmd.ExecuteReader();

                    var blogs = new List<Blog>(userProfileId);

                    while (reader.Read())
                    {
                        blogs.Add(NewBlogFromReader(reader));
                    }
                    reader.Close();
                    return blogs;
                }
            }
        }

        public List<Comment> GetCommentsByBlogId(int blogId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id, c.Content, c.CreateDateTime, c.UserProfileId,
                                               up.UserName
                                        FROM Comment c
                                        LEFT JOIN Blog ON Blog.Id = c.BlogId
                                        LEFT JOIN UserProfile up ON up.Id = c.UserProfileId
                                        WHERE c.BlogId = @blogId";

                    cmd.Parameters.AddWithValue("@blogId", blogId);
                    var reader = cmd.ExecuteReader();

                    List<Comment> comments = new List<Comment>();

                    while (reader.Read())
                    {
                        var comment = new Comment()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Content = reader.GetString(reader.GetOrdinal("Content")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                            UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                            UserProfile = new UserProfile()
                            {
                                UserName = reader.GetString(reader.GetOrdinal("UserName"))
                            }
                        };
                        comments.Add(comment);
                    }
                    reader.Close();
                    return comments;
                }
            }
        }

        public void Add(Blog blog)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Blog (
                                Title, Content, CreateDateTime, UserProfileId )
                        OUTPUT INSERTED.ID
                        VALUES (
                            @Title, @Content, @CreateDateTime, @UserProfileId )";

                    cmd.Parameters.AddWithValue("@Title", blog.Title);
                    cmd.Parameters.AddWithValue("@Content", blog.Content);
                    cmd.Parameters.AddWithValue("@CreateDateTime", blog.CreateDateTime);
                    cmd.Parameters.AddWithValue("@UserProfileId", blog.UserProfileId);

                    blog.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void UpdateBlog(Blog blog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Blog
                            SET
                                Title = @title,
                                Content = @content
                            WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", blog.Id);
                    cmd.Parameters.AddWithValue("@title", blog.Title);
                    cmd.Parameters.AddWithValue("@content", blog.Content);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteBlog(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Blog
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private Blog NewBlogFromReader(SqlDataReader reader)
        {
            return new Blog()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                UserProfile = new UserProfile()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FirebaseUserId = reader.GetString(reader.GetOrdinal("FirebaseUserId")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"))
                }
            };
        }
    }
}
