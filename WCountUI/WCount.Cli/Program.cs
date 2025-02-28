// See https://aka.ms/new-console-template for more information
using System;
using System.Resources;

using BasisBox.Cli.Tools.WCount.Commands;

using Spectre.Console.Cli;

CommandApp commandApp = new CommandApp();

commandApp.Configure(config =>
{
	//
	// WCount: 
	//
	config.AddBranch("wcount", conf =>
	{
		conf.AddCommand<WCountCommand>("")
		.WithAlias("wc")
		.WithDescription(Resources.WCount_App_Description)
		.WithExample("/path/to/example.txt")
		.WithExample("-l /path/to/foo.txt")
		.WithExample("/Path/To/foo.txt", "/Path/To/bar.txt");

		conf.AddCommand<WordCountOnlyCommand>("words")
		.WithExample("/path/to/example.txt")
		.WithDescription(Resources.Wcount_App_Words_Description);

		conf.AddCommand<CharCountOnlyCommand>("chars")
		.WithExample("/path/to/example.txt")
		.WithDescription(Resources.WCount_App_Chars_Description);
	});


});

return await commandApp.RunAsync(args);