namespace WCountCli.Helpers;

public static class FormattingHelpers
{
    public static string FormatOutput(string str, int requiredSpacing)
    {
        StringBuilder stringBuilder = new();
        stringBuilder.Append(' ');
        
        int spacesToAdd = Math.Abs(str.Length - requiredSpacing);
    
        for (long i = 0; i < spacesToAdd; i++)
        {
            stringBuilder.Append(' ');
        }
    
        return stringBuilder.ToString();
    }

    public static int CalculateRequiredSpacing(long[] stats)
    {
        int maximum = 0;
    
        foreach (int stat in stats)
        {
            maximum = int.Max(maximum, stat);
        }

        return maximum;
    }
}