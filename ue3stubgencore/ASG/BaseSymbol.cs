namespace UE3StubGenCore.ASG;

public abstract class BaseSymbol(BaseElement? parent) : BaseElement(parent), ISymbol, INameable
{
    // name is unique in its module
    public bool IsModuleUnique { get; protected set; }

    // name is unique across all modules
    public bool IsUniqueCrossModule { get; protected set; } = false;

    public abstract string ExportPathName();
    public abstract string Name();
}