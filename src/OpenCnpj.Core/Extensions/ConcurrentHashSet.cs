using System.Collections.Concurrent;

namespace OpenCnpj.Core.Extensions;
/// <summary>
/// Thread-safe HashSet implementation using ConcurrentDictionary
/// </summary>
/// <typeparam name="T">Type of elements in the set</typeparam>
public class ConcurrentHashSet<T> where T : notnull
{
    private readonly ConcurrentDictionary<T, byte> _dictionary = new();

    /// <summary>
    /// Adds an element to the set
    /// </summary>
    /// <param name="item">Element to add</param>
    /// <returns>True if the element was added, false if it already existed</returns>
    public bool Add(T item)
    {
        return _dictionary.TryAdd(item, 0);
    }

    /// <summary>
    /// Checks if the set contains the specified element
    /// </summary>
    /// <param name="item">Element to check</param>
    /// <returns>True if the element exists in the set</returns>
    public bool Contains(T item)
    {
        return _dictionary.ContainsKey(item);
    }

    /// <summary>
    /// Removes an element from the set
    /// </summary>
    /// <param name="item">Element to remove</param>
    /// <returns>True if the element was removed, false if it didn't exist</returns>
    public bool Remove(T item)
    {
        return _dictionary.TryRemove(item, out _);
    }

    /// <summary>
    /// Gets the number of elements in the set
    /// </summary>
    public int Count => _dictionary.Count;

    /// <summary>
    /// Removes all elements from the set
    /// </summary>
    public void Clear()
    {
        _dictionary.Clear();
    }

    /// <summary>
    /// Returns all elements in the set
    /// </summary>
    public IEnumerable<T> ToList()
    {
        return _dictionary.Keys;
    }
}