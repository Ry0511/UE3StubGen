using UELib;
using UELib.Core;
using UELib.Flags;
using UELib.Types;

namespace UE3StubGenCore.Export;

public class ExportField : BaseExport
{
    public string Name { get; }
    public PropertyType Type { get; }
    public string TargetTypeFullPath { get; }
    public bool IsOutParm { get; private set; }
    public bool IsOptionalParm { get; private set; }
    public bool IsReturnParm { get; private set; }
    public bool IsClassMember { get; private set; }
    public bool IsFunctionParameter { get; private set; }

    public ExportField(ExportContext ctx, UnrealPackage pkg, UProperty obj) : base(ctx, pkg, obj)
    {
        if (IsImport(obj))
        {
            obj = ctx.ResolveImport<UProperty>(obj);
        }

        Name = obj.Name;
        Type = obj.Type;

        // `optional out` is valid
        IsOutParm = obj.PropertyFlags.HasFlag(PropertyFlag.OutParm);
        IsOptionalParm = obj.PropertyFlags.HasFlag(PropertyFlag.OptionalParm);
        IsReturnParm = obj.PropertyFlags.HasFlag(PropertyFlag.ReturnParm);
        IsClassMember = obj.Outer is UClass;
        IsFunctionParameter = obj.Outer is UFunction;

        // TODO: this will probably need expanding and builtins will need to be identified i.e.,
        //  Core.Object, Core.IntProperty, etc
        TargetTypeFullPath = obj switch
        {
            UObjectProperty e => e.Object.GetPath(),
            UStructProperty e => e.Struct.GetPath(),
            _ => obj.Type.ToString(),
        };
    }
}