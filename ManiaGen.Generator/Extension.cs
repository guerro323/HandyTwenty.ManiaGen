using Microsoft.CodeAnalysis;

namespace ManiaGen.Generator;

public static class Extension
{
    public static string GetTypeName(this ITypeSymbol type)
    {
        return (type.TypeKind is TypeKind.TypeParameter || type.SpecialType is > 0 and <= SpecialType.System_String
            ? type.ToString()
            : $"global::{type}")!;
    }
}