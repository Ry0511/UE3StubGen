using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text.RegularExpressions;

namespace UE3StubGenCli;

public class CommandLineArgs
{
    public Option<string> InputDirectory { get; } = new("--input", "--in", "--i")
    {
        Description = "the directory containing decompressed .u files to export",
        Required = true,
    };

    public Option<string> OutputDirectory { get; } = new("--output", "--out", "--o")
    {
        Description = "the directory to output the generated files to",
        Required = true,
    };

    public Option<string> ImportRoot { get; } = new("--import-root")
    {
        Description = "denotes the base import path for generated files i.e., foo.baz which turns " +
                      "into `from foo.baz.WillowGame import WillowPlayerController` during generation",
        Required = false,
        DefaultValueFactory = _ => string.Empty,
    };

    public RootCommand Root { get; }

    public ParseResult Result { get; }

    public CommandLineArgs(string[] args)
    {
        InputDirectory.Validators.Add(ValidateInputDirectory);
        OutputDirectory.Validators.Add(ValidateInputDirectory);
        ImportRoot.Validators.Add(e =>
        {
            var text = e.GetValueOrDefault<string>();
            if (text.Length == 0)
            {
                return;
            }

            var re = new Regex(@"^\w+(\.\w+)*$");
            if (!re.IsMatch(e.GetValueOrDefault<string>()))
            {
                e.AddError(@"--import-root must match \w+(\.\w+)* i.e., foo.BaZ.BAR");
            }
        });
        Root = new RootCommand("UE3 Stub Generator")
        {
            InputDirectory,
            OutputDirectory,
            ImportRoot,
        };

        var ret = Root.Parse(args);
        if (ret.Errors.Count > 0)
        {
            foreach (var error in ret.Errors)
            {
                Console.WriteLine("[ERROR] " + error);
            }

            throw new Exception("one or more invalid arguments supplied");
        }

        Result = ret;
    }

    private static void ValidateInputDirectory(OptionResult result)
    {
        var path = result.GetValueOrDefault<string>();
        if (!Directory.Exists(path))
        {
            result.AddError($"Directory must exist {path}");
        }
    }
}