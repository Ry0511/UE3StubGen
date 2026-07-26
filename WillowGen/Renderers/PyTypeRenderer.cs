using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.ASG.Types;

namespace WillowGen.Renderers;

public class PyTypeRenderer(NamingScope scope)
{
    private enum Mode
    {
        MemberVariable,
        FunctionParam,
        FunctionReturn,
        RawParam,
        RawReturn,
    }

    public string RenderMemberVariable(BaseType ty) => Render(ty, Mode.MemberVariable, true);

    public string RenderFunctionParam(BaseType ty) => Render(ty, Mode.FunctionParam, true);

    public string RenderFunctionReturn(BaseType ty) => Render(ty, Mode.FunctionReturn, true);

    public string RenderRaw(BaseType ty) => Render(ty, Mode.RawParam, true);

    public string RenderRawReturn(BaseType ty) => Render(ty, Mode.RawReturn, true);

    private string Render(BaseType ty, Mode mode, bool topLevel)
    {
        return ty switch
        {
            ClassType _ => Decorate("UClass", mode, topLevel),
            DynArrayType a => $"WrappedArray[{Render(a.InnerType, mode, false)}]",
            StaticArrayType s => $"WrappedArray[{Render(s.HeldType, mode, false)}]",
            EngineBuiltinType b => GetBuiltinType(b),
            InterfaceType i => Decorate(GetRefTypeName(i.InterfaceClass.Ref, scope), mode, topLevel),
            NamedType n => RenderNamed(n, mode, topLevel),
            DelegateType _ => "Delegate",
            UnhandledType _ => Decorate("Any", mode, topLevel),
            _ => throw new ArgumentOutOfRangeException(nameof(ty)),
        };
    }

    private string RenderNamed(NamedType n, Mode mode, bool topLevel)
    {
        var name = GetRefTypeName(n.Ref, scope);
        if (n.IsEnumRef())
        {
            return name + " | int";
        }

        if (n.IsClassRef())
        {
            return Decorate(name, mode, topLevel);
        }

        return name;
    }

    private static string Decorate(string core, Mode mode, bool topLevel)
    {
        return mode switch
        {
            Mode.MemberVariable => topLevel ? $"AcceptsNone[{core}]" : $"{core} | None",
            Mode.FunctionParam => $"{core} | None",
            Mode.FunctionReturn => $"{core} | MaybeNone",
            _ => core,
        };
    }

    public static string GetRefTypeName(RefNode elem, NamingScope scope)
    {
        if (elem.ResolvedTo == null)
        {
            var split = elem.TargetFullPath.Split('.');
            return elem.AllModules().Any(e => e.Name() == split[^2])
                ? $"Unresolved[Literal[\"{elem.TargetFullPath}\"]]"
                : "Any";
        }

        return elem.ResolvedTo! switch
        {
            ClassDef ty => scope.LocalName(ty, ty.Name()),
            EnumDef ty => $"{scope.LocalName(ty, ty.Name())}",
            StructDef ty => $"{scope.LocalName(ty, ty.Name())}",
            _ => throw new Exception("invalid type hint: " + elem.ResolvedTo.Name()),
        };
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
            _ => throw new Exception("invalid builtin type: " + elem.Type),
        };
    }
}