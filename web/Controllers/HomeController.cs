using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolParentalSystem.Web.Models;

namespace SchoolParentalSystem.Web.Controllers;

//
// Class Name       : HomeController
// Purpose           : Handles the default homepage and general error pages.
// Re-use            : Used by MVC routing and corresponding services.
// Input Parameters  : Injected service dependencies.
// Output Type       : MVC controller class
//
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    //
    // Method Name       : HomeController
    // Purpose           : Initializes required dependencies for HomeController.
    // Re-use            : Invoked by HomeController or MVC requests.
    // Input Parameters  : ILogger<HomeController> logger
    // Output Type       : Constructor (initializes controller dependencies)
    //
    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    //
    // Method Name       : Index
    // Purpose           : Display the main page for this controller.
    // Re-use            : Invoked by HomeController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    public IActionResult Index()
    {
        return View();
    }

    //
    // Method Name       : Privacy
    // Purpose           : Display the site privacy information.
    // Re-use            : Invoked by HomeController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    public IActionResult Privacy()
    {
        return View();
    }

    //
    // Method Name       : Error
    // Purpose           : Display information about an application error.
    // Re-use            : Invoked by HomeController or MVC requests.
    // Input Parameters  : None
    // Output Type       : IActionResult
    //
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
