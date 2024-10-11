using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS_DEPI.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This will render Views/Teacher/Index.cshtml
        }

        // Add other teacher-specific actions
    }
}
