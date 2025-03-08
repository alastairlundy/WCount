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

using WCount.Cli.Localizations;

namespace WCount.Cli.Helpers
{
    internal static class FileArgumentHelpers
    {
        private static bool IsGlobbedPath(string filePath)
        {
            return Regex.IsMatch(filePath, @"[\*\?$$$$\{\}]");
        }
        
        public static string[] ResolveFilePaths(string[] files, bool isVerbose)
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
                            ExceptionHelper.PrintException(new FileNotFoundException(Resources.Exceptions_FileNotFound, file), isVerbose);
                        }
                    }
                    else
                    {
                        string newFile = Path.GetFullPath(file);

                        #if DEBUG
                        Console.WriteLine($"Old was {file}");
                        Console.WriteLine($"New is {newFile}");
                        #endif
                        
                        output.Add(file);
                    }
                }
                else
                {
                    output.Add(file);
                }
            }
            
            return output.ToArray();
        }
        
        internal static int HandleFileArgument(string[]? files, bool isVerbose)
        {
            if(files == null)
            {
                ExceptionHelper.PrintException(new NullReferenceException(Resources.Exceptions_NoFileProvided), isVerbose);
                return -1;
            }
            else if(files.Length == 0)
            {
                ExceptionHelper.PrintException(new ArgumentException(Resources.Exceptions_NoFileProvided), isVerbose);
                return -1;
            }

            return 0;
        }
    }
}
