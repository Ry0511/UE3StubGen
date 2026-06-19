using WillowGen.Sinks;

namespace WillowGen.Renderers;

public interface IRenderable
{
    public void Render(Sink sink);
}