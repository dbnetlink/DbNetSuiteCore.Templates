using DbNetSuiteCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        public string ControllerSourceCode { get; set; }
        public string PageName { get; set; }
        public string? CustomerId { get; set; } = null;
        public int? OrderId { get; set; } = null;
        public GridModel GridModel { get; set; }
        public List<GridModel> GridModels { get; set; } = new List<GridModel>();
        public FormModel FormModel { get; set; }
        public List<FormModel> FormModels { get; set; } = new List<FormModel>();
        public SelectModel SelectModel { get; set; }
        public List<SelectModel> SelectModels { get; set; } = new List<SelectModel>();


        public void OnGet(
            string? customerId = null,
            int? orderId = null
          )
        {
            CustomerId = customerId;
            OrderId = orderId;
            GetSourceCode();
        }

        public void GetSourceCode(HttpContext? httpContext = null)
        {
            try
            {
                if (httpContext == null)
                {
                    httpContext = HttpContext;
                }
                var routeName = httpContext.Request.Path.Value;
                routeName = routeName == "/" ? "index" : routeName;
                var subPath = routeName.StartsWith("/samples") ? $"pages{routeName}.cshtml" : $"views{routeName}.cshtml";
                var fileLines = GetSourceLines(subPath);
                var collect = false;
                var source = new List<string>();
                foreach (string line in fileLines)
                {
                    if (line.StartsWith("@section Control"))
                    {
                        collect = true;
                        continue;
                    }

                    if (line.StartsWith("@{"))
                    {
                        continue;
                    }

                    if (line.StartsWith("}}") && collect)
                    {
                        break;
                    }

                    if (collect)
                    {
                        source.Add(line);
                    }
                }
                source.Insert(0, string.Empty);
                // source = source.Select(s => s.TrimStart()).ToList();
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

        private string[] GetSourceLines(string filePath)
        {
            var fileInfo = _webHostEnvironment.ContentRootFileProvider.GetFileInfo(filePath);
            string fileContents = string.Empty;
            if (fileInfo.PhysicalPath != null)
            {
                using (var sr = new StreamReader(fileInfo.PhysicalPath))
                {
                    fileContents = sr.ReadToEnd();
                }
            }

            fileContents = fileContents.Replace(">", "&gt;").Replace("<", "&lt;");
            return fileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public void GetControllerSourceCode(HttpContext httpContext)
        {
            try
            {
                if (httpContext == null)
                {
                    httpContext = HttpContext;
                }
                var routeName = httpContext.Request.Path.Value;
                routeName = routeName == "/" ? "index" : routeName;
                var controllerName = $"{routeName.Split("/")[1]}controller.cs";
                var actionName = $"{routeName.Split("/")[2]}";
                var subPath = $"controllers/{controllerName}";

                var fileLines = GetSourceLines(subPath);
                var collect = false;
                var source = new List<string>() { string.Empty};
                foreach (string line in fileLines)
                {
                    if (line.Trim().ToLower().StartsWith($"public IActionResult {actionName}()".ToLower()))
                    {
                        collect = true;
                    }
                    if (collect)
                    {
                        if (line.Length > 4)
                        {
                            source.Add(line.Remove(0, 4));
                        }
                        else
                        {
                            source.Add(line);
                        }
                    }
                    if (line == "        }" && collect)
                    {
                        break;
                    }
                }

                ControllerSourceCode = string.Join(Environment.NewLine, source);
            }
            catch (Exception e)
            {
                ControllerSourceCode = e.Message;
            }
        }
    }
}
