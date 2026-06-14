using UELib;
using UELib.Core;

namespace ue3stubgencore.export;

public class ExportInterface(ExportContext ctx, UnrealPackage pkg, UClass obj)
    : ExportClass(ctx, pkg, obj);