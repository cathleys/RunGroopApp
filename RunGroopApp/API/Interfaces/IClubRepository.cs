﻿using API.Models;

namespace API.Interfaces;
public interface IClubRepository
{

    Task<IEnumerable<Club>> GetClubs();

    Task<Club> GetClubByIdAsync(int id);
    Task<Club> GetClubByIdAsyncNoTracking(int id);
    Task<IEnumerable<Club>> GetClubsByCity(string city);

    bool Add(Club club);
    bool Update(Club club);
    bool Delete(Club club);
    bool Save();
}
