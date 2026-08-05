using System.Collections.ObjectModel;

namespace Listen2Me.MVVM.Extensions;

public static class Collections
{
    /// <summary>
    /// Adds a range of items to an ObservableCollection.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="items">The items to add.</param>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        where T : class
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }
    
    /// <summary>
    /// Removes a range of items from an ObservableCollection.
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="items">The items to remove.</param>
    /// <typeparam name="T">The type of the items in the collection.</typeparam>
    public static void RemoveRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        where T : class
    {
        foreach (var item in items)
        {
            collection.Remove(item);
        }
    }
}