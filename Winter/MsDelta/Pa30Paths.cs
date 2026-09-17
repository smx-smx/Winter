namespace Smx.Winter.MsDelta;

/// <summary>
/// Well-known on-disk locations for PA30 tooling.
/// Diagnostic artifacts (hook captures, decoder outputs, state dumps) go to the
/// git-ignored staging directory. The only committed data file is the
/// wcp_base.patch test fixture at the repo root. Locations resolve dynamically
/// from the repository checkout, so no machine-specific paths are hardcoded.
/// </summary>
public static class Pa30Paths
{
    private static string? _repoRoot;

    /// <summary>
    /// Repo root, found by walking up from the running binary (then the working
    /// directory) to the directory containing Winter.sln (falling back to .git).
    /// </summary>
    public static string RepoRoot => _repoRoot ??= FindRepoRoot();

    /// <summary>Git-ignored scratch directory for PA30 diagnostic artifacts.</summary>
    public static string StagingDir => System.IO.Path.Combine(RepoRoot, "staging");

    /// <summary>Path of a committed test fixture under the repo root.</summary>
    public static string FixtureFile(string fileName) => System.IO.Path.Combine(RepoRoot, fileName);

    /// <summary>Path of a staging artifact (read side; creates nothing).</summary>
    public static string StagingRead(string fileName) => System.IO.Path.Combine(StagingDir, fileName);

    /// <summary>Path of a staging artifact, creating the staging directory first.</summary>
    public static string StagingWrite(string fileName)
    {
        System.IO.Directory.CreateDirectory(StagingDir);
        return StagingRead(fileName);
    }

    private static string FindRepoRoot()
    {
        var starts = new[]
        {
            System.AppContext.BaseDirectory,
            System.IO.Directory.GetCurrentDirectory()
        };
        foreach (var start in starts)
        {
            var dir = new System.IO.DirectoryInfo(start);
            while (dir != null)
            {
                if (System.IO.File.Exists(System.IO.Path.Combine(dir.FullName, "Winter.sln"))
                    || System.IO.Directory.Exists(System.IO.Path.Combine(dir.FullName, ".git")))
                    return dir.FullName;
                dir = dir.Parent;
            }
        }
        return System.IO.Directory.GetCurrentDirectory();
    }
}
