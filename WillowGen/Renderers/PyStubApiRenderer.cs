using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyStubApiRenderer : IRenderable
{
    public void Render(Sink sink)
    {
        sink.AppendLine("from unrealsdk.unreal import UObject, WrappedArray");
        sink.AppendLine();
        sink.AppendLine("type name = str | None");
        sink.AppendLine("type byte = int");
        sink.AppendLine("type UnresolvedClass = UObject | None");
        sink.AppendLine("type Opt[T] = T");
        sink.AppendLine("type Out[T] = T");
        sink.AppendLine("type OptOut[T] = T");
        sink.AppendLine("type Array[T] = WrappedArray[T] | list[T]");
        sink.AppendLine("type Delegate[T] = name");
    }
}
