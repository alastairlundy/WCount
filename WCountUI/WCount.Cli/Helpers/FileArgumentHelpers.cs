/*
	WCount CLI
	Copyright (c) Alastair Lundy 2024-2025
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Pathological.Globbing;
using Spectre.Console;

using WCount.Cli.Localizations;

namespace WCount.Cli.Helpers
{
    internal static class FileArgumentHelpers
    {
        private static bool IsGlobbedPath(string filePath)
        {
            return Regex.IsMatch(filePath, @"[\*\?$$$$\{\}]");
        }
        
        public static string[] ResolveFilePaths(string[] files)
        {
            List<string> output = new List<string>();
            
            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];

                if (Path.IsPathFullyQualified(file) == false)
                {
                    if (IsGlobbedPath(file))
                    {
                        Glob glob = new Glob();

                        string[] actualFiles = glob.GetMatches(file).ToArray();

                        if (actualFiles.Any())
                        {
                            output.AddRange(actualFiles);
                        }
                        else
                        {
                            AnsiConsole.WriteException(new FileNotFoundException(Resources.Exceptions_FileNotFound, file));
                        }
                    }
                    else
                    {
                        file = Path.GetFullPath(file);
                        
                        output.Add(file);
                    }
                }
            }
            
            return output.ToArray();
        }
        
        public static int HandleFileArgument(string? file, ExceptionFormats exceptionFormats)
        {
            if(file == null)
            {
                AnsiConsole.WriteException(new ArgumentException(Resources.Exceptions_NoFileProvided), exceptionFormats);
                return -1;
            }

            return 0;
        }
        
        internal static int HandleFileArgument(string[]? files, ExceptionFormats formats)
        {
            if(files == null)
            {
                AnsiConsole.WriteException(new NullReferenceException(Resources.Exceptions_NoFileProvided), formats);
                return -1;
            }
            else if(files.Length == 0)
            {
                AnsiConsole.WriteException(new ArgumentException(Resources.Exceptions_NoFileProvided));
                return -1;
            }

            return 0;
        }
    }
}
