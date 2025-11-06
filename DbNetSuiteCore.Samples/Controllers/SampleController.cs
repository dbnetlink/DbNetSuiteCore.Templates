using DbNetSuiteCore.Samples.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DbNetSuiteCore.Samples.Controllers
{
    public class SampleController : Controller
    {
        protected readonly IConfiguration _configuration;
        protected readonly IWebHostEnvironment _webHostEnvironment;
        public SampleController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        protected SampleModel GetSampleModel()
        {
            var sampleModel = new SampleModel(_configuration, _webHostEnvironment);
            sampleModel.GetSourceCode(HttpContext);
            sampleModel.GetControllerSourceCode(HttpContext);
            return sampleModel;
        }
    }
}