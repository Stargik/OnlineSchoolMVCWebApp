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
    public class TasksController : Controller
    {
        private readonly OnlineSchoolDbContext context;

        public TasksController(OnlineSchoolDbContext context)
        {
            this.context = context;
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
            var onlineSchoolDbContext = context.Tasks.Where(t => t.CourceId == id).Include(t => t.Cource);
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

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create(int courceid)
        {
            ViewData["CourceId"] = courceid;
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourceId,Title,TaskContent,SortOrder")] Models.Task task)
        {
            if (ModelState.IsValid)
            {
                context.Add(task);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = task.CourceId });
            }
            ViewData["CourceId"] = new SelectList(context.Cources, "Id", "Id", task.CourceId);
            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Tasks == null)
            {
                return NotFound();
            }

            var task = await context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourceId,Title,TaskContent,SortOrder")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
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
            ViewData["CourceId"] = new SelectList(context.Cources, "Id", "Id", task.CourceId);
            return View(task);
        }

        // GET: Tasks/Delete/5
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

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Tasks == null)
            {
                return Problem("Entity set 'OnlineSchoolDbContext.Tasks'  is null.");
            }
            var task = await context.Tasks.FindAsync(id);
            if (task != null)
            {
                context.Tasks.Remove(task);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = task.CourceId });
        }

        private bool TaskExists(int id)
        {
          return (context.Tasks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
