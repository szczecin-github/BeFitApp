using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BeFitApp.Controllers
{
    public class LanguageController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            // Set a cookie that stores the user's language preference
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // Redirect back to the page they were on
            return LocalRedirect(returnUrl);
        }
    }
}