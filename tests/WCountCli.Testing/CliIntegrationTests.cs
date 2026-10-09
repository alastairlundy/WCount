namespace WCountCli.Testing;

public sealed class CliIntegrationTests
{
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
        string filePath = CliTestRunner.FixturePath(file);
        string args = string.IsNullOrEmpty(flags) ? filePath : $"{flags} {filePath}";

        CliResult result = await CliTestRunner.RunAsync(args);
        string expected = await CliTestRunner.ReadBaselineAsync(baselineFile);

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    [Arguments("-l", "CRLF.txt", "crlf_line_count.txt")]
    [Arguments("", "EMPTY.txt", "empty_default.txt")]
    [Arguments("-w", "LARGE_WORD.txt", "large_word_count.txt")]
    [Arguments("-w -l", "WHITESPACE_ONLY.txt", "whitespace_word_line.txt")]
    public async Task EdgeCaseFiles_MatchesBaseline(string flags, string file, string baselineFile)
    {
        string filePath = CliTestRunner.FixturePath(file);
        string args = string.IsNullOrEmpty(flags) ? filePath : $"{flags} {filePath}";

        CliResult result = await CliTestRunner.RunAsync(args);
        string expected = await CliTestRunner.ReadBaselineAsync(baselineFile);

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task MultiFile_Total_MatchesBaseline()
    {
        string file1 = CliTestRunner.FixturePath("NATURE.txt");
        string file2 = CliTestRunner.FixturePath("CRLF.txt");

        CliResult result = await CliTestRunner.RunAsync($"-w -l {file1} {file2}");
        string expected = await CliTestRunner.ReadBaselineAsync("multi_file_total.txt");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task Help_ReturnsUsage()
    {
        CliResult result = await CliTestRunner.RunAsync("-?");
        string expected = await CliTestRunner.ReadBaselineAsync("help_output.txt");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task NonexistentFile_ReturnsError()
    {
        CliResult result = await CliTestRunner.RunAsync("nonexistent_file_xyz.txt");

        await Assert.That(result.ExitCode).IsEqualTo(1);
        await Assert.That(result.Stderr).Contains("nonexistent_file_xyz.txt");
    }

    [Test]
    public async Task CrlfFixture_ActuallyContainsCrlf()
    {
        string content = await File.ReadAllTextAsync(CliTestRunner.FixturePath("CRLF.txt"));

        await Assert.That(content).Contains("\r\n");
        await Assert.That(content.Replace("\r\n", "")).DoesNotContain("\r");
    }

    [Test]
    [Arguments("", "2 5 29\n")]
    [Arguments("-l", "2\n")]
    [Arguments("-w", "5\n")]
    [Arguments("-w -l", "2 5\n")]
    [Arguments("-w -l -m -c", "2 5 29 29\n")]
    [Arguments("-L", "16\n")]
    [Arguments("-l -L", "2 16\n")]
    public async Task StandardInput_HonoursRequestedCounts(string flags, string expected)
    {
        CliResult result = await CliTestRunner.RunAsync(flags, stdin: "hello world\nsecond line here\n");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(expected);
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task Total_Always_PrintsTotalForSingleFile()
    {
        string file = CliTestRunner.FixturePath("NATURE.txt");

        CliResult result = await CliTestRunner.RunAsync($"--total=always -w -l {file}");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(
            $"21 197 {CliTestRunner.TestFilesToken}NATURE.txt\n21 197 Total\n");
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task Total_Never_OmitsTotalForMultipleFiles()
    {
        string file1 = CliTestRunner.FixturePath("NATURE.txt");
        string file2 = CliTestRunner.FixturePath("CRLF.txt");

        CliResult result = await CliTestRunner.RunAsync($"--total=never -w -l {file1} {file2}");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(
            $"21 197 {CliTestRunner.TestFilesToken}NATURE.txt\n 3  24 {CliTestRunner.TestFilesToken}CRLF.txt\n");
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task Total_Only_PrintsTheTotalRowUnlabelled()
    {
        string file1 = CliTestRunner.FixturePath("NATURE.txt");
        string file2 = CliTestRunner.FixturePath("CRLF.txt");

        CliResult result = await CliTestRunner.RunAsync($"--total=only -w -l {file1} {file2}");

        await Assert.That(result.ExitCode).IsEqualTo(0);
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo("24 221\n");
        await Assert.That(result.Stderr).IsEmpty();
    }

    [Test]
    public async Task Total_InvalidWhen_ReturnsError()
    {
        string file = CliTestRunner.FixturePath("NATURE.txt");

        CliResult result = await CliTestRunner.RunAsync($"--total=sometimes {file}");

        await Assert.That(result.ExitCode).IsEqualTo(1);
        await Assert.That(result.Stderr).Contains("'--total'");
    }

    [Test]
    public async Task Files0From_ReadsNulSeparatedNames()
    {
        string listPath = Path.Combine(Path.GetTempPath(), $"wcount-files0-{Guid.NewGuid():N}.bin");

        try
        {
            string file1 = CliTestRunner.FixturePath("NATURE.txt");
            string file2 = CliTestRunner.FixturePath("CRLF.txt");
            await File.WriteAllTextAsync(listPath, $"{file1}\0{file2}\0");

            CliResult result = await CliTestRunner.RunAsync($"-w -l --files0-from={listPath}");
            string expected = await CliTestRunner.ReadBaselineAsync("multi_file_total.txt");

            await Assert.That(result.ExitCode).IsEqualTo(0);
            await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(expected);
            await Assert.That(result.Stderr).IsEmpty();
        }
        finally
        {
            File.Delete(listPath);
        }
    }

    [Test]
    public async Task Files0From_MissingList_ReturnsError()
    {
        CliResult result = await CliTestRunner.RunAsync("--files0-from=nonexistent_list_xyz.bin");

        await Assert.That(result.ExitCode).IsEqualTo(1);
        await Assert.That(result.Stderr).Contains("nonexistent_list_xyz.bin");
    }

    [Test]
    public async Task Files0From_CombinedWithFileOperands_ReturnsError()
    {
        string listPath = Path.Combine(Path.GetTempPath(), $"wcount-files0-{Guid.NewGuid():N}.bin");

        try
        {
            await File.WriteAllTextAsync(listPath, string.Empty);

            CliResult result = await CliTestRunner.RunAsync(
                $"--files0-from={listPath} {CliTestRunner.FixturePath("NATURE.txt")}");

            await Assert.That(result.ExitCode).IsEqualTo(1);
            await Assert.That(result.Stderr).Contains("cannot be combined");
        }
        finally
        {
            File.Delete(listPath);
        }
    }

    [Test]
    public async Task MissingFile_PrintsPerFileError_AndContinues()
    {
        string nature = CliTestRunner.FixturePath("NATURE.txt");
        string missing = CliTestRunner.FixturePath("nonexistent_file_xyz.txt");
        string crlf = CliTestRunner.FixturePath("CRLF.txt");

        CliResult result = await CliTestRunner.RunAsync($"-w -l {nature} {missing} {crlf}");

        await Assert.That(result.ExitCode).IsEqualTo(1);
        await Assert.That(result.Stderr).Contains("nonexistent_file_xyz.txt");
        await Assert.That(CliTestRunner.Normalise(result.Stdout)).IsEqualTo(
            $"21 197 {CliTestRunner.TestFilesToken}NATURE.txt\n" +
            $" 3  24 {CliTestRunner.TestFilesToken}CRLF.txt\n" +
            "24 221 Total\n");
    }
}
