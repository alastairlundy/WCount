/*
	WCount CLI
	Copyright (c) Alastair Lundy 2024-2025
 
  This Source Code Form is subject to the terms of the Mozilla Public
  License, v. 2.0. If a copy of the MPL was not distributed with this
  file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Globalization;
using System.Reflection;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Detectors;

using AlastairLundy.WCountLib.Counters;
using AlastairLundy.WCountLib.Detectors;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console.Cli;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using WCount.Cli.Commands;
using WCount.Cli.Localizations;

//IHostBuilder hostBuilder = CreateHostBuilder(args);

IServiceCollection services = new ServiceCollection();

services.AddTransient<IWordDetector, WordDetector>();
services.AddTransient<IWordCounter, WordCounter>();
services.AddTransient<IByteCounter, ByteCounter>();
services.AddTransient<ICharacterCounter, CharacterCounter>();
services.AddTransient<ILineCounter, LineCounter>();

using DependencyInjectionRegistrar registrar = new DependencyInjectionRegistrar(services);
	
CommandApp commandApp = new CommandApp(registrar);

commandApp.Configure(config =>
{
	config.PropagateExceptions();
	
	config.AddCommand<WCountCommand>("")
		.WithAlias("all")
		.WithDescription(Resources.WCount_App_Description)
		.WithExample("/path/to/example.txt")
		.WithExample("-l /path/to/foo.txt")
		.WithExample("/Path/To/foo.txt", "/Path/To/bar.txt");

	config.AddCommand<WordCountOnlyCommand>("words")
		.WithExample("/path/to/example.txt")
		.WithDescription(Resources.WCount_App_Words_Description);

	config.AddCommand<CharCountOnlyCommand>("chars")
		.WithExample("/path/to/example.txt")
		.WithDescription(Resources.WCount_App_Chars_Description);
		
	config.AddCommand<BytesCountOnlyCommand>("bytes")
		.WithExample("/path/to/example.txt")
		.WithDescription(Resources.WCount_App_Bytes_Description);
		
	config.AddCommand<CharCountOnlyCommand>("lines")
		.WithExample("/path/to/example.txt")
		.WithDescription(Resources.WCount_App_Lines_Description);
		
	commandApp.SetDefaultCommand<WCountCommand>();

	config.SetApplicationName(Resources.WCount_App_Name);
	
	string? assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

	if (assemblyVersion != null)
	{
		config.SetApplicationVersion(assemblyVersion);
	}

	config.SetApplicationCulture(CultureInfo.CurrentCulture);
});

return await commandApp.RunAsync(args);