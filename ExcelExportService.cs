using ClosedXML.Excel;
using CSVImportExportApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CSVImportExportApp.Services;

public class ExcelExportService
{
    public async Task ExportAsync(IQueryable<Person> query, string filePath)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Data");

        worksheet.Cell(1, 1).Value = "Date";
        worksheet.Cell(1, 2).Value = "First Name";
        worksheet.Cell(1, 3).Value = "Last Name";
        worksheet.Cell(1, 4).Value = "Sur Name";
        worksheet.Cell(1, 5).Value = "City";
        worksheet.Cell(1, 6).Value = "Country";

        var row = 2;

        await foreach (var person in query.AsAsyncEnumerable())
        {
            worksheet.Cell(row, 1).Value = person.Date;
            worksheet.Cell(row, 2).Value = person.FirstName;
            worksheet.Cell(row, 3).Value = person.LastName;
            worksheet.Cell(row, 4).Value = person.SurName;
            worksheet.Cell(row, 5).Value = person.City;
            worksheet.Cell(row, 6).Value = person.Country;

            row++;
        }

        worksheet.Columns().AdjustToContents();
        workbook.SaveAs(filePath);
    }
}