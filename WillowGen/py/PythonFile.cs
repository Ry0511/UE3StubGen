using UE3StubGenCore.Export;

namespace WillowGen.py;

public class PythonFile
{
    public List<PythonFileElement> Elements { get; private set; } = [];

    public PythonFile(ExportPackage pkg)
    {
    }

    public PythonFile(ExportClass cls)
    {
    }
}