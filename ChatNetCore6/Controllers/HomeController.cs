using System.Diagnostics;
using ChatNetCore6.Data;
using ChatNetCore6.Models;
using ChatNetCore6.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatNetCore6.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly IManageMessage _manageMessage;


        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, IManageMessage manageMessage)
        {
            _logger = logger;
            _userManager = userManager;
            _manageMessage = manageMessage;
        }

        public async Task<IActionResult> Index()
        {

            var currentUser = await _userManager.GetUserAsync(User);
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.CurrentUser = currentUser.UserName;
            }
            var messages = _manageMessage.GetMessages();
            return View(messages);
        }

        public async Task<IActionResult> Create(Message message)
        {
            try
            {
                var sender = await _userManager.GetUserAsync(User);
                message.UserName = User.Identity.Name;
                message.UserID = sender.Id;
                _manageMessage.CreateNewMessage(message);
                return Ok();
            }
            catch (Exception)
            {
                return Error();
            }
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