using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DbNetSuiteCore.Samples.Controllers
{
    public class SelectController : SampleController
    {
        public SelectController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
        }

        public IActionResult Basic()
        {
            SelectModel customerSelect = new SelectModel(DataSourceType.SQLite, "northwind", "Customers")
            {
                Caption = "Customer",
                EmptyOption = "Please select a customer",
                Columns = new List<SelectColumn>()
                {
                    new SelectColumn("CustomerID"),
                    new SelectColumn("CompanyName")
                }
            };

            var sampleModel = GetSampleModel();
            sampleModel.SelectModel = customerSelect;
            return View(sampleModel);
        }
        public IActionResult DataAttributes()
        {
            var employeeSelect = new SelectModel(DataSourceType.SQLite, "Northwind", "Employees") { Caption = "Employees", Layout = LayoutType.Row };
            employeeSelect.Columns = new List<SelectColumn>()
            {
                new SelectColumn("EmployeeID"),
                new SelectColumn("coalesce(LastName,'') || ', ' || coalesce(FirstName,'') as full_name"),
                new SelectColumn("LastName"),
                new SelectColumn("FirstName"),
                new SelectColumn("Title"),
                new SelectColumn("TitleOfCourtesy"),
                new SelectColumn("BirthDate"),
                new SelectColumn("HireDate"),
                new SelectColumn("Address"),
                new SelectColumn("City"),
                new SelectColumn("Region"),
                new SelectColumn("PostalCode"),
                new SelectColumn("Country"),
                new SelectColumn("HomePhone"),
                new SelectColumn("Extension"),
                new SelectColumn("Photo"),
                new SelectColumn("Notes"),
                new SelectColumn("ReportsTo") {Lookup = new Lookup("employees","EmployeeID","LastName || ',' || FirstName")}
            };
            var sampleModel = GetSampleModel();
            sampleModel.SelectModel = employeeSelect;
            return View(sampleModel);
        }
        public IActionResult Grouped()
        {
            var productSelect = new SelectModel(DataSourceType.SQLite, "Northwind", "Products join Categories on Products.CategoryID = Categories.CategoryID") { Search = true };
            productSelect.Columns = new List<SelectColumn>()
            {
                new SelectColumn("ProductID"),
                new SelectColumn("ProductName"),
                new SelectColumn("CategoryName") {OptionGroup = true}
            };
            var sampleModel = GetSampleModel();
            sampleModel.SelectModel = productSelect;
            return View(sampleModel);
        }
        public IActionResult Layout()
        {
            SelectModel customerSelect = new SelectModel(DataSourceType.SQLite, "northwind", "Customers")
            {
                Caption = "Customer",
                Search = true,
                Layout = LayoutType.Row,
                EmptyOption = "Please select a customer",
                Columns = new List<SelectColumn>()
                {
                    new SelectColumn("CustomerID"),
                    new SelectColumn("CompanyName")
                }
            };
            var sampleModel = GetSampleModel();
            sampleModel.SelectModel = customerSelect;
            return View(sampleModel);
        }
        public IActionResult Linked()
        {
            var citySelect = new SelectModel("Cities") { Caption = "City", Search = true, Layout = LayoutType.Row, EmptyOption = "Select a city" };
            citySelect.Columns = new List<SelectColumn>
            {
                new SelectColumn("cityid") {PrimaryKey = true },
                new SelectColumn("cityname"),
                new SelectColumn("stateid") { ForeignKey = true },
                new SelectColumn("latitude"),
                new SelectColumn("longitude")
            };
            citySelect.ClientEvents[SelectClientEvent.OptionSelected] = "showSelectedCity";

            var stateSelect = new SelectModel("States") { Caption = "State", Search = true, Layout = LayoutType.Row, EmptyOption = "Select a state" };
            stateSelect.Columns = new List<SelectColumn>
            {
                new SelectColumn("stateid") {PrimaryKey = true },
                new SelectColumn("statename"),
                new SelectColumn("countryid") { ForeignKey = true}
            };
            stateSelect.LinkedControl = citySelect;

            var countrySelect = new SelectModel(DataSourceType.SQLite, "DbNetSuiteCore", "Countries") { Caption = "Country", Search = true, Layout = LayoutType.Row, EmptyOption = "Select a country" };
            countrySelect.Columns = new List<SelectColumn>
            {
                new SelectColumn("countryid") {PrimaryKey = true },
                new SelectColumn("countryname")
            };
            countrySelect.LinkedControl = stateSelect;

            var sampleModel = GetSampleModel();
            sampleModel.SelectModels.Add(countrySelect);
            sampleModel.SelectModels.Add(stateSelect);
            sampleModel.SelectModels.Add(citySelect);
            return View(sampleModel);
        }
        public IActionResult LinkedGrid()
        {
            var orderDetailsGrid = new GridModel("[Order Details]");
            orderDetailsGrid.Columns = new List<GridColumn>
            {
                new GridColumn("OrderId", "Invoice Line ID") {ForeignKey = true },
                new GridColumn("ProductID", "Product")  {Lookup = new Lookup("Products","ProductID","ProductName")},
                new GridColumn("UNitPrice", "Price") { Format = "c"},
                new GridColumn("QUantity", "Quantity"),
                new GridColumn("UnitPrice*Quantity", "Cost") { Format = "c", DataType = typeof(Decimal)},
                new GridColumn("Discount", "Discount") { Format = "p"},
                new GridColumn( "UnitPrice*Quantity* (1-Discount)", "Net Cost") { Format = "c", DataType = typeof(Decimal)},
            };

            orderDetailsGrid.Caption = "Order Details";
            orderDetailsGrid.PageSize = 5;

            var ordersGrid = new GridModel("Orders");
            ordersGrid.Columns = new List<GridColumn>
            {
                new GridColumn("OrderId", "Order ID") {PrimaryKey = true },
                new GridColumn("customerid", "Customer ID") {ForeignKey = true },
                new GridColumn("employeeid", "Employee") { Lookup = new Lookup("Employees","EmployeeID","coalesce(LastName,'') || ', ' || coalesce(FirstName,'')" ) },
                new GridColumn("OrderDate", "Ordered"),
                new GridColumn("RequiredDate", "Required"),
                new GridColumn("ShippedDate", "Shipped"),
                new GridColumn("Freight", "Cost") { Format = "c"}
            };
            ordersGrid.LinkedControl = orderDetailsGrid;
            ordersGrid.Caption = "Orders";
            ordersGrid.PageSize = 5;

            var customerSelect = new SelectModel(DataSourceType.SQLite, "Northwind", "customers") { Caption = "Customers", EmptyOption = "Please select a customer", Search = true };

            customerSelect.Columns = new List<SelectColumn>
            {
                new SelectColumn("CustomerID"),
                new SelectColumn("coalesce(CompanyName,'') || ', ' || coalesce(Address,'') || ', ' || coalesce(City,'')") {DataType = typeof(String)}
            };
            customerSelect.LinkedControl = ordersGrid;

            var sampleModel = GetSampleModel();
            sampleModel.SelectModel = customerSelect;
            sampleModel.GridModels.Add(ordersGrid);
            sampleModel.GridModels.Add(orderDetailsGrid);
            return View(sampleModel);
        }
     
        public IActionResult Multiple()
        {
            SelectModel customerSelect = new SelectModel(DataSourceType.SQLite, "northwind", "Customers")
            {
                Caption = "Customer",
                Search = true,
                Size = 20,
                RowSelection = RowSelection.Multiple,
                Columns = new List<SelectColumn>()
                {
                    new SelectColumn("CustomerID"),
                    new SelectColumn("CompanyName")
                }
            };

            var sampleModel = GetSampleModel();
            sampleModel.SelectModel = customerSelect;
            return View(sampleModel);
        }
       
        public IActionResult Searchable()
        {
            SelectModel customerSelect = new SelectModel(DataSourceType.SQLite, "northwind", "Customers")
            {
                Caption = "Customer",
                Search = true,
                EmptyOption = "Please select a customer",
                Columns = new List<SelectColumn>()
                {
                    new SelectColumn("CustomerID"),
                    new SelectColumn("CompanyName")
                }
            };
            var sampleModel = GetSampleModel();
            sampleModel.SelectModel = customerSelect;
            return View(sampleModel);
        }
    }
}