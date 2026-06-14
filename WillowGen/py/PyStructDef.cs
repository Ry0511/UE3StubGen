using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyStructDef : PyBaseElement
{
    private ExportStruct _export;

    public PyStructDef(ExportStruct export, PyBaseElement? parent = null) : base(parent)
    {
        _export = export;
    }
}