#nullable enable
using System.IO;
using StardewModdingAPI;
using Newtonsoft.Json.Linq;

namespace SharedExp
{
    //I take no credit for this , this is work done @DaLion
    //Link here https://gitlab.com/daleao/sdv-mods/-/blob/dev/Shared/Extensions/SMAPI/ModHelperExtensions.cs
    public static class DaLionsModHelperExtension
    {
         /// <summary>Gets the <see cref="IMod"/> interface for the external mod identified by <paramref name="uniqueId"/>.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <param name="uniqueId">The unique ID of the external mod.</param>
    /// <returns>A <see cref="JObject"/> representing the contents of the config.</returns>
    /// <remarks>Will only for mods that implement <see cref="IMod"/>; i.e., will not work for content packs.</remarks>
    public static IMod? GetModEntryFor(this IModHelper helper, string uniqueId)
    {
        var modInfo = helper.ModRegistry.Get(uniqueId);
        if (modInfo is not null)
        {
            return (IMod)modInfo.GetType().GetProperty("Mod")!.GetValue(modInfo)!;
        }
        return null;
    }

    /// <summary>Reads the configuration file of the mod with the specified <paramref name="uniqueId"/>.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <param name="uniqueId">The unique ID of the external mod.</param>
    /// <returns>A <see cref="JObject"/> representing the contents of the config.</returns>
    /// <remarks>Will only for mods that implement <see cref="IMod"/>; i.e., will not work for content packs.</remarks>
    
    public static JObject? ReadConfigExt(this IModHelper helper, string uniqueId)
    {
        var modEntry = helper.GetModEntryFor(uniqueId);
        return modEntry?.Helper.ReadConfig<JObject>();
    }

    /// <summary>Reads the configuration file of the content pack with the specified <paramref name="uniqueId"/>.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <param name="uniqueId">The unique ID of the content pack.</param>
    /// <returns>A <see cref="JObject"/> representing the contents of the config.</returns>
    /// <remarks>Will work for any mod, but is reserved for content packs.</remarks>
    public static JObject? ReadContentPackConfig(this IModHelper helper, string uniqueId)
    {
        var modInfo = helper.ModRegistry.Get(uniqueId);
        if (modInfo is null)
        {
           
            return null;
        }

        var modPath = (string)modInfo.GetType().GetProperty("DirectoryPath")!.GetValue(modInfo)!;
        try
        {
            var config = JObject.Parse(File.ReadAllText(Path.Combine(modPath, "config.json")));
            return config;
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    }
}
