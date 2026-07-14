using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitFlow.Data;
using RecruitFlow.Models;
using RecruitFlow.Models.ViewModels;

namespace RecruitFlow.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public UserController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        private int CurrentUserId => int.Parse(User.FindFirstValue("UserId")!);

        public async Task<IActionResult> Dashboard()
        {
            ViewBag.History = await _db.JobApplications
                .Where(j => j.ApplicantUserId == CurrentUserId)
                .OrderByDescending(j => j.AppliedOn)
                .ToListAsync();

            return View(new JobApplicationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(JobApplicationViewModel model)
        {
            if (model.Resume == null || model.Resume.Length == 0)
            {
                ModelState.AddModelError(nameof(model.Resume), "Please attach your resume (PDF).");
            }
            else if (Path.GetExtension(model.Resume.FileName).ToLowerInvariant() != ".pdf")
            {
                ModelState.AddModelError(nameof(model.Resume), "Only PDF files are allowed.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.History = await _db.JobApplications
                    .Where(j => j.ApplicantUserId == CurrentUserId)
                    .OrderByDescending(j => j.AppliedOn)
                    .ToListAsync();
                return View("Dashboard", model);
            }

            var resumesFolder = Path.Combine(_env.WebRootPath, "resumes");
            Directory.CreateDirectory(resumesFolder);

            var storedName = $"{Guid.NewGuid()}.pdf";
            var fullPath = Path.Combine(resumesFolder, storedName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await model.Resume!.CopyToAsync(stream);
            }

            var application = new JobApplication
            {
                ApplicantUserId = CurrentUserId,
                FullName = model.FullName,
                ExperienceYears = model.ExperienceYears,
                CurrentCTC = model.CurrentCTC,
                ExpectedCTC = model.ExpectedCTC,
                JobRole = model.JobRole,
                ResumeFileName = model.Resume!.FileName,
                ResumeFilePath = $"/resumes/{storedName}",
                Status = ApplicationStatus.Pending
            };

            _db.JobApplications.Add(application);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Application submitted successfully.";
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Status(int id)
        {
            var application = await _db.JobApplications
                .FirstOrDefaultAsync(j => j.Id == id && j.ApplicantUserId == CurrentUserId);

            if (application == null) return NotFound();

            return View(application);
        }
    }
}
