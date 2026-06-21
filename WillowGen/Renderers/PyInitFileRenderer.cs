using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyInitFileRenderer(PackageDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        var exported = new List<string>();

        foreach (var cls in elem.Classes.Where(e => e.IsModuleUnique))
        {
            // import the class
            sink.Append($"from bl1.{elem.Name()}.{cls.Name()} import {cls.Name()}");
            exported.Add(cls.Name());

            // import all the module unique children
            foreach (
                var child in cls.Descendants().OfType<BaseSymbol>().Where(e => e.IsModuleUnique)
            )
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
