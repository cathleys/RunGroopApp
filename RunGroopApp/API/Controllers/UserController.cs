using API.Interfaces;
using API.Models;
using API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet("users")]
    public async Task<IActionResult> Index()
    {
        var users = await _userRepository.GetAllUsers();
        var result = new List<UserViewModel>();

        foreach (var user in users)
        {
            var userViewModels = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Pace = user.Pace,
                Mileage = user.MileAge,
                ProfileImageUrl = user.ProfileImageUrl
            };
            result.Add(userViewModels);
        }
        return View(result);
    }

    public async Task<IActionResult> Detail(string id)
    {
        var user = await _userRepository.GetUserById(id);

        if (user != null)
        {
            var UserDetailViewModel = new UserDetailViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Pace = user.Pace,
                Mileage = user.MileAge
            };
            return View(UserDetailViewModel);
        }
        return RedirectToAction("Index", "Home");
    }
}
