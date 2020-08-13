using System;

namespace Geekbot.Core.Logger
{
    public interface IGeekbotLogger
    {
        void Trace(LogSource source, string message, object extra = null);
        void Debug(LogSource source, string message, object extra = null);
        void Information(LogSource source, string message, object extra = null);
        void Warning(LogSource source, string message, Exception stackTrace = null, object extra = null);
        void Error(LogSource source, string message, Exception stackTrace, object extra = null);
        NLog.Logger GetNLogger();
        bool LogAsJson();
    }
}