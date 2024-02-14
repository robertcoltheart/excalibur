using NPOI.SS.UserModel;

namespace Excalibur.Input;

public static class CellExtensions
{
    public static string? AsString(this ICell? cell)
    {
        return cell?.ToString();
    }

    public static decimal? AsDecimal(this ICell? cell)
    {
        if (cell == null)
        {
            return null;
        }

        if (cell.CellType == CellType.String)
        {
            return decimal.Parse(cell.ToString()!);
        }

        return Convert.ToDecimal(cell.NumericCellValue);
    }

    public static DateTime? AsDateTime(this ICell? cell)
    {
        if (cell == null)
        {
            return null;
        }

        if (cell.CellType == CellType.String)
        {
            return DateTime.Parse(cell.ToString()!);
        }

        return cell.DateCellValue;
    }
}
