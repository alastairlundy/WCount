/*
    WCountLib.Providers.wc
    Copyright (C) 2025 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Linq;
using System.Threading;

namespace WCountLib.Providers.wc.Helpers;

internal class WcCommandExecutionHelper
{
    private readonly IProcessConfigurationFactory _processConfigurationFactory;
    
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
    
    internal WcCommandExecutionHelper(IProcessInvoker processInvoker, IProcessConfigurationFactory processConfigurationFactory)
    {
        _tempFilePath = string.Empty;
        _processInvoker = processInvoker;
        _processConfigurationFactory = processConfigurationFactory;
    }
    
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
    private async Task<BufferedProcessResult> ExecuteAsync(string argument, string tempFileName)
    {
       ProcessConfiguration processConfiguration = _processConfigurationFactory
           .Create("/usr/bin/wc", $"{argument}, {tempFileName}");
       
        BufferedProcessResult result = await _processInvoker.
            ExecuteBufferedAsync(processConfiguration, ProcessExitConfiguration.DefaultNoException,
                true, cancellationToken: CancellationToken.None);

        File.Delete(_tempFilePath);
        
        return result;
    }
    
#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
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

#if NET8_0_OR_GREATER
    [UnsupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("maccatalyst")]
    [SupportedOSPlatform("freebsd")]
    [UnsupportedOSPlatform("ios")]
    [UnsupportedOSPlatform("tvos")]
#endif
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