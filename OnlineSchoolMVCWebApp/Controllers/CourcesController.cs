using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineSchoolMVCWebApp.Data;
using OnlineSchoolMVCWebApp.Models;
using OnlineSchoolMVCWebApp.Services;

namespace OnlineSchoolMVCWebApp.Controllers
{
    public class CourcesController : Controller
    {
        private readonly OnlineSchoolDbContext context;
        private readonly ExcelService excelService;
        public CourcesController(OnlineSchoolDbContext context, ExcelService excelService)
        {
            this.context = context;
            this.excelService = excelService;
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
        public async Task<IActionResult> Create()
        {
            var authors = await context.Authors.ToListAsync();
            var levels = await context.Levels.ToListAsync();
            var subjectCategories = await context.SubjectCategories.ToListAsync();

            ViewData["AuthorId"] = new SelectList(authors, "Id", "LastName");
            ViewData["LevelId"] = new SelectList(levels, "Id", "Status");
            ViewData["SubjectCategoryId"] = new SelectList(subjectCategories, "Id", "Name");
             if (!(await context.SubjectCategories.AnyAsync()))
            {
                TempData["ErrorMessage"] = "Необхідна попередня наявність хоча б однієї каттегорії.";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        // POST: Cources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AuthorId,SubjectCategoryId,LevelId,Title,Description")] Cource cource)
        {
            var authors = await context.Authors.ToListAsync();
            var levels = await context.Levels.ToListAsync();
            var subjectCategories = await context.SubjectCategories.ToListAsync();
            if (cource.Title.Length > 50)
            {
                TempData["ErrorMessage"] = "Максимальна кількість символів для назви: 50";
                return RedirectToAction(nameof(Create));
            }
            if (ModelState.IsValid)
            {
                cource.CreationDate = DateTime.Now;
                await context.AddAsync(cource);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(authors, "Id", "LastName");
            ViewData["LevelId"] = new SelectList(levels, "Id", "Status");
            ViewData["SubjectCategoryId"] = new SelectList(subjectCategories, "Id", "Name");
            return View(cource);
        }

        // GET: Cources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var authors = await context.Authors.ToListAsync();
            var levels = await context.Levels.ToListAsync();
            var subjectCategories = await context.SubjectCategories.ToListAsync();
            if (id == null || context.Cources == null)
            {
                return NotFound();
            }

            var cource = await context.Cources.FindAsync(id);
            if (cource == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(authors, "Id", "LastName");
            ViewData["LevelId"] = new SelectList(levels, "Id", "Status");
            ViewData["SubjectCategoryId"] = new SelectList(subjectCategories, "Id", "Name");
            return View(cource);
        }

        // POST: Cources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AuthorId,SubjectCategoryId,LevelId,Title,Description, CreationDate")] Cource cource)
        {
            var authors = await context.Authors.ToListAsync();
            var levels = await context.Levels.ToListAsync();
            var subjectCategories = await context.SubjectCategories.ToListAsync();
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
            ViewData["AuthorId"] = new SelectList(authors, "Id", "LastName");
            ViewData["LevelId"] = new SelectList(levels, "Id", "Status");
            ViewData["SubjectCategoryId"] = new SelectList(subjectCategories, "Id", "Name");
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

        public async Task<IActionResult> ExcelConverter()
        {
            var cources = await context.Cources.ToListAsync();
            ViewData["CourcesId"] = new SelectList(cources, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportXl(IFormFile fileExcel)
        {
            if (ModelState.IsValid)
            {
                Author author = await context.Authors.FirstOrDefaultAsync();
                try
                {
                    await excelService.CreateCourcesByExcelFile(fileExcel, author);
                }
                catch (Exception)
                {

                    return BadRequest();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExportXl(List<int> courceIds)
        {
            if (ModelState.IsValid)
            {
                var cources = await context.Cources.Where(c => courceIds.Contains(c.Id)).ToListAsync();
                try
                {
                    return await excelService.CreateExcelFileByCources(cources);
                }
                catch (Exception)
                {

                    return BadRequest();
                }
            }
            return RedirectToAction(nameof(ExcelConverter));
        }

        private bool CourceExists(int id)
        {
          return (context.Cources?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
