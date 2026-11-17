using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace AlmostNewOutpost.Api.Repositories
{
    public class UserAdminRepostiory
    {
        public class UserAdminRepository : IUserAdminRepository
        {
            private readonly OutpostDbContext _db;
            public UserAdminRepository(OutpostDbContext db)
            {
                _db = db;
            }
            public async Task<List<AdminUserDto>> GetAllAsync(bool includeArchived)
            {
                List<AdminUserDto> users = await (
                from user in _db.Users
                join role in _db.Roles on user.RoleId equals role.RoleId
                where includeArchived || user.ArchivedAtUtc == null
                orderby user.Username
                select new AdminUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    RoleId = role.RoleId,
                    Role = role.RoleName,
                    ArchivedAtUtc = user.ArchivedAtUtc,
                }).ToListAsync();

                return users;
            }
        }
    }
}
