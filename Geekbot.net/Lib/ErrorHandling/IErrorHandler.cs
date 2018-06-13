using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Net;

namespace Geekbot.net.Lib.ErrorHandling
{
    public interface IErrorHandler
    {
        Task HandleCommandException(Exception e, ICommandContext context, string errorMessage = "def");
        Task HandleHttpException(HttpException e, ICommandContext context);
    }
}