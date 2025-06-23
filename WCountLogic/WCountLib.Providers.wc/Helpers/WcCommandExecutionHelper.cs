/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke.Core.Abstractions;
using AlastairLundy.CliInvoke.Core.Builders;
using AlastairLundy.CliInvoke.Core.Builders.Abstractions;
using AlastairLundy.CliInvoke.Core.Primitives;
using AlastairLundy.CliInvoke.Core.Primitives.Exceptions;
using AlastairLundy.CliInvoke.Core.Primitives.Results;

using AlastairLundy.DotExtensions.MsExtensions.System.Collections;

using Microsoft.Extensions.Primitives;

#if NETSTANDARD2_0 || NETSTANDARD2_1
using OperatingSystem = Polyfills.OperatingSystemPolyfill;
#endif

namespace AlastairLundy.WCountLib.Providers.wc.Helpers;

internal class WcCommandExecutionHelper
{
    private readonly IProcessInvoker _processInvoker;
    
    private string _tempFilePath;
    
    private string CreateTempFilePath(TextReader textReader)
    {
        string tempFilePath = Path.GetTempFileName();
        tempFilePath = Path.ChangeExtension(tempFilePath, ".txt");
        
        _tempFilePath = tempFilePath;
        
        using (var writer = new StreamWriter(tempFilePath))
        {
            writer.Write(textReader.ReadToEnd());
            writer.Close();
        }
        
        return tempFilePath;
    }
    
    internal WcCommandExecutionHelper(IProcessInvoker processInvoker)
    {
        _tempFilePath = string.Empty;
        _processInvoker = processInvoker;
    }

    internal TextReader GetSegmentsToTextReader(IEnumerable<StringSegment> segments)
    {
        return new StringReader(segments.ToString(' '));
    }
    
    private async Task<BufferedProcessResult> ExecuteAsync(string argument, TextReader textReader)
    {
        IProcessConfigurationBuilder processConfigurationBuilder = new ProcessConfigurationBuilder(
                "/usr/bin/wc")
                .WithArguments($"{argument}, {CreateTempFilePath(textReader)}")
            .WithValidation(ProcessResultValidation.ExitCodeZero);
        
        ProcessConfiguration processConfiguration = processConfigurationBuilder.Build();
        
        File.Delete(_tempFilePath);
        
        BufferedProcessResult result = await _processInvoker.
            ExecuteBufferedProcessAsync(processConfiguration, CancellationToken.None);

        return result;
    }

    internal int RunInt32(string argument, TextReader textReader)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }

        Task<BufferedProcessResult> resultTask =  ExecuteAsync(argument, textReader);
			
        resultTask.Start();

        resultTask.Wait();

        string resultString = resultTask.Result.StandardOutput.Split(' ').First();

        return int.Parse(resultString);
    }
  
    internal async Task<int> RunInt32Async(string argument, TextReader textReader)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
			
        BufferedProcessResult result = await ExecuteAsync(argument, textReader);

        string resultString = result.StandardOutput.Split(' ').First();

        return int.Parse(resultString);
    }
}