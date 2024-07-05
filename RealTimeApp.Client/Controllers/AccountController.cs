using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RealTimeApp.Client.ViewModels;
using System.Data;
using System.Text;

namespace RealTimeApp.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(Login_ViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var apiUrl = "https://localhost:7071/auth/login";

                using (var httpClient = _httpClientFactory.CreateClient())
                {
                    var json = JsonConvert.SerializeObject(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponseJson = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Token Response JSON: " + tokenResponseJson);

                        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenResponseJson);

                        if (tokenResponse != null && tokenResponse.Token != null)
                        {
                            Response.Cookies.Append("AccessToken", tokenResponse.Token.AccessToken, new CookieOptions
                            {
                                HttpOnly = true,
                                Secure = true,
                                Expires = tokenResponse.Token.Expiration
                            });

                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["Message"] = "Invalid token response.";
                            TempData["MessageType"] = "error";
                            return View(model);
                        }
                    }
                    else
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        TempData["Message"] = $"Login failed. Response: {errorResponse}";
                        TempData["MessageType"] = "error";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"An error occurred: {ex.Message}";
                TempData["MessageType"] = "error";
                return View(model);
            }
        }


        private async Task<List<SelectListItem>> GetRolesAsync()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var apiEndpoint = "https://localhost:7071/auth/get-all-roles";

            var roles = new List<SelectListItem>();

            try
            {
                var response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var listedRolesResponse = JsonConvert.DeserializeObject<ListedRolesQueryResponse>(content);

                    if (listedRolesResponse != null && listedRolesResponse.Roles != null)
                    {
                        roles = listedRolesResponse.Roles.Select(role => new SelectListItem
                        {
                            Value = role.Id.ToString(),
                            Text = role.Name
                        }).ToList();
                    }
                }
                else
                {
                    TempData["Message"] = "Failed to load roles.";
                    TempData["MessageType"] = "error";
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred while fetching roles: " + ex.Message);
            }

            return roles;
        }

        public async Task<IActionResult> SignUp()
        {
            var roles = await GetRolesAsync();
            TempData["Roles"] = roles;

            return View(new Register_ViewModel(null, null, null, null, null, null, null));
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(Register_ViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = await GetRolesAsync();
                TempData["Roles"] = roles;

                return View("SignUp", model);
            }

            var httpClient = _httpClientFactory.CreateClient();
            var apiEndpoint = "https://localhost:7071/users/register";

            try
            {
                var jsonPayload = new
                {
                    firstname = model.firstname,
                    lastname = model.lastname,
                    email = model.email,
                    phoneNumber = model.phoneNumber,
                    password = model.password,
                    roleId = model.roleId,
                };

                var json = JsonConvert.SerializeObject(jsonPayload);
                var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiEndpoint, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Registration is successful. Please login.";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("SignIn");
                }
                else
                {
                    var roles = await GetRolesAsync();
                    TempData["Roles"] = roles;

                    TempData["Message"] = "Operation failed. Try again.";
                    TempData["MessageType"] = "error";
                    return View("SignUp", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                return View("SignUp", model);
            }
        }

        [HttpGet]
        public IActionResult SendOtp()
        {
            return View(new SendOtp_ViewModel(null, null, null) { });
        }
        [HttpPost]
        public async Task<IActionResult> SendOtp(SendOtp_ViewModel model)
        {
            if (!ModelState.IsValid)
                return View("SendOtp", model);

            var httpClient = _httpClientFactory.CreateClient();

            var apiEndpoint = "https://localhost:7071/auth/send-otp-mail";

            try
            {
                var requestContent = new StringContent($"{{ \"email\": \"{model.email}\" }}");

                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var response = await httpClient.PostAsync(apiEndpoint, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Email"] = model.email;
                    return RedirectToAction("VerifyOTP");
                }
                else
                {
                    TempData["Message"] = "Operation failed. Try later";
                    TempData["MessageType"] = "error";
                    return View("SendOtp", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                return View("SendOtp", model);
            }
        }

        [HttpGet]
        public IActionResult VerifyOTP()
        {
            var email = TempData["Email"] as string;
            if (email == null)
            {
                TempData["Message"] = "Invalid email";
                TempData["MessageType"] = "error";
                return RedirectToAction("SendOtp");
            }

            TempData.Keep("Email");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string otp)
        {
            var email = TempData["Email"] as string;
            if (email == null)
            {
                TempData["Message"] = "Invalid email";
                TempData["MessageType"] = "error";
                return RedirectToAction("SendOtp");
            }

            if (string.IsNullOrEmpty(otp))
            {
                ModelState.AddModelError(string.Empty, "OTP is required");
                return View();
            }

            var httpClient = _httpClientFactory.CreateClient();
            var apiEndpoint = "https://localhost:7071/auth/verify-otp";

            try
            {
                var requestContent = new StringContent($"{{ \"userInfo\": \"{email}\", \"otpCode\": \"{otp}\" }}");
                requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

                var response = await httpClient.PostAsync(apiEndpoint, requestContent);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "OTP verification successful!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("SignUp");
                }
                else
                {
                    TempData["Message"] = "OTP verification failed. Try again.";
                    TempData["MessageType"] = "error";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Exception occurred: " + ex.Message);
                return View();
            }
        }

    }
}