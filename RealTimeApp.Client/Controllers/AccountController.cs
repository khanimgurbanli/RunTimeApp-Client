using Microsoft.AspNetCore.Mvc;
using RealTimeApp.Client.ViewModels;

namespace RealTimeApp.Client.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SendOtp()
        {
            return View(new SendOtp_ViewModel(null, null, null) { });
        }
        [HttpPost]
        public IActionResult SendOtp(SendOtp_ViewModel model)
        {
            if (!ModelState.IsValid)
                return View("SendOtp", model);

            return RedirectToAction("Account", "VerifyOTP");
        }

        public IActionResult VerifyOTP()
        {
            return View();
        }
    }
}
