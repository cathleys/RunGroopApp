using API.Data;
using API.Interfaces;
using API.Models;
using API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
public class ClubController : Controller
{
    private readonly IClubRepository _clubRepository;
    private readonly IPhotoService _photoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClubController(IClubRepository clubRepository,
    IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
    {
        _clubRepository = clubRepository;
        _photoService = photoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Club> clubs = await _clubRepository.GetClubs();
        //populate the view with data
        return View(clubs);
    }

    public async Task<IActionResult> Detail(int id)

    {
        //the navigation property(another entity/model) 
        //inside a model, EF won't bring the data automatically,
        //we need to include them

        // Club club = _context.Clubs.Include(a => a.Address).FirstOrDefault(c => c.Id == id);

        Club club = await _clubRepository
        .GetClubByIdAsync(id);
        return View(club);
    }

    public async Task<IActionResult> Create()
    {
        //gets the userId from the httpContext instead 
        //since it already holds the id of the user logged in
        //no need to go to the db
        var currentUserId = _httpContextAccessor
        .HttpContext?.User.GetUserId();

        var createClubViewModel = new CreateClubViewModel { AppUserId = currentUserId };
        return View(createClubViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateClubViewModel club)
    {
        if (ModelState.IsValid)
        {
            var result = await _photoService.AddPhotoAsync(club.Image);

            //alternative for automapper
            var createClub = new Club
            {
                Title = club.Title,
                Description = club.Description,
                Image = result.SecureUrl.AbsoluteUri,
                AppUserId = club.AppUserId,
                //adding one-many relationship
                Address = new Address
                {
                    City = club.Address.City,
                    State = club.Address.State,
                    Street = club.Address.Street
                }

            };
            _clubRepository.Add(createClub);
            return RedirectToAction("Index");
        }
        else
        {
            ModelState.AddModelError("", "Failed to add club");

        }
        return View(club);

    }


    //to only view the edit page with populated existing data 
    public async Task<IActionResult> Edit(int id)
    {
        var club = await _clubRepository.GetClubByIdAsync(id);


        if (club == null) return View("Error");

        var updateClub = new EditClubViewModel
        {
            Title = club.Title,
            Description = club.Description,
            Url = club.Image,
            AddressId = (int)club.AddressId,
            Address = club.Address,
            ClubCategory = club.ClubCategory

        };


        return View(updateClub);

    }


    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditClubViewModel clubViewModel)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to update club");
            return View("Edit", clubViewModel);
        }

        var getClub = await _clubRepository.GetClubByIdAsyncNoTracking(id);


        if (getClub != null)
        {

            try
            {
                var fi = new FileInfo(getClub.Image);
                var publicId = Path.GetFileNameWithoutExtension(fi.Name);
                await _photoService.DeletePhotoAsync(publicId);
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Failed to delete photo");
                return View(clubViewModel);
            }
            var photoResult = await _photoService.AddPhotoAsync(clubViewModel.Image);

            var updateClub = new Club
            {
                Id = id,
                Title = clubViewModel.Title,
                Description = clubViewModel.Description,
                Image = photoResult.SecureUrl.AbsoluteUri,
                AddressId = clubViewModel.AddressId,
                Address = clubViewModel.Address,

            };
            _clubRepository.Update(updateClub);

            return RedirectToAction("Index");
        }
        else
        {
            return View(clubViewModel);
        }

    }


    public async Task<IActionResult> Delete(int id)
    {
        Club club = await _clubRepository.GetClubByIdAsync(id);

        if (club == null) return View("Error");

        return View(club);

    }



    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteClub(int id)
    {
        var club = await _clubRepository.GetClubByIdAsync(id);

        if (club == null)
        {
            return View("Error");
        }

        if (!string.IsNullOrEmpty(club.Image))
        {
            _ = _photoService.DeletePhotoAsync(club.Image);
        }

        //pass the whole object to delete
        _clubRepository.Delete(club);
        return RedirectToAction("Index");

    }
}
