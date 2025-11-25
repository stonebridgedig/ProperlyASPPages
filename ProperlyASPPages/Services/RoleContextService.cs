using Properly.Models;

namespace ProperlyASPPages.Services
{
    public interface IRoleContextService
    {
        DomainUserType GetCurrentRole();
        void SetCurrentRole(DomainUserType role);
        void ClearCurrentRole();
    }

    public class RoleContextService : IRoleContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RoleContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private const string RoleSessionKey = "CurrentRole";

        public DomainUserType GetCurrentRole()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null && session.TryGetValue(RoleSessionKey, out var roleBytes))
            {
                if (roleBytes.Length == sizeof(int))
                {
                    var roleValue = BitConverter.ToInt32(roleBytes, 0);
                    return (DomainUserType)roleValue;
                }
            }
            return DomainUserType.None;
        }

        public void SetCurrentRole(DomainUserType role)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                var roleBytes = BitConverter.GetBytes((int)role);
                session.Set(RoleSessionKey, roleBytes);
            }
        }

        public void ClearCurrentRole()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session != null)
            {
                session.Remove(RoleSessionKey);
            }
        }
    }
}
