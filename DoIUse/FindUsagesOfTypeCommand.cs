using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

public sealed class FindUsagesOfTypeCommand : Command<FindUsagesOfTypeCommand.Options>
{
    public sealed class Options : CommandSettings
    {
        [Description("Fully qualified name of the type to search for.")]
        [CommandArgument(0, "<searchType>")]
        public string Type { get; set; }

        [Description("Folder to scan for assemblies.")]
        [CommandArgument(1, "<searchFolder>")]
        public string Folder { get; set; }

        [Description("Fully qualified name of the type to search for.")]
        [CommandOption("-o|--output")]
        [DefaultValue("console")]
        public string Output { get; set; }
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Options settings)
    {
        IDataExporter externalExporter = settings.Output.ToLower() switch
        {
            "json" => new JsonExporter(),
            "html" => new HtmlExporter(settings.Type),
            _ => new NullExporter()
        };

        var exporter = new ConsoleExporter(externalExporter);
        var files = Directory.GetFiles(settings.Folder, "*.*", SearchOption.AllDirectories)
            .Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || f.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            .ToArray();
        exporter.BeginScan(files.Count());

        var visitor = new TypeFinderVisitor(exporter, settings.Type);
        foreach (var file in files)
        {
            try
            {
                exporter.Scan(file);
                var decompiler = new CSharpDecompiler(file, new DecompilerSettings { ThrowOnAssemblyResolveErrors = false });
                var tree = decompiler.DecompileWholeModuleAsSingleFile();
                tree.AcceptVisitor(visitor);
            }
            catch (PEFileNotSupportedException)
            {
            }
            catch (DecompilerException ex)
            {
                exporter.WriteError(ex);
            }

        }
        exporter.EndScan();

        return 0;
    }
}