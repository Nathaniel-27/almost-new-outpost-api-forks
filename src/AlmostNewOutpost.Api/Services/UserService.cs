using AlmostNewOutpost.Api.Models.Responses;
using AlmostNewOutpost.Api.Repositories;

namespace AlmostNewOutpost.Api.Services
{
    public class UserService
    {
        private readonly IUserAdminRepository _userAdminRepository;
        public UserService(IUserAdminRepository userAdminRepository)
        {
            _userAdminRepository = userAdminRepository;
        }
        public async Task<List<AdminUserDto>> GetAllAsync(bool includeArchived)
        {
            return await _userAdminRepository.GetAllAsync(includeArchived);
        }

    }

}
