
namespace Hotbar.Type
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class HotbarKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        public HotbarKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    [Serializable]
    public class HotbarDictionary<TKey, TValue>
    {
        [SerializeField]
        private List<HotbarKeyValuePair<TKey, TValue>> entries = new();

        public List<HotbarKeyValuePair<TKey, TValue>> Entries => entries;

        public int Count => Entries.Count;

        public bool ContainsKey(TKey key)
        {
            return entries.Exists(e => EqualityComparer<TKey>.Default.Equals(e.Key, key));
        }

        public void Add(TKey key, TValue value)
        {
            if (ContainsKey(key))
                throw new ArgumentException("Key already exists: " + key);
            entries.Add(new HotbarKeyValuePair<TKey, TValue>(key, value));
        }

        public bool Remove(TKey key)
        {
            return entries.RemoveAll(e => EqualityComparer<TKey>.Default.Equals(e.Key, key)) > 0;
        }

        public void Clear()
        {
            entries.Clear();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var entry in entries)
            {
                if (EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                {
                    value = entry.Value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public TValue GetValueOrDefault(TKey key, TValue defaultValue = default)
        {
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }
            return defaultValue;
        }

        public TValue this[TKey key]
        {
            get
            {
                foreach (var entry in entries)
                {
                    if (EqualityComparer<TKey>.Default.Equals(entry.Key, key))
                        return entry.Value;
                }
                throw new KeyNotFoundException($"Key '{key}' not found.");
            }
            set
            {
                for (int i = 0; i < entries.Count; i++)
                {
                    if (EqualityComparer<TKey>.Default.Equals(entries[i].Key, key))
                    {
                        entries[i].Value = value;
                        return;
                    }
                }

                // Key not found, add new
                entries.Add(new HotbarKeyValuePair<TKey, TValue>(key, value));
            }
        }

        public Dictionary<TKey, TValue> ToDictionary()
        {
            Dictionary<TKey, TValue> dict = new();
            foreach (var kvp in entries)
            {
                if (!dict.ContainsKey(kvp.Key))
                    dict.Add(kvp.Key, kvp.Value);
            }
            return dict;
        }

        public void FromDictionary(Dictionary<TKey, TValue> dict)
        {
            entries.Clear();
            foreach (var kvp in dict)
            {
                entries.Add(new HotbarKeyValuePair<TKey, TValue>(kvp.Key, kvp.Value));
            }
        }
    }
}
