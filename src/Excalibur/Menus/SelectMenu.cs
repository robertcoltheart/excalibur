using DustInTheWind.ConsoleTools.Controls.Menus;
using Pastel;

namespace Excalibur.Menus;

public class SelectMenu<T>
{
    private readonly string question;

    private readonly T[] items;

    private readonly Func<T, string> keySelector;

    private readonly Func<T, string> valueSelector;

    public SelectMenu(string question, T[] items, Func<T, string> keySelector, Func<T, string> valueSelector)
    {
        this.question = question;
        this.items = items;
        this.keySelector = keySelector;
        this.valueSelector = valueSelector;
    }

    public string GetSelected()
    {
        var menu = new TextMenu
        {
            QuestionText = $"{question}: ".Pastel(ConsoleColor.Yellow)
        };

        for (var i = 1; i <= items.Length; i++)
        {
            var item = items[i - 1];

            menu.AddItem(new TextMenuItem
            {
                Id = i.ToString(),
                Text = valueSelector(item)
            });
        }

        menu.Display();

        return keySelector(items[menu.SelectedIndex!.Value]);
    }
}
