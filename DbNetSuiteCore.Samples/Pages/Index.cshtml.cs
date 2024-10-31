using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DbNetSuiteCoreSamples.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string CustomerId { get; set; } = null;
    public int? OrderId { get; set; } = null;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}
