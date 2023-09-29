using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;
public class DashboardRepository : IDashboardRepository
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DashboardRepository(DataContext context,
    IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<AppUser> GetUserById(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByIdNoTracking(string id)
    {
        return await _context.Users
        .AsNoTracking()
        .Where(u => u.Id == id)
        .FirstOrDefaultAsync();
    }

    public async Task<List<Club>> GetUserClubs()
    {
        var currentUser = _httpContextAccessor.HttpContext?.User.GetUserId();

        var userClubs = _context.Clubs
        .Where(c => c.AppUser.Id == currentUser);

        return await userClubs.ToListAsync();
    }

    public async Task<List<Race>> GetUserRaces()
    {
        var currentUser = _httpContextAccessor.HttpContext?.User.GetUserId();

        var userRaces = _context.Races
        .Where(r => r.AppUser.Id == currentUser);

        return await userRaces.ToListAsync();
    }

    public Task<bool> Update(AppUser user)
    {
        _context.Users.Update(user);
        return Save();
    }

    public async Task<bool> Save()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
