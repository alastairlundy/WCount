namespace WCount.Cli.Models;

public class TextFileModel
{
    public TextFileModel(string fileName, string text, bool isTemporaryFile)
    {
        FileName = fileName;
        Text = text;
        IsTemporaryFile = isTemporaryFile;
    }
    
    public bool IsTemporaryFile { get; private set; }
    
    public string FileName { get; private set; }
    
    public string Text { get; private set; }
}