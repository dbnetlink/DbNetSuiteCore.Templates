using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace DbNetSuiteCore.Blazor.Samples.Components.Pages
{
    public static class HelperFunctions
    {
        public static MarkupString GetSourceCode(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host = null)
        {
            try
            {
                var fileLines = GetSourceLines(httpContextAccessor, host);
                if (fileLines.Length == 0)
                {
                    return new MarkupString(string.Empty);
                }
                var collect = false;
                var source = new List<string>();
                foreach (string line in fileLines)
                {
                    if (line.StartsWith("@{"))
                    {
                        collect = true;
                    }
                    else if (line.Contains("SectionContent SectionName") && collect)
                    {
                        break;
                    }

                    if (collect)
                    {
                        source.Add(line);
                    }
                }
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
                return new MarkupString(string.Join(Environment.NewLine, indentedSource));
            }
            catch (Exception e)
            {
                {
                    return new MarkupString(e.Message);
                }
            }
        }

        public static MarkupString GetJSSourceCode(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host = null)
        {
            try
            {
                var fileLines = GetSourceLines(httpContextAccessor, host);
                if (fileLines.Length == 0)
                {
                    return new MarkupString(string.Empty);
                }
                var collect = false;
                var source = new List<string>();
                foreach (string line in fileLines)
                {
                    if (line.StartsWith("&lt;script type="))
                    {
                        collect = true;
                    }
                    else if (line.Contains("&lt;/script&gt;") && collect)
                    {
                        source.Add(line);
                        break;
                    }

                    if (collect)
                    {
                        source.Add(line);
                    }
                }
                if (source.Any() == false)
                {
                    return new MarkupString(string.Empty);
                }
                return new MarkupString(string.Join(Environment.NewLine, source));
            }
            catch (Exception e)
            {
                {
                    return new MarkupString(e.Message);
                }
            }
        }

        public static MarkupString GetJsLibraries(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host = null)
        {
            try
            {
                var fileLines = GetSourceLines(httpContextAccessor, host);
                if (fileLines.Length == 0)
                {
                    return new MarkupString(string.Empty);
                }
                var source = new List<string>();
                foreach (string line in fileLines)
                {
                    if (line.StartsWith("&lt;script src="))
                    {
                        source.Add(line);
                    }
                }
                if (source.Any() == false)
                {
                    return new MarkupString(string.Empty);
                }
                return new MarkupString(string.Join(Environment.NewLine, source));
            }
            catch (Exception e)
            {
                {
                    return new MarkupString(e.Message);
                }
            }
        }

        public static string GetNextSample(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host = null)
        {
            return GetAdjacentSample(httpContextAccessor,host,true);
        }

        public static string GetPreviousSample(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host = null)
        {
            return GetAdjacentSample(httpContextAccessor, host, false);
        }

        private static string GetAdjacentSample(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host, bool next)
        {
            var sourceFiles = GetSourceFiles(httpContextAccessor, host);

            var routeParts = (httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty).Split("/").ToList();
            var folderName = routeParts[1];
            var fileName = routeParts.Last() ?? string.Empty;

            var index = sourceFiles.IndexOf(fileName);

            if (next)
            {
                fileName = (index < sourceFiles.Count - 1) ? sourceFiles[index + 1] : sourceFiles.First();
            }
            else
            {
                fileName = (index > 0) ? sourceFiles[index - 1] : sourceFiles.Last();
            }
            return $"/{folderName}/{fileName}";
        }

        public static MarkupString Wiki(string link, string title)
        {
            return new MarkupString($"<a target=\"_blank\" href=\"https://github.com/dbnetlink/DbNetSuiteCore2/wiki/{link}\">{title}</a>");
        }

        private static string[] GetSourceLines(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host)
        {
            if (httpContextAccessor.HttpContext == null || host == null)
            {
                return new string[0];
            }
            var routeName = httpContextAccessor.HttpContext.Request.Path.Value;
            string cacheKey = $"SourceLines_{routeName}";

            if (httpContextAccessor.HttpContext.Items[cacheKey] is string[] sourceLines)
            {
                return sourceLines;
            }
            routeName = routeName == "/" ? "index" : routeName;
            var subPath = $"components/pages{routeName}.razor";
            var fileInfo = host.ContentRootFileProvider.GetFileInfo(subPath);
            string fileContents = string.Empty;
            if (fileInfo.PhysicalPath != null)
            {
                using (var sr = new StreamReader(fileInfo.PhysicalPath))
                {
                    fileContents = sr.ReadToEnd();
                }
            }

            fileContents = fileContents.Replace(">", "&gt;").Replace("<", "&lt;");

            httpContextAccessor.HttpContext.Items[cacheKey] = fileContents.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            return httpContextAccessor?.HttpContext.Items[cacheKey] as string[] ?? new string[0];
        }

        private static List<string> GetSourceFiles(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host)
        {
            if (httpContextAccessor.HttpContext == null || host == null)
            {
                return new List<string>();
            }
            var routeParts = (httpContextAccessor.HttpContext.Request.Path.Value ?? string.Empty).Split("/").ToList();
            routeParts.RemoveAt(routeParts.Count-1);
            var folderName = string.Join("/", routeParts);

            string cacheKey = $"SourceLines_{folderName}";
            if (httpContextAccessor.HttpContext.Items[cacheKey] is List<string> cachedFiles)
            {
                return cachedFiles;
            }

            var subPath = $"components/pages{folderName}";
            var directoryContents = host.ContentRootFileProvider.GetDirectoryContents(subPath);
            var files = new List<string>();
            foreach (var file in directoryContents)
            {
                if (file.Name.StartsWith("_"))
                {
                    continue;
                }   
                files.Add(file.Name.Split(".").First());
            }

            httpContextAccessor.HttpContext.Items[cacheKey] = files;
            return httpContextAccessor?.HttpContext.Items[cacheKey] as List<string> ?? new List<string>();
        }
    }
}
