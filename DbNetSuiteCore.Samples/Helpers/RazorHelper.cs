using Microsoft.AspNetCore.Html;

namespace DbNetSuiteCore.Samples.Helpers
{
    public static class RazorHelper
    {
        public static HtmlString HelpLink(string url, string text)
        {
            return new HtmlString($"<a style=\"font-weight:bold\" href=\"https://github.com/dbnetlink/DbNetSuiteCore2/wiki\" target=\"_blank\">{text}</a>");
        }
    }
}
