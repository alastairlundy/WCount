namespace WCountCli.Testing;

public sealed class CliIntegrationTests
{
    private static readonly string BaselinesDir = CliTestRunner.BaselinesDir;
    private static readonly string TestFilesDir = CliTestRunner.TestFilesDir;

    [Theory]
    [InlineData("-w", "NATURE.txt", "nature_word_count.txt")]
    [InlineData("-l", "NATURE.txt", "nature_line_count.txt")]
    [InlineData("-m", "NATURE.txt", "nature_char_count.txt")]
    [InlineData("-c", "NATURE.txt", "nature_byte_count.txt")]
    [InlineData("-w -l", "NATURE.txt", "nature_word_line_count.txt")]
    [InlineData("-w -l -m -c", "NATURE.txt", "nature_all_counts.txt")]
    [InlineData("", "NATURE.txt", "nature_default.txt")]
    public async Task SingleFile_WithFlags_MatchesBaseline(string flags, string file, string baselineFile)
    {
        var filePath = Path.Combine(TestFilesDir, file);
        var args = string.IsNullOrEmpty(flags) ? filePath : $"{flags} {filePath}";

        var result = await CliTestRunner.RunAsync(args);
        var expected = await File.ReadAllTextAsync(Path.Combine(BaselinesDir, baselineFile));

        Assert.Equal(0, result.ExitCode);
        Assert.Equal(expected, result.Stdout);
        Assert.Empty(result.Stderr);
    }

    [Theory]
    [InlineData("-l", "CRLF.txt", "crlf_line_count.txt")]
    [InlineData("", "EMPTY.txt", "empty_default.txt")]
    [InlineData("-w", "LARGE_WORD.txt", "large_word_count.txt")]
    [InlineData("-w -l", "WHITESPACE_ONLY.txt", "whitespace_word_line.txt")]
    public async Task EdgeCaseFiles_MatchesBaseline(string flags, string file, string baselineFile)
    {
        var filePath = Path.Combine(TestFilesDir, file);
        var args = string.IsNullOrEmpty(flags) ? filePath : $"{flags} {filePath}";

        var result = await CliTestRunner.RunAsync(args);
        var expected = await File.ReadAllTextAsync(Path.Combine(BaselinesDir, baselineFile));

        Assert.Equal(0, result.ExitCode);
        Assert.Equal(expected, result.Stdout);
        Assert.Empty(result.Stderr);
    }

    [Fact]
    public async Task MultiFile_Total_MatchesBaseline()
    {
        var file1 = Path.Combine(TestFilesDir, "NATURE.txt");
        var file2 = Path.Combine(TestFilesDir, "CRLF.txt");
        var args = $"-w -l {file1} {file2}";

        var result = await CliTestRunner.RunAsync(args);
        var expected = await File.ReadAllTextAsync(
            Path.Combine(BaselinesDir, "multi_file_total.txt"));

        Assert.Equal(0, result.ExitCode);
        Assert.Equal(expected, result.Stdout);
        Assert.Empty(result.Stderr);
    }

    [Fact]
    public async Task Help_ReturnsUsage()
    {
        var args = "-?";

        var result = await CliTestRunner.RunAsync(args);
        var expected = await File.ReadAllTextAsync(
            Path.Combine(BaselinesDir, "help_output.txt"));

        Assert.Equal(0, result.ExitCode);
        Assert.Equal(expected, result.Stdout);
        Assert.Empty(result.Stderr);
    }

    [Fact]
    public async Task NonexistentFile_ReturnsError()
    {
        var result = await CliTestRunner.RunAsync("nonexistent_file_xyz.txt");

        Assert.Equal(1, result.ExitCode);
        Assert.Contains("does not exist", result.Stderr);
        Assert.Empty(result.Stdout);
    }
}
