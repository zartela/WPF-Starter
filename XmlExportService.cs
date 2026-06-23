using CSVImportExportApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace CSVImportExportApp.Services;

public class XmlExportService
{
    public async Task ExportAsync(IQueryable<Person> query, string filePath)
    {
        var settings = new XmlWriterSettings { Indent = true, Async = true };
        await using var writer = XmlWriter.Create(filePath, settings);

        await writer.WriteStartDocumentAsync();
        await writer.WriteStartElementAsync(null, "TestProgram", null);

        var index = 1;
        await foreach (var person in query.AsAsyncEnumerable())
        {
            await writer.WriteStartElementAsync(null, "Record", null);
            await writer.WriteAttributeStringAsync(null, "id", null, index.ToString());
            await writer.WriteElementStringAsync(null, "Date", null, person.Date.ToString("yyyy-MM-dd"));
            await writer.WriteElementStringAsync(null, "FirstName", null, person.FirstName);
            await writer.WriteElementStringAsync(null, "LastName", null, person.LastName);
            await writer.WriteElementStringAsync(null, "SurName", null, person.SurName);
            await writer.WriteElementStringAsync(null, "City", null, person.City);
            await writer.WriteElementStringAsync(null, "Country", null, person.Country);
            await writer.WriteEndElementAsync();

            index++;
        }

        await writer.WriteEndElementAsync();
        await writer.WriteEndDocumentAsync();
    }
}