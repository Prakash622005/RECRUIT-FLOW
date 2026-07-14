using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitFlow.Data;
using RecruitFlow.Models;
using RecruitFlow.Models.ViewModels;
using RecruitFlow.Services;

namespace RecruitFlow.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ---------- Registration (applicants only) ----------

        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _db.ApplicantUsers.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "This email is already registered.");
                return View(model);
            }

            var user = new ApplicantUser
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = PasswordHasher.Hash(model.Password)
            };

            _db.ApplicantUsers.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Registration successful. Please log in.";
            return RedirectToAction(nameof(Login));
        }

        // ---------- Login (shared entry point for both actors) ----------

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Companies use predefined credentials - checked first.
            var company = await _db.Companies.FirstOrDefaultAsync(c => c.Username == model.UsernameOrEmail);
            if (company != null && PasswordHasher.Verify(model.Password, company.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, company.CompanyName),
                    new(ClaimTypes.Role, "Company"),
                    new("CompanyId", company.Id.ToString())
                };
                await SignInAsync(claims);
                return RedirectToAction("Dashboard", "Company");
            }

            // Otherwise, treat the input as an applicant's registered email.
            var user = await _db.ApplicantUsers.FirstOrDefaultAsync(u => u.Email == model.UsernameOrEmail);
            if (user != null && PasswordHasher.Verify(model.Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, user.FullName),
                    new(ClaimTypes.Role, "User"),
                    new("UserId", user.Id.ToString())
                };
                await SignInAsync(claims);
                return RedirectToAction("Dashboard", "User");
            }

            ModelState.AddModelError(string.Empty, "Invalid email/username or password.");
            return View(model);
        }

        private async Task SignInAsync(List<Claim> claims)
        {
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }
    }
}
