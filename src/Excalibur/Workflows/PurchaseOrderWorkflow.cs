using DustInTheWind.ConsoleTools.Controls.Menus;
using Excalibur.Excel;

namespace Excalibur.Workflows;

public class PurchaseOrderWorkflow : Workflow
{
    public async Task ProcessPurchaseOrder()
    {
        var config = await Authenticate();

        var items = GetWorkbookItems().ToArray();
    }

    private IEnumerable<LineItem> GetWorkbookItems()
    {
        var sheets = Directory.GetFiles(Environment.CurrentDirectory, "*.xls*");

        var excelMenu = new TextMenu
        {
            EraseAfterClose = true,
            QuestionText = "Select input Excel file: "
        };

        for (var i = 0; i < sheets.Length; i++)
        {
            excelMenu.AddItem(new TextMenuItem
            {
                Id = (i + 1).ToString(),
                Text = Path.GetFileName(sheets[i])
            });
        }

        excelMenu.Display();

        Console.WriteLine($"Processing {excelMenu.SelectedItem.Text}");

        var excel = new ExcelReader();

        return excel.ReadLineItems(sheets[excelMenu.SelectedIndex!.Value]);
    }
}
