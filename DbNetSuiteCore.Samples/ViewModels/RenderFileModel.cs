using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace DbNetSuiteCoreSamples.ViewModels
{
    public class RenderFileModel : SampleModel
    {
        [BindProperty]
        public string FileName { get; set; }
        public string ConnectionString => $"Data Source=~{FileName};Cache=Shared;";
        public DataSourceType DataSourceType => GetDataSourceType();
        public List<SelectListItem> Tables { get; set; } = new List<SelectListItem>();

        [BindProperty]
        public string TableName { get; set; } = string.Empty;
        public RenderFileModel(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base (configuration, webHostEnvironment)
        {
        }


        public async override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod != null)
            {
                if (context.HandlerMethod.MethodInfo.Name == "OnGet")
                {
                    FileName = Request.Query["fileName"];
                   
                }
            }
            GetSourceCode();
            if (DataSourceType == DataSourceType.SQLite)
            {
                Tables = DbHelper.GetTables(ConnectionString, DataSourceType, _configuration, _webHostEnvironment).Select(t => new SelectListItem(t,t)).ToList();
                if (string.IsNullOrEmpty(TableName))
                {
                    TableName = Tables.First().Value;
                }
            }
            // Triggers the OnGet, OnPost etc on the child / inherited class
            await base.OnPageHandlerExecutionAsync(context, next);
        }

        public DataSourceType GetDataSourceType() {
            var dataSourceType = DataSourceType.Excel;
            var extension = FileName.Split(".").Last().ToLower();
            switch (extension)
            {
                case "db":
                    dataSourceType = DataSourceType.SQLite;
                    break;
                case "json":
                    dataSourceType = DataSourceType.JSON;
                    break;
                case "xlsx":
                case "xls":
                case "csv":
                    dataSourceType = DataSourceType.Excel;
                    break;
            }
            return dataSourceType;
        }

    }
}
