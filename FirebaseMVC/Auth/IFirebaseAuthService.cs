using System.Threading.Tasks;
using NoBull.Auth.Models;

namespace NoBull.Auth
{
    public interface IFirebaseAuthService
    {
        Task<FirebaseUser> Login(Credentials credentials);
        Task<FirebaseUser> Register(Registration registration);
    }
}