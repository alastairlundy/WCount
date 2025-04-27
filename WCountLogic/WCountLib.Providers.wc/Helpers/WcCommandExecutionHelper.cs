/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke;
using AlastairLundy.CliInvoke.Abstractions;
using AlastairLundy.CliInvoke.Builders;
using AlastairLundy.CliInvoke.Builders.Abstractions;
using AlastairLundy.CliInvoke.Exceptions;

using AlastairLundy.DotExtensions.MsExtensions.System.Collections;
using AlastairLundy.Extensions.Processes.Abstractions;

using Microsoft.Extensions.Primitives;

#if NETSTANDARD2_0 || NETSTANDARD2_1
using OperatingSystem = Polyfills.OperatingSystemPolyfill;
#endif

namespace AlastairLundy.WCountLib.Providers.wc.Helpers;

internal class WcCommandExecutionHelper
{
    private readonly ICliCommandInvoker _commandInvoker;

    private CliCommandConfiguration _commandConfiguration;

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
    
    internal WcCommandExecutionHelper(ICliCommandInvoker commandInvoker)
    {
        _tempFilePath = string.Empty;
        _commandInvoker = commandInvoker;
        ICliCommandConfigurationBuilder commandConfigurationBuilder = new CliCommandConfigurationBuilder(
                "/usr/bin/wc")
            .WithValidation(ProcessResultValidation.None);
         
         _commandConfiguration = commandConfigurationBuilder.Build();
    }

    internal TextReader GetSegmentsToTextReader(IEnumerable<StringSegment> segments)
    {
        return new StringReader(segments.ToString(' '));
    }
    
    private async Task<BufferedProcessResult> ExecuteAsync(string argument, TextReader textReader)
    {
        ICliCommandConfigurationBuilder configurationBuilder = new CliCommandConfigurationBuilder(_commandConfiguration)
            .WithArguments($"{argument}, {CreateTempFilePath(textReader)}");
        
        _commandConfiguration = configurationBuilder.Build();
        
        File.Delete(_tempFilePath);
        
        BufferedProcessResult result = 
            await _commandInvoker.ExecuteBufferedAsync(_commandConfiguration, CancellationToken.None);

        return result;
    }

    internal int RunInt32(string argument, TextReader textReader)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }

        Task<BufferedProcessResult> resultTask =  ExecuteAsync("-w", textReader);
			
        resultTask.Start();

        resultTask.Wait();

        if (resultTask.Result.ExitCode != 0 || resultTask.Result.StandardOutput.ToLower().Contains("illegal"))
        {
            throw new CliCommandNotSuccessfulException(resultTask.Result.ExitCode, _commandConfiguration);
        }

        string resultString = resultTask.Result.StandardOutput.Split(' ').First();

        return int.Parse(resultString);
    }
    
    internal ulong RunUInt64(string argument, TextReader textReader)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }

        Task<BufferedProcessResult> resultTask =  ExecuteAsync("-w", textReader);
			
        resultTask.Start();

        resultTask.Wait();

        if (resultTask.Result.ExitCode != 0 || resultTask.Result.StandardOutput.ToLower().Contains("illegal"))
        {
            throw new CliCommandNotSuccessfulException(resultTask.Result.ExitCode, _commandConfiguration);
        }

        string resultString = resultTask.Result.StandardOutput.Split(' ').First();

        return ulong.Parse(resultString);
    }
  
    internal async Task<int> RunInt32Async(string argument, TextReader textReader)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
			
        BufferedProcessResult result = await ExecuteAsync(argument, textReader);
			
        if (result.ExitCode != 0 || result.StandardOutput.ToLower().Contains("illegal"))
        {
            throw new CliCommandNotSuccessfulException(result.ExitCode, _commandConfiguration);
        }

        string resultString = result.StandardOutput.Split(' ').First();

        return int.Parse(resultString);
    }
    
    internal async Task<ulong> RunUInt64Async(string argument, TextReader textReader)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
			
        BufferedProcessResult result = await ExecuteAsync(argument, textReader);
			
        if (result.ExitCode != 0 || result.StandardOutput.ToLower().Contains("illegal"))
        {
            throw new CliCommandNotSuccessfulException(result.ExitCode, _commandConfiguration);
        }

        string resultString = result.StandardOutput.Split(' ').First();

        return ulong.Parse(resultString);
    }
}