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
    public partial class MainWindow : Window
    {
        private readonly IWordCounter _wordCounter;
        private readonly ICharacterCounter _characterCounter;


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

            wordCountLabel.Text = WordCount.ToString();
            charCountLabel.Text = CharacterCount.ToString();

            wordsLabel.Text = WordCountText;
            charsLabel.Text = CharacterCountText;

            InitializeNumericUpDown();
        }

        private void InitializeNumericUpDown()
        {
            //Assign a default value.
            fontSizeSelector.Value = 12;
            fontSizeSelector.HideUpDownButtons = false;

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = textBox.Text;

            Task task = Task.Run(() => {
                WordCount = _wordCounter.CountWords(new StringReader(Text));
                CharacterCount = _characterCounter.CountCharacters(new StringReader(Text), Encoding.Default);
            });

            task.Wait();

            wordCountLabel.Text = WordCount.ToString();
            charCountLabel.Text = CharacterCount.ToString();

            wordsLabel.Text = WordCountText;
            charsLabel.Text = CharacterCountText;
        }

        private void fontSizeSelector_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            textBox.FontSize = double.Parse(fontSizeSelector.Value.ToString()!);
        }

        private void clearTextBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            textBox.Text = string.Empty;
        }

        private void clipboardCopyBtn_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox.Text);
        }
    }
}