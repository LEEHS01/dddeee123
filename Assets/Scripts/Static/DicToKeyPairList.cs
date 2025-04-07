using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static List<KeyValuePair<K, V>> ToKeyValueList<K, V>(this Dictionary<K, V> dictionary)
    {
        return new List<KeyValuePair<K, V>>(dictionary);
    }
}

public static class ListExtensions
{
    public static Dictionary<K, V> ToDictionary<K, V>(this List<KeyValuePair<K, V>> keyValueList)
    {
        Dictionary<K, V> dictionary = new Dictionary<K, V>();
        foreach (var kvp in keyValueList)
        {
            dictionary[kvp.Key] = kvp.Value; // 기존 키가 있으면 덮어씀
        }
        return dictionary;
    }
}
