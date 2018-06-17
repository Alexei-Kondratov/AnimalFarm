using System;
using System.Fabric;

namespace AnimalFarm.Service.Utils.Tracing
{
    public class ServiceLogger : ILogger
    {
        private readonly ServiceContext _serviceContext;
        private readonly ServiceEventSource _eventSource;
        private readonly IRequestContextAccessor _requestContextAccessor;

        public ServiceLogger(ServiceContext serviceContext, ServiceEventSource eventSource, IRequestContextAccessor requestContextAccessor)
        {
            _serviceContext = serviceContext;
            _eventSource = eventSource;
            _requestContextAccessor = requestContextAccessor;
        }

        public void Log(string message, params object[] args)
        {
            _eventSource.ServiceMessage(_serviceContext, message, args);
        }

        #region Public methods: Log request

        public void LogRequestStart(string requestPath, string requestId)
        {
            string userId = _requestContextAccessor?.Context?.UserId ?? String.Empty;

            _eventSource.ServiceRequestStarted(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestId,
                userId,
                requestPath);
        }

        public void LogRequestStop(string requestPath, string requestId, Exception e)
        {
            string userId = _requestContextAccessor?.Context?.UserId ?? String.Empty;

            _eventSource.ServiceRequestFailed(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestId,
                userId,
                requestPath,
                e.Message,
                e.StackTrace);
        }

        public void LogRequestStop(string requestPath, string requestId)
        {
            string userId = _requestContextAccessor?.Context?.UserId ?? String.Empty;

            _eventSource.ServiceRequestStopped(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestId,
                userId,
                requestPath);
        }

        #endregion Public methods: Log request

        #region Public methods: Log operation
        
        public void LogOperationStart(string operationRunId)
        {
            RequestContext requestContext = _requestContextAccessor?.Context;
            string requestId = requestContext?.RequestId ?? String.Empty;
            string userId = requestContext?.UserId ?? String.Empty;

            _eventSource.OperationStarted(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestId,
                userId,
                operationRunId);
        }

        public void LogOperationStop(string operationRunId)
        {
            RequestContext requestContext = _requestContextAccessor?.Context;
            string requestId = requestContext?.RequestId ?? String.Empty;
            string userId = requestContext?.UserId ?? String.Empty;

            _eventSource.OperationStopped(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestId,
                userId,
                operationRunId);
        }

        public void LogOperationTimeout(string operationRunId)
        {
            RequestContext requestContext = _requestContextAccessor?.Context;
            string requestId = requestContext?.RequestId ?? String.Empty;
            string userId = requestContext?.UserId ?? String.Empty;

            _eventSource.OperationTimedOut(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestId,
                userId,
                operationRunId);
        }

        public void LogOperationException(string operationRunId, Exception ex)
        {
            RequestContext requestContext = _requestContextAccessor?.Context;
            string requestId = requestContext?.RequestId ?? String.Empty;
            string userId = requestContext?.UserId ?? String.Empty;

            _eventSource.OperationFailed(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestId,
                userId,
                operationRunId,
                ex.Message,
                ex.StackTrace);
        }

        public void LogOperationRetry(string operationRunId, int retryAttempt, int totalRetries)
        {
            RequestContext requestContext = _requestContextAccessor?.Context;
            string requestId = requestContext?.RequestId ?? String.Empty;
            string userId = requestContext?.UserId ?? String.Empty;

            _eventSource.OperationRetried(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestId,
                userId,
                operationRunId,
                retryAttempt,
                totalRetries);
        }

        #endregion Public methods: Log operation

        private static long GetReplicaOrInstanceId(ServiceContext context)
        {
            StatelessServiceContext stateless = context as StatelessServiceContext;
            if (stateless != null)
            {
                return stateless.InstanceId;
            }

            StatefulServiceContext stateful = context as StatefulServiceContext;
            if (stateful != null)
            {
                return stateful.ReplicaId;
            }

            throw new NotSupportedException("Context type not supported.");
        }
    }
}
