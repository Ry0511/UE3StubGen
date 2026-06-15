using UE3StubGenCore.ASG;
using UE3StubGenCore.Render;

namespace WillowGen;

public class WillowSdkGenerator : IExporter
{
    public void Export(ExportModel model)
    {
        Project py = new(model);
        Dictionary<string, List<RefNode>> unresolved = new();

        foreach (var elem in py.Descendants().OfType<RefNode>().Where(e => e.ResolvedTo == null))
        {
            var found = unresolved.GetValueOrDefault(elem.TargetFullPath);
            if (found == null)
            {
                found = new();
                unresolved.Add(elem.TargetFullPath, found);
            }

            if (found.Count < 5)
            {
                found.Add(elem);
            }
        }

        Console.WriteLine("Unresolved References:");
        foreach (var elem in unresolved)
        {
            Console.WriteLine($"  {elem.Key}");
            foreach (var pyRef in elem.Value)
            {
                Console.WriteLine($"    {pyRef.TargetFullPath} - {pyRef.Export.ObjectHandle.GetPath()}");
                foreach (var parent in pyRef.Ancestors().OfType<ISymbol>())
                {
                    Console.WriteLine($"      {parent.ExportPathName()} :: {parent.GetType().Name}");
                }
            }
        }
    }
}