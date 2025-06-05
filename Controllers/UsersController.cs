using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Pz_Proj_11_12.Controllers
{
    public class UsersController : Controller
    {
        private readonly PlannerContext _context;

        public UsersController(PlannerContext context)
        {
            _context = context;
        }

        // GET: Users/Index
        [Authorize]
        public async Task<IActionResult> Index()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.Include(u => u.Planners).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login");
            return View(user);
        }

        // GET: Users/Details
        [Authorize]
        public async Task<IActionResult> Details()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
           
            var user = await _context.Users.Include(u => u.Planners).FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToAction("Login");
            return View(user);
        }

        // GET: Users/Register
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string login, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || password != confirmPassword)
            {
                ViewBag.Error = "Invalid data or passwords do not match.";
                return View();
            }

            var userExists = await _context.Users.AnyAsync(u => u.Login == login);
            if (userExists)
            {
                ViewBag.Error = "Login already exists.";
                return View();
            }

            var user = new User { Login = login, Password = password };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Login));
        }

        // GET: Users/Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // POST: Users/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string login, string password)
        {

            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);

            if(user is null)
            {
                ViewBag.Error = "Invalid login or password.";
                return View();
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Login),
            };

            var identity = new ClaimsIdentity(claims, "Cookie");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("Cookie", principal);

            return RedirectToAction("Index");
            
        }

        // GET: Users/Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookie");
            return RedirectToAction("Login");
        }

        // GET: Users/Edit
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToAction("Login");
            return View(user);
        }

        // POST: Users/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string login, string ConfirmedPassword, string newPassword, string confirmNewPassword)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToAction("Login");

            if (user.Password != ConfirmedPassword)
            {
                ViewBag.Error = "Wrong current password.";
                return View(user);
            }

            if (!string.IsNullOrEmpty(login))
            {
                user.Login = login;
            }

            if (!string.IsNullOrEmpty(newPassword) && newPassword == confirmNewPassword)
            {
                user.Password = newPassword;
            }
            else if (!string.IsNullOrEmpty(newPassword))
            {
                ViewBag.Error = "New passwords do not match.";
                return View(user);
            }

            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details");
        }

        // GET: Users/Delete
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return RedirectToAction("Login");

            return View(user);
        }

        // POST: Users/ConfirmDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ConfirmDelete(string ConfirmedPassword)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = await _context.Users.FindAsync(userId);

            if (user == null || user.Password != ConfirmedPassword)
            {
                ViewBag.Error = "Incorrect password.";
                return View("Delete");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
			await HttpContext.SignOutAsync("Cookie");
			return RedirectToAction("Login");
		}
    }
}
