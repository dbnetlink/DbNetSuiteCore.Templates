using DbNetSuiteCoreSamples.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DbNetSuiteCoreSamples.Pages.Samples.DbNetGrid
{
    public class LocalizationModel : SampleModel
    {
        public LocalizationModel(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }
    }
}
