using CatalogApi.Enums;

namespace CatalogApi.Pagination;

public class GenericParameters : IGenericParameters
{
    public int Page { get; set; } = 1;
    private const int MaxLimit = 25;
    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? 1 : (value > MaxLimit ? MaxLimit : value);
    }
    public string Search { get; set; } = "";
    public OrderEnum OrderByName { get; set; } = OrderEnum.Asc;
}