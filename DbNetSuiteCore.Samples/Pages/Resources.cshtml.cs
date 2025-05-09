using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Extensions;
using DbNetSuiteCore.Helpers;
using DbNetSuiteCore.Models;
using DbNetSuiteCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace DbNetSuiteCoreSamples.Pages;

public class ResourceModel : IndexModel
{
    public List<SelectListItem> Cultures { get; set; } = new List<SelectListItem>();

    [BindProperty]
    public string Culture { get; set; } = string.Empty;
    public string CultureText => Cultures.FirstOrDefault(c => c.Value == Culture)?.Text ?? string.Empty;

    private IConfiguration configuration;
    private IWebHostEnvironment? env;
    public ResourceModel(IConfiguration configuration, IWebHostEnvironment? env = null)
    {
        this.configuration = configuration;
        this.env = env;
    }
    public void OnGet()
    {
        LoadCultures();
    }

    public void OnPost()
    {
        LoadCultures();
        DataTable resourcesTable = GetResourcesTable();
        if (resourcesTable.Rows.Count > 0)
        {
            UpdateResources();
        }
    }

    private DataTable GetResourcesTable()
    {
        using (var connection = DbHelper.GetConnection("dbnetsuitecore", DataSourceType.SQLite, configuration, env))
        {
            connection.Open();
            QueryCommandConfig query = new QueryCommandConfig() { Sql = "select * from resources where culturecode = @culturecode", Params = new Dictionary<string, object>() { { "culturecode", Culture } } };

            return DbHelper.RunQuery(query, connection);
        }
    }


    public void OnPostUpdateResources()
    {
        LoadCultures();
        UpdateResources();
    }

    private void UpdateResources()
    {
        var resources = ResourceHelper.GetAllResourceStrings();
        var cultureResources = ResourceHelper.GetAllResourceStrings(Culture);
        using (var connection = DbHelper.GetConnection("dbnetsuitecore", DataSourceType.SQLite, configuration, env))
        {
            connection.Open();

            DataTable resourcesTable = GetResourcesTable();

            foreach (string key in resources.Keys)
            {
                var match = resourcesTable.Rows.Cast<DataRow>().FirstOrDefault(x => x.Field<string>("ResourceKey") == key);

                if (match == null)
                {
                    CommandConfig insert = new CommandConfig();
                    insert.Sql = $"insert into Resources (CultureCode, ResourceKey, EnglishText) values (@CultureCode, @ResourceKey, @EnglishText)";
                    insert.Params["@CultureCode"] = Culture;
                    insert.Params["@ResourceKey"] = key;
                    insert.Params["@EnglishText"] = resources[key];

                    if (resources[key] != cultureResources[key])
                    {
                        insert.Params["@CultureText"] = cultureResources[key];
                    }

                    using (IDbCommand command = DbHelper.ConfigureCommand(insert, connection, CommandType.Text))
                    {
                        ((DbCommand)command).ExecuteNonQuery();
                    }
                }
            }

        }
    }


    public void LoadCultures()
    {
        Cultures = CultureInfo.GetCultures(CultureTypes.NeutralCultures).OrderBy(c => c.DisplayName).Select(c => new SelectListItem(c.DisplayName, c.Name)).ToList();
        if (string.IsNullOrEmpty(Culture))
        {
            Culture = Cultures.FirstOrDefault()?.Value ?? string.Empty;
        }
    }

    public FileResult OnPostDownloadResx()
    {
        DataTable resourcesTable = GetResourcesTable();

        XElement root = new XElement("root");
        XNamespace xml = "http://www.w3.org/XML/1998/namespace";

        foreach (DataRow dataRow in resourcesTable.Rows.Cast<DataRow>())
        {
            var spaceAttr = new XAttribute(xml + "space", "preserve");
            XElement dataElement = new XElement("data", new XAttribute("name", dataRow["ResourceKey"]), spaceAttr,
            new XElement("value", dataRow["CultureText"] ?? String.Empty));
            root.Add(dataElement);
        }

        return File(Encoding.UTF8.GetBytes(root.ToString()), "application/resx", $"strings.{Culture}.resx");
    }
}