[![pipeline status](https://git.boerlage.me/open/Geekbot.net/badges/master/pipeline.svg)](https://git.boerlage.me/open/Geekbot.net/commits/master)

# [Geekbot.net](https://geekbot.pizzaandcoffee.rocks/)

A General Purpose Discord Bot written in DotNet Core.

You can invite Geekbot to your server with [this link](https://discordapp.com/oauth2/authorize?client_id=171249478546882561&scope=bot&permissions=1416834054)

## Technologies

* DotNet Core 2
* Redis
* Discord.net

## Running

Make sure redis is running

Run these commands

* `dotnet restore`
* `dotnet run`

On your first run geekbot will ask for your bot token, everything else is taken care of.

### Launch Parameters

| Parameter | Description |
| --- | --- |
| `--verbose` | Show more log information |
| `--disable-api` | Disables the webapi on startup |
| `--reset` | Resets certain parts of the bot |
| `--migrate` | Migrates the database from V3.1 to the new format from V3.2<br> (make sure to backup before running this) | 

## Contributing

Everyone is free to open an issue or create a pull request
