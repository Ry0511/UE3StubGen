using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyInitFileRenderer(string importRoot, PackageDef elem) : IRenderable
{
    private readonly string _importRoot = importRoot.Length == 0 ? string.Empty : importRoot + '.';

    public void Render(Sink sink)
    {
        var exported = new List<string>();

        foreach (var cls in elem.Classes.Where(e => e.IsModuleUnique))
        {
            // import the class
            sink.Append($"from {_importRoot}{elem.Name()}.{cls.Name()} import {cls.Name()}");
            exported.Add(cls.Name());

            // import all the module unique children
            foreach (
                var child in cls.Descendants().OfType<BaseSymbol>().Where(e => e.IsModuleUnique))
            {
                sink.AppendRaw($", {child.Name()}");
                exported.Add(child.Name());
            }

            sink.AppendLine();
        }

        sink.AppendLine();
        sink.AppendLine("__all__: tuple[str, ...] = (");
        sink.PushIndent();
        foreach (var name in exported)
        {
            sink.AppendLine($"\"{name}\",");
        }

        sink.PopIndent();
        sink.AppendLine(")");
    }
}