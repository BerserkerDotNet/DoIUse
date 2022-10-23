using Spectre.Console;

public sealed class ConsoleExporter : IDataExporter
{
    private readonly IDataExporter _exporter;
    private bool _isFinished;
    private ProgressTask _scanTask;
    private string _currentFile;

    public ConsoleExporter(IDataExporter exporter)
    {
        _exporter = exporter;
    }

    public void BeginScan(int totalFilesToScan)
    {

        AnsiConsole.Progress()
            .AutoRefresh(false)
            .AutoClear(false)
            .HideCompleted(false)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new ElapsedTimeColumn(),
                new SpinnerColumn(),
            })
            .StartAsync(async ctx =>
            {
                _scanTask = ctx.AddTask("[green]Scaninng assemblies[/]", maxValue: totalFilesToScan);
                while (!_isFinished)
                {
                    await Task.Delay(10);
                    ctx.Refresh();

                    _scanTask.Description = $"[green]Scaninng {_currentFile}[/]";
                }
            });
        _exporter.BeginScan(totalFilesToScan);
    }

    public void EndScan()
    {
        _isFinished = true;
        _exporter.EndScan();
    }

    public void Scan(string fileName)
    {
        _exporter.Scan(fileName);
        _currentFile = fileName;
        _scanTask.Increment(1);
    }

    public void WriteError(Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
        _exporter.WriteError(ex);
    }

    public void WriteMatch(TypeReference reference)
    {
        AnsiConsole.MarkupLine($"[yellow]Usage in {new FileInfo(_currentFile).Name}. Detected in {reference.Type}[/]");
        _exporter.WriteMatch(reference);
    }
}
