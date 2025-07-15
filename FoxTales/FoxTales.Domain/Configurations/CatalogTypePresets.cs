using System.Collections.Immutable;
using FoxTales.Domain.Enums;

namespace FoxTales.Domain.Configurations;

public static class CatalogTypePresets
{
    private static readonly ImmutableDictionary<CatalogTypePresetName, ImmutableList<int>> _presets =
        new Dictionary<CatalogTypePresetName, ImmutableList<int>>
        {
            [CatalogTypePresetName.DEFAULT_SIZES] = ImmutableList.Create((int)CatalogTypeName.Small, (int)CatalogTypeName.Medium, (int)CatalogTypeName.Large),
            [CatalogTypePresetName.PREMIUM] = ImmutableList.Create((int)CatalogTypeName.Large, (int)CatalogTypeName.NoLimit)
        }.ToImmutableDictionary();

    public static IReadOnlyDictionary<CatalogTypePresetName, ImmutableList<int>> Presets => _presets;

    public static ImmutableList<int> GetPresetIds(CatalogTypePresetName presetName)
    {
        return _presets.TryGetValue(presetName, out var ids) ? ids : [];
    }
}