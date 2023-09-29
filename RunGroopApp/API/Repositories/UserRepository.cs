using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;
public class UserRepository : IUserRepository
{
    private readonly DataContext _context;

    public UserRepository(DataContext context)
    {
        _context = context;
    }

    public bool Add(AppUser user)
    {
        _context.Add(user);
        return Save();
    }

    public bool Delete(AppUser user)
    {
        _context.Remove(user);
        return Save();
    }

    public async Task<IEnumerable<AppUser>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<AppUser> GetUserById(string id)
    {
        return await _context.Users.FindAsync(id);
    }

    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }

    public bool Update(AppUser user)
    {
        _context.Update(user);
        return Save();
    }
}
