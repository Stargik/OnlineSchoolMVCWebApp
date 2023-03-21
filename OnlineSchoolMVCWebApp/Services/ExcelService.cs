using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchoolMVCWebApp.Data;
using OnlineSchoolMVCWebApp.Models;
using Author = OnlineSchoolMVCWebApp.Models.Author;
using Task = System.Threading.Tasks.Task;

namespace OnlineSchoolMVCWebApp.Services
{
    public class ExcelService
    {
        private readonly OnlineSchoolDbContext context;

        public ExcelService(OnlineSchoolDbContext context)
        {
            this.context = context;
        }

        public async Task CreateCourcesByExcelFile(IFormFile fileExcel, Author author)
        {
            if (fileExcel != null)
            {
                using (var stream = fileExcel.OpenReadStream())
                {
                    using (XLWorkbook workBook = new XLWorkbook(stream))
                    {
                        foreach (IXLWorksheet worksheet in workBook.Worksheets)
                        {
                            SubjectCategory newCategory;

                            var category = await context.SubjectCategories.Where(c => c.Name.Contains(worksheet.Name)).FirstOrDefaultAsync();

                            if (category is not null)
                            {
                                newCategory = category;
                            }
                            else
                            {
                                newCategory = new SubjectCategory();
                                newCategory.Name = worksheet.Name;
                                await context.SubjectCategories.AddAsync(newCategory);
                            }
                 
                            foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                            {
                                try
                                {
                                    Cource cource = new Cource();
                                    cource.Title = row.Cell(1).Value.ToString();
                                    cource.Description = row.Cell(3).Value.ToString();
                                    cource.SubjectCategory = newCategory;
                                    cource.Author = author;
                                    cource.CreationDate = DateTime.Now;
                                    Level level = await context.Levels.FirstOrDefaultAsync(l => l.Status == row.Cell(2).Value.ToString());
                                    if (level is null) {
                                        throw new ArgumentException("Неправильний рівень складності");
                                    }
                                    cource.Level = level;
                                    await context.Cources.AddAsync(cource);

                                    const int tasksCountIndex = 4;
                                    const int attachmentsCountIndex = 5;
                                    const int infoStartIndex = 6;
                                    const int taskLength = 3;
                                    const int attachmentLength = 2;
                                    int tasksCount = (int)row.Cell(tasksCountIndex).Value.GetNumber();
                                    int attachmentsCount = (int)row.Cell(attachmentsCountIndex).Value.GetNumber();
                                    int attachmentsInfoStartIndex = infoStartIndex + taskLength * tasksCount;
                                    for (int i = 0; i < tasksCount; i++)
                                    {
                                        Models.Task task = new Models.Task();
                                        task.Title = row.Cell(infoStartIndex + i * taskLength).Value.ToString();
                                        task.TaskContent = row.Cell(infoStartIndex + i * taskLength + 1).Value.ToString();
                                        task.SortOrder = (int)row.Cell(infoStartIndex + i * taskLength + 2).Value.GetNumber();
                                        task.Cource = cource;
                                        await context.Tasks.AddAsync(task);
                                    }

                                    for (int i = 0; i < attachmentsCount; i++)
                                    {
                                        Attachment attachment = new Attachment();
                                        attachment.Title = row.Cell(attachmentsInfoStartIndex + i * attachmentLength).Value.ToString();
                                        attachment.Link = row.Cell(attachmentsInfoStartIndex + i * attachmentLength + 1).Value.ToString();
                                        attachment.Cource = cource;
                                        await context.Attachments.AddAsync(attachment);
                                    }
                                }
                                catch (Exception e)
                                {
                                    throw new Exception("Помилка при додаванні данних", e);
                                }
                            }
                        }
                    }
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<FileContentResult> CreateExcelFileByCources(List<Cource> cources)
        {
            using (XLWorkbook workbook = new XLWorkbook())
            {
                var subjectCategories = await context.SubjectCategories.Include(c => c.Cources).ToListAsync();

                foreach (var subjectCategory in subjectCategories)
                {
                    if (cources.Select(c => c.SubjectCategoryId).Contains(subjectCategory.Id))
                    {
                        var worksheet = workbook.Worksheets.Add(subjectCategory.Name);

                        worksheet.Cell("A1").Value = "Назва";
                        worksheet.Cell("B1").Value = "Рівень складності";
                        worksheet.Cell("C1").Value = "Короткий опис";
                        worksheet.Cell("D1").Value = "Дата створення";
                        worksheet.Cell("E1").Value = "Ім'я автора";
                        worksheet.Cell("F1").Value = "Прізвище автора";
                        worksheet.Cell("G1").Value = "Email автора";
                        worksheet.Cell("H1").Value = "Кількість завдань (заголовок | котент | порядок сортквання)";
                        worksheet.Cell("I1").Value = "Кількість додаткових матеріалів (заголовок | посилання)";
                        worksheet.Row(1).Style.Fill.BackgroundColor = XLColor.Green;
                        worksheet.Cell(1, 8).Style.Fill.BackgroundColor = XLColor.Red;
                        worksheet.Cell(1, 9).Style.Fill.BackgroundColor = XLColor.Red;
                        int index = 0;
                        var categoryCources = cources.Where(c => c.SubjectCategoryId == subjectCategory.Id);
                        foreach (var cource in categoryCources)
                        {
                            if (subjectCategory.Cources.Contains(cource))
                            {
                                worksheet.Cell(index + 2, 1).Value = cource.Title;
                                worksheet.Cell(index + 2, 3).Value = cource.Description;
                                worksheet.Cell(index + 2, 4).Value = cource.CreationDate.ToShortDateString();

                                var tasks = await context.Tasks.Where(t => t.CourceId == cource.Id).OrderBy(t => t.SortOrder).ToListAsync();
                                var attachments = await context.Attachments.Where(a => a.CourceId == cource.Id).ToListAsync();
                                var author = await context.Authors.FirstOrDefaultAsync(a => a.Id == cource.AuthorId);
                                var level = await context.Levels.FirstOrDefaultAsync(l => l.Id == cource.LevelId);

                                worksheet.Cell(index + 2, 2).Value = level.Status;
                                worksheet.Cell(index + 2, 5).Value = author.FirstName;
                                worksheet.Cell(index + 2, 6).Value = author.LastName;
                                worksheet.Cell(index + 2, 7).Value = author.Email;

                                int tasksCount = tasks.Count();
                                int attachmentsCount = attachments.Count();
                                worksheet.Cell(index + 2, 8).Value = tasksCount;
                                worksheet.Cell(index + 2, 9).Value = attachmentsCount;

                                const int taskLength = 3;
                                const int authorLength = 3;
                                const int attachmentLength = 2;

                                int taskStartIndex = 10;
                                int attachmentStartIndex = taskStartIndex + taskLength * tasksCount;
                                int authorStartIndex = attachmentStartIndex + attachmentLength * attachmentsCount;

                                for (int j = 0; j < tasksCount; j++)
                                {
                                    worksheet.Cell(index + 2, taskStartIndex + j * taskLength).Value = tasks[j].Title;
                                    worksheet.Cell(index + 2, taskStartIndex + j * taskLength + 1).Value = tasks[j].TaskContent;
                                    worksheet.Cell(index + 2, taskStartIndex + j * taskLength + 2).Value = tasks[j].SortOrder;
                                }
                                for (int j = 0; j < attachmentsCount; j++)
                                {
                                    worksheet.Cell(index + 2, attachmentStartIndex + j * attachmentLength).Value = attachments[j].Title;
                                    worksheet.Cell(index + 2, attachmentStartIndex + j * attachmentLength + 1).Value = attachments[j].Link;
                                }
                                index++;
                            }
                        }
                    }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {

                        FileDownloadName = $"SchoolCources_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
            
        }
    }
}
