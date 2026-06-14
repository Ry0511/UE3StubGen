using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyTypedParam(ExportField export, PyBaseElement? parent = null) : PyBaseElement(parent)
{
    private ExportField _export = export;
}