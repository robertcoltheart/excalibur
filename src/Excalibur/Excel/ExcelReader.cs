using NPOI.XSSF.UserModel;

namespace Excalibur.Excel;

public class ExcelReader
{
    public IEnumerable<LineItem> ReadLineItems(string filename)
    {
        var workbook = new XSSFWorkbook(filename);
        var sheet = workbook.GetSheetAt(0);

        for (var i = 1; i <= sheet.LastRowNum; i++)
        {
            var row = sheet.GetRow(i);

            var item = new LineItem
            {
                PurchaseOrderNumber = row.GetCell(0).ToString(),
                Context = row.GetCell(1).ToString(),
                Date = row.GetCell(2).ToString(),
                DeliveryDate = row.GetCell(3).ToString(),
                Reference = row.GetCell(4).ToString(),
                Status = row.GetCell(5).ToString(),
                AccountCode = row.GetCell(6).ToString(),
                Description = row.GetCell(7).ToString(),
                Quantity = row.GetCell(8).ToString(),
                UnitAmount = row.GetCell(9).ToString(),
                TaxType = row.GetCell(10).ToString()
            };

            yield return item;
        }
    }
}
