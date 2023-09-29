using API.Data;
using API.Models;
using API.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly DataContext _context;

    public AccountController(UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager, DataContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    //this is a httpget for the login view
    public IActionResult Login()
    {
        var holdData = new LoginViewModel();
        return View(holdData);
    }

    //httpget and another method is like a tandem in aspnet 
    // when with a same function/action method name
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel)
    {
        if (!ModelState.IsValid) return View(loginViewModel);

        var userLogin = await _userManager.FindByEmailAsync(loginViewModel.EmailAddress);

        if (userLogin != null)
        {
            var password = await _userManager.CheckPasswordAsync(userLogin, loginViewModel.Password);
            if (password)
            {
                var result = await _signInManager.PasswordSignInAsync(userLogin, loginViewModel.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Race");
                }

            }
            TempData["Error"] = "Invalid password, Please try again";
            return View(loginViewModel);
        }
        TempData["Error"] = "User doesn't exist, please try again";
        return View(loginViewModel);
    }

    public async Task<IActionResult> Register()
    {
        var register = new RegisterViewModel();
        return View(register);
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ModelState.IsValid) return View(registerViewModel);

        var user = await _userManager.FindByEmailAsync(registerViewModel.EmailAddress);

        if (user != null)
        {
            TempData["Error"] = "Email is already in use";
            return View(registerViewModel);
        }

        var newUser = new AppUser
        {
            Email = registerViewModel.EmailAddress,
            UserName = registerViewModel.EmailAddress
        };
        var newUserResult = await _userManager.CreateAsync(newUser, registerViewModel.Password);

        if (newUserResult.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, UserRoles.User);

        }
        return View("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Race");
    }
}
