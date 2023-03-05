using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchoolMVCWebApp.Data;
using System.Globalization;

namespace OnlineSchoolMVCWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly OnlineSchoolDbContext context;

        public ChartController(OnlineSchoolDbContext context)
        {
            this.context = context;
        }

        [HttpGet("JsonCourcesCountByCategoryData")]
        public async Task<JsonResult> JsonCourcesCountByCategoryData()
        {
            var categories = await context.SubjectCategories.Include(sc => sc.Cources).ToListAsync();
            List<object> catCource = new List<object>();
            catCource.Add(new[] { "Предмет", "Кількість курсів" });

            foreach (var category in categories)
            {
                catCource.Add(new object[] {category.Name, category.Cources.Count()});
            }
            return new JsonResult(catCource);
        }

        [HttpGet("JsonCourcesCountByAuthorData")]
        public async Task<JsonResult> JsonCourcesCountByAuthorData()
        {
            var authors = await context.Authors.Include(a => a.Cources).ToListAsync();
            List<object> AuthorCource = new List<object>();
            AuthorCource.Add(new[] { "Автор", "Кількість курсів" });

            foreach (var author in authors)
            {
                AuthorCource.Add(new object[] { author.FirstName + " " + author.LastName, author.Cources.Count() });
            }
            return new JsonResult(AuthorCource);
        }

        [HttpGet("JsonCourcesCountByMonthData")]
        public async Task<JsonResult> JsonCourcesCountByMonthData()
        {
            var cources = await context.Cources.ToListAsync();
            var monthes = cources.GroupBy(c => new { c.CreationDate.Year, c.CreationDate.Month });
            List<object> AuthorCource = new List<object>();
            AuthorCource.Add(new[] { "Місяць", "Кількість доданих курсів" });

            foreach (var month in monthes)
            {
                AuthorCource.Add(new object[] { CultureInfo.GetCultureInfo("uk-UA").DateTimeFormat.GetMonthName(month.Key.Month) + " " + month.Key.Year, month.Count() });
            }
            return new JsonResult(AuthorCource);
        }
    }
}
