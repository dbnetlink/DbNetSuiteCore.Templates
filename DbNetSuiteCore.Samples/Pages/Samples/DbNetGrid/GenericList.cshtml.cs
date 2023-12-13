using DbNetSuiteCoreSamples.Models;
using DbNetSuiteCoreSamples.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DbNetSuiteCoreSamples.Pages.Samples.DbNetGrid
{
    public class GenericListModel : SampleModel
    {
        private NobelPrizes? _nobelPrizes { get; set; }

        public List<NobelPrizeLaureate> NobelPrizeLaureates { get; set; }
        public GenericListModel(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        public async override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod?.MethodInfo.Name == nameof(OnGet))
            {

                IFileInfo fileInfo = _webHostEnvironment.WebRootFileProvider.GetFileInfo("/data/nobel_prize_winners.json");
                if (fileInfo != null)
                {
                    string json = System.IO.File.ReadAllText(fileInfo.PhysicalPath ?? string.Empty);
                    _nobelPrizes = System.Text.Json.JsonSerializer.Deserialize<NobelPrizes>(json);
                    if (_nobelPrizes != null)
                    {
                        NobelPrizeLaureates = _nobelPrizes.prizes.Where(p => p.laureates != null).SelectMany(p => p.laureates.Select(l => new { p, l })).Select(x => new NobelPrizeLaureate(x.p, x.l)).ToList();
                    }
                }
                /*
                string json = await GetUrlContent("https://api.nobelprize.org/v1/prize.json");
                _nobelPrizes = System.Text.Json.JsonSerializer.Deserialize<NobelPrizes>(json);
                NobelPrizeLaureates = _nobelPrizes.prizes.Where(p => p.laureates != null).SelectMany(p => p.laureates.Select(l => new { p, l })).Select(x => new NobelPrizeLaureate(x.p, x.l)).ToList();
                */

            }
            await base.OnPageHandlerExecutionAsync(context, next);
        }

        public static async Task<string?> GetUrlContent(string url)
        {
            using (var client = new HttpClient())
            using (var result = await client.GetAsync(url))
                return result.IsSuccessStatusCode ? System.Text.Encoding.Default.GetString(await result.Content.ReadAsByteArrayAsync()) : null;
        }
    }
}