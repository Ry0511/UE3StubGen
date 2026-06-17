using UE3StubGenCore.ASG;
using UE3StubGenCore.ASG.Defs;
using UE3StubGenCore.Render;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    public void Export(ExportModel model)
    {
        MultiModuleProject py = new(model);
        var unresolved = new SortedSet<RefNode>();

        foreach (var elem in py.Descendants().OfType<RefNode>().Where(e => e.ResolvedTo == null))
        {
            unresolved.Add(elem);
        }

        Console.WriteLine($"unresolved: {unresolved.Count}");
        foreach (var node in unresolved)
        {
            Console.WriteLine($"  {node.TargetFullPath}");
        }

        var cls = py
            .Descendants()
            .OfType<ClassDef>()
            .Where(e => e.Module!.ExportPathName() == "WillowGame")
            .Where(e => e.Fields.Count > 3)
            .Take(10)
            .Select(e => e.BuildName());

        foreach (var elem in cls)
        {
            Console.WriteLine(elem);
        }

    }
}