using Microsoft.EntityFrameworkCore;
using myApp.Helpers.QueryParameters;
using MyApp.Models;

namespace myApp.Helpers.Queryable;

public static class MovieQueryableExtension
{
    public static IQueryable<Movie> ApplyFilters(
        this IQueryable<Movie> query,
        MovieQueryParameters qp)
    {
        if (qp.MinYear.HasValue)
            query = query.Where(m => m.Year >= qp.MinYear.Value);

        if (qp.MaxYear.HasValue)
            query = query.Where(m => m.Year <= qp.MaxYear.Value);

        if (!string.IsNullOrWhiteSpace(qp.Genre))
            query = query.Where(m => m.Genre == qp.Genre);

        if (!string.IsNullOrWhiteSpace(qp.Search))
            query = query.Where(m => EF.Functions.Like(m.Title, $"%{qp.Search.Trim()}%"));

        return query;
    }

    public static IQueryable<Movie> ApplySort(this IQueryable<Movie> query, string? sort)
    {
        return sort switch
        {
            "title_asc" => query.OrderBy(m => m.Title),
            "title_desc" => query.OrderByDescending(m => m.Title),
            "year_asc" => query.OrderBy(m => m.Year),
            "year_desc" => query.OrderByDescending(m => m.Year),
            _ => query.OrderBy(m => m.Id)
        };
    }

}
