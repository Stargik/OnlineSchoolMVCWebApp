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
    public class CourcesController : Controller
    {
        private readonly OnlineSchoolDbContext context;

        public CourcesController(OnlineSchoolDbContext context)
        {
            this.context = context;
        }

        // GET: Cources
        public async Task<IActionResult> Index(int? id, string? name)
        {
            var onlineSchoolDbContext = context.Cources.Include(c => c.Author).Include(c => c.Level).Include(c => c.SubjectCategory);
            var courcesByCategory = await onlineSchoolDbContext.ToListAsync();
            ViewBag.SubjectCategoryName = "Всі курси";
            if (id is not null)
            {
                ViewBag.SubjectCategoryId = id; 
                ViewBag.SubjectCategoryName = name;
                courcesByCategory = courcesByCategory.Where(c => c.SubjectCategoryId == id).ToList();
            }
            return View(courcesByCategory);
        }


        // GET: Cources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.Cources == null)
            {
                return NotFound();
            }

            var cource = await context.Cources
                .Include(c => c.Author)
                .Include(c => c.Level)
                .Include(c => c.SubjectCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cource == null)
            {
                return NotFound();
            }
            return View(cource);
        }

        // GET: Cources/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(context.Authors, "Id", "LastName");
            ViewData["LevelId"] = new SelectList(context.Levels, "Id", "Status");
            ViewData["SubjectCategoryId"] = new SelectList(context.SubjectCategories, "Id", "Name");
            return View();
        }

        // POST: Cources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AuthorId,SubjectCategoryId,LevelId,Title,Description")] Cource cource)
        {
            if (cource.Title.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів для назви: 50";
                return RedirectToAction(nameof(Create));
            }
            if (ModelState.IsValid)
            {
                cource.CreationDate = DateTime.Now;
                context.Add(cource);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(context.Authors, "Id", "Id", cource.AuthorId);
            ViewData["LevelId"] = new SelectList(context.Levels, "Id", "Id", cource.LevelId);
            ViewData["SubjectCategoryId"] = new SelectList(context.SubjectCategories, "Id", "Id", cource.SubjectCategoryId);
            return View(cource);
        }

        // GET: Cources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Cources == null)
            {
                return NotFound();
            }

            var cource = await context.Cources.FindAsync(id);
            if (cource == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(context.Authors, "Id", "LastName", cource.AuthorId);
            ViewData["LevelId"] = new SelectList(context.Levels, "Id", "Status", cource.LevelId);
            ViewData["SubjectCategoryId"] = new SelectList(context.SubjectCategories, "Id", "Name", cource.SubjectCategoryId);
            return View(cource);
        }

        // POST: Cources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AuthorId,SubjectCategoryId,LevelId,Title,Description, CreationDate")] Cource cource)
        {
            if (id != cource.Id)
            {
                return NotFound();
            }
            if (cource.Title.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів для назви: 50";
                return RedirectToAction(nameof(Edit), new {Id = id});
            }
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(cource);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourceExists(cource.Id))
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
            ViewData["AuthorId"] = new SelectList(context.Authors, "Id", "Id", cource.AuthorId);
            ViewData["LevelId"] = new SelectList(context.Levels, "Id", "Id", cource.LevelId);
            ViewData["SubjectCategoryId"] = new SelectList(context.SubjectCategories, "Id", "Id", cource.SubjectCategoryId);
            return View(cource);
        }

        // GET: Cources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Cources == null)
            {
                return NotFound();
            }

            var cource = await context.Cources
                .Include(c => c.Author)
                .Include(c => c.Level)
                .Include(c => c.SubjectCategory)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cource == null)
            {
                return NotFound();
            }

            return View(cource);
        }

        // POST: Cources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Cources == null)
            {
                return Problem("Entity set 'OnlineSchoolDbContext.Cources'  is null.");
            }
            var cource = await context.Cources.FindAsync(id);
            if (cource != null)
            {
                context.Cources.Remove(cource);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourceExists(int id)
        {
          return (context.Cources?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
