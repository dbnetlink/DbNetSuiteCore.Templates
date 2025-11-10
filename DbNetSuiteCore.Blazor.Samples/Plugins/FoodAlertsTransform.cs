using DbNetSuiteCore.Models;
using DbNetSuiteCore.Plugins.Interfaces;
using System.Text.Json.Serialization;


namespace DbNetSuiteCore.Samples.Plugins
{
    public class FoodAlertsTransform : IJsonTransformPlugin
    {
        public Meta meta { get; set; } = new Meta();
        public List<Item> items { get; set; } = new List<Item>();

        public object Transform(GridModel gridModel, HttpContext httpContext, IConfiguration configuration)
        {
            return items.Select(i => new
            {
                Id = i.id ?? string.Empty,
                Title = i.title ?? string.Empty,
                Notation = i.notation ?? string.Empty,
                Created = i.created ?? string.Empty,
                Modified = i.modified,
                Type = string.Join(",", (i.type ?? new List<string>()).Select(t => t)),
                ShortTitle = i.shortTitle,
                Status = i.status?.label ?? string.Empty,
                AlertURL = i.alertURL,
                ReportingBusiness = GetReportingBusiness(i.reportingBusiness),
                Problem = i.problem?.FirstOrDefault()?.riskStatement ?? string.Empty,
                ProductDetails = i.productDetails?.FirstOrDefault()?.productName ?? string.Empty,
                Country = string.Join("/",(i.country ?? new List<Country>()).Select(c => c.label.FirstOrDefault() ?? string.Empty))
            });
        }

        private string GetReportingBusiness(ReportingBusiness ? reportingBusiness)
        {
            string name = reportingBusiness?.commonName ?? string.Empty;
            return name.Length < 30 ? name : string.Empty;
        }
    }

    public class Allergen
    {
        [JsonPropertyName("@id")]
        public string id { get; set; } = string.Empty;
        public string label { get; set; } = string.Empty;
    }

    public class Country
    {
        [JsonPropertyName("@id")]
        public string id { get; set; } = string.Empty;
        public List<string> label { get; set; } = new List<string>();
    }

    public class Item
    {
        [JsonPropertyName("@id")]
        public string id { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string notation { get; set; } = string.Empty;
        public string created { get; set; } = string.Empty;
        public DateTime modified { get; set; }
        public List<string> type { get; set; } = new List<string>();
        public string shortTitle { get; set; } = string.Empty;
        public Status? status { get; set; } = null;
        public string alertURL { get; set; } = string.Empty;
        public ReportingBusiness? reportingBusiness { get; set; } = null;
        public List<Problem> problem { get; set; } = new List<Problem>();
        public List<ProductDetail> productDetails { get; set; } = new List<ProductDetail>();
        public List<Country> country { get; set; } = new List<Country>();
    }

    public class Meta
    {
        [JsonPropertyName("@id")]
        public string id { get; set; } = string.Empty;
        public string publisher { get; set; } = string.Empty;
        public string license { get; set; } = string.Empty;
        public string licenseName { get; set; } = string.Empty;
        public string comment { get; set; } = string.Empty;
        public string version { get; set; } = string.Empty;
        public List<string> hasFormat { get; set; } = new List<string>();
        public int limit { get; set; }
    }

    public class Problem
    {
        [JsonPropertyName("@id")]
        public string id { get; set; } = string.Empty;
        public string riskStatement { get; set; } = string.Empty;
        public List<Allergen> allergen { get; set; } = new List<Allergen>();
    }

    public class ProductDetail
    {
        [JsonPropertyName("@id")]
        public string id { get; set; } = string.Empty;
        public string productName { get; set; } = string.Empty;
    }

    public class ReportingBusiness
    {
        public string commonName { get; set; } = string.Empty;
    }


    public class Status
    {
        [JsonPropertyName("@id")]
        public string id { get; set; } = string.Empty;
        public string label { get; set; } = string.Empty;
    }
}
