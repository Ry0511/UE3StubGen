using UE3StubGenCore.ASG.Defs;
using WillowGen.Sinks;

namespace WillowGen.Renderers;

public class PyInitFileRenderer(PackageDef elem) : IRenderable
{
    public void Render(Sink sink)
    {
        foreach (var cls in elem.Classes)
        {
            sink.Append($"from bl1.{elem.Name()}.{cls.Name()} import {cls.Name()}");
            foreach (var @enum in cls.Enums) sink.AppendRaw(", " + @enum.Name());
            foreach (var @struct in cls.Structs) sink.AppendRaw(", " + @struct.Name());
            foreach (var dele in cls.Functions.Where(e => e.IsDelegate)) sink.AppendRaw(", _delegate_" + dele.Name());
            sink.AppendLine();
        }

        sink.AppendLine();
        sink.AppendLine("__all__: tuple[str, ...] = (");
        sink.PushIndent();
        foreach (var cls in elem.Classes)
        {
            sink.AppendLine($"\"{cls.Name()}\",");
            foreach (var @enum in cls.Enums) sink.AppendLine($"\"{@enum.Name()}\",");
            foreach (var @struct in cls.Structs) sink.AppendLine($"\"{@struct.Name()}\",");
            foreach (var dele in cls.Functions.Where(e => e.IsDelegate)) sink.AppendLine($"\"_delegate_{dele.Name()}\",");
        }

        sink.PopIndent();
        sink.AppendLine(")");
    }
}