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
using AlastairLundy.CliInvoke.Builders;
using AlastairLundy.CliInvoke.Core;
using AlastairLundy.CliInvoke.Core.Builders;

using AlastairLundy.CliInvoke.Core.Primitives;

using Microsoft.Extensions.Primitives;

namespace AlastairLundy.WCountLib.Providers.wc.Helpers;

internal class WcCommandExecutionHelper
{
    private readonly IProcessInvoker _processInvoker;
    
    private string _tempFilePath;
    
    private async Task<string> CreateTempFilePathAsync(string text)
    {
        string tempFilePath = Path.GetTempFileName();
        tempFilePath = Path.ChangeExtension(tempFilePath, ".txt");
        
        _tempFilePath = tempFilePath;
        
        await File.WriteAllTextAsync(tempFilePath, text);
        
        return tempFilePath;
    }
    
    internal WcCommandExecutionHelper(IProcessInvoker processInvoker)
    {
        _tempFilePath = string.Empty;
        _processInvoker = processInvoker;
    }

    internal string GetSegmentsToString(IEnumerable<StringSegment> segments)
    {
        return string.Join(' ', segments);
    }
    
    private async Task<BufferedProcessResult> ExecuteAsync(string argument, string tempFileName)
    {
        IProcessConfigurationBuilder processConfigurationBuilder = new ProcessConfigurationBuilder(
                "/usr/bin/wc")
            .WithArguments($"{argument}, {tempFileName}");
        
        ProcessConfiguration processConfiguration = processConfigurationBuilder.Build();
        
        File.Delete(_tempFilePath);
        
        BufferedProcessResult result = await _processInvoker.
            ExecuteBufferedAsync(processConfiguration, ProcessExitInfo.Default, CancellationToken.None);

        return result;
    }
    
    internal int RunInt32(string argument, string text)
    {
        if (OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException();
        }
        
        Task<string> tempFile = CreateTempFilePathAsync(text);
        tempFile.Start();
        tempFile.Wait();
        
        Task<BufferedProcessResult> resultTask =  ExecuteAsync(argument, tempFile.Result);
        resultTask.Start();
        resultTask.Wait();

        string resultString = resultTask.Result.StandardOutput.Split(' ').First();

        return int.Parse(resultString);
    }

    internal async Task<int> RunInt32Async(string argument, string text)
    {
        if (OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException();
        
        string tempFile = await CreateTempFilePathAsync(text);
			
        BufferedProcessResult result = await ExecuteAsync(argument, tempFile);

        string resultString = result.StandardOutput.Split(' ').First();

        return int.Parse(resultString);
    }
}