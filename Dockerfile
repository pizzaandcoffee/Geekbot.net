FROM microsoft/dotnet:2.1-aspnetcore-runtime

COPY Geekbot.net/Binaries /app/

EXPOSE 12995/tcp
WORKDIR /app
ENTRYPOINT ./run.sh
