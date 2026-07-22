namespace CatalogApi.Pagination;

public class ProductsParameters
{
    public int Page { get; set; } = 1;
    
    private const int MaxLimit = 25;
    private int _pageSize;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value >  MaxLimit ? MaxLimit : value;
    }
}