namespace Application.Abstraction.Cache
{
    public interface ICacheProvider
    {
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> acquire, TimeSpan expiration);
        void Remove(string key);
    }
}
