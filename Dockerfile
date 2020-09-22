FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY ./app /app/

EXPOSE 12995/tcp
WORKDIR /app
ENTRYPOINT ./Geekbot
