using DbNetSuiteCore.Samples.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DbNetSuiteCore.Samples.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View(new SampleModel(_configuration, _webHostEnvironment) );
        }
     
    }
}
