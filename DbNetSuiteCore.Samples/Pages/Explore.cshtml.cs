using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Helpers;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;

namespace DbNetSuiteCoreSamples.Pages;

public class ExploreModel : PageModel
{
    public List<DataSourceType> DataSourceTypes => new List<DataSourceType> { DataSourceType.SQLite, DataSourceType.MSSQL, DataSourceType.MySql, DataSourceType.PostgreSql, DataSourceType.MongoDB };
    public List<SelectListItem> Tables { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Databases { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> Connections { get; set; } = new List<SelectListItem>();
    public List<SelectListItem> DataSourceTypeItems => DataSourceTypes.Select(x => new SelectListItem(x.ToString(), x.ToString())).ToList();

    public DataSourceType DataSourceType => (DataSourceType)Enum.Parse(typeof(DataSourceType), DataSourceTypeName);

    [BindProperty]
    public string DataSourceTypeName { get; set; } = string.Empty;
    [BindProperty]
    public string TableName { get; set; } = string.Empty;
    [BindProperty]
    public string DatabaseName { get; set; } = string.Empty;
    [BindProperty]
    public string ConnectionAlias { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;

    private IConfiguration configuration;
    private IWebHostEnvironment? env;
    public ExploreModel(IConfiguration configuration, IWebHostEnvironment? env = null)
    {
        this.configuration = configuration;
        this.env = env;
    }
    public void OnGet()
    {
        LoadConnections();
        LoadDatabases();
        LoadTables();
    }

    public void OnPost()
    {
        LoadConnections();
        LoadDatabases();
        LoadTables();
    }

    public void LoadConnections()
    {
        if (string.IsNullOrEmpty(DataSourceTypeName))
        {
            DataSourceTypeName = DataSourceTypes.First().ToString();
        }

        Connections = DbHelper.GetConnections(configuration).Select(c => new SelectListItem(c.Key, c.Key)).ToList();
        if (string.IsNullOrEmpty(ConnectionAlias))
        {
            ConnectionAlias = Connections.FirstOrDefault()?.Value ?? string.Empty;
        }
    }

    public void LoadDatabases()
    {
        if (string.IsNullOrEmpty(ConnectionAlias))
        {
            return;
        }

        if (DataSourceType == DataSourceType.MongoDB)
        {
            try
            {
                Databases = DbHelper.GetDatabases(ConnectionAlias, configuration).Select(c => new SelectListItem(c, c)).ToList();
                if (string.IsNullOrEmpty(DatabaseName) || Databases.Select(d => d.Value).ToList().Contains(DatabaseName) == false)
                {
                    DatabaseName = Databases.FirstOrDefault()?.Value ?? string.Empty;
                }
            }
            catch (Exception ex)
            {
                TableName = string.Empty;
                ErrorMessage = $"Unable to connect to <b>{DataSourceType}</b> data source. Please check your selected connection.";
            }
        }
    }

    public void LoadTables()
    {
        if (string.IsNullOrEmpty(ConnectionAlias))
        {
            return;
        }

        try
        {
            if (DataSourceType == DataSourceType.MongoDB)
            {
                if (string.IsNullOrEmpty(DatabaseName))
                {
                    return;
                }
                Tables = DbHelper.GetTables(ConnectionAlias, configuration, DatabaseName).Select(c => new SelectListItem(c, c)).ToList();
            }
            else
            {
                Tables = DbHelper.GetTables(ConnectionAlias, DataSourceType, configuration, env).Select(c => new SelectListItem(c, c)).ToList();
            }

            if (string.IsNullOrEmpty(TableName) || Tables.Select(d => d.Value).ToList().Contains(TableName) == false)
            {
                TableName = Tables.FirstOrDefault()?.Value ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            Tables = new List<SelectListItem>();
        }

        if (Tables.Any() == false)
        {
            ErrorMessage = $"Unable to connect to <b>{DataSourceType}</b> data source. Please check your selected connection.";
            TableName = string.Empty;
        }
    }
}