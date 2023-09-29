using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;
public class ClubRepository : IClubRepository
{
    private readonly DataContext _context;

    public ClubRepository(DataContext context)
    {
        _context = context;
    }

    public bool Add(Club club)
    {
        _context.Add(club);
        return Save();
    }

    public bool Delete(Club club)
    {
        _context.Remove(club);
        _context.Addresses.Remove(club.Address);
        return Save();
    }

    public async Task<Club> GetClubByIdAsync(int id)
    {
        return await _context.Clubs
        .Include(a => a.Address)
        .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Club> GetClubByIdAsyncNoTracking(int id)
    {
        return await _context.Clubs
        .Include(a => a.Address)
        .AsNoTracking()
        .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Club>> GetClubs()
    {
        return await _context.Clubs.ToListAsync();
    }

    public async Task<IEnumerable<Club>> GetClubsByCity(string city)
    {
        return await _context.Clubs.Where(c => c.Address.City.Contains(city))
        .ToListAsync();


    }

    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }

    public bool Update(Club club)
    {
        _context.Update(club);
        return Save();
    }
}
