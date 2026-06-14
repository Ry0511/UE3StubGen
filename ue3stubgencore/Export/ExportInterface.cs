using UELib;
using UELib.Core;

namespace ue3stubgencore.export;

/**
 * Interfaces are just classes extending Interface and are always abstract
 */
public class ExportInterface(ExportContext ctx, UnrealPackage pkg, UClass obj)
    : ExportClass(ctx, pkg, obj);