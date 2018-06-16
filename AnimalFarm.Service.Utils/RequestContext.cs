
namespace AnimalFarm.Service.Utils
{
    public class RequestContext
    {
        public string RequestId { get; }
        public string UserId { get; }
        public bool IsExternal { get; }

        public RequestContext(string requestId, string userId, bool isExternal)
        {
            RequestId = requestId;
            UserId = userId;
            isExternal = IsExternal;
        }
    }
}
