// See https://aka.ms/new-console-template for more information

using System.Globalization;
using System.Reflection;

using AlastairLundy.WCountLib.Abstractions.Counters;
using AlastairLundy.WCountLib.Abstractions.Detectors;

using AlastairLundy.WCountLib.Counters;
using AlastairLundy.WCountLib.Detectors;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Spectre.Console.Cli;

using WCount.Cli.Commands;
using WCount.Cli.DependencyInjection;
using WCount.Cli.Localizations;

using IHost host = CreateHostBuilder(args).Build();

IHostBuilder CreateHostBuilder(string[] args)
{
	return Host.CreateDefaultBuilder(args)
		.ConfigureServices(services =>
		{
			services.AddTransient<IWordDetector, WordDetector>();
			services.AddTransient<IWordCounter, WordCounter>();
			services.AddTransient<IByteCounter, ByteCounter>();
			services.AddTransient<ICharacterCounter, CharacterCounter>();
			services.AddTransient<ILineCounter, LineCounter>();
		});
}

using IServiceScope serviceScope = host.Services.CreateScope();

	
CommandApp commandApp = new CommandApp(new TypeRegistrar(host.Services));

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