using Spectre.Console;

namespace iamryanmacdonald.Console.CodingTracker;

internal class CodingSessionController
{
    private readonly Database _database;

    internal CodingSessionController(Database database)
    {
        _database = database;
    }

    internal void DeleteItem()
    {
        var deletableCodingSessions = _database.GetCodingSessions();

        if (deletableCodingSessions.Count == 0)
        {
            AnsiConsole.MarkupLine("No coding sessions found.");
        }
        else
        {
            var codingSessionToDelete = AnsiConsole.Prompt(new SelectionPrompt<CodingSession>()
                .Title("Select a coding session to delete:").UseConverter(cs => $"{cs.StartTime:f} -> {cs.EndTime:f}")
                .AddChoices(deletableCodingSessions));

            _database.DeleteCodingSession(codingSessionToDelete.Id);

            AnsiConsole.MarkupLine(
                $"[green]Coding session that went from {codingSessionToDelete.StartTime:f} to {codingSessionToDelete.EndTime:f} deleted successfully![/]");
        }
    }

    internal void InsertItem()
    {
        var start = DateTime.Now;

        var startConfirmation = false;
        while (!startConfirmation)
        {
            start = AnsiConsole.Prompt(
                new TextPrompt<DateTime>(
                        "Enter a start [yellow]date[/] and [yellow]time[/] (in 24hr format) for the coding session: (Format: yyyy-mm-dd hh:mm:ss)")
                    .DefaultValue(start).WithConverter(dt => dt.ToString("yyyy-MM-dd HH:mm:ss"))
            );
            if (start.TimeOfDay == TimeSpan.Zero)
                startConfirmation =
                    AnsiConsole.Prompt(new TextPrompt<bool>("Your start time is midnight. Is this okay?")
                        .AddChoice(true).AddChoice(false).DefaultValue(false)
                        .WithConverter(choice => choice ? "y" : "n"));
            else
                startConfirmation = true;
        }

        var end = DateTime.Now;

        var endConfirmation = false;
        while (!endConfirmation)
        {
            end = AnsiConsole.Prompt(
                new TextPrompt<DateTime>(
                        "Enter an end [yellow]date[/] and [yellow]time[/] (in 24hr format) for the coding session: (Format: yyyy-mm-dd hh:mm:ss)")
                    .DefaultValue(end).WithConverter(dt => dt.ToString("yyyy-MM-dd HH:mm:ss"))
            );

            if (end <= start)
            {
                AnsiConsole.MarkupLine("[red]Your end date and time must be after your start date and time.[/]");
            }
            else
            {
                if (end.TimeOfDay == TimeSpan.Zero)
                    endConfirmation =
                        AnsiConsole.Prompt(new TextPrompt<bool>("Your end time is midnight. Is this okay?")
                            .AddChoice(true).AddChoice(false).DefaultValue(false)
                            .WithConverter(choice => choice ? "y" : "n"));
                else
                    endConfirmation = true;
            }
        }

        var duration = CodingSession.CalculateDuration(start, end);
        var durationTimeSpan = TimeSpan.FromSeconds(duration);
        _database.InsertCodingSession(start, end, duration);
        AnsiConsole.MarkupLine(
            $"Successfully added coding session from {start:f} to {end:f} ({durationTimeSpan:%h} hours, {durationTimeSpan:%m} minutes, {durationTimeSpan:%s} seconds)");
    }

    internal void UpdateItem()
    {
        var updateableCodingSessions = _database.GetCodingSessions();

        if (updateableCodingSessions.Count == 0)
        {
            AnsiConsole.MarkupLine("No coding sessions found.");
        }
        else
        {
            var codingSessionToUpdate = AnsiConsole.Prompt(new SelectionPrompt<CodingSession>()
                .Title("Select a coding session to update:").UseConverter(cs => $"{cs.StartTime:f} -> {cs.EndTime:f}")
                .AddChoices(updateableCodingSessions));

            var updatedStart = codingSessionToUpdate.StartTime;

            var startConfirmation = false;
            while (!startConfirmation)
            {
                updatedStart = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>(
                            "Enter a start [yellow]date[/] and [yellow]time[/] (in 24hr format) for the coding session: (Format: yyyy-mm-dd hh:mm:ss)")
                        .DefaultValue(updatedStart).WithConverter(dt => dt.ToString("yyyy-MM-dd HH:mm:ss"))
                );
                if (updatedStart.TimeOfDay == TimeSpan.Zero)
                    startConfirmation =
                        AnsiConsole.Prompt(new TextPrompt<bool>("Your start time is midnight. Is this okay?")
                            .AddChoice(true).AddChoice(false).DefaultValue(false)
                            .WithConverter(choice => choice ? "y" : "n"));
                else
                    startConfirmation = true;
            }

            var updatedEnd = codingSessionToUpdate.EndTime;

            var endConfirmation = false;
            while (!endConfirmation)
            {
                updatedEnd = AnsiConsole.Prompt(
                    new TextPrompt<DateTime>(
                            "Enter an end [yellow]date[/] and [yellow]time[/] (in 24hr format) for the coding session: (Format: yyyy-mm-dd hh:mm:ss)")
                        .DefaultValue(updatedEnd).WithConverter(dt => dt.ToString("yyyy-MM-dd HH:mm:ss"))
                );

                if (updatedEnd <= updatedStart)
                {
                    AnsiConsole.MarkupLine("[red]Your end date and time must be after your start date and time.[/]");
                }
                else
                {
                    if (updatedEnd.TimeOfDay == TimeSpan.Zero)
                        endConfirmation =
                            AnsiConsole.Prompt(new TextPrompt<bool>("Your end time is midnight. Is this okay?")
                                .AddChoice(true).AddChoice(false).DefaultValue(false)
                                .WithConverter(choice => choice ? "y" : "n"));
                    else
                        endConfirmation = true;
                }
            }

            var updatedDuration = CodingSession.CalculateDuration(updatedStart, updatedEnd);
            var updatedDurationTimeSpan = TimeSpan.FromSeconds(updatedDuration);
            _database.UpdateCodingSession(codingSessionToUpdate.Id, updatedStart, updatedEnd, updatedDuration);
            AnsiConsole.MarkupLine(
                $"Successfully updated coding session to be from {updatedStart:f} to {updatedEnd:f} ({updatedDurationTimeSpan:%h} hours, {updatedDurationTimeSpan:%m} minutes, {updatedDurationTimeSpan:%s} seconds)");
        }
    }

    internal void ViewItems()
    {
        var codingSessions = _database.GetCodingSessions();

        if (codingSessions.Count == 0)
        {
            AnsiConsole.MarkupLine("No coding sessions found.");
        }
        else
        {
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.Title("[bold underline]Coding Sessions[/]");

            table.AddColumn("Start Time");
            table.AddColumn("End Time");
            table.AddColumn("Duration");

            foreach (var codingSession in codingSessions)
            {
                var duration = TimeSpan.FromSeconds(codingSession.Duration);

                table.AddRow(codingSession.StartTime.ToString("f"), codingSession.EndTime.ToString("f"),
                    $"{duration:%h} hours, {duration:%m} minutes, {duration:%s} seconds");
            }


            AnsiConsole.Write(table);
        }
    }
}