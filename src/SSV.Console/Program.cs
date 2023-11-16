using System.IO.Compression;
using Microsoft.Extensions.Configuration;
using SSV.Console;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

Settings? settings = config.GetRequiredSection("Settings").Get<Settings>();