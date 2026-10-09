namespace WCountCli.Testing;

/// <summary>
/// Pins the flag-to-CountRequest mapping: a flagless run asks for words, lines,
/// and bytes, and any count flag makes the run selective with -L opt-in.
/// </summary>
public class CountRequestMapperTests
{
    [Test]
    public async Task Flagless_MapsToWordsLinesAndBytes()
    {
        CountRequest request = CountRequestMapper.ToRequest(
            words: false, lines: false, characters: false, bytes: false, maximumLineLength: false);

        await Assert.That(request.Words).IsTrue();
        await Assert.That(request.Lines).IsTrue();
        await Assert.That(request.Bytes).IsTrue();
        await Assert.That(request.Characters).IsFalse();
        await Assert.That(request.MaximumLineLength).IsFalse();
    }

    [Test]
    [Arguments(true, false, false, false)]
    [Arguments(false, true, false, false)]
    [Arguments(false, false, true, false)]
    [Arguments(false, false, false, true)]
    public async Task SingleFlag_SelectsOnlyThatCount(
        bool words, bool lines, bool characters, bool bytes)
    {
        CountRequest request = CountRequestMapper.ToRequest(words, lines, characters, bytes,
            maximumLineLength: false);

        await Assert.That(request.Words).IsEqualTo(words);
        await Assert.That(request.Lines).IsEqualTo(lines);
        await Assert.That(request.Characters).IsEqualTo(characters);
        await Assert.That(request.Bytes).IsEqualTo(bytes);
        await Assert.That(request.MaximumLineLength).IsFalse();
    }

    [Test]
    public async Task MaximumLineLengthOnly_SelectsOnlyTheLength()
    {
        // -L is opt-in and off by default; alone it does not drag the GNU
        // default set along.
        CountRequest request = CountRequestMapper.ToRequest(
            words: false, lines: false, characters: false, bytes: false, maximumLineLength: true);

        await Assert.That(request.Words).IsFalse();
        await Assert.That(request.Lines).IsFalse();
        await Assert.That(request.Characters).IsFalse();
        await Assert.That(request.Bytes).IsFalse();
        await Assert.That(request.MaximumLineLength).IsTrue();
    }

    [Test]
    public async Task EveryFlag_SelectsEveryCount()
    {
        CountRequest request = CountRequestMapper.ToRequest(
            words: true, lines: true, characters: true, bytes: true, maximumLineLength: true);

        await Assert.That(request.Words).IsTrue();
        await Assert.That(request.Lines).IsTrue();
        await Assert.That(request.Characters).IsTrue();
        await Assert.That(request.Bytes).IsTrue();
        await Assert.That(request.MaximumLineLength).IsTrue();
    }
}
