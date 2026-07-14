using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecruitFlow.Data;
using RecruitFlow.Models;
using RecruitFlow.Models.ViewModels;

namespace RecruitFlow.Controllers
{
    [Authorize(Roles = "Company")]
    public class CompanyController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public CompanyController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        public async Task<IActionResult> Dashboard()
        {
            var applications = await _db.JobApplications
                .Include(j => j.ApplicantUser)
                .OrderByDescending(j => j.AppliedOn)
                .ToListAsync();

            return View(applications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select(int id, ScheduleInterviewViewModel model)
        {
            var application = await _db.JobApplications.FindAsync(id);
            if (application == null) return NotFound();

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please provide a valid interview date, time and venue.";
                return RedirectToAction(nameof(Dashboard));
            }

            application.Status = ApplicationStatus.SelectedForInterview;
            application.InterviewDate = model.InterviewDate;
            application.InterviewTime = model.InterviewTime;
            application.InterviewVenue = model.InterviewVenue;

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var application = await _db.JobApplications.FindAsync(id);
            if (application == null) return NotFound();

            application.Status = ApplicationStatus.Rejected;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> DownloadResume(int id)
        {
            var application = await _db.JobApplications.FindAsync(id);
            if (application == null) return NotFound();

            var filePath = Path.Combine(_env.WebRootPath, application.ResumeFilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, "application/pdf", application.ResumeFileName);
        }
    }
}
