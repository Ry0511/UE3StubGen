using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyInitFileRenderer(PackageDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        foreach (var cls in elem.Classes)
        {
            sink.AppendLine($"from bl1.{elem.Name()}.{cls.Name()} import {cls.Name()}");
        }

        sink.AppendLine();
        sink.AppendLine("__all__: tuple[str, ...] = (");
        sink.PushIndent();
        foreach (var cls in elem.Classes)
        {
            sink.AppendLine($"\"{cls.Name()}\",");
        }

        sink.PopIndent();
        sink.AppendLine(")");
    }
}