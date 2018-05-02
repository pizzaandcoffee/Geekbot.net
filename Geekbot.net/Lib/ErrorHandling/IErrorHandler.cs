using System;
using Discord.Commands;
using Discord.Net;

namespace Geekbot.net.Lib.ErrorHandling
{
    public interface IErrorHandler
    {
        void HandleCommandException(Exception e, ICommandContext context, string errorMessage = "def");
        void HandleHttpException(HttpException e, ICommandContext context);
    }
}