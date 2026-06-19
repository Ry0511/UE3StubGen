using UE3StubGenCore.ASG.Defs;
using WillowGen.Renderer;

namespace WillowGen.PyRenderers;

public class PyEnumRenderer(EnumDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        sink.AppendLine($"class {elem.Name()}(IntEnum):");
        sink.PushIndent();
        for (var i = 0; i < elem.Values.Count; i++)
        {
            sink.AppendLine($"{elem.Values[i]} = {i}");
        }

        sink.PopIndent();
    }
}