using DbNetSuiteCore.Constants;
using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Models;
using DbNetSuiteCoreSamples.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DbNetSuiteCore.Samples.Controllers
{
    public class GridController : SampleController
    {
        public GridController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
        }

        public IActionResult Basic()
        {
            var sampleModel = GetSampleModel();
            sampleModel.GridModel = new GridModel(DataSourceType.SQLite, "northwind", "Customers");
            return View(sampleModel);
        }
        public IActionResult Columns()
        {
            GridModel customerGrid = new GridModel(DataSourceType.SQLite, "northwind", "Customers");
            customerGrid.Columns = new List<GridColumn>
            {
                new GridColumn("CompanyName", "Name"),
                new GridColumn("coalesce(Address,'') || ', ' || coalesce(City,'')", "Address") { DataType = typeof(string)},
                new GridColumn("coalesce(Phone,'') || ' / ' || coalesce(Fax,'')", "Phone/Fax") { DataType = typeof(string)}
            };
            var sampleModel = GetSampleModel();
            sampleModel.GridModel = customerGrid;
            return View(sampleModel);
        }
        public IActionResult ComparingCells()
        {
            GridModel productGrid = new GridModel(DataSourceType.SQLite, "Northwind", "Products")
            {
                ViewDialog = new ViewDialog() { LayoutColumns = 2 }
            };
            productGrid.Columns = new List<GridColumn>()
            {
                new GridColumn("ProductID") { PrimaryKey = true },
                new GridColumn("ProductName"),
                new GridColumn("SupplierID","Supplier") { Lookup = new Lookup("Suppliers", "SupplierId", "CompanyName"), Filter = FilterType.Default },
                new GridColumn("CategoryID","Category") { Lookup = new Lookup("Categories", "CategoryID", "CategoryName"), Filter = FilterType.Default },
                new GridColumn("QuantityPerUnit", "Qty."),
                new GridColumn("UnitPrice","Price") { Format = "c"},
                new GridColumn("UnitsInStock", "Stock"),
                new GridColumn("UnitsOnOrder","On Order"),
                new GridColumn("ReorderLevel"),
                new GridColumn("Discontinued") { DataType = typeof(Boolean)}
            };
            var sampleModel = GetSampleModel();
            sampleModel.GridModel = productGrid;
            return View(sampleModel);
        }
        public IActionResult CrossTab()
        {
            var invoiceColumns = new List<GridColumn>
            {
                new GridColumn("ProductName", "Product")
            };

            for (var i = 1; i <= 12; i++)
            {
                var monthName = new DateTime(2024, i, 1).ToString("MMMM");
                var column = new GridColumn($"(case when strftime ('%d', OrderDate) = '{i.ToString("00")}' then Quantity else 0 end) as {monthName.ToLower()}", monthName) { Aggregate = AggregateType.Sum, DataType = typeof(Int32) };
                invoiceColumns.Add(column);
            }

            var invoiceGrid = new GridModel(DataSourceType.SQLite, "Northwind", "invoices");
            invoiceGrid.Columns = invoiceColumns;
            var sampleModel = GetSampleModel();
            sampleModel.GridModel = invoiceGrid;
            return View(sampleModel);
        }
        public IActionResult Dashboard()
        {
            var leagues = new Dictionary<string, string>
            {
                { "E0", "Premier League" },{ "E1", "EFL Championship" },{ "E2", "EFL League One" },{ "E3", "EFL League Two" },{ "EC", "National League" }
            };
            var sampleModel = GetSampleModel();
            foreach (string league in leagues.Keys)
            {
                var resultsGrid = new GridModel(DataSourceType.SQLite, "dbnetsuitecore", "matches") { FixedFilter = $"div = '{league}' and date = (select max(date) from matches where div = '{league}')" };
                resultsGrid.Columns = new List<GridColumn>()
                {
                    new GridColumn("HomeTeam"),
                    new GridColumn("FTHG") { Style = "font-weight:bold"},
                    new GridColumn("FTAG") { Style = "font-weight:bold"},
                    new GridColumn("AwayTeam"),
                    new GridColumn("FTR")
                };
                resultsGrid.ToolbarPosition = ToolbarPosition.Hidden;
                resultsGrid.Caption = leagues[league];
                resultsGrid.HeadingMode = HeadingMode.Hidden;

                sampleModel.GridModels.Add(resultsGrid);
            }
            return View(sampleModel);
        }
        public IActionResult LinkedGrids()
        {
            var invoiceLineGridModel = new GridModel("Invoice_items");
            invoiceLineGridModel.Columns = new List<GridColumn>
            {
                new GridColumn("invoicelineid", "Invoice Line ID"),
                new GridColumn("invoiceid", "Invoice ID") {ForeignKey = true, DataOnly = true },
                new GridColumn("trackid", "Track ID") {Lookup = new Lookup("tracks","trackid","name")},
                new GridColumn("unitprice", "Price") { Format = "c"},
                new GridColumn("quantity", "Qty"),
            };
            invoiceLineGridModel.Caption = "Invoice Lines";
            invoiceLineGridModel.PageSize = 10;
            invoiceLineGridModel.ToolbarPosition = ToolbarPosition.Hidden;

            var invoiceGridModel = new GridModel("Invoices");
            invoiceGridModel.Columns = new List<GridColumn>
            {
                new GridColumn("invoiceid", "Invoice ID") {PrimaryKey = true },
                new GridColumn("customerid", "Customer ID") {ForeignKey = true, DataOnly = true },
                new GridColumn("invoicedate", "Date") { DataType = typeof(DateTime) },
                new GridColumn("billingaddress", "Address"),
                new GridColumn("billingcity", "City"),
                new GridColumn("billingstate", "State"),
                new GridColumn("billingcountry", "Country"),
                new GridColumn("billingpostalcode", "Post Code")
            };
            invoiceGridModel.LinkedControl = invoiceLineGridModel;
            invoiceGridModel.Caption = "Invoices";
            invoiceGridModel.PageSize = 10;
            invoiceGridModel.ToolbarPosition = ToolbarPosition.Hidden;

            var customerGridModel = new GridModel(DataSourceType.SQLite, "Chinook", "Customers");
            customerGridModel.Columns = new List<GridColumn>
            {
                new GridColumn("customerid", "CustomerID") {PrimaryKey = true },
                new GridColumn("firstname", "Forename"),
                new GridColumn("lastname", "Surname"),
                new GridColumn("email", "Email Address") {Format = FormatType.Email },
                new GridColumn("address", "Address"),
                new GridColumn("city", "City") { InitialSortOrder = SortOrder.Desc},
                new GridColumn("postalcode", "Post Code"),
            };
            customerGridModel.LinkedControl = invoiceGridModel;
            customerGridModel.PageSize = 10;

            var sampleModel = GetSampleModel();
            sampleModel.GridModels.Add(customerGridModel);
            sampleModel.GridModels.Add(invoiceGridModel);
            sampleModel.GridModels.Add(invoiceLineGridModel);

            return View(sampleModel);
        }
        public IActionResult NestedGrid()
        {
            var invoiceLineGridModel = new GridModel("Invoice_items");
            invoiceLineGridModel.Columns = new List<GridColumn>
            {
                new GridColumn("invoicelineid", "Invoice Line ID"),
                new GridColumn("invoiceid", "Invoice ID") {ForeignKey = true, DataOnly = true },
                new GridColumn("trackid", "Track") { Lookup = new Lookup("tracks","trackid", "name") },
                new GridColumn("unitprice", "Price") {Format = "c"},
                new GridColumn("quantity", "Qty"),
            };
            invoiceLineGridModel.Caption = "Invoice Lines";
            invoiceLineGridModel.ToolbarPosition = ToolbarPosition.Hidden;

            var invoiceGridModel = new GridModel("Invoices");
            invoiceGridModel.Columns = new List<GridColumn>
            {
                new GridColumn("invoiceid", "Invoice ID") {PrimaryKey = true },
                new GridColumn("customerid", "Customer ID") {ForeignKey = true, DataOnly = true },
                new GridColumn("invoicedate", "Date") { DataType = typeof(DateTime)},
                new GridColumn("billingaddress", "Address"),
                new GridColumn("billingcity", "City"),
                new GridColumn("billingstate", "State"),
                new GridColumn("billingcountry", "Country"),
                new GridColumn("billingpostalcode", "Post Code"),
            };
            invoiceGridModel.NestedGrid = invoiceLineGridModel;
            invoiceGridModel.Caption = "Invoices";
            invoiceLineGridModel.ToolbarPosition = ToolbarPosition.Hidden;

            var customerGridModel = new GridModel(DataSourceType.SQLite, "Chinook", "Customers");
            customerGridModel.Columns = new List<GridColumn>
            {
                new GridColumn("customerid", "CustomerID") {PrimaryKey = true },
                new GridColumn("firstname", "Forename"),
                new GridColumn("lastname", "Surname"),
                new GridColumn("email", "Email Address") {Format = FormatType.Email },
                new GridColumn("address", "Address"),
                new GridColumn("city", "City") { InitialSortOrder = SortOrder.Desc},
                new GridColumn("postalcode", "Post Code"),
            };

            customerGridModel.NestedGrid = invoiceGridModel;

            var sampleModel = GetSampleModel();
            sampleModel.GridModel = customerGridModel;
            return View(sampleModel);
        }
        public IActionResult Orders()
        {
            var ordersGrid = new GridModel(DataSourceType.SQLite, "northwind", "Orders");
            ordersGrid.Columns = new List<GridColumn>()
            {
                new GridColumn("OrderID") {Filter = FilterType.Default },
                new GridColumn("CustomerID", "Customer") {Filter = FilterType.Default, Lookup = new Lookup("Customers", "CustomerID", "CompanyName")},
                new GridColumn("EmployeeID", "Employee") {Filter = FilterType.Default, Lookup = new Lookup("Employees", "EmployeeID", "LastName || ', ' || FirstName")},
                new GridColumn("OrderDate", "Ordered") {Filter = FilterType.Default, InitialSortOrder = SortOrder.Desc},
                new GridColumn("RequiredDate", "Required") {Filter = FilterType.Default},
                new GridColumn("ShippedDate", "Shipped"),
                new GridColumn("ShipVia") {Filter = FilterType.Default, Lookup = new Lookup("Shippers", "ShipperID", "CompanyName")},
                new GridColumn("Freight") {Format = "c"},
                new GridColumn("ShipName"),
                new GridColumn("ShipAddress"),
                new GridColumn("ShipCity") {Filter = FilterType.Distinct},
                new GridColumn("ShipRegion") {Filter = FilterType.Distinct},
                new GridColumn("ShipPostalCode"),
                new GridColumn("ShipCountry") {Filter = FilterType.Distinct}
            };
            var sampleModel = GetSampleModel();
            sampleModel.GridModel = ordersGrid;
            return View(sampleModel);
        }

        public IActionResult ProductsEdit()
        {
            var productsGrid = new GridModel(DataSourceType.SQLite, "Northwind", "Products") { ViewDialog = new ViewDialog() { LayoutColumns = 2 } };
            productsGrid.Columns = new List<GridColumn>() {
                new GridColumn("ProductID") { PrimaryKey = true },
                new GridColumn("ProductName") { FormColumn = new FormColumn()},
                new GridColumn("SupplierID","Supplier") { Lookup = new Lookup("Suppliers", "SupplierId", "CompanyName"), FormColumn = new FormColumn() },
                new GridColumn("CategoryID","Category") { Lookup = new Lookup("Categories", "CategoryID", "CategoryName"), FormColumn = new FormColumn() },
                new GridColumn("QuantityPerUnit", "Qty.") { FormColumn = new FormColumn()},
                new GridColumn("UnitPrice","Price") { Format = "c",FormColumn = new FormColumn()},
                new GridColumn("UnitsInStock", "Stock") { FormColumn = new FormColumn()},
                new GridColumn("UnitsOnOrder","On Order") { FormColumn = new FormColumn()},
                new GridColumn("ReorderLevel") { FormColumn = new FormColumn()},
                new GridColumn("Discontinued") { DataType = typeof(Boolean),FormColumn = new FormColumn()}
            };
            var sampleModel = GetSampleModel();
            sampleModel.GridModel = productsGrid;
            return View(sampleModel);
        }
    }
}

