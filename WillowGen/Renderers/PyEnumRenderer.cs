using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyEnumRenderer(EnumDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        // an ordinal appears more than once
        var isBadEnum = elem.Values.Distinct().Count() != elem.Values.Count;

        sink.AppendLine($"class {elem.Name()}(UnrealEnum):" + (isBadEnum ? " # bad enum" : string.Empty));
        sink.PushIndent();
        for (var i = 0; i < elem.Values.Count; i++)
        {
            sink.AppendLine((isBadEnum ? "# " : string.Empty) + $"{elem.Values[i]} = auto()");
        }

        if (isBadEnum)
        {
            sink.AppendLine("...");
        }

        sink.PopIndent();
    }
}