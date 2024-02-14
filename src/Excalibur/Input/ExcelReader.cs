using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Excalibur.Input;

public class ExcelReader : IInputReader<LineItem>
{
    public IEnumerable<LineItem> ReadItems(string fileName)
    {
        using var workbook = new XSSFWorkbook(fileName);

        var sheet = workbook.GetSheetAt(0);

        var trackingName = GetTrackingCategoryName(sheet.GetRow(0).GetCell(11));

        for (var i = 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            var item = new LineItem
            {
                PurchaseOrderNumber = GetStringColumn(row.GetCell(0)),
                Contact = GetStringColumn(row.GetCell(1)),
                Date = GetDateColumn(row.GetCell(2)),
                DeliveryDate = GetDateColumn(row.GetCell(3)),
                Reference = GetStringColumn(row.GetCell(4)),
                Status = GetStringColumn(row.GetCell(5)),
                AccountCode = GetStringColumn(row.GetCell(6)),
                Description = GetStringColumn(row.GetCell(7)),
                Quantity = GetNumberColumn(row.GetCell(8)),
                UnitAmount = GetNumberColumn(row.GetCell(9)),
                TaxType = GetStringColumn(row.GetCell(10)),
                TrackingCategoryName = trackingName,
                TrackingCategoryOption = GetStringColumn(row.GetCell(11))
            };

            yield return item;
        }
    }

    private string? GetStringColumn(ICell? cell)
    {
        if (cell == null)
        {
            return null;
        }

        return cell.ToString();
    }

    private DateTime? GetDateColumn(ICell? cell)
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

    private decimal? GetNumberColumn(ICell? cell)
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

    private string? GetTrackingCategoryName(ICell? cell)
    {
        var name = cell?.ToString();

        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        var index = name!.IndexOf('.');

        return name.Substring(index + 1);
    }
}
