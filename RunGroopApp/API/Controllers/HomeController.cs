using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Interfaces;
using API.Helpers;
using API.ViewModels;
using System.Net;
using Newtonsoft.Json;
using System.Globalization;

namespace API.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IClubRepository _clubRepository;

    public HomeController(ILogger<HomeController> logger,
    IClubRepository clubRepository)
    {
        _logger = logger;
        _clubRepository = clubRepository;
    }

    public async Task<IActionResult> Index()
    {
        _ = new IPInfo();
        var homeViewModel = new HomeViewModel();


        try
        {
            var token = Environment.GetEnvironmentVariable("TOKEN");
            var url = $"https://ipinfo.io?token={token}";
            var info = new WebClient().DownloadString(url);
            IPInfo? ipInfo = JsonConvert.DeserializeObject<IPInfo>(info);

            RegionInfo myRII = new RegionInfo(ipInfo.Country);

            ipInfo.Country = myRII.EnglishName;

            homeViewModel.City = ipInfo.City;
            homeViewModel.State = ipInfo.Region;

            if (homeViewModel.City != null)
            {
                homeViewModel.Clubs = await _clubRepository
                .GetClubsByCity(homeViewModel.City);
            }
            else
            {
                homeViewModel.Clubs = null;
            }
            return View(homeViewModel);
        }
        catch (System.Exception)
        {

            homeViewModel.Clubs = null;
        }
        return View(homeViewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
