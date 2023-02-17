using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public AttachmentsController(OnlineSchoolDbContext context)
        {
            this.context = context;
        }

        // GET: Attachments
        public async Task<IActionResult> Index()
        {
            var onlineSchoolDbContext = context.Attachments.Include(a => a.Cource);
            return View(await onlineSchoolDbContext.ToListAsync());
        }

        // GET: Attachments/Details/5
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

            return View(attachment);
        }

        // GET: Attachments/Create
        public IActionResult Create()
        {
            ViewData["CourceId"] = new SelectList(context.Cources, "Id", "Id");
            return View();
        }

        // POST: Attachments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourceId,Title,Link")] Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                context.Add(attachment);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourceId"] = new SelectList(context.Cources, "Id", "Id", attachment.CourceId);
            return View(attachment);
        }

        // GET: Attachments/Edit/5
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
            ViewData["CourceId"] = new SelectList(context.Cources, "Id", "Id", attachment.CourceId);
            return View(attachment);
        }

        // POST: Attachments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourceId,Title,Link")] Attachment attachment)
        {
            if (id != attachment.Id)
            {
                return NotFound();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourceId"] = new SelectList(context.Cources, "Id", "Id", attachment.CourceId);
            return View(attachment);
        }

        // GET: Attachments/Delete/5
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

            return View(attachment);
        }

        // POST: Attachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Attachments == null)
            {
                return Problem("Entity set 'OnlineSchoolDbContext.Attachments'  is null.");
            }
            var attachment = await context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                context.Attachments.Remove(attachment);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttachmentExists(int id)
        {
          return context.Attachments.Any(e => e.Id == id);
        }
    }
}
