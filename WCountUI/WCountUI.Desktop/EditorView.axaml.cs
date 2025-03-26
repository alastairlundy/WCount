using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace WCountUI.Desktop;

public partial class EditorView : UserControl
{
    private readonly IWordCounter _wordCounter;
    private readonly ICharacterCounter _characterCounter;

    public string Words { get; set; }

    public ulong WordCount { get; set; }
    public ulong CharacterCount { get; set; }

    public EditorView(IWordCounter wordCounter, ICharacterCounter characterCounter)
    {
        InitializeComponent();

        WordCount = 0;
        CharacterCount = 0;

        Words = string.Empty;

        _wordCounter = wordCounter;
        _characterCounter = characterCounter; 
    }

    public async Task UpdateWordCountAsync()
    {
        WordCount = await _wordCounter.CountWordsAsync(new StringReader(Words));
        CharacterCount = await _characterCounter.CountCharactersAsync(new StringReader(Words), Encoding.Default);
    }

    private void TextEditor_TextChanged(object? sender, TextChangedEventArgs e)
    {
        Dispatcher.UIThread.InvokeAsync(() => { using var _ = UpdateWordCountAsync(); });
    }
}