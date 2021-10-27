using NoBull.Models;
using System.Collections.Generic;

namespace NoBull.Repositories
{
    public interface IUserProfileRepository
    {
        void Add(UserProfile userProfile);
        UserProfile GetByFirebaseUserId(string firebaseUserId);
        UserProfile GetUserById(int id);
        List<UserProfile> GetAllUsers();
    }
}