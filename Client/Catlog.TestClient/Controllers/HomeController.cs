using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Catlog.TestClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using IdentityModel.Client;

namespace Catlog.TestClient.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            //Console.WriteLine("---------- 尝试访问 catlog.api ----------");
            //var client = new HttpClient();
            //var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5004");
            //if (disco.IsError)
            //{
            //    Console.WriteLine($"disco 错误：\n{disco.Error}");
            //}
            //var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            //client.SetBearerToken(accessToken);
            //var response = await client.GetAsync("http://localhost:5002/api");
            //if (!response.IsSuccessStatusCode)
            //{
            //    Console.WriteLine(response.StatusCode);
            //}
            //else
            //{
            //    var content = await response.Content.ReadAsStringAsync();
            //    Console.WriteLine($"成功访问 API 资源：\n{content.ToString()}");
            //}
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            Console.WriteLine("-------------------------A------------------------");
            var githubAccessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
            Console.WriteLine(githubAccessToken);
            Console.WriteLine("-------------------------B------------------------");
            Console.WriteLine(User.Identity.Name);
            foreach (var item in User.Claims)
            {
                Console.WriteLine($"{item.Type} -> {item.Value}");
            }
            Console.WriteLine("-------------------------C------------------------");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
