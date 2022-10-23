using Spectre.Console.Cli;

var app = new CommandApp<FindUsagesOfTypeCommand>();
return app.Run(args);