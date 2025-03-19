using System.Text.RegularExpressions;
using Spectre.Console;

namespace iamryanmacdonald.Console.CodingTracker;

internal class UserInterface
{
    private readonly CodingSessionController _codingSessionController;

    internal UserInterface(Database database)
    {
        _codingSessionController = new CodingSessionController(database);
    }

    internal void MainMenu()
    {
        var closeApp = false;
        while (!closeApp)
        {
            System.Console.Clear();

            var actionChoice = AnsiConsole.Prompt(new SelectionPrompt<MenuAction>()
                .Title("What would you like to do?")
                .UseConverter(a => Regex.Replace(a.ToString(), "(\\B[A-Z])", " $1"))
                .AddChoices(Enum.GetValues<MenuAction>()));

            switch (actionChoice)
            {
                case MenuAction.NewCodingSession:
                    _codingSessionController.InsertItem();
                    break;
                case MenuAction.UpdateCodingSession:
                    _codingSessionController.UpdateItem();
                    break;
                case MenuAction.DeleteCodingSession:
                    _codingSessionController.DeleteItem();
                    break;
                case MenuAction.ViewCodingSessions:
                    _codingSessionController.ViewItems();
                    break;
                case MenuAction.CloseApplication:
                    AnsiConsole.MarkupLine("Goodbye!");
                    closeApp = true;
                    break;
            }

            AnsiConsole.MarkupLine("Press any key to continue...");
            System.Console.ReadLine();
        }
    }
}