using DbNetSuiteCore.Enums;
using DbNetSuiteCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace DbNetSuiteCore.Samples.Controllers
{
    public class FormController : SampleController
    {
        public FormController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment)
        {
        }

        public IActionResult Basic()
        {
            var sampleModel = GetSampleModel();
            sampleModel.FormModel = new FormModel(DataSourceType.SQLite, "northwind", "Customers") { Insert = true, Delete = false };
            return View(sampleModel);

        }
        public IActionResult CategoryProducts()
        {
            var categorySelect = new SelectModel(DataSourceType.SQLite, "Northwind", "Categories") { Caption = "Categories", Search = true, Layout = LayoutType.Row };
            categorySelect.Columns = new List<SelectColumn>() 
            {
                new SelectColumn("CategoryId"),
                new SelectColumn("CategoryName")
            };
            var productForm = new FormModel("Products") { Insert = true, Delete = true, ToolbarPosition = ToolbarPosition.Bottom, Caption = "Products" };
            productForm.Columns = new List<FormColumn>() 
            {
                new FormColumn("ProductID","ID"),
                new FormColumn("ProductName", "Name"),
                new FormColumn("SupplierID","Supplier") { Lookup = new Lookup("Suppliers", "SupplierId", "CompanyName"),Required = true },
                new FormColumn("CategoryID","Category") { Lookup = new Lookup("Categories", "CategoryID", "CategoryName"),Required = true, ForeignKey = true },
                new FormColumn("QuantityPerUnit", "Qty"){ InitialValue = 0},
                new FormColumn("UnitPrice", "Price") {Required = true,InitialValue = 0, MinValue = 0, MaxValue = 100 },
                new FormColumn("UnitsInStock", "Stock") {Required = true,InitialValue = 0, MinValue = 0 },
                new FormColumn("UnitsOnOrder", "On Order") {Required = true,InitialValue = 0 },
                new FormColumn("ReorderLevel", "Re-order Level") {Required = true,InitialValue = 0, MaxValue = 50 },
                new FormColumn("Discontinued") {DataType = typeof(bool), InitialValue = true}
            };

            categorySelect.LinkedControl = productForm;

            var sampleModel = GetSampleModel();
            sampleModel.FormModel = productForm;
            sampleModel.SelectModel = categorySelect;
            return View(sampleModel);

        }
        public IActionResult Columns()
        {
            var filmForm = new FormModel(DataSourceType.SQLite, "Sakila", "Film") { Insert = true, Delete = true };
            filmForm.Columns = new List<FormColumn>()
            {
                new FormColumn("Film_Id"),
                new FormColumn("Title"){ColSpan = 3 },
                new FormColumn("Description"){ColSpan = 4, HelpText = "Use ColSpan to extend the width of the input control within the Grid layout"},
                new FormColumn("Release_Year") {MinValue = 2006, MaxValue = DateTime.Today.Year, HelpText = $"Entered value must be between 2006 and {DateTime.Today.Year}"},
                new FormColumn("Language_Id", "Language") {Lookup = new Lookup("language","language_id","name"), HelpText = "Select against a foreign key table"},
                new FormColumn("Original_Language_Id", "Original Language") {Lookup = new Lookup("language","language_id","name")},
                new FormColumn("Rental_Duration","Duration (days)"){ControlType = FormControlType.Auto,LookupRange = Enumerable.Range(3,5), HelpText = "Use IEnumerable to create number based lookup list"},
                new FormColumn("Rental_Rate") {Format = "C", HelpText = "Use standard .NET formatting"},
                new FormColumn("Length") { ControlType = FormControlType.Number, HelpText = "Make input number only"},
                new FormColumn("Replacement_Cost") {Format = "C", Style = "background-color:whitesmoke", HelpText = "Apply custom styling"},
                new FormColumn("Special_Features") {RowSpan = 2, ControlType = FormControlType.SelectMultiple, LookupList = new List<string>(){"Behind the Scenes","Commentaries","Deleted Scenes","Trailers" }, HelpText = "Select multiple values against a custom list"},
                new FormColumn("Rating") {ColSpan = 1, ControlType = FormControlType.Text, LookupDictionary = new Dictionary<string, string>(){{"G", "General audiences (G)"},{"PG", "Parental Guidance (PG)" },{"PG-13", "Parents strongly cautioned (PG-13)"},{"R","Restricted (R)"},{"NC-17", "No children under 17 (NC-17)"}}, HelpText = "Create value suggestions with a custom dictionary"},
                new FormColumn("Last_Update") { DataType = typeof(DateTime), ReadOnly = ReadOnlyMode.InsertAndUpdate, HelpText = "Make input read only"}
            };

            var sampleModel = GetSampleModel();
            sampleModel.FormModel = filmForm;
            return View(sampleModel);
        }
        public IActionResult Employees()
        {
            var employeeForm = new FormModel(DataSourceType.SQLite, "Northwind", "Employees") { LayoutColumns = 4, Insert = true, Delete = true };
            employeeForm.Columns = new List<FormColumn>()
            {
                new FormColumn("EmployeeID") { PrimaryKey = true},
                new FormColumn("LastName"),
                new FormColumn("FirstName"),
                new FormColumn("BirthDate"),
                new FormColumn("Title") { ColSpan = 2 },
                new FormColumn("TitleOfCourtesy"),
                new FormColumn("HireDate") { MaxValue = DateTime.Today, HelpText = "Hire date cannot be after today", InitialValue = DateTime.Today },
                new FormColumn("ReportsTo") {Lookup = new Lookup("employees","EmployeeID","LastName || ',' || FirstName")},
                new FormColumn("Notes") {ControlType = FormControlType.TextArea, TextAreaRows = 10, ColSpan = 3, RowSpan = 4},
                new FormColumn("Address") {ControlType = FormControlType.TextArea, TextAreaRows = 2},
                new FormColumn("City"),
                new FormColumn("Region"),
                new FormColumn("PostalCode"),
                new FormColumn("Country"),
                new FormColumn("HomePhone"),
                new FormColumn("Extension")
            };
            var sampleModel = GetSampleModel();
            sampleModel.FormModel = employeeForm;
            return View(sampleModel);
        }
        public IActionResult GridForm()
        {
            var productsGrid = new GridModel(DataSourceType.SQLite, "Northwind", "Products") { ViewDialog = new ViewDialog() { LayoutColumns = 2 }, PageSize = 5, Caption = "Products" };
            productsGrid.Columns = new List<GridColumn>() 
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
                new GridColumn("Discontinued") { DataType = typeof(Boolean), Filter = FilterType.Default}
            };
            var productForm = new FormModel("Products") { Insert = true, Delete = true };
            productForm.Columns = new List<FormColumn>() 
            {
                new FormColumn("ProductID","ID") { ForeignKey = true },
                new FormColumn("ProductName", "Name"),
                new FormColumn("SupplierID","Supplier") { Lookup = new Lookup("Suppliers", "SupplierId", "CompanyName"),Required = true },
                new FormColumn("CategoryID","Category") { Lookup = new Lookup("Categories", "CategoryID", "CategoryName"),Required = true},
                new FormColumn("QuantityPerUnit", "Qty"){ InitialValue = 0},
                new FormColumn("UnitPrice", "Price") {Required = true,InitialValue = 0, MinValue = 0, MaxValue = 100 },
                new FormColumn("UnitsInStock", "Stock") {Required = true,InitialValue = 0, MinValue = 0 },
                new FormColumn("UnitsOnOrder", "On Order") {Required = true,InitialValue = 0 },
                new FormColumn("ReorderLevel", "Re-order Level") {Required = true,InitialValue = 0, MaxValue = 50 },
                new FormColumn("Discontinued") {DataType = typeof(bool), InitialValue = false}
            };

            productsGrid.LinkedControl = productForm;

            var sampleModel = GetSampleModel();
            sampleModel.GridModel = productsGrid;
            sampleModel.FormModel = productForm;
            return View(sampleModel);
        }

        public IActionResult Linked()
        {
            var customerForm = new FormModel(DataSourceType.SQLite, "Northwind", "Customers") { Insert = true, Delete = true, Caption = "Customers" };
            customerForm.Columns = new List<FormColumn>() 
            {
                new FormColumn("CustomerID") { TextTransform = TextTransform.Uppercase},
                new FormColumn("CompanyName") {Required = true},
                new FormColumn("ContactName"),
                new FormColumn("ContactTitle"),
                new FormColumn("Address") { ColSpan = 2},
                new FormColumn("City"),
                new FormColumn("Region"),
                new FormColumn("PostalCode"),
                new FormColumn("Country"),
                new FormColumn("Phone"),
                new FormColumn("Fax")
            };

            var orderForm = new FormModel("Orders") { Insert = true, Delete = true, Caption = "Orders" };
            orderForm.Columns = new List<FormColumn>() 
            {
                new FormColumn("OrderID"),
                new FormColumn("CustomerID") { ForeignKey = true, DataOnly = true},
                new FormColumn("EmployeeID")  { Lookup = new Lookup("Employees", "EmployeeID", "LastName || ',' || FirstName"),Required = true },
                new FormColumn("OrderDate") { DataType = typeof(DateTime), ControlType = FormControlType.Date },
                new FormColumn("RequiredDate") { DataType = typeof(DateTime), ControlType = FormControlType.Date },
                new FormColumn("ShippedDate") { DataType = typeof(DateTime), ControlType = FormControlType.Date },
                new FormColumn("ShipVia") { Lookup = new Lookup("Shippers", "ShipperId", "CompanyName"),Required = true },
                new FormColumn("Freight") { Format = "c" },
                new FormColumn("ShipName") { ColSpan = 2},
                new FormColumn("ShipAddress") { ColSpan = 2},
                new FormColumn("ShipCity"),
                new FormColumn("ShipRegion"),
                new FormColumn("ShipPostalCode"),
                new FormColumn("ShipCountry")
            };

            customerForm.LinkedControl = orderForm;

            var sampleModel = GetSampleModel();
            sampleModel.FormModels.Add(customerForm);
            sampleModel.FormModels.Add(orderForm);
            return View(sampleModel);
        }
    
    }
}