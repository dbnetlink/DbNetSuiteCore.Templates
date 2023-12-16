using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Text.RegularExpressions;
using DbNetSuiteCoreSamplesEnums = DbNetSuiteCoreSamples.Enums;
using System.Globalization;
using DbNetSuiteCore.Utilities;
using Microsoft.AspNetCore.Html;
using DbNetSuiteCoreSamples.Pages.Samples.DbNetGrid;
using DbNetSuiteCoreSamples.Pages.Samples;
using System.Xml;
using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Samples.Pages.Samples;

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

        public Dictionary<Enums.Components, List<SampleApp>> Samples { get; set; } = new Dictionary<Enums.Components, List<SampleApp>>();
        public SampleApp SampleApp { get; set; }
        public SampleApp NextSampleApp { get; set; }
        public SampleApp PreviousSampleApp { get; set; }
        public Enums.Components Component { get; set; }
        public string Title { get; set; }
        public string SourceCode { get; set; }

        public string PageName { get; set; }
        public string FontFamily { get; set; }
        public string FontSize { get; set; }
        public ToolbarButtonStyle ToolbarButtonStyle { get; set; } = ToolbarButtonStyle.Image;
        public ToolbarPosition ToolbarLocation { get; set; } = ToolbarPosition.Top;
        public MultiRowSelectLocation MultiRowSelectLocation { get; set; } = MultiRowSelectLocation.Left;
        public List<string> FontSizes { get; set; } = new List<string>() { "Small", "Medium", "Large" };
        public List<string> FontFamilies { get; set; } = new List<string>() { "Verdana", "Tahoma", "Georgia", "Arial" };

        public List<CultureInfo> Cultures { get; set; } = new List<CultureInfo>();
        public string Culture { get; set; } = null;
        public string CustomerId { get; set; } = null;
        public int? OrderId { get; set; } = null;
        public bool ShowMenu { get; set; } = false;
        public XmlDocument Sitemap { get; set; } = new XmlDocument();

        public void OnGet(
            string fontFamily = "verdana",
            string fontSize = "small",
            string? db = null,
            string? table = null,
            string? view = null,
            ToolbarButtonStyle? toolbarButtonStyle = null,
            ToolbarPosition? toolbarLocation = null,
            MultiRowSelectLocation? multiRowSelectLocation = null,
            string? culture = null,
            string? customerId = null,
            int? orderId = null

          )
        {
            if (this is FontModel)
            {
                FontFamily = fontFamily;
                FontSize = fontSize;
            }

            if (this is LocalizationModel)
            {
                Culture = culture ?? Thread.CurrentThread.CurrentCulture.Name;
                GetCultures();
            }

            if (this is MenuModel)
            {
                PopulateSamples();
                ShowMenu = true;
            }
            else
            {
                SampleNavigation();
            }

            if (toolbarButtonStyle.HasValue)
            {
                ToolbarButtonStyle = toolbarButtonStyle.Value;
            }
            if (toolbarLocation.HasValue)
            {
                ToolbarLocation = toolbarLocation.Value;
            }
            if (multiRowSelectLocation.HasValue)
            {
                MultiRowSelectLocation = multiRowSelectLocation.Value;
            }

            CustomerId = customerId;
            OrderId = orderId;

            try
            {
                GetSourceCode();
            }
            catch (Exception)
            {
                SourceCode = string.Empty;
            }
        }


        protected void SampleNavigation()
        {
            PopulateSamples();
            var segments = HttpContext.Request.Path.ToString().Split("/");
            Component = (Enums.Components)Enum.Parse(typeof(Enums.Components), segments[^2], true);
            PageName = $"{Component}/{segments.Last()}";

            var samples = new List<SampleApp>();

            foreach (var key in Samples.Keys)
            {
                samples.AddRange(Samples[key]);
            }

            SampleApp = samples.FirstOrDefault(s => s.Url.ToLower() == PageName.ToLower());
            SampleApp ??= samples.First();

            Title = $"{SampleApp.Title}";

            int currentIdx = samples.FindIndex(app => app.Url.Equals(SampleApp.Url, StringComparison.Ordinal));

            NextSampleApp = AdjacentApp(samples, currentIdx, true);
            PreviousSampleApp = AdjacentApp(samples, currentIdx, false);
        }

        private SampleApp AdjacentApp(List<SampleApp> samples, int idx, bool next)
        {
            SampleApp app;
            do
            {
                if (next)
                {
                    idx = idx + 1;
                    app = (idx < samples.Count) ? samples[idx] : samples.First();
                }
                else
                {
                    idx = idx - 1;
                    app = (idx >= 0) ? samples[idx] : samples.Last();
                }
            }
            while (app.Menu == false);
            return app;
        }

        private void GetSourceCode()
        {
            var routeName = (HttpContext.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText;
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
                if (collect)
                {
                    if (line.StartsWith("}"))
                    {
                        break;
                    }
                    source.Add(line);
                }
                if (line.StartsWith("@section Control"))
                {
                    collect = true;
                }
            }
            if (source.Any())
            {
                if (source.First() == "{")
                {
                    source.Remove(source.First());
                }
            }
            if (source.Any())
            {
                if (source.Last() == "}")
                {
                    source.Remove(source.Last());
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
        protected void PopulateSamples()
        {
            Samples.Add(Enums.Components.DbNetGrid, DbNetGridSamples());
            Samples.Add(Enums.Components.DbNetEdit, DbNetEditSamples());
            Samples.Add(Enums.Components.DbNetCombo, DbNetComboSamples());
            Samples.Add(Enums.Components.DbNetFile, DbNetFileSamples());
        }
        private List<SampleApp> DbNetGridSamples()
        {
            List<SampleApp> samples = new List<SampleApp>
            {
                new DbNetGridSampleApp("simple", "Simple"),
                new DbNetGridSampleApp("columns", "Specifying Columns"),
                new DbNetGridSampleApp("joiningtables", "Joining Tables"),
                new DbNetGridSampleApp("style", "Column styling"),
                new DbNetGridSampleApp("edit", "Editing The Grid"),
                new DbNetGridSampleApp("linkededit", "Linked Edit"),
                new DbNetGridSampleApp("multirowselect", "Selecting Multiple Rows"),
                new DbNetGridSampleApp("nestedgrid", "Nesting Grids"),
                new DbNetGridSampleApp("linkedgrids", "Linking Grids"),
                new DbNetGridSampleApp("quicksearch", "Quick Search"),
                new DbNetGridSampleApp("columnfilters", "Column Filters"),
                new DbNetGridSampleApp("toolbarPosition", "Positioning/Styling Toolbar"),
                new DbNetGridSampleApp("frozenHeader", "Freezing the headings"),
                new DbNetGridSampleApp("master", "Linking to grids in another page"),
                new DbNetGridSampleApp("detail", "Linked detail grid", false),
                new DbNetGridSampleApp("groupHeader", "Group headings"),
                new DbNetGridSampleApp("comparingCells", "Comparing cell values"),
                new DbNetGridSampleApp("cellTransformation", "Cell value transformation"),
                new DbNetGridSampleApp("binaryData", "Handling binary data"),
                new DbNetGridSampleApp("viewingRecord", "Viewing records"),
                new DbNetGridSampleApp("font", "Changing the font size/family"),
                new DbNetGridSampleApp("totals", "Adding totals"),
                new DbNetGridSampleApp("subtotals", "Adding sub-totals"),
                new DbNetGridSampleApp("groupBy", "Aggregating data"),
                new DbNetGridSampleApp("crosstab", "Cross tabulation"),
                new DbNetGridSampleApp("localisation", "Localisation"),
                new DbNetGridSampleApp("groupByChart", "Chart integration"),
                new DbNetGridSampleApp("MultiSeriesChart", "Multi-series Chart"),
                new DbNetGridSampleApp("Dashboard", "Dashboard"),
                new DbNetGridSampleApp("Css", "Customising the styling"),
                new DbNetGridSampleApp("JsonFile", "Using JSON as a data source"),
                new DbNetGridSampleApp("GenericList", "Using an API as a data source"),
            };
            return samples;
        }

        private List<SampleApp> DbNetComboSamples()
        {
            List<SampleApp> samples = new List<SampleApp>
            {
                new DbNetComboSampleApp("simplecombo", "Simple"),
                new DbNetComboSampleApp("filtered", "Filtered"),
                new DbNetComboSampleApp("linked", "Linked"),
                new DbNetComboSampleApp("multiple", "Size/Multiple"),
                new DbNetComboSampleApp("linkedgrid", "Linked Grid"),
                new DbNetComboSampleApp("datacolumns", "Data Columns"),
                new DbNetComboSampleApp("distinct", "Distinct"),
                new DbNetComboSampleApp("linkededit", "Linked Edit")
           };

            return samples;
        }

        private List<SampleApp> DbNetEditSamples()
        {
            List<SampleApp> samples = new List<SampleApp>
            {
                new DbNetEditSampleApp("simple", "Simple"),
                new DbNetEditSampleApp("columns", "Columns"),
                new DbNetEditSampleApp("layout", "Layout"),
                new DbNetEditSampleApp("browse", "Browse"),
                new DbNetEditSampleApp("fixedfilter", "Pre-filtering the dataset"),
                new DbNetEditSampleApp("toolbarPosition", "Positioning/Styling Toolbar"),
                new DbNetEditSampleApp("quicksearch", "Quick Search"),
                new DbNetEditSampleApp("linkededits", "Linked Forms"),
                new DbNetEditSampleApp("linkedgrid", "Linked Grid"),
                new DbNetEditSampleApp("binarydata", "Uploading files"),
                new DbNetEditSampleApp("dependentlookup", "Dependent lookups"),
                new DbNetEditSampleApp("products", "Conditional configuration"),
                new DbNetEditSampleApp("controltypes", "Control Types"),
                new DbNetEditSampleApp("genericedit", "Editing an API based data source"),

         };

            return samples;
        }

        private List<SampleApp> DbNetFileSamples()
        {
            List<SampleApp> samples = new List<SampleApp>
            {
                new DbNetFileSampleApp("simple", "Basic usage"),
                new DbNetFileSampleApp("columns", "Configuring columns"),
                new DbNetFileSampleApp("treeview", "Using TreeView for navigation")
         };

            return samples;
        }
    
        public HtmlString HelpLink(string url, string text)
        {
            url = $"https://docs.dbnetsuitecore.com/topics/{url.Replace("_", "-")}";
            return new HtmlString($"<a target=\"_blank\" href=\"{url}\">{text}</a>");
        }

        private void GetCultures()
        {
            List<string> cultures = new List<string>() { "ar-QA", "en-GB", "en-US", "es-ES", "fr-FR", "ja-JP", "it-IT" };

            if (cultures.Contains(Culture) == false)
            {
                cultures.Add(Culture);
            }
            Cultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(c => cultures.Contains(c.Name)).OrderBy(c => c.DisplayName).ToList();
        }
    }

    public class SampleApp
    {
        public Enums.Components Component { get; set; }
        public string Page { get; set; }
        public string Title { get; set; }
        public string Url => $"{Component}/{Page}";
        public bool Menu { get; set; } = true;

        public SampleApp(string page, string title, Enums.Components component = Enums.Components.DbNetGrid, bool menu = true)
        {
            Component = component;
            Page = page;
            Title = title;
            Menu = menu;
        }
    }

    public class DbNetGridSampleApp : SampleApp
    {
        public DbNetGridSampleApp(string page, string title, bool menu = true) : base(page, title, Enums.Components.DbNetGrid, menu)
        {
        }
    }

    public class DbNetEditSampleApp : SampleApp
    {
        public DbNetEditSampleApp(string page, string title) : base(page, title, Enums.Components.DbNetEdit)
        {
        }
    }

    public class DbNetComboSampleApp : SampleApp
    {
        public DbNetComboSampleApp(string page, string title) : base(page, title, Enums.Components.DbNetCombo)
        {
        }
    }
    public class DbNetFileSampleApp : SampleApp
    {
        public DbNetFileSampleApp(string page, string title) : base(page, title, Enums.Components.DbNetFile)
        {
        }
    }
}