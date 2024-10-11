// Components/UserNameViewComponent.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using LMS_DEPI.APP.Database; // Adjust this to your namespace

public class UserNameViewComponent : ViewComponent
{
    private readonly UserManager<UserIdentity> _userManager;

    public UserNameViewComponent(UserManager<UserIdentity> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var userName = user != null ? user.UserName : "Guest";
        return View("Default", userName);
    }
}
