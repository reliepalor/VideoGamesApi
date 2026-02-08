using VideoGameApi.Api.Repositories;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Auth.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(Data.VideoGameDbContext context) : base(context)
        {
        }
    }
}
