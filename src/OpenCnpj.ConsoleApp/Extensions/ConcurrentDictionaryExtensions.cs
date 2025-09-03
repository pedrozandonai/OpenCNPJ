using System.Collections.Concurrent;

namespace OpenCnpj.ConsoleApp.Extensions;

public static class ConcurrentDictionaryExtensions
{
    public static async Task<TValue> GetOrAddAsync<TKey, TValue>(
        this ConcurrentDictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, Task<TValue>> valueFactory)
    {
        // tenta pegar o valor já existente
        if (dictionary.TryGetValue(key, out var existingValue))
            return existingValue;

        // cria novo valor
        var newValue = await valueFactory(key);

        // coloca no dicionário se ainda não existir
        return dictionary.GetOrAdd(key, newValue);
    }
}