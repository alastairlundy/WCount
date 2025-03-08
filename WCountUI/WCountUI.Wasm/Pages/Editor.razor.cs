using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
	
using AlastairLundy.WCountLib.Abstractions.Counters;

using Microsoft.AspNetCore.Components;

using Microsoft.JSInterop;
using WCountUI.Wasm.Localizations;


namespace WCountUI.Wasm.Pages;

public partial class Editor
{
    private const string TextEditorId = "text-editor";

    private readonly IWordCounter _wordCounter;
    private readonly ICharacterCounter _charCounter;

	private string _editorText;
	private double _editorFontSize;

    private readonly IJSRuntime jsRuntime;

    private readonly int editorIndex;

	public ulong WordCount { get; private set; }
	public int CharCount { get; private set; }


	public Editor(IWordCounter wordCounter, ICharacterCounter charCounter, IJSRuntime jsRuntime)
    {
        editorIndex = 0;

        _wordCounter = wordCounter;
        _charCounter = charCounter;
        _editorText = string.Empty;

        WordCount = 0;
        this.jsRuntime = jsRuntime;

		_editorFontSize = 11.0;
		EditorFontSizeChanged(_editorFontSize);

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
			_editorFontSize = fontSize;

			await jsRuntime.InvokeVoidAsync("setEditorFontSize", [TextEditorId, _editorFontSize]);
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

        using StringReader reader = new StringReader(_editorText);
        
        WordCount = await _wordCounter.CountWordsAsync(reader);
        CharCount = await Task.FromResult(_charCounter.CountCharacters(reader, Encoding.Default));
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
            Console.WriteLine(Resources.Editor_PasteClipboard_Exception);
            Console.WriteLine(ex.Message);
        }
	}
}