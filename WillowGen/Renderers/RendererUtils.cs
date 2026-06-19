using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;

namespace WillowGen.Renderers;

public static class RendererUtils
{
    public static IRenderable Create(BaseElement elem)
    {
        return elem switch
        {
            ClassDef e => new PyClassRenderer(e),
            StructDef e => new PyStructRenderer(e),
            EnumDef e => new PyEnumRenderer(e),
            FunctionDef e => e.IsDelegate ? new PyDelegateRenderer(e) : new PyFunctionRenderer(e),
            TypedParamDef e => new PyParamRenderer(e),
            _ => throw new Exception("unsupported element type: " + elem.GetType().Name + "")
        };
    }

    public static string GetTypeName(BaseType elem)
    {
        return elem switch
        {
            ClassType _ => "UClass",
            DynArrayType ty => $"WrappedArray[{GetTypeName(ty.InnerType)}]",
            EngineBuiltinType ty => GetBuiltinType(ty),
            InterfaceType ty => GetTypeName(ty.InterfaceClass),
            NamedType ty => GetRefTypeName(ty.Ref),
            StaticArrayType ty => $"WrappedArray[{GetTypeName(ty.HeldType)}]",
            DelegateType ty => CreateDelegateSignature(ty),
            UnhandledType _ => "Any", // maps fall into this category
            _ => throw new ArgumentOutOfRangeException(nameof(elem))
        };
    }

    public static string GetRefTypeName(RefNode elem)
    {
        if (elem.ResolvedTo == null)
        {
            // pretty sure all of these are classes - we can check by validating the parent is a package
            // i.e., Engine.StaticMesh -> Engine is a Package

            var split = elem.TargetFullPath.Split('.');

            // direct child of a module is a class
            if (elem.AllModules().Any(e => e.Name() == split[^2]))
            {
                return "UnresolvedClass";
            }

            return "Any";
        }

        return elem.ResolvedTo! switch
        {
            ClassDef ty => ty.Name(),
            EnumDef ty => GetImportName(ty),
            StructDef ty => GetImportName(ty),
            _ => throw new Exception("invalid type hint: " + elem.ResolvedTo.Name())
        };

        static string GetImportName(BaseSymbol elem)
        {
            return elem switch
            {
                EnumDef ty => $"Enum[{ty.Name()}]",
                StructDef ty => $"Struct[{ty.Name()}]",
                _ => throw new Exception("unexpected type: " + elem.GetType().Name)
            };
        }
    }

    public static string GetBuiltinType(EngineBuiltinType elem)
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

    public static string CreateDelegateSignature(DelegateType elem)
    {
        if (elem.Function.ResolvedTo == null)
        {
            throw new Exception("unresolved delegate");
        }

        if (elem.Function.ResolvedTo is not FunctionDef func)
        {
            throw new Exception("invalid delegate");
        }

        return "name | " + "_delegate_" + func.Name();
    }
}