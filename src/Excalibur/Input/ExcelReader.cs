using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Pastel;

namespace Excalibur.Input;

public class ExcelReader : IInputReader<LineItem>
{
    private readonly Dictionary<PurchaseOrderColumns, string> columnMappings = new()
        {
            {PurchaseOrderColumns.PurchaseOrderNumber, "PurchaseOrderNumber"},
            {PurchaseOrderColumns.Contact, "Contact"},
            {PurchaseOrderColumns.Contact, "Date"},
            {PurchaseOrderColumns.DeliveryDate, "DeliveryDate"},
            {PurchaseOrderColumns.Reference, "Reference"},
            {PurchaseOrderColumns.Status, "Status"},
            {PurchaseOrderColumns.AccountCode, "Line Items.AccountCode"},
            {PurchaseOrderColumns.Description, "Line Items.Description"},
            {PurchaseOrderColumns.Quantity, "Line Items.Quantity"},
            {PurchaseOrderColumns.UnitAmount, "Line Items.UnitAmount"},
            {PurchaseOrderColumns.TaxType, "Line Items.TaxType"}
        };

    public IEnumerable<LineItem> ReadItems(string fileName)
    {
        using var workbook = new XSSFWorkbook(fileName);

        var sheet = workbook.GetSheetAt(0);

        var mappings = GetColumnMappings(sheet);

        if (!mappings.Any())
        {
            yield break;
        }

        var trackingName = GetTrackingCategoryName(mappings, sheet.GetRow(0));

        for (var i = 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            var item = new LineItem
            {
                PurchaseOrderNumber = row.GetString(mappings.GetValueOrDefault(PurchaseOrderColumns.PurchaseOrderNumber, int.MaxValue)),
                Contact = row.GetString(mappings.GetValueOrDefault(PurchaseOrderColumns.Contact, int.MaxValue)),
                Date = row.GetDateTime(mappings.GetValueOrDefault(PurchaseOrderColumns.Date, int.MaxValue)),
                DeliveryDate = row.GetDateTime(mappings.GetValueOrDefault(PurchaseOrderColumns.DeliveryDate, int.MaxValue)),
                Reference = row.GetString(mappings.GetValueOrDefault(PurchaseOrderColumns.Reference, int.MaxValue)),
                Status = row.GetString(mappings.GetValueOrDefault(PurchaseOrderColumns.Status, int.MaxValue)),
                AccountCode = row.GetString(mappings.GetValueOrDefault(PurchaseOrderColumns.AccountCode, int.MaxValue)),
                Description = row.GetString(mappings.GetValueOrDefault(PurchaseOrderColumns.Description, int.MaxValue)),
                Quantity = row.GetDecimal(mappings.GetValueOrDefault(PurchaseOrderColumns.Quantity, int.MaxValue)),
                UnitAmount = row.GetDecimal(mappings.GetValueOrDefault(PurchaseOrderColumns.UnitAmount, int.MaxValue)),
                TaxType = row.GetString(mappings.GetValueOrDefault(PurchaseOrderColumns.TaxType, int.MaxValue)),
                TrackingCategoryName = trackingName,
                TrackingCategoryOption = row.GetString(mappings.GetValueOrDefault(PurchaseOrderColumns.TrackingCategory, int.MaxValue))
            };

            yield return item;
        }
    }

    private Dictionary<PurchaseOrderColumns, int> GetColumnMappings(ISheet? sheet)
    {
        var mappings = new Dictionary<PurchaseOrderColumns, int>();

        if (sheet == null)
        {
            Console.WriteLine("WARNING: No sheet found in workbook");

            return mappings;
        }

        var row = sheet.GetRow(0);

        foreach (var key in columnMappings.Keys)
        {
            var column = GetColumn(row, columnMappings[key]);

            if (column != null)
            {
                mappings[key] = column.Value;
            }
        }

        var trackingColumn = GetTrackingColumn(row);

        if (trackingColumn != null)
        {
            mappings[PurchaseOrderColumns.TrackingCategory] = trackingColumn.Value;
        }

        return mappings;
    }

    private int? GetColumn(IRow? row, string name)
    {
        if (row == null)
        {
            return null;
        }

        for (var i = 0; i < row.Cells.Count; i++)
        {
            var value = row.Cells[i]?.ToString();

            if (name.Equals(value, StringComparison.OrdinalIgnoreCase))
            {
                return i;
            }
        }

        return null;
    }

    private int? GetTrackingColumn(IRow? row)
    {
        if (row == null)
        {
            return null;
        }

        for (var i = 0; i < row.Cells.Count; i++)
        {
            var value = row.Cells[i]?.ToString();

            if (value?.StartsWith("Tracking.", StringComparison.OrdinalIgnoreCase) == true)
            {
                return i;
            }
        }

        return null;
    }

    private string? GetTrackingCategoryName(Dictionary<PurchaseOrderColumns, int> mappings, IRow? row)
    {
        if (row == null)
        {
            return null;
        }

        if (!mappings.TryGetValue(PurchaseOrderColumns.TrackingCategory, out var column))
        {
            return null;
        }

        var value = row.GetString(column);

        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        if (!value.StartsWith("Tracking."))
        {
            Console.WriteLine("WARNING: Expected a tracking category of 'Tracking.[Name]' in column L".Pastel(ConsoleColor.Yellow));

            return null;
        }

        return value.Substring(9);
    }
}
