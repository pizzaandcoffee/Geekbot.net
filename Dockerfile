FROM mcr.microsoft.com/dotnet/core/aspnet:5.0-focal

COPY ./app /app/

EXPOSE 12995/tcp
WORKDIR /app
ENTRYPOINT ./Geekbot.net
