using System.IO;
using System.Text;
using System.Threading.Tasks;

using AlastairLundy.WCountLib.Abstractions.Counters;

using CommunityToolkit.Mvvm.ComponentModel;

using WCountUI.XPlatApp.Localizations;

namespace WCountUI.XPlatApp.ViewModels;

public partial class EditorViewModel : ViewModelBase
{
    private readonly IWordCounter _wordCounter;
    private readonly ICharacterCounter _characterCounter;

    public EditorViewModel(IWordCounter wordCounter, ICharacterCounter characterCounter)
    {
        _wordCounter = wordCounter;
        _characterCounter = characterCounter;
        Text = string.Empty;
    }
    
    [ObservableProperty]
    private ulong _wordCount = 0;
    
    [ObservableProperty]
    private ulong _characterCount = 0;
    
    
    [ObservableProperty]
    private string _text;
    
    private string WordCountText
    {
        get
        {
            if (WordCount == 1)
            {
                return $"{WordCount} {Resources.App_Labels_Words_Singular}";
            }
            else
            {
                return $"{WordCount} {Resources.App_Labels_Words_Plural}";
            }
        }
    }
    
    private async Task UpdateCountsAsync()
    {
        StringReader stringReader = new(Text);
        StringReader stringReader2 = new(Text);
        
       WordCount = await _wordCounter.CountWordsAsync(stringReader);
       CharacterCount = await _characterCounter.CountCharactersAsync(stringReader2, Encoding.Default);
    }
}