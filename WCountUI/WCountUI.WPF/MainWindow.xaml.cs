using System.ComponentModel;
using System.IO;
using System.Text;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using AlastairLundy.WCountLib.Abstractions.Counters;

namespace WCountUI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IWordCounter _wordCounter;
        private readonly ICharacterCounter _characterCounter;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Text { get; set; }

        public ulong WordCount { get; set; }
        public int CharacterCount { get; set; }

        public string WordCountText
        {
            get
            {
                if(WordCount == 1)
                {
                    return  Localizations.Resources.Labels_Words_Singular;
                }
                else
                {
                    return Localizations.Resources.Labels_Words_Plural;
                }
            }
        }

        public string CharacterCountText
        {
            get
            {
                if (CharacterCount == 1)
                {
                    return Localizations.Resources.Labels_Characters_Singular;
                }
                else
                {
                    return Localizations.Resources.Labels_Characters_Plural;
                }
            }
        }

        public MainWindow(IWordCounter wordCounter, ICharacterCounter characterCounter)
        {
            InitializeComponent();
            Title = Localizations.Resources.App_Labels_Name;

            _wordCounter = wordCounter;
            _characterCounter = characterCounter;

            Text = string.Empty;
            WordCount = 0;
            CharacterCount = 0; 
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task task = Task.Run(() => {
                WordCount = _wordCounter.CountWords(new StringReader(Text));
                CharacterCount = _characterCounter.CountCharacters(new StringReader(Text), Encoding.Default);
            });
        }
    }
}