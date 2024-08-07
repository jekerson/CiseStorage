﻿using Application.Abstraction.Cache;
using Domain.Abstraction;
using Domain.Entities;
using Domain.Errors.Staff;
using Domain.Repositories.Staff;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Staff
{
    public class UserRepository : IUserRepository
    {
        private readonly SiceDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;
        private const string UsersCacheKey = "usersCache";
        private const string UserCacheKeyPrefix = "userCache_";

        public UserRepository(SiceDbContext dbContext, ICacheProvider cacheProvider)
        {
            _dbContext = dbContext;
            _cacheProvider = cacheProvider;
        }

        public async Task<Result<IEnumerable<UserInfo>>> GetAllUsersAsync()
        {
            var users = await _cacheProvider.GetAsync<IEnumerable<UserInfo>>(UsersCacheKey);
            if (users == null)
            {
                users = await _dbContext.UserInfos.AsNoTracking().Where(u => !u.IsDeleted).ToListAsync();
                await _cacheProvider.SetAsync(UsersCacheKey, users, TimeSpan.FromHours(1));
            }
            return Result<IEnumerable<UserInfo>>.Success(users);
        }

        public async Task<Result> AddUserAsync(UserInfo userInfo)
        {
            if (await IsUserExistByUsernameAsync(userInfo.Username))
                return Result.Failure(UserErrors.UserAlreadyExistByUsername(userInfo.Username));

            await _dbContext.UserInfos.AddAsync(userInfo);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UsersCacheKey);
            return Result.Success();
        }

        public async Task<Result<UserInfo>> GetUserByIdAsync(int id)
        {
            var cacheKey = $"{UserCacheKeyPrefix}{id}";
            var user = await _cacheProvider.GetAsync<UserInfo>(cacheKey);
            if (user == null)
            {
                user = await _dbContext.UserInfos.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
                if (user == null)
                    return Result<UserInfo>.Failure(UserErrors.UserNotFoundById(id));

                await _cacheProvider.SetAsync(cacheKey, user, TimeSpan.FromHours(1));
            }
            return Result<UserInfo>.Success(user);
        }

        public async Task<Result<UserInfo>> GetUserByUsernameAsync(string username)
        {
            var user = await _dbContext.UserInfos.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
            if (user == null)
                return Result<UserInfo>.Failure(UserErrors.UserNotFoundByUsername(username));

            return Result<UserInfo>.Success(user);
        }

        public async Task<Result> UpdateUserAsync(UserInfo userInfo)
        {
            var existingUser = await _dbContext.UserInfos.FindAsync(userInfo.Id);
            if (existingUser == null)
                return Result.Failure(UserErrors.UserNotFoundById(userInfo.Id));

            if (existingUser.Username != userInfo.Username && await IsUserExistByUsernameAsync(userInfo.Username))
                return Result.Failure(UserErrors.UserAlreadyExistByUsername(userInfo.Username));

            _dbContext.Entry(existingUser).CurrentValues.SetValues(userInfo);
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UsersCacheKey);
            await _cacheProvider.RemoveAsync($"{UserCacheKeyPrefix}{userInfo.Id}");
            return Result.Success();
        }

        public async Task<Result> DeleteUserAsync(int id)
        {
            var user = await _dbContext.UserInfos.FindAsync(id);
            if (user == null)
                return Result.Failure(UserErrors.UserNotFoundById(id));

            user.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            await _cacheProvider.RemoveAsync(UsersCacheKey);
            await _cacheProvider.RemoveAsync($"{UserCacheKeyPrefix}{id}");
            return Result.Success();
        }

        public async Task<Result<IEnumerable<UserInfo>>> GetAllDeletedUsersAsync()
        {
            var deletedUsers = await _dbContext.UserInfos.AsNoTracking().Where(u => u.IsDeleted).ToListAsync();
            return Result<IEnumerable<UserInfo>>.Success(deletedUsers);
        }

        public async Task<Result<UserInfo>> GetDeletedUserByUsernameAsync(string username)
        {
            var user = await _dbContext.UserInfos.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username && u.IsDeleted);
            if (user == null)
                return Result<UserInfo>.Failure(UserErrors.UserNotFoundByUsername(username));

            return Result<UserInfo>.Success(user);
        }

        public async Task<Result<UserInfo>> GetDeletedUserByIdAsync(int id)
        {
            var user = await _dbContext.UserInfos.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id && u.IsDeleted);
            if (user == null)
                return Result<UserInfo>.Failure(UserErrors.UserNotFoundById(id));

            return Result<UserInfo>.Success(user);
        }

        private async Task<bool> IsUserExistByUsernameAsync(string username)
        {
            return await _dbContext.UserInfos.AnyAsync(u => u.Username == username && !u.IsDeleted);
        }
    }
}
