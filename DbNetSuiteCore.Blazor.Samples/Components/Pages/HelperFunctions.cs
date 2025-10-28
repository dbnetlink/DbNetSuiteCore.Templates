using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;

namespace DbNetSuiteCore.Blazor.Samples.Components.Pages
{
    public static class HelperFunctions
    {
        public static MarkupString GetSourceCode(IHttpContextAccessor httpContextAccessor, IWebHostEnvironment? host = null)
        {
            try
            {
                if (httpContextAccessor.HttpContext == null || host == null)
                {
                    return new MarkupString(string.Empty);
                }
                var routeName = httpContextAccessor.HttpContext.Request.Path.Value;
                routeName = routeName == "/" ? "index" : routeName;
                var subPath = $"components/pages{routeName}.razor";
                var fileLines = GetSourceLines(subPath, host);
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
                return new MarkupString(string.Join(Environment.NewLine, indentedSource));
            }
            catch (Exception e)
            {
                {
                    return new MarkupString(e.Message);
                }
            }
        }

        public static MarkupString Wiki(string link, string title)
        {
            return new MarkupString($"<a target=\"_blank\" href=\"https://github.com/dbnetlink/DbNetSuiteCore2/wiki/{link}\">{title}</a>");
        }
        private static string[] GetSourceLines(string filePath, IWebHostEnvironment? host)
        {
            var fileInfo = host.ContentRootFileProvider.GetFileInfo(filePath);
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
    }
}
