using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PyFunctionDef(ExportFunction export, PyBaseElement? parent = null) : PyBaseElement(parent)
{
    private ExportFunction _export = export;
}