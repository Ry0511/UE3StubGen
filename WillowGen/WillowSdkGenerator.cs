using System.Text;
using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using UE3StubGenCore.Render;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    private static int _indent = 0;

    public void Export(ExportModel model)
    {
        MultiModuleProject py = new(model);

        var cls = py
            .Descendants()
            .OfType<ClassDef>()
            .Where(e => e.Functions.Count == 5)
            .FirstOrDefault(e => e.Enums.Count > 0);

        StringBuilder sb = new();
        RenderPyClass(sb, cls!);
        Console.WriteLine("# CLASS RENDER");
        Console.WriteLine(sb);
    }

    private static void RenderPyClass(StringBuilder sb, ClassDef cls)
    {
        var parents = new List<string>();

        // this is basically never true since UObject/Object is always the super
        if (cls.Super != null)
        {
            parents.Add(cls.Super.ResolvedTo?.Name() ?? "__UnresolvedSymbol__");
        }

        parents.AddRange(
            cls.Interfaces
                .Select(iface => iface.ResolvedTo?.Name() ?? "__UnresolvedSymbol__")
        );

        sb.AppendLine($"class {cls.Name()}({string.Join(", ", parents)}):");
        _indent += 2;
        RenderPyClassParams(sb, cls);
        RenderPyClassMethods(sb, cls);
        if (cls.Fields.Count == 0 && cls.Functions.Count == 0)
        {
            sb.AppendLine($"{new string(' ', _indent)}...");
        }

        _indent -= 2;
    }

    private static void RenderPyClassParams(StringBuilder sb, ClassDef cls)
    {
        var indent = new string(' ', _indent);
        foreach (var field in cls.Fields)
        {
            sb.AppendLine($"{indent}{field.Name()}: {field.ParamType.Name()}");
        }

        sb.AppendLine();
    }

    private static void RenderPyClassMethods(StringBuilder sb, ClassDef cls)
    {
        var indent = new string(' ', _indent);
        bool lastWasStatic = false;
        foreach (var func in cls.Functions)
        {
            if (func.IsStatic)
            {
                sb.AppendLine($"{indent}@staticmethod");
            }

            sb.Append($"{indent}def {func.Name()}(");
            bool first = true;
            foreach (var param in func.Params)
            {
                if (!first) sb.Append(", ");
                first = false;
                sb.Append($"{param.Name()}: {param.ParamType.Name()}");
            }

            sb.AppendLine($") -> {func.ReturnValue?.ParamType.Name() ?? "None"}: ...");

            if (lastWasStatic && !func.IsStatic)
            {
                sb.AppendLine();
            }

            lastWasStatic = func.IsStatic;
        }
    }
}