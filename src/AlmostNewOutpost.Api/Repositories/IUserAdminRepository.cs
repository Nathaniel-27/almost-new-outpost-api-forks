using AlmostNewOutpost.Api.Models.Responses;

namespace AlmostNewOutpost.Api.Repositories
{
    public interface IUserAdminRepository
    {
        Task<List<AdminUserDto>> GetAllAsync(bool includeArchived);

    }
}
