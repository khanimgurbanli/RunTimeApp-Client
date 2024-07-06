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
