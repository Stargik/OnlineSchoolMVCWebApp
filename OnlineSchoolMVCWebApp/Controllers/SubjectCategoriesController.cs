using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineSchoolMVCWebApp.Data;
using OnlineSchoolMVCWebApp.Models;

namespace OnlineSchoolMVCWebApp.Controllers
{
    public class SubjectCategoriesController : Controller
    {
        private readonly OnlineSchoolDbContext context;

        public SubjectCategoriesController(OnlineSchoolDbContext context)
        {
            this.context = context;
        }

        // GET: SubjectCategories
        public async Task<IActionResult> Index()
        {
              return context.SubjectCategories != null ? 
                          View(await context.SubjectCategories.ToListAsync()) :
                          Problem("Entity set 'OnlineSchoolDbContext.SubjectCategories'  is null.");
        }

        // GET: SubjectCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || context.SubjectCategories == null)
            {
                return NotFound();
            }

            var subjectCategory = await context.SubjectCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectCategory == null)
            {
                return NotFound();
            }
            return RedirectToAction("Index", "Cources", new { id = subjectCategory.Id, name = subjectCategory.Name });
        }

        // GET: SubjectCategories/Create
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        // POST: SubjectCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Create([Bind("Id,Name")] SubjectCategory subjectCategory)
        {
            if (subjectCategory.Name.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів: 50";
                return RedirectToAction(nameof(Create));
            }
            if (ModelState.IsValid)
            {
                await context.AddAsync(subjectCategory);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subjectCategory);
        }

        // GET: SubjectCategories/Edit/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.SubjectCategories == null)
            {
                return NotFound();
            }

            var subjectCategory = await context.SubjectCategories.FindAsync(id);
            if (subjectCategory == null)
            {
                return NotFound();
            }
            return View(subjectCategory);
        }

        // POST: SubjectCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] SubjectCategory subjectCategory)
        {
            if (id != subjectCategory.Id)
            {
                return NotFound();
            }
            if (subjectCategory.Name.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів: 50";
                return RedirectToAction(nameof(Edit), new { Id = id });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(subjectCategory);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubjectCategoryExists(subjectCategory.Id))
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
            return View(subjectCategory);
        }

        // GET: SubjectCategories/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.SubjectCategories == null)
            {
                return NotFound();
            }

            var subjectCategory = await context.SubjectCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subjectCategory == null)
            {
                return NotFound();
            }

            return View(subjectCategory);
        }

        // POST: SubjectCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.SubjectCategories == null)
            {
                return Problem("Entity set 'OnlineSchoolDbContext.SubjectCategories'  is null.");
            }  
            var courcesBySubjectCategory = await context.Cources.Where(c => c.SubjectCategoryId == id).ToListAsync();
            if (courcesBySubjectCategory.Any())
            {
                TempData["ErrorMessage"] = "Помилка. Неможливо видалити категорію, яка містить курси.";
                return RedirectToAction(nameof(Index));
            }
            var subjectCategory = await context.SubjectCategories.FindAsync(id);
            if (subjectCategory != null)
            {
                context.SubjectCategories.Remove(subjectCategory);
            }
            
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubjectCategoryExists(int id)
        {
          return (context.SubjectCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
