using Dapper;
using DbNetSuiteCoreSamples.Models;
using DbNetSuiteCore.Components;
using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Models.DbNetEdit;
using DbNetSuiteCore.Web.UI.Models;
using DbNetSuiteCoreSamples.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbNetSuiteCoreSamples.Pages.Samples.DbNetEdit
{
    public class GenericEditModel : SampleModel
    {
        private string _connectionString => _configuration?.GetConnectionString("northwind")?.
                  Replace("~", _webHostEnvironment.WebRootPath) ?? string.Empty;

        public IEnumerable<Product> Products { get; set; }
        public Dictionary<int, string> SupplierLookup { get; set; }
        public Dictionary<int, string> CategoryLookup { get; set; }
        public DbNetEditCore ProductsEdit { get; set; }
        public GenericEditModel(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }

        public async override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (context.HandlerMethod.MethodInfo.Name == nameof(OnGet))
            {
                using (var conn = new SqliteConnection(_connectionString))
                {
                    SqlMapper.AddTypeHandler(new BooleanTypeHandler());
                    SqlMapper.AddTypeHandler(new DecimalTypeHandler());
                    Products = await conn.QueryAsync<Product>("select * from products;");

                    SupplierLookup = await GetLookup(conn, "select SupplierID as Key, CompanyName as Value from Suppliers order by 2");
                    CategoryLookup = await GetLookup(conn, "select CategoryID as Key, CategoryName as Value from Categories order by 2");
                }

            }
            await base.OnPageHandlerExecutionAsync(context, next);
        }

        public async Task<IActionResult> OnPost([FromBody] JsonUpdateRequest jsonUpdateRequest)
        {
            string message = "";
            bool success = true;

            using (var conn = new SqliteConnection(_connectionString))
            {
                string sql = string.Empty;
                var paramValues = new DynamicParameters();
                switch (jsonUpdateRequest.EditMode)
                {
                    case EditMode.Update:
                        string set = string.Join(",", jsonUpdateRequest.Changes.Keys.Select(k => $"{k} = @{k}"));
                        paramValues.Add($"@{jsonUpdateRequest.PrimaryKeyName}", jsonUpdateRequest.PrimaryKeyValue);
                        foreach (string key in jsonUpdateRequest.Changes.Keys)
                        {
                            paramValues.Add($"@{key}", jsonUpdateRequest.Changes[key]);
                        }
                        sql = $"update products set {set} where {jsonUpdateRequest.PrimaryKeyName} = @{jsonUpdateRequest.PrimaryKeyName}";
                        message = "Product updated";
                        break;
                    case EditMode.Insert:
                        string columns = string.Join(",", jsonUpdateRequest.Changes.Keys.Select(k => $"{k}"));
                        string values = string.Join(",", jsonUpdateRequest.Changes.Keys.Select(k => $"@{k}"));

                        foreach (string key in jsonUpdateRequest.Changes.Keys)
                        {
                            paramValues.Add($"@{key}", jsonUpdateRequest.Changes[key]);
                        }
                        sql = $"insert into products({columns}) values({values})";
                        message = "Product added";
                        break;
                    case EditMode.Delete:
                        paramValues.Add($"@{jsonUpdateRequest.PrimaryKeyName}", jsonUpdateRequest.PrimaryKeyValue);
                        sql = $"delete from products where {jsonUpdateRequest.PrimaryKeyName} = @{jsonUpdateRequest.PrimaryKeyName}";
                        message = "Product deleted";
                        break;
                }

                try
                {
                    conn.Execute(sql, paramValues);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    success = false;
                }

                var products = await conn.QueryAsync<Product>("select * from products;");
                return new JsonResult(new JsonUpdateResponse { Success = success, Message = message, DataSet = products });
            }

        }

        private async Task<Dictionary<int, string>> GetLookup(SqliteConnection conn, string sql)
        {
            Dictionary<int, string> lookup = (await conn.QueryAsync<KeyValuePair<int, string>>(sql)).ToDictionary(row => row.Key, row => row.Value);
            return lookup;
        }
    }
}

