using UE3StubGenCore.Export;
using UELib.Core;

namespace WillowGen.py;

public class PyRef : PyBaseElement
{
    public BaseExport Export { get; }
    public string FullPath { get; }
    public bool ReferencesBuiltin { get; }
    public bool DirectInit { get; }

    public PyRef(BaseExport export, PyBaseElement? parent) : base(parent)
    {
        Export = export;
        FullPath = Export.ObjectHandle.GetPath();
        DirectInit = true;
    }

    public PyRef(ExportField export, PyBaseElement? parent) : base(parent)
    {
        Export = export;
        FullPath = export.ObjectHandle switch
        {
            UObjectProperty e => e.Object.GetPath(),
            UStructProperty e => e.Struct.GetPath(),
            UProperty e => "Core." + e.Type,
            _ => throw new ArgumentException($"unhandled type {export.Type}, {export.ObjectHandle.GetType().Name}")
        };

        ReferencesBuiltin = FullPath.StartsWith("Core.");
        DirectInit = false;
    }
}