using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Tools.Audio.Mus;

/// <summary>
/// Provides functionality to generate a sequence of item indices by traversing
/// a map structure until a loop is detected.
/// </summary>
/// <remarks>
/// This class is intended for use with map data structures where items
/// reference each other in a sequence. It is static and cannot be
/// instantiated.
/// </remarks>
public static class MapStitcher
{
    /// <summary>
    /// Traverses the item sequence in the specified map, returning the ordered
    /// indices visited and the index at which a loop is detected.
    /// </summary>
    /// <remarks>
    /// The traversal starts from the first item in the map and follows the
    /// first available jump from each item. The method stops when it
    /// encounters an item that has already been visited, indicating a loop.
    /// The returned array does not include the loop-starting item, which is
    /// returned separately.
    /// </remarks>
    /// <param name="map">
    /// The map to traverse. Must not be <see langword="null"/> and should
    /// contain a valid sequence of items with defined jumps.
    /// </param>
    /// <param name="startingPoint">
    /// The starting index for the traversal. If not specified, the traversal will
    /// start from the index defined by <see cref="MapFile.FirstItem"/>.
    /// </param>
    /// <returns>
    /// A tuple containing an array of item indices representing the traversal
    /// order, and the index at which the sequence first loops.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="map"/> is <see langword="null"/>.
    /// </exception>
    public static (int[] indices, int loopStart) Stitch(MapFile map, int startingPoint)
    {
        ArgumentNullException.ThrowIfNull(map);
        List<int> sequence = [];
        int current = startingPoint;
        while (!sequence.Contains(current))
        {
            sequence.Add(current);
            current = map.Items[current].Jumps.FirstOrDefault()?.NextItem ?? 0;
        }
        return (sequence.ToArray(), current);
    }

    /// <summary>
    /// Traverses the item sequence in the specified map, returning the ordered
    /// indices visited and the index at which a loop is detected.
    /// </summary>
    /// <remarks>
    /// The traversal starts from the first item in the map and follows the
    /// first available jump from each item. The method stops when it
    /// encounters an item that has already been visited, indicating a loop.
    /// The returned array does not include the loop-starting item, which is
    /// returned separately.
    /// </remarks>
    /// <param name="map">
    /// The map to traverse. Must not be <see langword="null"/> and should
    /// contain a valid sequence of items with defined jumps.
    /// </param>
    /// <returns>
    /// A tuple containing an array of item indices representing the traversal
    /// order, and the index at which the sequence first loops.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="map"/> is <see langword="null"/>.
    /// </exception>
    public static (int[] indices, int loopStart) Stitch(MapFile map)
    {
        return Stitch(map, map.FirstItem);
    }
}
