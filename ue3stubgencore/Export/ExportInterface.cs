using UELib;
using UELib.Core;

namespace UE3StubGenCore.Export;

public class ExportInterface(ExportContext ctx, UnrealPackage pkg, UClass obj)
    : ExportClass(ctx, pkg, obj)
{
}
