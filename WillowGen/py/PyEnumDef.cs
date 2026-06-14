using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyEnumDef(ExportEnum export, PyBaseElement? parent = null) : PyBaseElement(parent), IPyExportSymbol
{
    public IReadOnlyList<string> Values { get; } = export.Ordinals;
}