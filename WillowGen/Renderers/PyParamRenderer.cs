using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyParamRenderer(TypedParamDef elem) : IRenderable
{
    
    // TODO: we need need some type of mechanism to register an import and another to resolve it.
    //  That mechanism must consider cross-package imports and potentially duplicated names i.e.,
    //    from foo import baz
    //    from bar import baz # baz is already in use
    //
    // Probably want two things a top level completely unique reference and a file-scoped-reference
    //
    
    public void Render(Sink sink)
    {
        sink.Append(elem.Name());
        sink.AppendRaw(": " + GetTypeName(elem.ParamType));
    }

    private static string GetTypeName(BaseType elem)
    {
        return elem switch
        {
            ClassType _ => "UClass",
            DynArrayType ty => $"WrappedArray[{GetTypeName(ty.InnerType)}]",
            EngineBuiltinType ty => GetBuiltinType(ty),
            InterfaceType _ => "UClass",
            NamedType ty => GetRefTypeName(ty.Ref),
            StaticArrayType ty => $"WrappedArray[{GetTypeName(ty.HeldType)}]",
            UnhandledType ty => $"unresolved.{ty.HeldType.Name}", // TODO: this is just Maps and Delegates
            _ => throw new ArgumentOutOfRangeException(nameof(elem))
        };
    }

    private static string GetRefTypeName(RefNode elem)
    {
        if (elem.ResolvedTo == null)
        {
            // pretty sure all of these are classes - we can check by validating the parent is a package
            // i.e., Engine.StaticMesh -> Engine is a Package
            return "unresolved." + elem.TargetFullPath.Split('.').Last();
        }

        return elem.ResolvedTo! switch
        {
            ClassDef ty => ty.ExportPathName(),
            EnumDef ty => GetImportName(ty),
            StructDef ty => GetImportName(ty),
            _ => throw new Exception("invalid type hint: " + elem.ResolvedTo.Name())
        };

        static string GetImportName(BaseSymbol elem)
        {
            return elem switch
            {
                EnumDef ty => $"Enum<{ty.Name()}>",
                StructDef ty => $"Struct<{ty.Name()}>",
                _ => throw new Exception("unexpected type: " + elem.GetType().Name)
            };
        }
    }

    private static string GetBuiltinType(EngineBuiltinType elem)
    {
        return elem.Type switch
        {
            EngineBuiltin.Integer => "int",
            EngineBuiltin.Float => "float",
            EngineBuiltin.Bool => "bool",
            EngineBuiltin.Byte => "byte",
            EngineBuiltin.Name => "name",
            EngineBuiltin.String => "str",
            _ => throw new Exception("invalid builtin type: " + elem.Type)
        };
    }
}