
using API.Controllers;
using API.Interfaces;
using API.Models;
using API.Repositories;
using API.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace RunGroopApp.Tests.Controllers
{
    public class ClubControllerTests
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClubController _clubController;
        public ClubControllerTests()
        {
            //Dependencies
            _clubRepository = A.Fake<IClubRepository>();
            _photoService = A.Fake<IPhotoService>();
            _httpContextAccessor = A.Fake<IHttpContextAccessor>();

            //SUT
            _clubController = new ClubController(
                _clubRepository,
                _photoService,
                _httpContextAccessor
                );
        }

        [Fact]
        public void ClubController_Index_ReturnsSuccess()
        {
            //Arrange
            var clubs = A.Fake<IEnumerable<Club>>();
            A.CallTo(() => _clubRepository.GetClubs()).Returns(clubs);
            //Act
            var result = _clubController.Index();
            //Assert - for controller tests -> test the actions
            //which are gonna be objects
            //test the view model 

            result.Should().BeOfType<Task<IActionResult>>();

        }

        [Fact]
        public void ClubController_Detail_ReturnsSuccess()

        {
            //Arrange
            var id = 1;
            var club = A.Fake<Club>();
            A.CallTo(() => _clubRepository.GetClubByIdAsync(id)).Returns(club);

            //Act - the function under test

            var result = _clubController.Detail(id);
            //Assert

            result.Should().BeOfType<Task<IActionResult>>();

        }
        [Fact]
        public void ClubController_Create_ReturnsSuccess()
        {
            //Arrange
           
            var createClub = new CreateClubViewModel
            {
                Title = "Club test",
                Description = "Description",
               // Image = "image",
                AppUserId = "ab123",
                //adding one-many relationship
                Address = new Address
                {
                    City = "Manila",
                    State = "Manila",
                    Street = "Manila"
                }
            };

            //Act
            var result = _clubController.Create(createClub);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();
            result.Should().NotBeNull();
            
        }


        [Fact]
        public void ClubController_Delete_ReturnsSuccess()
        {
            //Arrange
            var id = 1;
            var club = A.Fake<Club>();
            A.CallTo(() => _clubRepository.GetClubByIdAsync(id)).Returns(club);

            //Act - the function under test

            var result = _clubController.Delete(id);

            //Assert
            result.Should().BeOfType<Task<IActionResult>>();
        }
    }
}