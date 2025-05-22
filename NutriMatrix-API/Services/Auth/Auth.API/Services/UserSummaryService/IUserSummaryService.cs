using Auth.API.Models.DTO;

namespace Auth.API.Services.UserSummaryService
{
    public interface IUserSummaryService
    {
        public Task<UserSummaryDTO> GetUserSummary(string id);
        public Task<UserSummaryDTO> UpdateUserSummaryById(UpdateUserDTO dto, string userId);
    }
}
