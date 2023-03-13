using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineSchoolMVCWebApp.Data;
using OnlineSchoolMVCWebApp.Models;

namespace OnlineSchoolMVCWebApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly OnlineSchoolDbContext context;
        private readonly UserManager<User> userManager;
        public TasksController(OnlineSchoolDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(int? id, string? title)
        {
            if (id is null)
            {
                return RedirectToAction("Index", "Cources");
            }
            if (title is null)
            {
                title = (await context.Cources.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id)).Title;
            }
            ViewBag.CourceId = id;
            ViewBag.CourceTitle = title;
            ViewBag.CourceAuthor = (await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == id)).Author;
            var attachments = context.Attachments.Where(c => c.CourceId == id);
            if (await attachments.CountAsync() != 0)
            {
                ViewBag.Attachments = await attachments.ToListAsync();
            }
            var onlineSchoolDbContext = context.Tasks.Where(t => t.CourceId == id).Include(t => t.Cource).OrderBy(t => t.CourceId).ThenBy(t => t.SortOrder);
            return View(await onlineSchoolDbContext.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.Tasks == null)
            {
                return NotFound();
            }

            var task = await context.Tasks
                .Include(t => t.Cource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            ViewBag.CourceAuthor = (await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == task.CourceId)).Author;
            return View(task);
        }

        // GET: Tasks/Create
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Create(int? courceid)
        {
            if (courceid is null)
            {
                return NotFound();
            }

            Cource cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == courceid);
            if (cource == null)
            {
                return NotFound();
            }
            
            if (!(await HasPermition(cource)))
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourceId"] = courceid;
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Create([Bind("Id,CourceId,Title,TaskContent,SortOrder")] Models.Task task)
        {
            Cource cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == task.CourceId);
            if (cource == null)
            {
                return NotFound();
            }
            if (!(await HasPermition(cource)))
            {
                return RedirectToAction(nameof(Index));
            }
            if (task.Title.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів для заголовку: 50";
                return RedirectToAction(nameof(Create), new { courceid = task.CourceId });
            }
            if (ModelState.IsValid)
            {
                await context.AddAsync(task);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = task.CourceId });
            }
            return View(task);
        }

        // GET: Tasks/Edit/5
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Tasks == null)
            {
                return NotFound();
            }

            var task = await context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            var cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == task.CourceId);
            if (cource == null)
            {
                return NotFound();
            }
            if (!(await HasPermition(cource)))
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourceId,Title,TaskContent,SortOrder")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }
            if (task.Title.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів для заголовку: 50";
                return RedirectToAction(nameof(Edit), new { Id = id });
            }
            Cource cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == task.CourceId);
            if (cource == null)
            {
                return NotFound();
            }

            if (!(await HasPermition(cource)))
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(task);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = task.CourceId });
            }
            return View(task);
        }

        // GET: Tasks/Delete/5
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Tasks == null)
            {
                return NotFound();
            }

            var task = await context.Tasks
                .Include(t => t.Cource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }
            var cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == task.CourceId);
            if (cource == null)
            {
                return NotFound();
            }
            if (!(await HasPermition(cource)))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Tasks == null)
            {
                return Problem("Entity set 'OnlineSchoolDbContext.Tasks'  is null.");
            }
            var task = await context.Tasks.Include(t => t.Cource).FirstOrDefaultAsync(t => t.Id == id);
            if (task != null)
            {
                var cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == task.CourceId);
                if (cource == null)
                {
                    return NotFound();
                }
                if (!(await HasPermition(cource)))
                {
                    return RedirectToAction(nameof(Index));
                }
                context.Tasks.Remove(task);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = task.CourceId });
        }

        private bool TaskExists(int id)
        {
          return (context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private async Task<bool> HasPermition(Cource cource)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            return User.IsInRole(SettingStrings.AdminRole) || user.UserName == cource.Author.Email;
        }
    }
}
