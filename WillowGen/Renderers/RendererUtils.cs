using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;
using UELib.Core;

namespace WillowGen.Renderers;

public static class RendererUtils
{
    public static IRenderable Create(BaseElement elem, NamingScope scope)
    {
        return elem switch
        {
            ClassDef e => new PyClassRenderer(e),
            StructDef e => new PyStructRenderer(e, scope),
            EnumDef e => new PyEnumRenderer(e),
            FunctionDef e => e.IsDelegate ? new PyDelegateRenderer(e, scope) : new PyFunctionRenderer(e, scope),
            TypedParamDef e => new PyParamRenderer(e, scope),
            _ => throw new Exception("unsupported element type: " + elem.GetType().Name + "")
        };
    }

    public static string GetTypeName(BaseType elem, NamingScope scope)
    {
        return elem switch
        {
            ClassType ty => ty.MetaClass != null
                ? $"Class[{GetRefTypeName(ty.MetaClass!.Ref, scope)}]"
                : "UClass",
            DynArrayType ty => $"WrappedArray[{GetTypeName(ty.InnerType, scope)}]",
            EngineBuiltinType ty => GetBuiltinType(ty),
            InterfaceType ty => GetTypeName(ty.InterfaceClass, scope),
            NamedType ty => GetRefTypeName(ty.Ref, scope),
            StaticArrayType ty => $"WrappedArray[{GetTypeName(ty.HeldType, scope)}]",
            DelegateType ty => "name | " + CreateDelegateSignature(ty, scope),
            UnhandledType _ => "Any", // maps fall into this category
            _ => throw new ArgumentOutOfRangeException(nameof(elem))
        };
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
                return "UnresolvedClass";
            }

            return "Any";
        }

        return elem.ResolvedTo! switch
        {
            ClassDef ty => scope.LocalName(ty, ty.Name()),
            EnumDef ty => $"{scope.LocalName(ty, ty.Name())}",
            StructDef ty => $"{scope.LocalName(ty, ty.Name())}",
            _ => throw new Exception("invalid type hint: " + elem.ResolvedTo.Name())
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
            _ => throw new Exception("invalid builtin type: " + elem.Type)
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
            return func.Name() + "Delegate";
        }

        return scope.LocalName(func, func.Name() + "Delegate");
    }
}