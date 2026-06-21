using UE3StubGenCore.Export;
using UE3StubGenCore.Render;
using WillowGen;

namespace UE3StubGenCli;

internal class LoggingExporter : IExporter
{
    public void Export(ExportModel model)
    {
        foreach (var pkg in model.Packages)
        {
            Console.WriteLine($"Package {pkg.Name()} has {pkg.Classes.Count} classes");
        }
    }

    public static void Main(string[] args)
    {
        try
        {
            var cli = new CommandLineArgs(args);

            var input = cli.Result.GetValue(cli.InputDirectory);
            var output = cli.Result.GetValue(cli.OutputDirectory);
            var importPath = cli.Result.GetValue(cli.ImportRoot);

            var model = new ExportModel(
                output!,
                importPath ?? string.Empty,
                new ExportContext(input!));
            model.ExportAll(new LoggingExporter());
            model.ExportAll(new WillowSdkGenerator());

            Console.WriteLine("Willow SDK generation complete");
            Console.WriteLine(" * it is recommended to run `ruff check . --fix && ruff format .` on the generated files");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}