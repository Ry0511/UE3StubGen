using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyRef(BaseExport export, PyBaseElement? parent = null) : PyBaseElement(parent)
{
    private BaseExport _export = export;
}