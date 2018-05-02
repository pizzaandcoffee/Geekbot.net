using System;

namespace Geekbot.net.Lib.Logger
{
    public interface IGeekbotLogger
    {
        void Trace(string source, string message, object extra = null);
        void Debug(string source, string message, object extra = null);
        void Information(string source, string message, object extra = null);
        void Warning(string source, string message, Exception stackTrace = null, object extra = null);
        void Error(string source, string message, Exception stackTrace, object extra = null);
    }
}