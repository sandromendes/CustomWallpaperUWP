using System.Collections.Generic;

namespace CustomWallpaper.Services.States
{
    public class PageStateService : IPageStateService
    {
        private readonly Dictionary<string, object> _state = new Dictionary<string, object>();

        public void Set<T>(string key, T value) => _state[key] = value;
        public T Get<T>(string key) => _state.TryGetValue(key, out var value) ? (T)value : default;
        public void Remove(string key) => _state.Remove(key);
        public void Clear() => _state.Clear();
    }
}
