namespace WCountCli.Testing;

public sealed record CliResult(int ExitCode, string Stdout, string Stderr);

public static class CliTestRunner
{
    /// <summary>
    /// Stands in for the fixture directory in baselines, so they do not embed
    /// an absolute path from whichever checkout captured them.
    /// </summary>
    public const string TestFilesToken = "{TESTFILES}";

    private static readonly string CliDllPath =
        Path.Combine(AppContext.BaseDirectory, "wcount.dll");

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
        AppContext.BaseDirectory, "TestData", "Baselines");

    public static string TestFilesDir => Path.Combine(
        AppContext.BaseDirectory, "test-files");

    public static string FixturePath(string fileName) => Path.Combine(TestFilesDir, fileName);

    public static async Task<string> ReadBaselineAsync(string baselineFile, CancellationToken ct = default) =>
        NormaliseNewlines(await File.ReadAllTextAsync(Path.Combine(BaselinesDir, baselineFile), ct));

    /// <summary>
    /// Reduces CLI output to a form that is stable across checkouts and operating
    /// systems: the fixture directory becomes a token, and newlines are unified so
    /// a single baseline copy works on both CRLF and LF platforms.
    /// </summary>
    public static string Normalise(string output)
    {
        string normalised = output
            .Replace(TestFilesDir + Path.DirectorySeparatorChar, TestFilesToken, StringComparison.Ordinal)
            .Replace(TestFilesDir, TestFilesToken, StringComparison.Ordinal);

        return NormaliseNewlines(normalised);
    }

    private static string NormaliseNewlines(string text) =>
        text.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace("\r", "\n", StringComparison.Ordinal);
}
