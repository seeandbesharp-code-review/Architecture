namespace ApiProject.Services.Interface
{
    public interface ICacheService
    {
        // מחזיר את הנתון מה-Cache, או null אם לא קיים / פג תוקף
        Task<T?> GetAsync<T>(string key);

        // שומר נתון ב-Cache עם TTL
        Task SetAsync<T>(string key, T value, TimeSpan ttl);

        // מוחק רשומה מה-Cache (Cache Invalidation)
        Task RemoveAsync(string key);
    }
}
