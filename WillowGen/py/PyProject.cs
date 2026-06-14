using UE3StubGenCore.Render;

namespace WillowGen.py;

public class PyProject : PyBaseElement
{
    public IReadOnlyList<PyModule> Modules { get; }

    public PyProject(ExportModel model)
    {
        Modules = model.Packages.Select(elem => new PyModule(elem, this)).ToList();
    }

    public override IEnumerable<PyBaseElement> Children() => Modules;
}