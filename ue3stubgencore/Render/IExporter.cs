using UE3StubGenCore.Export;

namespace UE3StubGenCore.Render;

public interface IExporter
{
    void Export(ExportModel model, ExportPackage pkg);
}