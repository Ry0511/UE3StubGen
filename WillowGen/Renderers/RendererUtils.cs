using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;

namespace WillowGen.Renderers;

public static class RendererUtils
{
    public static string GetTypeName(BaseType elem, NamingScope scope)
    {
        return elem switch
        {
            ClassType _ => "UClass",
            DynArrayType ty => $"WrappedArray[{GetTypeName(ty.InnerType, scope)}]",
            EngineBuiltinType ty => GetBuiltinType(ty),
            InterfaceType ty => GetTypeName(ty.InterfaceClass, scope),
            NamedType ty => GetNamedTypeName(ty, scope),
            StaticArrayType ty => $"WrappedArray[{GetTypeName(ty.HeldType, scope)}]",
            DelegateType ty => $"Delegate[{CreateDelegateSignature(ty, scope)}]",
            UnhandledType _ => "Any", // maps fall into this category
            _ => throw new ArgumentOutOfRangeException(nameof(elem)),
        };
    }

    public static string GetReturnTypeName(BaseType elem, NamingScope scope)
    {
        return elem switch
        {
            ClassType _ => "UClass",
            DynArrayType ty => $"WrappedArray[{GetReturnTypeName(ty.InnerType, scope)}]",
            EngineBuiltinType ty => GetBuiltinType(ty),
            InterfaceType ty => GetReturnTypeName(ty.InterfaceClass, scope),
            NamedType ty => GetNamedTypeName(ty, scope),
            StaticArrayType ty => $"WrappedArray[{GetReturnTypeName(ty.HeldType, scope)}]",
            DelegateType ty => $"{CreateDelegateSignature(ty, scope)}",
            UnhandledType _ => "Any", // maps fall into this category
            _ => throw new ArgumentOutOfRangeException(nameof(elem)),
        };
    }

    public static string GetNamedTypeName(NamedType elem, NamingScope scope)
    {
        var name = GetRefTypeName(elem.Ref, scope);
        if (elem.IsEnumRef())
        {
            return name + " | int";
        }

        return name;
    }

    public static string GetRefTypeName(RefNode elem, NamingScope scope)
    {
        if (elem.ResolvedTo == null)
        {
            // pretty sure all of these are classes - we can check by validating the parent is a package
            // i.e., Engine.StaticMesh -> Engine is a Package
            var split = elem.TargetFullPath.Split('.');

            // direct child of a module is a class
            if (elem.AllModules().Any(e => e.Name() == split[^2]))
            {
                return $"Unresolved[Literal[\"{elem.TargetFullPath}\"]]";
            }

            return "Any";
        }

        return elem.ResolvedTo! switch
        {
            ClassDef ty => scope.LocalName(ty, ty.Name()),
            EnumDef ty => $"{scope.LocalName(ty, ty.Name())}",
            StructDef ty => $"{scope.LocalName(ty, ty.Name())}",
            _ => throw new Exception("invalid type hint: " + elem.ResolvedTo.Name()),
        };
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
            _ => throw new Exception("invalid builtin type: " + elem.Type),
        };
    }

    public static string CreateDelegateSignature(DelegateType elem, NamingScope scope)
    {
        if (elem.Function.ResolvedTo == null)
        {
            throw new Exception("unresolved delegate");
        }

        if (elem.Function.ResolvedTo is not FunctionDef func)
        {
            throw new Exception("invalid delegate");
        }

        return CreateDelegateSignature(func, scope);
    }

    public static string CreateDelegateSignature(FunctionDef func, NamingScope? scope = null)
    {
        if (scope == null)
        {
            return func.Name() + "Fn";
        }

        return scope.LocalName(func, func.Name() + "Fn");
    }
}