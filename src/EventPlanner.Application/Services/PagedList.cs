using Microsoft.EntityFrameworkCore;

namespace EventPlanner.Application.Services;

public class PagedList<T>
{
    public List<T> Items { get; private set; }
    public int Page { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public int TotalPages { get; private set; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    private PagedList(List<T> items, int count, int page, int pageSize)
    {
        Items = items;
        TotalCount = count;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
    
    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int page, int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 15;
        var count = await source.CountAsync();
        var items = await source
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>(items,  count, page, pageSize);
    }
    
}