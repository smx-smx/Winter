using Smx.Winter.Cbs.Native;

namespace Smx.Winter.Gui.Models;

public class CbsPackageViewModel
{
    private readonly ICbsPackage _package;

    public string StringId { get; }
    public string DisplayName { get; }
    public string Description { get; }
    public string ReleaseType { get; }
    public string Company { get; }
    public string SupportInformation { get; }
    public string PackageSize { get; }
    public bool IsHidden { get; }
    public bool IsStackUpdate { get; }
    public string Icon { get; }
    public string InstallTimeStamp { get; }
    public string ProductVersion { get; }

    public IReadOnlyList<(string Label, string Value)> Details { get; }

    public CbsPackageViewModel(ICbsPackage package)
    {
        _package = package;
        StringId = SafeGet(() => _package.GetIdentity().GetStringId()) ?? "?";

        DisplayName = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyDisplayName)) ?? "";
        Description = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyDescription)) ?? "";
        ReleaseType = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyReleaseType)) ?? "";
        Company = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyCompany)) ?? "";
        SupportInformation = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertySupportInformation)) ?? "";
        PackageSize = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyPackageSize)) ?? "";
        InstallTimeStamp = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyInstallTimeStamp)) ?? "";
        ProductVersion = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyProductVersion)) ?? "";

        IsHidden = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyHidden)) == "1";
        IsStackUpdate = SafeGet(() => _package.GetProperty(_CbsPackageProperty.CbsPackagePropertyStackUpdate)) == "1";

        Icon = DetermineIcon(StringId, ReleaseType);

        Details = new List<(string, string)>
        {
            ("Identity", StringId ?? "-"),
            ("Display Name", DisplayName ?? "-"),
            ("Description", Description ?? "-"),
            ("Release Type", ReleaseType ?? "-"),
            ("Product Version", ProductVersion ?? "-"),
            ("Company", Company ?? "-"),
            ("Support Info", SupportInformation ?? "-"),
            ("Package Size", FormatSize(PackageSize) ?? "-"),
            ("Install Time", InstallTimeStamp ?? "-"),
            ("Hidden", IsHidden ? "Yes" : "No"),
            ("Servicing Stack", IsStackUpdate ? "Yes" : "No"),
        };
    }

    private static string? SafeGet(Func<string> getter)
    {
        try { return getter(); }
        catch { return null; }
    }

    private static string DetermineIcon(string id, string releaseType)
    {
        var lower = id.ToLowerInvariant();
        if (lower.Contains("languagepack")) return "language";
        if (lower.Contains("foundation")) return "build";
        if (lower.Contains("edition")) return "style";
        if (lower.Contains("featureondemand") || lower.Contains("capability")) return "extension";
        if (lower.Contains("update") || lower.Contains("-kb")) return "system_update";
        if (!string.IsNullOrEmpty(releaseType))
        {
            if (releaseType.Contains("Update")) return "system_update";
            if (releaseType.Contains("Language")) return "language";
        }
        return "inventory_2";
    }

    private static string? FormatSize(string? sizeBytes)
    {
        if (string.IsNullOrEmpty(sizeBytes) || !long.TryParse(sizeBytes, out var bytes))
            return sizeBytes;
        if (bytes == 0) return "0 B";
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024L * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
    }
}
