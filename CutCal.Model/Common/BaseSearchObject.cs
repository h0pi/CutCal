namespace CutCal.Model.Common;

public class BaseSearchObject
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public bool IncludeTotalCount { get; set; } = true;
    public string? SortBy { get; set; }
}
