using Spectre.Console;
using System.Text.Json;

public sealed class JsonExporter : IDataExporter
{
    private record FindReferencesResult(string AssemblyFile, IList<string> Matches);

    private const string FileName = "report.json";
    private IList<FindReferencesResult> _results;
    private FindReferencesResult _currentResult;

    public void BeginScan(int totalFilesToScan)
    {
        _results = new List<FindReferencesResult>(totalFilesToScan);
    }

    public void EndScan()
    {
        var resultsJson = JsonSerializer.Serialize(_results);
        File.WriteAllText(FileName, resultsJson);
        AnsiConsole.MarkupLine($"[green]Results are saved to {FileName}[/]");
    }

    public void Scan(string fileName)
    {
        if (_currentResult is not null)
        {
            _results.Add(_currentResult);
        }

        _currentResult = new FindReferencesResult(fileName, new List<string>());
    }

    public void WriteMatch(TypeReference reference)
    {
        _currentResult.Matches.Add(reference.Type);
    }

    public void WriteError(Exception ex)
    {
    }
}
