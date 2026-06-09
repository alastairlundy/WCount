namespace WCountCli.Testing;

public sealed record CliResult(int ExitCode, string Stdout, string Stderr);

public static class CliTestRunner
{
    private static readonly string CliDllPath;

    static CliTestRunner()
    {
        var repoRoot = FindRepoRoot();
        var config = DetectConfiguration();
        CliDllPath = Path.Combine(repoRoot, "src", "WCountCli", "bin", config, "net10.0", "wcount.dll");
    }

    public static async Task<CliResult> RunAsync(string arguments, string? stdin = null)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"exec \"{CliDllPath}\" {arguments}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = stdin is not null,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            }
        };

        process.Start();

        if (stdin is not null)
        {
            await process.StandardInput.WriteAsync(stdin);
            process.StandardInput.Close();
        }

        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        return new CliResult(process.ExitCode, stdout, stderr);
    }

    public static string BaselinesDir => Path.Combine(
        FindRepoRoot(), "tests", "WCountCli.Testing", "TestData", "Baselines");

    public static string TestFilesDir => Path.Combine(
        FindRepoRoot(), "test-files");

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Detection logic")]
    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            try
            {
                if (Directory.Exists(Path.Combine(dir.FullName, ".git")))
                    return dir.FullName;
            }
            catch
            {
            }
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Repository root not found.");
    }

    private static string DetectConfiguration()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        if (dir.Parent?.Parent?.Name == "bin")
            return dir.Parent!.Name;
        return "Debug";
    }
}
