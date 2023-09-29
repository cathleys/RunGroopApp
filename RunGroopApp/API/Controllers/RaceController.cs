using API.Data;
using API.Interfaces;
using API.Models;
using API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
public class RaceController : Controller
{
    private readonly IRaceRepository _raceRepository;
    private readonly IPhotoService _photoService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RaceController(IRaceRepository raceRepository,
    IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
    {
        _raceRepository = raceRepository;
        _photoService = photoService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Race> races = await _raceRepository.GetRaces();
        return View(races);
    }

    public async Task<IActionResult> Detail(int id)
    {
        Race race = await _raceRepository.GetRaceByIdAsync(id);
        return View(race);
    }
    public IActionResult Create()
    {

        var currentUserId = _httpContextAccessor
        .HttpContext?.User.GetUserId();

        var createRaceViewModel = new CreateRaceViewModel { AppUserId = currentUserId };

        return View(createRaceViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateRaceViewModel race)
    {
        if (ModelState.IsValid)
        {
            var imageResult = await _photoService.AddPhotoAsync(race.Image);
            //alternative to automapper (Viewmodel -> model)
            var createRace = new Race
            {
                Title = race.Title,
                Description = race.Description,
                Image = imageResult.SecureUrl.AbsoluteUri,
                AppUserId = race.AppUserId,
                //adding one many relationship
                Address = new Address
                {
                    City = race.Address.City,
                    State = race.Address.State,
                    Street = race.Address.Street
                }

            };

            _raceRepository.Add(createRace);
            return RedirectToAction("Index");
        }
        else
        {
            ModelState.AddModelError("", "Failed to create race");
        }
        return View(race);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var race = await _raceRepository.GetRaceByIdAsync(id);

        if (race == null) return View("Error");

        //Model db -> ViewModel web page
        //passing and transforming data from Club Model to EditRaceViewModel
        var updateRace = new EditRaceViewModel
        {
            Title = race.Title,
            Description = race.Description,
            Url = race.Image,
            AddressId = (int)race.AddressId,
            Address = race.Address,
            RaceCategory = race.RaceCategory

        };

        return View(updateRace);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, EditRaceViewModel raceVm)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to update race");
            return View("Edit", raceVm);
        }
        var race = await _raceRepository.GetRaceByIdAsyncNoTracking(id);

        if (race != null)
        {
            try
            {
                var fi = new FileInfo(race.Image);
                var publicId = Path.GetFileNameWithoutExtension(fi.Name);
                await _photoService.DeletePhotoAsync(publicId);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Failed to delete photo");
                return View(raceVm);

            }

            var photoResult = await _photoService.AddPhotoAsync(raceVm.Image);
            //ViewModel -> Model back to db

            var updateRace = new Race
            {
                Id = id,
                Title = raceVm.Title,
                Description = raceVm.Description,
                Image = photoResult.SecureUrl.AbsoluteUri,
                AddressId = raceVm.AddressId,
                Address = raceVm.Address
            };
            _raceRepository.Update(updateRace);
            return RedirectToAction("Index");
        }
        else
        {
            return View(raceVm);
        }

    }

    public async Task<IActionResult> Delete(int id)
    {
        Race race = await _raceRepository.GetRaceByIdAsync(id);
        if (race == null) return View("Error");
        return View(race);
    }

    [HttpDelete, ActionName("Delete")]
    public async Task<IActionResult> DeleteRace(int id)
    {
        var race = await _raceRepository.GetRaceByIdAsync(id);

        if (race == null) return View("Error");

        if (!string.IsNullOrEmpty(race.Image))
        {
            _ = _photoService.DeletePhotoAsync(race.Image);
        }

        //pass the whole object to delete
        _raceRepository.Delete(race);
        return RedirectToAction("Index");
    }
}
