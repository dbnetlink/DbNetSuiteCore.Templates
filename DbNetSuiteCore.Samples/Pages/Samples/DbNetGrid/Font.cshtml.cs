using DbNetSuiteCoreSamples.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DbNetSuiteCoreSamples.Pages.Samples.DbNetGrid
{
    public class FontModel : SampleModel
    {
        public FontModel(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }
    }
}
