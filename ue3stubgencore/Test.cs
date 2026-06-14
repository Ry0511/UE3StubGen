using System.Text;
using ue3stubgencore.export;
using UELib;
using UELib.Core;

namespace ue3stubgencore;

public static class UClassInspector
{
    public static void Hello()
    {
        string root = @"C:\mod_tools\decompressed_packages\BL1\decompressed";
        ExportContext ctx = new(root);
        var pkg = ctx.GetPackage("Engine")!;

        StringBuilder text = new();
        foreach (var obj in pkg.Objects)
        {
            if (obj is UClass cls)
            {
                try
                {
                    ExportClass e = new(ctx, pkg, cls);
                    if (e.IsBuiltinClass)
                    {
                        continue;
                    }

                    StringBuilder sb = new();
                    PrintExportClass(sb, e);
                    text.AppendLine(sb.ToString());
                }
                catch (Exception)
                {
                    Console.WriteLine($"Failed to export {cls.GetReferencePath()}");
                }
            }
        }

        File.WriteAllText(root + @"\log.txt", text.ToString());
    }

    private static void PrintExportClass(StringBuilder text, ExportClass e)
    {
        text.Append($"class {e.Name}(");
        text.Append(e.Super?.Name ?? "UObject");

        if (e.Interfaces.Count > 0)
        {
            text.Append(", ");
            text.AppendJoin(", ", e.Interfaces.Select(i => i.Name));
        }

        text.AppendLine("):");

        foreach (var f in e.Fields)
        {
            if (!f.IsClassMember)
            {
                throw new Exception("this should not happen");
            }

            text.AppendLine($"  {f.Name}: {f.Type.ToString()}");
        }

        if (e.Functions.Count > 0)
        {
            text.AppendLine();
        }

        foreach (var f in e.Functions)
        {
            if (!f.IsRegularFunction)
            {
                continue;
            }

            if (f.IsStatic)
            {
                text.AppendLine("  @staticmethod");
            }

            text.Append($"  def {f.Name}(");

            List<string> parms = new();
            foreach (var p in f.Parameters)
            {
                StringBuilder parm = new();

                parm.Append(p.Name + ": ");

                if (p.IsOutParm)
                {
                    parm.Append("Out[");
                }

                if (p.IsOptionalParm)
                {
                    parm.Append("Optional[");
                }

                parm.Append(p.Type.ToString());

                if (p.IsOptionalParm)
                {
                    parm.Append("]");
                }

                if (p.IsOutParm)
                {
                    parm.Append("]");
                }

                parms.Add(parm.ToString());
            }

            text.AppendJoin(", ", parms);
            text.Append(") -> ");

            if (f.HasReturnParameter && f.HasOutParms)
            {
                text.Append("Tuple[");
                text.Append(f.ReturnParameter!.Type.ToString());
                text.Append(", ");
                text.AppendJoin(", ", f.OutParameters.Select(p => p.Type.ToString()));
                text.Append("]");
            }
            else if (f.HasReturnParameter)
            {
                text.Append(f.ReturnParameter!.Type.ToString());
            }
            else if (f.HasOutParms)
            {
                text.Append("Tuple[");
                text.AppendJoin(", ", f.OutParameters.Select(p => p.Type.ToString()));
                text.Append("]");
            }
            else
            {
                text.Append("None");
            }

            text.AppendLine(":");

            UnrealConfig.Indention = "  ";
            UnrealConfig.SuppressSignature = true;
            UnrealConfig.SuppressComments = true;
            UnrealConfig.PreBeginBracket = " ";
            UnrealConfig.PreEndBracket = "\r\n{0}";

            StringBuilder decomp = new();
            decomp.AppendLine("\"\"\"");
            decomp.Append(f.ObjectHandle.Decompile());
            decomp.AppendLine("\n    \"\"\"");
            decomp.AppendLine("...");

            foreach (var line in decomp.ToString().Split("\r\n"))
            {
                text.AppendLine($"    {line}");
            }
        }
    }
}