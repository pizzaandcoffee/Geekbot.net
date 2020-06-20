[![pipeline status](https://gitlab.com/dbgit/open/geekbot/badges/master/pipeline.svg)](https://gitlab.com/dbgit/open/geekbot/commits/master)

# [Geekbot.net](https://geekbot.pizzaandcoffee.rocks/)

A General Purpose Discord Bot written in C#

You can invite Geekbot to your server with [this link](https://discordapp.com/oauth2/authorize?client_id=171249478546882561&scope=bot&permissions=1416834054)

## Technologies

* .NET 5
* PostgreSQL
* Discord.net

## Running

You can start geekbot with: `dotnet run`

On your first run geekbot will ask for your bot token.

You might need to pass some additional configuration (e.g. database credentials), these can be passed as commandline arguments or environment variables.

For a list of commandline arguments and environment variables use `dotnet run -- -h` 

All Environment Variables must be prefixed with `GEEKBOT_`

## Contributing

Everyone is free to open an issue or create a pull request
