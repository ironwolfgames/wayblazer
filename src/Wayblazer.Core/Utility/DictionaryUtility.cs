namespace Wayblazer.Core.Utility;

public static class DictionaryUtility
{
	public static TValue GetOrAddValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory) where TKey : notnull
	{
		if (!dictionary.TryGetValue(key, out var value))
		{
			value = valueFactory();
			dictionary[key] = value;
		}

		return value;
	}
}
