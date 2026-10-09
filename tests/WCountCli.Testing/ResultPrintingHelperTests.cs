namespace WCountCli.Testing;

/// <summary>
/// Pins the table builder's contract: column order, table-wide right-aligned
/// widths including the Total row, single-column spacing, and unlabelled rows.
/// </summary>
public class ResultPrintingHelperTests
{
    private static CountResult Result(
        long words = 0, long lines = 0, long bytes = 0, long characters = 0, long maxLength = -1) =>
        new(words, lines, bytes, characters, maxLength);

    private static async Task<string> PrintAsync(CountRequest request, params ResultRow[] rows)
    {
        StringWriter writer = new();
        await ResultPrintingHelper.PrintTableAsync(writer, request, rows);
        return writer.ToString().ReplaceLineEndings("\n");
    }

    private static CountRequest WordsAndLines =>
        new(Words: true, Lines: true, Bytes: false, Characters: false);

    [Test]
    public async Task EmptyTable_PrintsNothing()
    {
        string table = await PrintAsync(WordsAndLines);

        await Assert.That(table).IsEmpty();
    }

    [Test]
    public async Task SingleColumn_PrintsWithoutLeadingSpace()
    {
        // A lone selected column is not padded on the left: no wider row in the
        // table demands a leading digit slot.
        CountRequest wordsOnly = new(Words: true, Lines: false, Bytes: false, Characters: false);

        string table = await PrintAsync(wordsOnly, new ResultRow("a.txt", Result(words: 197)));

        await Assert.That(table).IsEqualTo("197 a.txt\n");
    }

    [Test]
    public async Task SingleColumn_WiderRowPadsNarrowerOnes()
    {
        CountRequest wordsOnly = new(Words: true, Lines: false, Bytes: false, Characters: false);

        string table = await PrintAsync(wordsOnly,
            new ResultRow("a.txt", Result(words: 5)),
            new ResultRow("Total", Result(words: 1234)));

        await Assert.That(table).IsEqualTo("   5 a.txt\n1234 Total\n");
    }

    [Test]
    public async Task MultipleColumns_SeparatedBySingleSpace()
    {
        string table = await PrintAsync(WordsAndLines, new ResultRow("a.txt", Result(words: 5, lines: 21)));

        await Assert.That(table).IsEqualTo("21 5 a.txt\n");
    }

    [Test]
    public async Task Columns_EmittedInLinesWordsCharactersBytesMaxLengthOrder()
    {
        CountRequest everything = new(Words: true, Lines: true, Bytes: true, Characters: true,
            MaximumLineLength: true);

        string table = await PrintAsync(everything,
            new ResultRow("a.txt", Result(words: 1, lines: 2, bytes: 3, characters: 4, maxLength: 5)));

        // Order: lines, words, characters, bytes, maximum line length.
        await Assert.That(table).IsEqualTo("2 1 4 3 5 a.txt\n");
    }

    [Test]
    public async Task UnselectedColumns_AreNeverPrinted()
    {
        // The result carries every counted value; only the requested columns
        // reach the output.
        CountRequest wordsOnly = new(Words: true, Lines: false, Bytes: false, Characters: false);

        string table = await PrintAsync(wordsOnly,
            new ResultRow("a.txt", Result(words: 197, lines: 21, bytes: 1297, characters: 1297)));

        await Assert.That(table).IsEqualTo("197 a.txt\n");
    }

    [Test]
    public async Task Columns_AlignAcrossAllRowsIncludingTotal()
    {
        // The Total row participates in column sizing: every row's counts line
        // up digit-for-digit once the widest row (the total) is known.
        string table = await PrintAsync(WordsAndLines,
            new ResultRow("NATURE.txt", Result(words: 197, lines: 21)),
            new ResultRow("CRLF.txt", Result(words: 24, lines: 3)),
            new ResultRow("Total", Result(words: 221, lines: 24)));

        await Assert.That(table).IsEqualTo(
            "21 197 NATURE.txt\n" +
            " 3  24 CRLF.txt\n" +
            "24 221 Total\n");
    }

    [Test]
    public async Task UnlabelledRow_EndsWithItsCounts()
    {
        // Standard input, and the Total row under --total=only, print as counts
        // with no trailing label and no trailing space.
        string table = await PrintAsync(WordsAndLines,
            new ResultRow(string.Empty, Result(words: 5, lines: 2)),
            new ResultRow("a.txt", Result(words: 197, lines: 21)));

        await Assert.That(table).IsEqualTo(
            " 2   5\n" +
            "21 197 a.txt\n");
    }

    [Test]
    public async Task MaximumLineLengthColumn_AlignsWithinItsOwnColumn()
    {
        CountRequest linesAndLength = new(Words: false, Lines: true, Bytes: false, Characters: false,
            MaximumLineLength: true);

        string table = await PrintAsync(linesAndLength,
            new ResultRow("a.txt", Result(lines: 21, maxLength: 8)),
            new ResultRow("Total", Result(lines: 24, maxLength: 16)));

        await Assert.That(table).IsEqualTo(
            "21  8 a.txt\n" +
            "24 16 Total\n");
    }
}
