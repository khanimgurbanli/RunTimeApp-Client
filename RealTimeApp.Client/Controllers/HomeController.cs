using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using RealTimeApp.Client.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using RealTimeApp.Client.ViewModels;

namespace RealTimeApp.Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private async Task<List<SelectListItem>> GetUsersAsync()
        {
            string role = "SuperUser";

            var httpClient = _httpClientFactory.CreateClient();
            var apiEndpoint = $"https://taskapi.perspektiv.az/users/{role}";

            var users = new List<SelectListItem>();

            try
            {
                var response = await httpClient.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var listedUsersResponse = JsonConvert.DeserializeObject<ListedUsersQueryResponse>(content);

                    if (listedUsersResponse != null && listedUsersResponse.Users != null)
                    {
                        users = listedUsersResponse.Users.Select(user => new SelectListItem
                        {
                            Value = user.Id.ToString(),
                            Text = user.Firstname + " " + user.Lastname
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
                ModelState.AddModelError(string.Empty, "Exception occurred while fetching users: " + ex.Message);
            }

            return users;
        }


        [HttpGet]
        [TokenAuthorize]
        public async Task<IActionResult> Admin()
        {
            var users = await GetUsersAsync();
            TempData["Users"] = users;
            return View();
        }

        [HttpGet]
        [TokenAuthorize]
        public async Task<IActionResult> User()
        {

            return View();
        }


        //[HttpGet]
        //public async Task<IActionResult> Index(string message)
        //{
        //    try
        //    {
        //        // 1. Token alınması gerekiyor
        //        var accessToken = TempData["AccessToken"] as string;

        //        if (string.IsNullOrEmpty(accessToken))
        //        {
        //            TempData["Message"] = "Access token not found. Please sign in.";
        //            TempData["MessageType"] = "error";
        //            return RedirectToAction("SignIn");
        //        }

        //        // 2. HTTP Client oluşturulması
        //        using (var httpClient = _httpClientFactory.CreateClient())
        //        {
        //            // 3. API URL belirleme ve parametre eklemek
        //            var apiUrl = $"https://localhost:7071/users/send-task/{message}";

        //            // 4. Authorization header eklemek
        //            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        //            // 5. GET isteği oluşturmak
        //            var response = await httpClient.GetAsync(apiUrl);

        //            // 6. Başarılı cevap alındığında işlemler
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var taskResponse = await response.Content.ReadAsStringAsync();
        //                // Burada işlenecek bir response yok, çünkü işleminiz gerçekleşti ve geriye Unit döndü.
        //                // Ancak başarılı bir durumda yapılacak işlemleri burada ekleyebilirsiniz.
        //                TempData["Message"] = "Task sent successfully.";
        //                TempData["MessageType"] = "success";
        //                return RedirectToAction("Index", "Home");
        //            }
        //            else
        //            {
        //                // Başarısız durumları işleme
        //                TempData["Message"] = "Failed to send task. Please try again later.";
        //                TempData["MessageType"] = "error";
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Hata durumlarını işleme
        //        TempData["Message"] = $"An error occurred: {ex.Message}";
        //        TempData["MessageType"] = "error";
        //    }

        //    // Başarısız durumlarda tekrar giriş sayfasına yönlendirme
        //    return RedirectToAction("SignIn");
        //}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
