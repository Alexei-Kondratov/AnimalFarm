using System;
using System.Fabric;

namespace AnimalFarm.Service.Utils.Tracing
{
    public class ServiceLogger
    {
        private readonly ServiceContext _serviceContext;
        private readonly ServiceEventSource _eventSource;

        public ServiceLogger(ServiceContext serviceContext, ServiceEventSource eventSource)
        {
            _serviceContext = serviceContext;
            _eventSource = eventSource;
        }

        public void Log(string message, params object[] args)
        {
            _eventSource.ServiceMessage(_serviceContext, message, args);
        }

        public void LogRequestStart(string requestPath, string requestId)
        {
            _eventSource.ServiceRequestStart(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestPath,
                requestId);
        }

        public void LogRequestStop(string requestPath, string requestId, Exception e)
        {
            _eventSource.ServiceRequestStopDueToException(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestPath,
                requestId,
                e.Message,
                e.StackTrace);
        }

        public void LogRequestStop(string requestPath, string requestId)
        {
            _eventSource.ServiceRequestStop(
                _serviceContext.ServiceName.ToString(),
                _serviceContext.ServiceTypeName,
                GetReplicaOrInstanceId(_serviceContext),
                _serviceContext.PartitionId,
                _serviceContext.CodePackageActivationContext.ApplicationName,
                _serviceContext.CodePackageActivationContext.ApplicationTypeName,
                _serviceContext.NodeContext.NodeName,
                requestPath,
                requestId);
        }

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
