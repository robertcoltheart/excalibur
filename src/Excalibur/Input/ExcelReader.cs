using NPOI.XSSF.UserModel;

namespace Excalibur.Input;

public class ExcelReader : IInputReader<LineItem>
{
    public IEnumerable<LineItem> ReadItems(string fileName)
    {
        using var workbook = new XSSFWorkbook(fileName);

        var sheet = workbook.GetSheetAt(0);

        for (var i = 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            var item = new LineItem
            {
                PurchaseOrderNumber = row.GetCell(0).ToString(),
                Contact = row.GetCell(1).ToString(),
                Date = row.GetCell(2).DateCellValue,
                DeliveryDate = row.GetCell(3).DateCellValue,
                Reference = row.GetCell(4).ToString(),
                Status = row.GetCell(5).ToString(),
                AccountCode = row.GetCell(6).ToString(),
                Description = row.GetCell(7).ToString(),
                Quantity = Convert.ToDecimal(row.GetCell(8).NumericCellValue),
                UnitAmount = Convert.ToDecimal(row.GetCell(9).NumericCellValue),
                TaxType = row.GetCell(10).ToString()
            };

            yield return item;
        }
    }
}
