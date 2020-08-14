using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Geekbot.Core.ErrorHandling
{
    public interface IErrorHandler
    {
        Task HandleCommandException(Exception e, ICommandContext context, string errorMessage = "def");
    }
}