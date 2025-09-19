using System.Collections.Concurrent;

namespace Utils.Enumerable
{
    public static class DictionaryExtenstions
    {

        #region Dictionary
        public static bool Exists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            if (dictionary == null) return false;
            if (key == null) return false;
            return dictionary.ContainsKey(key);
        }

        public static TValue? Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
            where TKey : notnull
        {
            if (dictionary == null) return defaultValue;
            if (key == null) return defaultValue;
            if (dictionary.TryGetValue(key, out var value))
                return value;
            return defaultValue;
        }

        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TKey : notnull
        {
            if (dictionary == null) return;
            if (key == null) return;
            dictionary[key] = value;
        }

        public static bool RemoveKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
        {
            if (dictionary == null) return false;
            if (key == null) return false;
            return dictionary.Remove(key);
        }

        public static TEnum GetEnumValue<TEnum>(this IDictionary<string, object?> dict, string key)
            where TEnum : struct, Enum
        {
            if (dict.TryGetValue(key, out var value) && value != null)
            {
                if (value is TEnum enumValue)
                {
                    return enumValue;
                }

                if (Enum.TryParse(value.ToString(), out TEnum parsedValue))
                {
                    return parsedValue;
                }
            }

            throw new ArgumentException($"Can not convert '{key}' to enum type: {typeof(TEnum).Name}");
        }

        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd) where TKey : notnull
        {
            dic = dic ?? new Dictionary<TKey, TValue>();

            if (dicToAdd == null)
            {
                return;
            }

            foreach (var pair in dicToAdd)
            {
                dic.AddOrUpdate(pair.Key, pair.Value);
            }
        }

        #endregion Dictionary

        #region ConcurrentDictionary

        public static bool Exists<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
        {
            if (dictionary == null) return false;
            if (key == null) return false;
            return dictionary.ContainsKey(key);
        }

        public static TValue? GetValueOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue = default)
            where TKey : notnull
        {
            if (dictionary == null) return defaultValue;
            if (key == null) return defaultValue;
            if (dictionary.TryGetValue(key, out var value))
                return value;
            return defaultValue;
        }

        public static void AddOrUpdate<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TKey : notnull
        {
            if (dictionary == null) return;
            if (key == null) return;
            dictionary[key] = value;
        }

        public static bool RemoveKey<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
        {
            if (dictionary == null) return false;
            if (key == null) return false;
            return dictionary.TryRemove(key, out _);
        }

        #endregion ConcurrentDictionary
    }
}
