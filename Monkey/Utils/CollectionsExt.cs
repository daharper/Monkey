using System.Text;

namespace Monkey.Utils;

public static class CollectionsExt
{
    /// <summary>
    /// Skips a specified number of elements from the end of an enumerable sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="source">The enumerable sequence to skip elements from.</param>
    /// <param name="count">The number of elements to skip from the end of the sequence.</param>
    /// <returns>
    /// An enumerable sequence that contains the elements from the source sequence without
    /// the specified number of elements from the end.
    /// </returns>
    public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
    {
        var queue = new Queue<T>(count + 1);

        foreach (var x in source)
        {
            queue.Enqueue(x);

            if (queue.Count > count)
                yield return queue.Dequeue();
        }
    }

    /// <summary>
    /// Performs the specified action on each element of the source collection,
    /// passing the index and the element to the action.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source collection.</typeparam>
    /// <param name="source">The source collection to perform the action on.</param>
    /// <param name="action">
    /// The action to perform on each element. The action accepts two parameters:
    /// the index of the element and the element itself.
    /// </param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
    {
        var index = 0;

        foreach (var item in source)
            action(index++, item);
    }

    /// <summary>
    /// Returns a tuple containing the index of the first character that does not satisfy the predicate,
    /// and the substring of the source string that satisfies the predicate. If a substring is found,
    /// index points one character past the end of the substring. If a substring is not found, index
    /// will be the same as the startIndex parameter, and value will be an empty string. 
    /// </summary>
    /// <param name="source">The string to take characters from.</param>
    /// <param name="startIndex">The starting index in the string to begin taking characters from.</param>
    /// <param name="predicate">
    /// A function that determines when to stop taking characters.
    /// Returns true to keep taking characters, false otherwise.
    /// </param>
    /// <returns>A tuple consisting of the index and the substring taken.</returns>
    /// <throws>
    /// <see cref="ArgumentOutOfRangeException"/> if startIndex is greater than the length of the source string.
    /// </throws>
    public static (int index, string value) TakeFrom(this string source, int startIndex, Func<char, bool> predicate)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(startIndex, source.Length);

        var index = startIndex;
        var sb = new StringBuilder();
        var ch = source[index];

        while (index < source.Length && predicate(ch))
        {
            sb.Append(ch);
            ch = source[++index];
        }
        
        return (index, sb.ToString());
    }
}