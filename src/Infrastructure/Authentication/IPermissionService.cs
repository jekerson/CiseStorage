﻿namespace Infrastructure.Authentication
{
    public interface IPermissionService
    {
        Task<HashSet<string>> GetPermissionAsync(int userId);
    }
}
