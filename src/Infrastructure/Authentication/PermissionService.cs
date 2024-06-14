using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authentication
{
    public class PermissionService : IPermissionService
    {
        private readonly SiceDbContext _context;

        public PermissionService(SiceDbContext context)
        {
            _context = context;
        }
        public async Task<HashSet<string>> GetPermissionAsync(int userId)
        {
            var permissions = await _context.UserInfos
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserRoles.Select(ur => ur.RoleId))
                .Join(_context.RolePermissions, userRoleId => userRoleId, rp => rp.RoleId, (userRoleId, rp) => rp.PermissionId)
                .Join(_context.Permissions, permissionId => permissionId, p => p.Id, (permissionId, p) => p.Name)
                .Distinct()
                .ToListAsync();

            return new HashSet<string>(permissions);
        }
    }
}
