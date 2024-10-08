using Microsoft.EntityFrameworkCore;

namespace Adda.API.Helpers;

public class PageList<T> : List<T>
{
    public PageList(IList<T> items, int count, int pageNumber, int pageSize)
    {
        CurrrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        PageSize = pageSize;
        TotalCount = count;
        AddRange(items);
    }

    public int CurrrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public static async Task<PageList<T>> CreateAsync(
        IQueryable<T> source,
        int pageNumber,
        int pageSize
    )
    {
        int count = await source.CountAsync();
        var item = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PageList<T>(item, count, pageNumber, pageSize);
    }
}
