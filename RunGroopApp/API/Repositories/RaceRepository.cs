using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;
public class RaceRepository : IRaceRepository
{
    private readonly DataContext _context;

    public RaceRepository(DataContext context)
    {
        _context = context;
    }

    public bool Add(Race race)
    {
        _context.Add(race);
        return Save();
    }

    public bool Delete(Race race)
    {
        _context.Remove(race);
        _context.Addresses.Remove(race.Address);
        return Save();
    }

    public async Task<Race> GetRaceByIdAsync(int id)
    {
        //the navigation property(another entity/model) 
        //inside a model, EF won't bring the data automatically(lazy loading),
        //we need to include them (one-many relationships)
        return await _context.Races
        .Include(a => a.Address)
        .FirstOrDefaultAsync(r => r.Id == id);
    }
    public async Task<Race> GetRaceByIdAsyncNoTracking(int id)
    {

        return await _context.Races
        .Include(a => a.Address)
        .AsNoTracking()
        .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<Race>> GetRaces()
    {
        return await _context.Races.ToListAsync();
    }

    public async Task<IEnumerable<Race>> GetRacesByCity(string city)
    {
        return await _context.Races.Where(r => r.Address.City.Contains(city))
        .ToListAsync();
    }

    public bool Save()
    {
        return _context.SaveChanges() > 0;
    }

    public bool Update(Race race)
    {
        _context.Update(race);
        return Save();
    }
}
