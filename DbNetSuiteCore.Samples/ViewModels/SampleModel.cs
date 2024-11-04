using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using DbNetSuiteCore.Enums;
using System.Dynamic;

namespace DbNetSuiteCoreSamples.ViewModels
{
    public class SampleModel : PageModel
    {
        protected readonly IConfiguration _configuration;
        protected readonly IWebHostEnvironment _webHostEnvironment;

        public SampleModel(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public Enums.Components Component { get; set; }
        public string Title { get; set; }
        public string SourceCode { get; set; }
        public string PageName { get; set; }
        public string CustomerId { get; set; } = null;
        public int? OrderId { get; set; } = null;

        public void OnGet(
            string? customerId = null,
            int? orderId = null
          )
        {
            CustomerId = customerId;
            OrderId = orderId;
            GetSourceCode();
        }

        protected void GetSourceCode()
        {
            try
            {
                var routeName = (HttpContext.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText;
                routeName = String.IsNullOrEmpty(routeName) ? "index" : routeName;
                var fileInfo = _webHostEnvironment.ContentRootFileProvider.GetFileInfo($"pages/{routeName}.cshtml");
                string fileContents = string.Empty;
                if (fileInfo.PhysicalPath != null)
                {
                    using (var sr = new StreamReader(fileInfo.PhysicalPath))
                    {
                        fileContents = sr.ReadToEnd();
                    }
                }

                var fileLines = fileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var collect = false;
                var source = new List<string>();
                foreach (string line in fileLines)
                {
                    if (line.StartsWith("@section Control"))
                    {
                        collect = true;
                        continue;
                    }

                    if (line.StartsWith("}") && collect)
                    {
                        break;
                    }

                    if (collect)
                    {
                        source.Add(line);
                    }
                }
                source.Insert(0, string.Empty);
                source = source.Select(s => s.TrimStart()).ToList();
                int indent = 0;
                var indentedSource = new List<string>();
                foreach (string line in source)
                {
                    if (line.StartsWith("}"))
                    {
                        indent--;
                    }
                    string indentedLine = string.Empty;
                    for (var i = 0; i < indent; i++)
                    {
                        indentedLine += '\t';
                    }
                    indentedLine += line;
                    indentedSource.Add(indentedLine);
                    if (line.StartsWith("@{") || line.StartsWith("{"))
                    {
                        indent++;
                    }
                }
                SourceCode = string.Join(Environment.NewLine, indentedSource);
            }
            catch (Exception e)
            {
                {
                    SourceCode = e.Message;
                }
            }
        }
    }
}
