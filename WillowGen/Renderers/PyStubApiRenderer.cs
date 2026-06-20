using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyStubApiRenderer : IRenderable
{
    public void Render(Sink sink)
    {
        sink.AppendLine("from unrealsdk.unreal import UObject");
        sink.AppendLine();
        sink.AppendLine("type name = str | None");
        sink.AppendLine("type byte = int");
        sink.AppendLine("type UnresolvedClass = UObject | None");
    }
}