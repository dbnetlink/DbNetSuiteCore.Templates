using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DbNetSuiteCore.Samples.Controllers
{
    public class TreeController : SampleController
    {
        public TreeController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
        }

        public IActionResult CategoryProducts()
        {
            var productForm = new FormModel(DataSourceType.SQLite, "Northwind2", "Products") { LayoutColumns = 3 };
            productForm.Columns = new List<FormColumn>() {
        new FormColumn("ProductID","ID") { DataOnly = true},
        new FormColumn("ProductName", "Name") {Required = true, MinLength = 3, MaxLength = 20 },
        new FormColumn("SupplierID","Supplier") { Lookup = new Lookup("Suppliers", "SupplierId", "CompanyName"),Required = true },
        new FormColumn("CategoryID","Category") { Lookup = new Lookup("Categories", "CategoryID", "CategoryName"),Required = true, HelpText = "Select a category for the product" },
        new FormColumn("QuantityPerUnit", "Qty"){ InitialValue = 0},
        new FormColumn("UnitPrice", "Price") {Required = true,InitialValue = 0, MinValue = 0.00, MaxValue = 100.00, Format = "c" },
        new FormColumn("UnitsInStock", "Stock") {Required = true,InitialValue = 0, MinValue = 0, MaxValue = 1000 },
        new FormColumn("UnitsOnOrder", "On Order") {Required = true,InitialValue = 0, MinValue = 0, MaxValue = 1000 },
        new FormColumn("ReorderLevel", "Re-order Level") {Required = true,InitialValue = 0, MaxValue = 50, LookupRange = Enumerable.Range(0,50) },
        new FormColumn("Discontinued") {DataType = typeof(bool), InitialValue = true, HelpText = "Discontinued products must have a re-order level of 0"}
    };

            productForm.FixedFilter = "ProductID = @ProductID";
            productForm.FixedFilterParameters.Add(new DbParameter("ProductID", -1));
            productForm.Bind(FormClientEvent.Initialised, "saveFormReference");

            var productsLevel = new TreeModel("Products");
            productsLevel.Columns = new List<TreeColumn>() {
                new TreeColumn("ProductID") { PrimaryKey = true },
                new TreeColumn("ProductName"),
                new TreeColumn("CategoryID") { ForeignKey = true }
            };

            productsLevel.FixedFilter = "discontinued = @discontinued";
            productsLevel.FixedFilterParameters.Add(new DbParameter("discontinued", 0));
            var categoryProductTree = new TreeModel(DataSourceType.SQLite, "Northwind2", "Categories");
            categoryProductTree.Columns = new List<TreeColumn>()
            {
                new TreeColumn("CategoryID") { PrimaryKey = true },
                new TreeColumn("CategoryName")
            };

            categoryProductTree.NestedLevel = productsLevel;
            categoryProductTree.SelectionTitle = "Selected Product";
            categoryProductTree.ClientEvents[TreeClientEvent.Initialised] = "saveTreeReference";
            categoryProductTree.Expand = true;
            categoryProductTree.ClientEvents[TreeClientEvent.ItemSelected] = "productSelected";

            var sampleModel = GetSampleModel();
            sampleModel.TreeModel = categoryProductTree;
            sampleModel.FormModel = productForm;
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

            var ordersGrid = new GridModel(DataSourceType.SQLite, "Northwind2", "Orders");
            ordersGrid.Columns = new List<GridColumn>() {
                new GridColumn("OrderID") { PrimaryKey = true},
                new GridColumn("CustomerID") { ForeignKey = true, DataOnly = true},
                new GridColumn("EmployeeID")  { Lookup = new Lookup("Employees", "EmployeeID", "LastName || ',' || FirstName") },
                new GridColumn("OrderDate"),
                new GridColumn("RequiredDate"),
                new GridColumn("ShippedDate"),
                new GridColumn("ShipVia") { Lookup = new Lookup("Shippers", "ShipperId", "CompanyName") },
                new GridColumn("Freight") { Format = "c" },
                new GridColumn("ShipName"),
                new GridColumn("ShipAddress"),
                new GridColumn("ShipCity"),
                new GridColumn("ShipRegion"),
                new GridColumn("ShipPostalCode"),
                new GridColumn("ShipCountry")
            };

            ordersGrid.Caption = "Orders";
            ordersGrid.FixedFilter = "customerid = @customerid";
            ordersGrid.FixedFilterParameters.Add(new DbParameter("customerid", ""));
            ordersGrid.Bind(GridClientEvent.Initialised, "saveOrdersGridReference");

            ordersGrid.LinkedControl = orderDetailsGrid;

            var customersLevel = new TreeModel("Customers");
            customersLevel.Columns = new List<TreeColumn>() {
                new TreeColumn("CustomerID") {  },
                new TreeColumn("CompanyName") {  },
                new TreeColumn("Country") { ForeignKey = true }
            };

            var countriesLevel = new TreeModel("Customers");
            countriesLevel.Columns = new List<TreeColumn>() {
                new TreeColumn("Country") { PrimaryKey = true },
                new TreeColumn("Region") { ForeignKey = true }
            };
            countriesLevel.Distinct = true;
            countriesLevel.NestedLevel = customersLevel;

            var regionsTree = new TreeModel(DataSourceType.SQLite, "Northwind2", "Customers");
            regionsTree.Columns = new List<TreeColumn>()
            {
                new TreeColumn("Region") { PrimaryKey = true }
            };

            regionsTree.NestedLevel = countriesLevel;
            regionsTree.Distinct = true;
            regionsTree.Expand = true;
            regionsTree.DropDown = false;
            regionsTree.MaxHeight = "70rem";

            regionsTree.Bind(TreeClientEvent.ItemSelected, "customerSelected");
            var sampleModel = GetSampleModel();
            sampleModel.TreeModel = regionsTree;
            sampleModel.GridModels.Add(ordersGrid);
            sampleModel.GridModels.Add(orderDetailsGrid);
            return View(sampleModel);
        }

        public IActionResult Menu()
        {
            var samplesTree = new TreeModel(DataSourceType.FileSystem, "../Views") { FixedFilter = "(IsDirectory = 1 or Name like '%.cshtml') and Name not in ('Shared','_ViewStart.cshtml','index.cshtml','_viewimports.cshtml','explore.cshtml','resources.cshtml','error.cshtml')" };
            samplesTree.Columns = new List<TreeColumn>()
            {
                new TreeColumn(FileSystemColumn.Name.ToString())
            };
            samplesTree.Bind(TreeClientEvent.ItemSelected, "fileSelected");
            samplesTree.Bind(TreeClientEvent.Initialised, "cleanFileNames");

            var sampleModel = GetSampleModel();
            sampleModel.TreeModel = samplesTree;
            return View(sampleModel);
        }
       }
}