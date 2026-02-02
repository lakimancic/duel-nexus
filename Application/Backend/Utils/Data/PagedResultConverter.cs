using AutoMapper;

namespace Backend.Utils.Data;

public class PagedResultConverter<TSource, TDestination>
    : ITypeConverter<PagedResult<TSource>, PagedResult<TDestination>>
{
    public PagedResult<TDestination> Convert(
        PagedResult<TSource> source,
        PagedResult<TDestination> destination,
        ResolutionContext context)
    {
        return new PagedResult<TDestination>
        {
            Items = context.Mapper.Map<List<TDestination>>(source.Items),
            TotalCount = source.TotalCount,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            TotalPages = source.TotalPages
        };
    }
}
