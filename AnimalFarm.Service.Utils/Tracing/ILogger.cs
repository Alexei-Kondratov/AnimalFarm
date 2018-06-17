using System;

namespace AnimalFarm.Service.Utils.Tracing
{
    public interface ILogger
    {
        void Log(string message, params object[] args);
        void LogOperationException(string operationRunId, Exception ex);
        void LogOperationRetry(string operationRunId, int retryAttempt, int totalRetries);
        void LogOperationStart(string operationRunId);
        void LogOperationStop(string operationRunId);
        void LogOperationTimeout(string operationRunId);
        void LogRequestStart(string requestPath, string requestId);
        void LogRequestStop(string requestPath, string requestId);
        void LogRequestStop(string requestPath, string requestId, Exception e);
    }
}