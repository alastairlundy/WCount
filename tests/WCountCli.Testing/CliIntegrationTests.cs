namespace WCountCli.Testing;

public sealed class CliIntegrationTests
{
    private static readonly string BaselinesDir = CliTestRunner.BaselinesDir;
    private static readonly string TestFilesDir = CliTestRunner.TestFilesDir;

    [Test]
    [Arguments("-w", "NATURE.txt", "nature_word_count.txt")]
    [Arguments("-l", "NATURE.txt", "nature_line_count.txt")]
    [Arguments("-m", "NATURE.txt", "nature_char_count.txt")]
    [Arguments("-c", "NATURE.txt", "nature_byte_count.txt")]
    [Arguments("-w -l", "NATURE.txt", "nature_word_line_count.txt")]
    [Arguments("-w -l -m -c", "NATURE.txt", "nature_all_counts.txt")]
    [Arguments("", "NATURE.txt", "nature_default.txt")]
    public async Task SingleFile_WithFlags_MatchesBaseline(string flags, string file, string baselineFile)
    {
        var filePath = Path.Combine(TestFilesDir, file);
        var args = string.IsNullOrEmpty(flags) ? filePath : $"{flags} {filePath}";

        var result = await CliTestRunner.RunAsync(args);
        var expected = await File.ReadAllTextAsync(Path.Combine(BaselinesDir, baselineFile));

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Stdout).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    [Arguments("-l", "CRLF.txt", "crlf_line_count.txt")]
    [Arguments("", "EMPTY.txt", "empty_default.txt")]
    [Arguments("-w", "LARGE_WORD.txt", "large_word_count.txt")]
    [Arguments("-w -l", "WHITESPACE_ONLY.txt", "whitespace_word_line.txt")]
    public async Task EdgeCaseFiles_MatchesBaseline(string flags, string file, string baselineFile)
    {
        var filePath = Path.Combine(TestFilesDir, file);
        var args = string.IsNullOrEmpty(flags) ? filePath : $"{flags} {filePath}";

        var result = await CliTestRunner.RunAsync(args);
        var expected = await File.ReadAllTextAsync(Path.Combine(BaselinesDir, baselineFile));

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Stdout).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task MultiFile_Total_MatchesBaseline()
    {
        var file1 = Path.Combine(TestFilesDir, "NATURE.txt");
        var file2 = Path.Combine(TestFilesDir, "CRLF.txt");
        var args = $"-w -l {file1} {file2}";

        var result = await CliTestRunner.RunAsync(args);
        var expected = await File.ReadAllTextAsync(
            Path.Combine(BaselinesDir, "multi_file_total.txt"));

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Stdout).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task Help_ReturnsUsage()
    {
        var args = "-?";

        var result = await CliTestRunner.RunAsync(args);
        var expected = await File.ReadAllTextAsync(
            Path.Combine(BaselinesDir, "help_output.txt"));

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(result.Stdout).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task NonexistentFile_ReturnsError()
    {
        var result = await CliTestRunner.RunAsync("nonexistent_file_xyz.txt");

        await Assert.That(result.ExitCode).IsEqualTo(1);
        await Assert.That(result.Stderr).Contains("does not exist");
        await Assert.That(result.Stdout).IsEmpty();
    }
}
