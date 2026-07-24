using UE3StubGenCore.Sinks;

namespace WillowGen.Renderers;

public class PyStubApiRenderer : IRenderable
{
    public void Render(Sink sink)
    {
        sink.AppendLine("# pyright: reportExplicitAny=false");
        sink.AppendLine("# pyright: reportAny=false");
        sink.AppendLine();
        sink.AppendLine("from _typeshed import MaybeNone");
        sink.AppendLine("from typing import Any, Callable, Concatenate, override, overload");
        sink.AppendLine("from unrealsdk.unreal import UObject, WrappedStruct, BoundFunction");
        sink.AppendLine();
        sink.AppendLine("type name = str | MaybeNone");
        sink.AppendLine("type byte = int");
        sink.AppendLine("type Unresolved[T] = UObject | MaybeNone");
        sink.AppendLine("type Out[T] = T");
        sink.AppendLine("type Delegate[T] = name");
        sink.AppendLine();
        sink.AppendLine("class HookableFunction[**P, R](BoundFunction):");
        sink.AppendLine("  @overload");
        sink.AppendLine("  def __call__(self, args: WrappedStruct, /) -> R: ...");
        sink.AppendLine("  @overload");
        sink.AppendLine("  def __call__(self, *args: P.args, **kwargs: P.kwargs) -> R: ...");
        sink.AppendLine("  @override");
        sink.AppendLine("  def __call__(self, *args: Any, **kwargs: Any) -> Any: ...");
        sink.AppendLine();
        sink.AppendLine("def bound_function[**P, R](f: Callable[Concatenate[Any, P], R]) -> HookableFunction[P, R]: ...");
        sink.AppendLine();
        sink.AppendLine("class AcceptsNone[T=Any]:");
        sink.AppendLine("  @overload");
        sink.AppendLine("  def __get__(self, obj: None, ty: type | None = None) -> AcceptsNone[T]: ...");
        sink.AppendLine("  @overload");
        sink.AppendLine("  def __get__(self, obj: object, ty: type | None = None) -> T: ...");
        sink.AppendLine("  def __set__(self, obj: object, value: T | None) -> None: ...");
        sink.AppendLine();
    }
}