namespace Exam_3.Controllers;

using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Exam_3.Models;
using Microsoft.Extensions.Logging;
using Exam_3.Dtos.User;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly InMemoryDatabase _inMemoryDatabase;

    public HomeController(ILogger<HomeController> logger, InMemoryDatabase inMemoryDatabase)
    {
        _logger = logger;
        _inMemoryDatabase = inMemoryDatabase;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult UserLoginPage()
    {
        if (this.HttpContext.User.Identity != null && this.HttpContext.User.Identity.IsAuthenticated)
        {
            return LocalRedirect("/Home");
        }

        return View("Login");
    }

    [AllowAnonymous]
    public async Task<IActionResult> UserLogin(UserRetrieveDto userDto)
    {
        if (this.HttpContext.User.Identity != null && this.HttpContext.User.Identity.IsAuthenticated)
        {
            return LocalRedirect("/Home");
        }

        User? user = _inMemoryDatabase.Users.FirstOrDefault<User?>(u => u != null && u.Username == userDto.Username && u.Password == userDto.Password, null);
        if (user == null)
            return View("NotFound");

        AuthenticationProperties authProperties = new()
        {
            AllowRefresh = true,
            IsPersistent = true,
            ExpiresUtc = DateTime.UtcNow.AddHours(1)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new System.Security.Claims.ClaimsPrincipal(
                new List<ClaimsIdentity>() { new ClaimsIdentity(new List<Claim>() {
                    new Claim(type: ClaimTypes.NameIdentifier, value: user.Id.ToString())
                }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme)}
            ),
            authProperties
        );

        return LocalRedirect("/");
    }

    public async Task<IActionResult> UserLogout()
    {
        if (this.HttpContext.User.Identity == null || !this.HttpContext.User.Identity.IsAuthenticated)
        {
            return LocalRedirect("/Home");
        }

        await HttpContext.SignOutAsync();

        return LocalRedirect("/Home");
    }

    public IActionResult GetMe()
    {
        int id = (int)Int64.Parse(s: ControllerContext.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        User? user = _inMemoryDatabase.Users.FirstOrDefault<User?>(u => u != null && u.Id == id, null);
        if (user == null)
            return View("NotFound");

        return View("Users", new List<User>() { user });
    }

    public IActionResult GetUserPage()
    {
        return View("GetUserPage");
    }

    public IActionResult GetUser(int id)
    {
        User? user = _inMemoryDatabase.Users.FirstOrDefault<User?>(u => u != null && u.Id == id, null);
        if (user == null)
            return View("NotFound");

        return View("Users", new List<User>() { user });
    }

    [AllowAnonymous]
    public IActionResult GetUsers()
    {
        if (_inMemoryDatabase.Users.Count == 0)
            return View("NotFound");

        return View("Users", _inMemoryDatabase.Users);
    }

    public IActionResult GetMyCars()
    {
        int id = (int)Int64.Parse(s: ControllerContext.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        User? user = _inMemoryDatabase.Users.FirstOrDefault<User?>(u => u != null && u.Id == id, null);
        if (user == null || user.Cars.Count == 0)
            return View("NotFound");

        return View("Cars", user.Cars);
    }

    public IActionResult GetUserCarsPage()
    {
        return View("GetUserCarsPage");
    }

    public IActionResult GetUserCars(int user_id)
    {
        User? user = _inMemoryDatabase.Users.FirstOrDefault<User?>(u => u != null && u.Id == user_id, null);
        if (user == null || user.Cars.Count == 0)
            return View("NotFound");

        return View("Cars", user.Cars);
    }

    public IActionResult GetCar(int car_id)
    {
        if (_inMemoryDatabase.Cars.Count == 0)
            return View("NotFound");

        return View("Cars", _inMemoryDatabase.Cars.Where<Car>(c => c.Id == car_id).ToList());
    }

    public IActionResult GetCars()
    {
        if (_inMemoryDatabase.Cars.Count == 0)
            return View("NotFound");

        return View("Cars", _inMemoryDatabase.Cars);
    }

    public IActionResult GetMyVisits()
    {
        int id = (int)Int64.Parse(s: ControllerContext.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        User? user = _inMemoryDatabase.Users.FirstOrDefault<User?>(u => u != null && u.Id == id, null);
        if (user == null || user.Cars.Count == 0)
            return View("NotFound");

        List<Visit> visits = new();
        foreach (Car car in user.Cars)
        {
            visits = visits.Concat(_inMemoryDatabase.Visits.Where(v => car.VisitIds.Contains(v.Id))).ToList();
        }

        if (visits.Count == 0)
            return View("NotFound");

        return View("Visits", visits);
    }

    public IActionResult GetUserVisitsPage()
    {
        return View("GetUserVisitsPage");
    }

    public IActionResult GetUserVisits(int user_id)
    {
        User? user = _inMemoryDatabase.Users.FirstOrDefault<User?>(u => u != null && u.Id == user_id, null);
        if (user == null || user.Cars.Count == 0)
            return View("NotFound");

        List<Visit> visits = new();
        foreach (Car car in user.Cars)
        {
            visits = visits.Concat(_inMemoryDatabase.Visits.Where(v => car.VisitIds.Contains(v.Id))).ToList();
        }

        if (visits.Count == 0)
            return View("NotFound");

        return View("Visits", visits);
    }

    public IActionResult GetCarVisitsPage()
    {
        return View("GetCarVisitsPage");
    }

    public IActionResult GetCarVisits(int car_id)
    {
        Car? car = _inMemoryDatabase.Cars.FirstOrDefault<Car?>(u => u != null && u.Id == car_id, null);
        if (car == null || car.VisitIds.Count == 0)
            return View("NotFound");

        return View("Visits", _inMemoryDatabase.Visits.Where(v => car.VisitIds.Contains(v.Id)).ToList());
    }

    public IActionResult GetVisits()
    {
        if (_inMemoryDatabase.Visits.Count == 0)
            return View("NotFound");

        return View("Visits", _inMemoryDatabase.Visits);
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Unauthenticated()
    {
        return View("Unauthenticated");
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
