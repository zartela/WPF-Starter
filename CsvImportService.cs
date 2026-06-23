using CsvHelper;
using CsvHelper.Configuration;
using CSVImportExportApp.Data;
using CSVImportExportApp.Models;
using System.Globalization;

namespace CSVImportExportApp.Services;

public class CsvImportService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task ClearTableAsync()
    {
        _context.Persons.RemoveRange(_context.Persons);
        await _context.SaveChangesAsync();
    }

    public async Task<int> ImportAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = null
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        var records = csv.GetRecords<dynamic>();
        var batch = new List<Person>();
        var total = 0;

        foreach (var row in records)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var person = new Person
            {
                Date = DateTime.Parse(row.Date),
                FirstName = row.FirstName?.ToString() ?? "",
                LastName = row.LastName?.ToString() ?? "",
                SurName = row.SurName?.ToString() ?? "",
                City = row.City?.ToString() ?? "",
                Country = row.Country?.ToString() ?? ""
            };

            batch.Add(person);
            total++;

            if (batch.Count >= 1000)
            {
                await _context.Persons.AddRangeAsync(batch, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                batch.Clear();
            }
        }

        if (batch.Count != 0)
        {
            await _context.Persons.AddRangeAsync(batch, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return total;
    }
}