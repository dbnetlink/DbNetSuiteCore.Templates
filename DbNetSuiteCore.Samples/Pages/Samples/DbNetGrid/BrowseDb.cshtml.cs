using DbNetSuiteCoreSamples.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DbNetSuiteCoreSamples.Pages.Samples.DbNetGrid
{
    public class BrowseDbModel : SampleModel
    {
        public BrowseDbModel(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }
    }
}
