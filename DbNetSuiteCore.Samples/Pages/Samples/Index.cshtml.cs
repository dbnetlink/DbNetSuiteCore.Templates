using DbNetSuiteCoreSamples.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DbNetSuiteCoreSamples.Pages.Samples
{
    public class MenuModel : SampleModel
    {
        public MenuModel(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }
    }
}
