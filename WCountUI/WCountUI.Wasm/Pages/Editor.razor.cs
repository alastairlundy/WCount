using System;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using Microsoft.AspNetCore.Components;

using Microsoft.JSInterop;

using WCountLib.Counters.Abstractions;

namespace WCountUI.Wasm.Pages;

public partial class Editor
{
    private const string TextEditorId = "text-editor";

    private readonly IWordCounter _wordCounter;
    private readonly ICharCounter _charCounter;

	private string _editorText;
	private double editorFontSize;

    private IJSRuntime jsRuntime;

    private int editorIndex;

	public ulong WordCount { get; private set; }
	public int CharCount { get; private set; }


	public Editor(IWordCounter wordCounter, ICharCounter charCounter, IJSRuntime jsRuntime)
    {
        editorIndex = 0;

        _wordCounter = wordCounter;
        _charCounter = charCounter;
        _editorText = string.Empty;

        WordCount = 0;
        this.jsRuntime = jsRuntime;

		editorFontSize = 11.0;
		EditorFontSizeChanged(editorFontSize);

        Timer automaticCountTimer = new Timer(TimeSpan.FromMinutes(1));
		automaticCountTimer.Elapsed += AutomaticCountTimer_Elapsed;
        automaticCountTimer.Start();
    }

	private void AutomaticCountTimer_Elapsed(object? sender, ElapsedEventArgs e)
	{
        ulong priorCount = WordCount;

        ChangeEventArgs arg = new ChangeEventArgs();
        arg.Value = _editorText;

        Console.WriteLine("Automatic Editor Word Count");
        PerformCount(arg);

        ulong newCount = WordCount;

        if(priorCount != newCount)
        {
			Console.WriteLine($"Fixed word count from {priorCount} to {newCount}");
		}
	}

    private async void EditorFontSizeChanged(double fontSize)
    {
        try
        {
			editorFontSize = fontSize;

			await jsRuntime.InvokeVoidAsync("setEditorFontSize", [TextEditorId, editorFontSize]);
		}
        catch (Exception ex)
        {
            
        }
    }

  //  private async void CountCallback(ChangeEventArgs e)
  //  {
  //      await PerformCount(e);

		////Update Editor Index

		////var position = await jsRuntime.InvokeAsync<string>("getEditorCaretPosition", TextEditorId);
  //     // Console.WriteLine($"Position is {position}");
  //  }



	private async void PerformCount(ChangeEventArgs e)
    {
        _editorText = e.Value!.ToString()!;

        WordCount = await _wordCounter.CountWordsAsync(_editorText);
        CharCount = await Task.FromResult(_charCounter.CountCharacters(_editorText, Encoding.Default));
    }

    private void ClearEditor()
    {
        _editorText = string.Empty;
        WordCount = 0;
        CharCount = 0;
    }

    private async void PasteClipboard()
    {
        try
        {
            string? clipboardText = await jsRuntime.InvokeAsync<string>("getClipboardText", TextEditorId);

            Console.WriteLine($"The clipboard is {clipboardText}");

            if (clipboardText != null)
            {
				if (_editorText.Length == 0)
				{
					_editorText = clipboardText;
				}
				else
				{
					_editorText = _editorText.Insert(editorIndex, clipboardText);
				}
			}
            else
            {
                throw new NullReferenceException("The clipboard was null.");
            }
		}
        catch(Exception ex)
        {
            Console.WriteLine("Pasting clipboard failed. Here's the exception in case you need it:");
            Console.WriteLine(ex.Message);
        }
	}
}