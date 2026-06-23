using CSVImportExportApp.Data;
using CSVImportExportApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CSVImportExportApp.Services;

public class DataQueryService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public IQueryable<Person> GetQuery(QueryFilters filters)
    {
        var query = _context.Persons.AsQueryable();

        if (filters.FromDate.HasValue)
            query = query.Where(p => p.Date >= filters.FromDate.Value);

        if (filters.ToDate.HasValue)
            query = query.Where(p => p.Date <= filters.ToDate.Value);

        if (!string.IsNullOrWhiteSpace(filters.City))
            query = query.Where(p => p.City.Contains(filters.City));

        if (!string.IsNullOrWhiteSpace(filters.LastName))
            query = query.Where(p => p.LastName.Contains(filters.LastName));

        if (!string.IsNullOrWhiteSpace(filters.FirstName))
            query = query.Where(p => p.FirstName.Contains(filters.FirstName));

        if (!string.IsNullOrWhiteSpace(filters.SurName))
            query = query.Where(p => p.SurName.Contains(filters.SurName));

        if (!string.IsNullOrWhiteSpace(filters.Country))
            query = query.Where(p => p.Country.Contains(filters.Country));

        return query;
    }

    public async Task<int> GetTotalCountAsync(QueryFilters filters)
    {
        return await GetQuery(filters).CountAsync();
    }
}

public class QueryFilters
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? City { get; set; }
    public string? LastName { get; set; }
    public string? FirstName { get; set; }
    public string? SurName { get; set; }
    public string? Country { get; set; }
}