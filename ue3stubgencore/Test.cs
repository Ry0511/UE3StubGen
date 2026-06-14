using System.Text;
using ue3stubgencore.export;
using UELib;
using UELib.Core;
using UELib.Flags;

namespace ue3stubgencore;

public static class UClassInspector
{
    public static void Hello()
    {
        string root = @"C:\mod_tools\decompressed_packages\BL1\decompressed";
        ExportContext ctx = new(root, loadAll: false);
        var pkg = ctx.LoadPackage("Engine")!;
        
        StringBuilder sb = new();
        foreach (var obj in pkg.Objects)
        {
            if (obj is UClass cls && cls.Name == "AttributeInitializationDefinition")
            {
                ExportClass test = new(ctx, pkg, cls);

                sb.Append($"class {cls.Name}");
                if (cls.ImplementedInterfaces?.Count > 0 || cls.Super != null)
                {
                    sb.Append("(");
                    if (cls.Super != null)
                    {
                        sb.Append(cls.Super.Name);
                    }

                    if (cls.ImplementedInterfaces?.Count > 0)
                    {
                        sb.AppendJoin(",", cls.ImplementedInterfaces);
                    }

                    sb.Append(")");
                }

                sb.AppendLine();
                sb.AppendLine();

                foreach (var elem in cls.EnumerateFields<UEnum>())
                {
                    sb.AppendLine($"  enum {elem.Name}:");
                    for (int i = 0; i < elem.Names.Count; i++)
                    {
                        sb.AppendLine($"    {elem.Names[i]} = {i}");
                    }

                    sb.AppendLine();
                }

                foreach (var elem in cls.EnumerateFields<UScriptStruct>())
                {
                    sb.AppendLine($"  struct {elem.Name}:");
                    foreach (var prop in elem.EnumerateFields<UProperty>())
                    {
                        sb.AppendLine($"    {prop.Name}: {prop.Type}");
                    }

                    sb.AppendLine();
                }

                foreach (var prop in cls.EnumerateFields<UProperty>())
                {
                    sb.AppendLine($"  {prop.Name}: {prop.Type}");
                }

                sb.AppendLine();
                foreach (var func in cls.EnumerateFields<UFunction>())
                {
                    if (func.FunctionFlags.HasFlag(FunctionFlag.Static))
                    {
                        sb.AppendLine("  @staticmethod");
                    }

                    StringBuilder parms = new();
                    List<string> parmsList = new();
                    foreach (var elem in func.EnumerateFields<UProperty>())
                    {
                        var optional = elem.PropertyFlags.HasFlag(PropertyFlag.OptionalParm);
                        var outparm = elem.PropertyFlags.HasFlag(PropertyFlag.OutParm);

                        ExportField f = new(ctx, pkg, elem);

                        StringBuilder parm = new();
                        parm.Append($"{elem.Name}: ");

                        if (optional)
                        {
                            parm.Append("Optional[");
                            parm.Append(f.TypeName);
                            parm.Append("]");
                        }
                        else if (outparm)
                        {
                            parm.Append("Out[");
                            parm.Append(f.TypeName);
                            parm.Append("]");
                        }
                        else
                        {
                            parm.Append(f.TypeName);
                        }

                        parmsList.Add(parm.ToString());
                    }

                    parms.AppendJoin(", ", parmsList);

                    sb.AppendLine(
                        $"  def {func.Name}({parms.ToString()}) -> {func.ReturnProperty.Type}:");
                }

                sb.AppendLine();
            }
        }

        Console.WriteLine(sb);
    }
}