using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyStubApiRenderer : IRenderable
{
    public void Render(Sink sink)
    {
        sink.AppendLine("from typing import TypeVar, TypeAlias, Generic");
        sink.AppendLine("from unrealsdk.unreal import UObject, UClass");
        sink.AppendLine();
        sink.AppendLine("type name = str | None");
        sink.AppendLine("type byte = int");
        sink.AppendLine("type UnresolvedClass = UObject | None");
        sink.AppendLine();
        sink.AppendLine("T = TypeVar(\"T\")");
        sink.AppendLine("Opt: TypeAlias = T | None");
        sink.AppendLine("Out: TypeAlias = T | None");
        sink.AppendLine("OptOut: TypeAlias = T | None");
        sink.AppendLine();
        sink.AppendLine("_ClassType = TypeVar(\"_ClassType\", bound=UObject, covariant=True)");
        sink.AppendLine("class Class(UClass, Generic[_ClassType]): ...");
    }
}