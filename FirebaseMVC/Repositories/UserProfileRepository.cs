using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using NoBull.Models;
using System.Linq;
using System;

namespace NoBull.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration config) : base(config) { }
        
        public List<UserProfile> GetAllUsers()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                SELECT up.Id, up.Email, up.FirebaseUserId, up.FirstName, up.LastName,
                                       up.UserName, up.CreateDateTime
                                FROM UserProfile up
                                ORDER BY UserName ASC";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<UserProfile> users = new List<UserProfile>();

                    while (reader.Read())
                    {
                        UserProfile user = new UserProfile
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirebaseUserId = reader.GetString(reader.GetOrdinal("FirebaseUserId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"))
                        };

                        users.Add(user);
                    }

                    reader.Close();
                    return users;
                }
            }
        }

        public UserProfile GetUserById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT up.Id, up.Email, up.FirebaseUserId, up.FirstName, up.LastName,
                                           up.UserName, up.CreateDateTime
                                    FROM UserProfile up
                                    WHERE up.Id = @Id";

                    cmd.Parameters.AddWithValue("@Id", id);

                    UserProfile userProfile = null;

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userProfile = new UserProfile
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirebaseUserId = reader.GetString(reader.GetOrdinal("FirebaseUserId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"))
                        };
                    }
                    reader.Close();

                    return userProfile;
                }
            }
        }

        public UserProfile GetByFirebaseUserId(string firebaseUserId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT up.Id, up.Email, up.FirebaseUserId, up.FirstName, up.LastName,
                                           up.UserName, up.CreateDateTime
                                    FROM UserProfile up
                                    WHERE FirebaseUserId = @FirebaseuserId";

                    cmd.Parameters.AddWithValue("@FirebaseUserId", firebaseUserId);

                    UserProfile userProfile = null;

                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        userProfile = new UserProfile
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                            FirebaseUserId = reader.GetString(reader.GetOrdinal("FirebaseUserId"))
                        };
                    }
                    reader.Close();

                    return userProfile;
                }
            }
        }

        public void Add(UserProfile userProfile)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        INSERT INTO
                                        UserProfile (Email, FirebaseUserId, FirstName, LastName, UserName, CreateDateTime) 
                                        OUTPUT INSERTED.ID
                                        VALUES(@email, @firebaseUserId, @firstName, @lastName, @userName, @createDateTime)";

                    cmd.Parameters.AddWithValue("@email", userProfile.Email);
                    cmd.Parameters.AddWithValue("@firebaseUserId", userProfile.FirebaseUserId);
                    cmd.Parameters.AddWithValue("@firstName", userProfile.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", userProfile.LastName);
                    cmd.Parameters.AddWithValue("@userName", userProfile.UserName);
                    cmd.Parameters.AddWithValue("@createDateTime", userProfile.CreateDateTime);

                    userProfile.Id = (int)cmd.ExecuteScalar();
                }
            }
        }
    }
}
