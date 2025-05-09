using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DbNetSuiteCoreSamples.Pages;

public class IndexModel : PageModel
{
    public string CustomerId { get; set; } = null;
    public int? OrderId { get; set; } = null;

    public IndexModel()
    {
    }
}
