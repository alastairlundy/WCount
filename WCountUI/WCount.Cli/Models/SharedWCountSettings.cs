using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spectre.Console.Cli;

namespace BasisBox.Cli.Tools.WCount.Settings
{
    public class SharedWCountSettings : CommandSettings
    {
        [CommandArgument(1, "<files>")]
        public string[]? Files { get; init; }

        [CommandOption("-v|--verbose")]
        [DefaultValue(false)]
        public bool Verbose { get; init; }
    }
}
