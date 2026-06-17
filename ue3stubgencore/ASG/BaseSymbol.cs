namespace UE3StubGenCore.ASG;

public abstract class BaseSymbol(BaseElement? parent) : BaseElement(parent), ISymbol, INameable
{
    public abstract string ExportPathName();
    public abstract string Name();
}