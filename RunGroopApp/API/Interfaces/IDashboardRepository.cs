using API.Models;

namespace API.Interfaces;
public interface IDashboardRepository
{
    Task<List<Race>> GetUserRaces();
    Task<List<Club>> GetUserClubs();
    Task<AppUser> GetUserById(string id);
    Task<AppUser> GetUserByIdNoTracking(string id);

    Task<bool> Update(AppUser user);
    Task<bool>Save();


}
