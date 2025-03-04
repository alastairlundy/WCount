using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AlastairLundy.CliInvoke;
using AlastairLundy.CliInvoke.Abstractions;
using AlastairLundy.CliInvoke.Builders;
using AlastairLundy.CliInvoke.Builders.Abstractions;
using AlastairLundy.CliInvoke.Exceptions;
using AlastairLundy.Extensions.Processes;

using AlastairLundy.WCountLib.Abstractions.Counters;



#if NETSTANDARD2_0 || NETSTANDARD2_1
using OperatingSystem = Polyfills.OperatingSystemPolyfill;
#endif

namespace WCountLib.Providers.wc.Counters
{
	public class WcWordCounter : IWordCounter
	{
		private readonly ICliCommandInvoker _cliCommandInvoker;

		public WcWordCounter(ICliCommandInvoker cliCommandInvoker)
		{
			_cliCommandInvoker = cliCommandInvoker;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="textReader"></param>
		/// <returns></returns>
		/// <exception cref="PlatformNotSupportedException">Thrown if run on Windows</exception>
		public ulong CountWords(TextReader textReader)
		{
			if (OperatingSystem.IsWindows())
			{
				throw new PlatformNotSupportedException();
			}

			string tempFilePath = Path.GetTempFileName();
			tempFilePath =	Path.ChangeExtension(tempFilePath, ".txt");

			using (var writer = new StreamWriter(tempFilePath))
			{
				writer.Write(textReader.ReadToEnd());
				writer.Close();
			}

			ICliCommandConfigurationBuilder commandConfigurationBuilder = new CliCommandConfigurationBuilder(
				"/usr/bin/wc")
				.WithArguments($"-w {tempFilePath}")
				.WithValidation(AlastairLundy.Extensions.Processes.ProcessResultValidation.ExitCodeZero);

			CliCommandConfiguration cliCommandConfiguration = commandConfigurationBuilder.Build();

			Task<BufferedProcessResult> resultTask = _cliCommandInvoker.ExecuteBufferedAsync(cliCommandConfiguration, CancellationToken.None);

			resultTask.Start();

			resultTask.Wait();

			File.Delete(tempFilePath);

			if (resultTask.Result.ExitCode != 0 || resultTask.Result.StandardOutput.ToLower().Contains("illegal"))
			{
				throw new CliCommandNotSuccessfulException(resultTask.Result.ExitCode, cliCommandConfiguration);
			}

			string numberOfWordsString = resultTask.Result.StandardOutput.Split(' ').First();

			return ulong.Parse(numberOfWordsString);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="textReader"></param>
		/// <returns></returns>
		/// <exception cref="PlatformNotSupportedException"></exception>
		/// <exception cref="CliCommandNotSuccessfulException"></exception>
		public async Task<ulong> CountWordsAsync(TextReader textReader)
		{
			if (OperatingSystem.IsWindows())
			{
				throw new PlatformNotSupportedException();
			}

			string tempFilePath = Path.GetTempFileName();
			tempFilePath = Path.ChangeExtension(tempFilePath, ".txt");

			using (var writer = new StreamWriter(tempFilePath))
			{
				await writer.WriteAsync(await textReader.ReadToEndAsync());
				writer.Close();
			}

			ICliCommandConfigurationBuilder commandConfigurationBuilder = new CliCommandConfigurationBuilder(
				"/usr/bin/wc")
				.WithArguments($"-w {tempFilePath}")
				.WithValidation(ProcessResultValidation.None);

			CliCommandConfiguration cliCommandConfiguration = commandConfigurationBuilder.Build();

			BufferedProcessResult result = await _cliCommandInvoker.ExecuteBufferedAsync(cliCommandConfiguration, CancellationToken.None);

			File.Delete(tempFilePath);

			if (result.ExitCode != 0 || result.StandardOutput.ToLower().Contains("illegal"))
			{
				throw new CliCommandNotSuccessfulException(result.ExitCode,cliCommandConfiguration);
			}

			string numberOfWordsString = result.StandardOutput.Split(' ').First();

			return ulong.Parse(numberOfWordsString);
		}
	}
}
