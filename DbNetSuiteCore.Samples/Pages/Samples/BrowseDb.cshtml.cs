using Microsoft.AspNetCore.Mvc.RazorPages;
using DbNetSuiteCore.Utilities;
using DbNetSuiteCore.Enums;

namespace DbNetSuiteCore.Samples.Pages.Samples
{
    public class BrowseDbModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public string Db { get; set; }
        public string Table { get; set; }
        public string View { get; set; }
        public string? ErrorMessage { get; set; }
        public string? FromPart => QualifiedObjectName();
        public bool? IsTable => string.IsNullOrEmpty(View);
        public DatabaseType DatabaseType { get; set; }
        public List<DbObject> Tables { get; set; } = new List<DbObject>();
        public List<DbObject> Views { get; set; } = new List<DbObject>();
        public Dictionary<string, DatabaseType> Connections { get; set; } = new Dictionary<string, DatabaseType>();
        public BrowseDbModel(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public void OnGet(string? db = null, string? table = null, string? view = null, DatabaseType? databaseType = null)
        {
            Table = table;
            View = view;
            BrowseDbPopulate(db,databaseType);
        }

        public void BrowseDbPopulate(string? db = null, DatabaseType? databaseType = null)
        {
            var connectonStrings = _configuration.GetSection("ConnectionStrings").GetChildren();
            connectonStrings = connectonStrings.AsEnumerable().ToList();

            var connectionAlias = db ?? connectonStrings.AsEnumerable().First().Key;
            var connectionString = _configuration.GetConnectionString(connectionAlias);

            Connections = connectonStrings.AsEnumerable().Select(c => c.Key).ToDictionary(c => c, c => DbNetDataCore.DeriveDatabaseType(_configuration.GetConnectionString(c)));
            Db = connectionAlias;
            DatabaseType = databaseType.HasValue ? databaseType.Value : Connections[connectionAlias];

            try
            {
                using (var connection = new DbNetDataCore(connectionAlias, _webHostEnvironment, _configuration, DatabaseType))
                {
                    connection.Open();
                    Tables = connection.InformationSchema(DbNetDataCore.MetaDataType.Tables);
                    Views = connection.InformationSchema(DbNetDataCore.MetaDataType.Views);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unable to connect to database => {ex.Message}";
                return;
            }

            if (string.IsNullOrEmpty(Table) && string.IsNullOrEmpty(View))
            {
                Table = Tables.First().QualifiedTableName;
            }
        }
        private string QualifiedObjectName()
        {
            string fromPart = string.IsNullOrEmpty(View) ? Table : View;
            if (fromPart.Contains(" "))
            {
                if (fromPart.StartsWith("[") == false)
                {
                    fromPart = $"[{string.Join("].[", fromPart.Split("."))}]";
                }
            }
            return fromPart;
        }
    }
}
