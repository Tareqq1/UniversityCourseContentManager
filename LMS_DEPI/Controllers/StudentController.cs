using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS_DEPI.Controllers
{
    [Authorize(Roles = "User")]
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This will render Views/Student/Index.cshtml
        }

        // Add other student-specific actions
    }
}
