
using API.Data;
using API.Data.Enum;
using API.Models;
using API.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace RunGroopApp.Tests.Repositories
{
    public class ClubRepositoryTests
    {
     
        private async Task<DataContext> GetDbContext()
        {

            var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();

            if(await databaseContext.Clubs.CountAsync() < 0)
            {
                for ( int i = 0; i < 10;  i++)
                {
                    
                databaseContext.Add(

                new Club()
                {
                    Title = "Running Club 1",
                    Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
                    Description = "This is the description of the first cinema",
                    ClubCategory = ClubCategory.City,
                    Address = new Address()
                    {
                        Street = "123 Main St",
                        City = "Charlotte",
                        State = "NC"
                    }
                });

                await databaseContext.SaveChangesAsync();
                }
            }
            return databaseContext;
        }
    
        [Fact]
    public async void ClubRepository_Add_ReturnsBool()
        {

            //Arrange
            var club = new Club()
            {
                Title = "Running Club 1",
                Image = "https://www.eatthis.com/wp-content/uploads/sites/4/2020/05/running.jpg?quality=82&strip=1&resize=640%2C360",
                Description = "This is the description of the first cinema",
                ClubCategory = ClubCategory.City,
                Address = new Address()
                {
                    Street = "123 Main St",
                    City = "Charlotte",
                    State = "NC"
                }
            };

            var dataContext = await GetDbContext();
            var clubRepository = new ClubRepository(dataContext);
            //Act
            var result = clubRepository.Add(club);
            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ClubRepository_GetByIdAsync_ReturnsClub()
        {
            //Assert
            var id = 1;
            var databaseContext = await GetDbContext();
            var clubRepository =  new ClubRepository(databaseContext);

            //Act
            var result = clubRepository.GetClubByIdAsync(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<Club>>();        }


    }
}
