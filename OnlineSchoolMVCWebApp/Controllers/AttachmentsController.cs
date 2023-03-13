using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineSchoolMVCWebApp.Data;
using OnlineSchoolMVCWebApp.Models;

namespace OnlineSchoolMVCWebApp.Controllers
{
    public class AttachmentsController : Controller
    {
        private readonly OnlineSchoolDbContext context;
        private readonly UserManager<User> userManager;
        public AttachmentsController(OnlineSchoolDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        // GET: Attachments
        public async Task<IActionResult> Index(int? courceid)
        {
            if (courceid is null)
            {
                return NotFound();
            }
            ViewBag.CourceId = courceid;
            ViewBag.CourceTitle = (await context.Cources.FirstOrDefaultAsync(t => t.Id == courceid)).Title;
            ViewBag.CourceAuthor = (await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == courceid)).Author;
            var onlineSchoolDbContext = context.Attachments.Where(t => t.CourceId == courceid).Include(t => t.Cource);
            return View(await onlineSchoolDbContext.ToListAsync());
        }

        // GET: Attachments/Details/5
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.Attachments == null)
            {
                return NotFound();
            }

            var attachment = await context.Attachments
                .Include(a => a.Cource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attachment == null)
            {
                return NotFound();
            }
            ViewBag.CourceAuthor = (await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == attachment.CourceId)).Author;
            return View(attachment);
        }

        // GET: Attachments/Create
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

        // POST: Attachments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Create([Bind("Id,CourceId,Title,Link")] Attachment attachment)
        {
            Cource cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == attachment.CourceId);
            if (cource == null)
            {
                return NotFound();
            }
            if (!(await HasPermition(cource)))
            {
                return RedirectToAction(nameof(Index));
            }
            if (attachment.Title.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів для заголовку: 50";
                return RedirectToAction(nameof(Create), new { courceid = attachment.CourceId });
            }
            if (ModelState.IsValid)
            {
                context.Add(attachment);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), "Tasks", new { id = attachment.CourceId });
            }
            ViewData["CourceId"] = new SelectList(context.Cources, "Id", "Id", attachment.CourceId);
            return View(attachment);
        }

        // GET: Attachments/Edit/5
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Attachments == null)
            {
                return NotFound();
            }

            var attachment = await context.Attachments.FindAsync(id);
            if (attachment == null)
            {
                return NotFound();
            }
            var cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == attachment.CourceId);
            if (cource == null)
            {
                return NotFound();
            }
            if (!(await HasPermition(cource)))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(attachment);
        }

        // POST: Attachments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourceId,Title,Link")] Attachment attachment)
        {
            if (id != attachment.Id)
            {
                return NotFound();
            }
            if (attachment.Title.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів для заголовку: 50";
                return RedirectToAction(nameof(Edit), new { Id = id });
            }
            Cource cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == attachment.CourceId);
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
                    context.Update(attachment);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttachmentExists(attachment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { courceid = attachment.CourceId });
            }
            return View(attachment);
        }

        // GET: Attachments/Delete/5
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Attachments == null)
            {
                return NotFound();
            }

            var attachment = await context.Attachments
                .Include(a => a.Cource)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attachment == null)
            {
                return NotFound();
            }
            var cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == attachment.CourceId);
            if (cource == null)
            {
                return NotFound();
            }
            if (!(await HasPermition(cource)))
            {
                return RedirectToAction(nameof(Index));
            }
            return View(attachment);
        }

        // POST: Attachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Attachments == null)
            {
                return Problem("Entity set 'OnlineSchoolDbContext.Attachments'  is null.");
            }
            var attachment = await context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                var cource = await context.Cources.Include(c => c.Author).FirstOrDefaultAsync(c => c.Id == attachment.CourceId);
                if (cource == null)
                {
                    return NotFound();
                }
                if (!(await HasPermition(cource)))
                {
                    return RedirectToAction(nameof(Index));
                }
                context.Attachments.Remove(attachment);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { courceid = attachment.CourceId });
        }

        private bool AttachmentExists(int id)
        {
          return context.Attachments.Any(e => e.Id == id);
        }

        private async Task<bool> HasPermition(Cource cource)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            return User.IsInRole(SettingStrings.AdminRole) || user.UserName == cource.Author.Email;
        }
    }
}
