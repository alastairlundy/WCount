using System.ComponentModel;
using Spectre.Console.Cli;

namespace WCount.Cli.Models
{
    public class SharedWCountSettings : CommandSettings
    {
        [CommandOption("--locale <locale_code>")]
        [DefaultValue("")]
        public string? Locale { get; init; }

        [CommandArgument(1, "<files>")]
        public string[]? Files { get; init; }

        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }
    }
}
