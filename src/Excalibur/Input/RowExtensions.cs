using NPOI.SS.UserModel;

namespace Excalibur.Input;

public static class RowExtensions
{
    public static string? GetString(this IRow? row, int column)
    {
        return row?.GetCell(column).AsString();
    }

    public static decimal? GetDecimal(this IRow? row, int column)
    {
        return row?.GetCell(column).AsDecimal();
    }

    public static DateTime? GetDateTime(this IRow? row, int column)
    {
        return row?.GetCell(column).AsDateTime();
    }
}
