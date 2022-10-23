using Spectre.Console;
using System.Text;
using System.Text.RegularExpressions;

public sealed class HtmlExporter : IDataExporter
{
    private record FindReferencesResult(string AssemblyFile, IList<string> Matches);

    private const string FileName = "report.html";
    private readonly string _typeToFind;
    private string _currentFile = string.Empty;
    private StringBuilder _data = new StringBuilder();

    public HtmlExporter(string typeToFind)
    {
        _typeToFind = typeToFind;
    }

    public void BeginScan(int totalFilesToScan)
    {
    }

    public void EndScan()
    {
        var template = File.ReadAllText("report_template.html");
        template = Regex.Replace(template, "{Title}", $"Usages of {_typeToFind}");
        template = Regex.Replace(template, "{Data}", _data.ToString());
        File.WriteAllText(FileName, template);

        AnsiConsole.MarkupLine($"[green]Results are saved to {FileName}[/]");
    }

    public void Scan(string fileName)
    {
        _currentFile = fileName;
    }

    public void WriteMatch(TypeReference reference)
    {
        _data.AppendLine("<tr>");
        _data.AppendLine($"<td>{_currentFile}</td>");
        _data.AppendLine($"<td>{reference.Type}</td>");
        _data.AppendLine("</tr>");
    }

    public void WriteError(Exception ex)
    {
        _data.AppendLine("<tr>");
        _data.AppendLine($"<td>{_currentFile}</td>");
        _data.AppendLine($"<td class=\"table-danger\">{ex.Message}</td>");
        _data.AppendLine("</tr>");
    }
}
