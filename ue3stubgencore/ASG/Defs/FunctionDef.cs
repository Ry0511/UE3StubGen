using UE3StubGenCore.Export;

namespace UE3StubGenCore.ASG.Defs;

public class FunctionDef : BaseSymbol
{
    public ExportFunction Export { get; }

    public List<TypedParamDef> Params { get; }

    public TypedParamDef? ReturnValue { get; }

    public bool IsStatic => Export.IsStatic;

    public bool IsDelegate => Export.IsDelegate;

    public bool HasOutParms => Export.HasOutParms;

    public bool HasOptionalParms => Export.HasOptionalParms;

    public FunctionDef? Overrides { get; private set; }

    public bool IsOverride { get; private set; }

    public bool IsNaughtyOverride { get; private set; }

    public bool FamilyHasNaughtyOverride { get; internal set; }

    public FunctionDef(ExportFunction export, BaseElement? parent = null)
        : base(parent)
    {
        if (!export.IsRegularFunction && !export.IsDelegate)
        {
            throw new Exception(
                "only regular functions are supported (iterator and operator functions are not allowed)");
        }

        Export = export;
        Params = export.Parameters.Select(elem => new TypedParamDef(elem, this)).ToList();
        ReturnValue =
            export.ReturnParameter != null ? new TypedParamDef(export.ReturnParameter, this) : null;
    }

    public override IEnumerable<BaseElement> Children()
    {
        foreach (var elem in Params)
        {
            yield return elem;
        }

        if (ReturnValue != null)
        {
            yield return ReturnValue;
        }
    }

    public override string ExportPathName()
    {
        return Export.ObjectHandle.GetPath();
    }

    public override string Name()
    {
        return Export.Name();
    }

    public bool HasSameSignatureAs(FunctionDef other)
    {
        if (Params.Count != other.Params.Count)
        {
            return false;
        }

        for (var i = 0; i < Params.Count; i++)
        {
            if (Params[i].ParamType.Name() != other.Params[i].ParamType.Name())
            {
                return false;
            }
        }

        return ReturnValue?.ParamType.Name() == other.ReturnValue?.ParamType.Name();
    }

    public bool ParameterNamesMatch(FunctionDef other)
    {
        return Params.Select(p => p.Name()).SequenceEqual(other.Params.Select(p => p.Name()));
    }

    public override void PostEvaluate(BaseElement root)
    {
        if (Parent is not ClassDef cls)
        {
            return;
        }

        var matches = cls.InheritedTypes()
            .SelectMany(inherited => inherited.Functions)
            .Where(i => i.Name() == Name())
            .ToList();

        Overrides = matches.FirstOrDefault(i => i.HasSameSignatureAs(this))
                    ?? matches.FirstOrDefault();
        IsOverride = Overrides != null;
        IsNaughtyOverride = IsOverride && !ParameterNamesMatch(Overrides!);
    }

    public string SignatureKey()
    {
        var ps = string.Join(",", Params.Select(p => p.ParamType.Name()));
        return $"{Name()}({ps}):{ReturnValue?.ParamType.Name() ?? string.Empty}";
    }

    public bool HasSparseOptionalParams(Predicate<TypedParamDef>? isOptional = null)
    {
        // Foo() and Foo(a) can never cause issues
        if (!HasOptionalParms || Params.Count < 2)
        {
            return false;
        }

        isOptional ??= p => p.IsOptionalParam;

        // Ensure all optional parameters are at the end
        var lastWasOptional = isOptional(Params[^1]);
        foreach (var param in Params.AsEnumerable().Reverse().Skip(1))
        {
            var isOptionalParam = isOptional(param);
            if (isOptionalParam && !lastWasOptional)
            {
                return false;
            }

            lastWasOptional = isOptionalParam;
        }

        return true;
    }
}