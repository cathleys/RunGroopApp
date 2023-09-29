using API.Data;
using API.Interfaces;
using API.Models;
using API.ViewModels;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPhotoService _photoService;

    public DashboardController(
        IDashboardRepository dashboardRepository,
        IHttpContextAccessor httpContextAccessor,
        IPhotoService photoService)
    {
        _dashboardRepository = dashboardRepository;
        _httpContextAccessor = httpContextAccessor;
        _photoService = photoService;
    }

    private void MapUserEdit(AppUser user, EditUserDashboardViewModel editVm,
    ImageUploadResult photoResult)
    {
        //ViewModel -> Model


        user.Id = editVm.Id;
        user.Pace = editVm.Pace;
        user.MileAge = editVm.MileAge;
        user.ProfileImageUrl = photoResult.SecureUrl.AbsoluteUri;
        user.City = editVm.City;
        user.State = editVm.State;

    }
    public async Task<IActionResult> Index()
    {
        var userRaces = await _dashboardRepository.GetUserRaces();
        var userClubs = await _dashboardRepository.GetUserClubs();

        var dashboardViewModel = new DashboardViewModel
        {
            Races = userRaces,
            Clubs = userClubs,
        };
        return View(dashboardViewModel);
    }

    public async Task<IActionResult> EditUserProfile()
    {
        var currentUserId = _httpContextAccessor
        .HttpContext?.User.GetUserId();

        var user = await _dashboardRepository.GetUserById(currentUserId);

        if (user == null) return View("Error");

        var editUserDashboardViewModel = new EditUserDashboardViewModel
        {
            Id = currentUserId,
            Pace = user.Pace,
            MileAge = user.MileAge,
            ProfileImageUrl = user.ProfileImageUrl,
            City = user.City,
            State = user.State
        };

        //pass the edit user so that it populates page with data
        return View(editUserDashboardViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditUserProfile(
        EditUserDashboardViewModel editVm)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to edit profile");
            return View("EditUserProfile", editVm);
        }

        var user = await _dashboardRepository.GetUserByIdNoTracking(editVm.Id);

        if (user.ProfileImageUrl == "" || user.ProfileImageUrl == null)
        {
            var photoResult = await _photoService.AddPhotoAsync(editVm.Image);
            //Optimistice Concurrency = "Tracking error" you'll encounter someday...
            //solution 1 = use no tracking
            MapUserEdit(user, editVm, photoResult);

            _dashboardRepository.Update(user);
            return RedirectToAction("Index");
        }
        else
        {
            try
            {
                await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to delete profile photo");
                return View(editVm);
            }

            var photoResult = await _photoService.AddPhotoAsync(editVm.Image);
            MapUserEdit(user, editVm, photoResult);

            _dashboardRepository.Update(user);
            return RedirectToAction("Index");
        }



    }

}
