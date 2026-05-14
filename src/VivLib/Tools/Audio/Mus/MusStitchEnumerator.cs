using System.Collections;
using TheXDS.Vivianne.Models.Audio.Mus;

namespace TheXDS.Vivianne.Tools.Audio.Mus;

/// <summary>
/// Enumerates ASF substreams from a MUS file according to a specified map,
/// supporting looping as defined by the map.
/// </summary>
/// <remarks>
/// This enumerator yields ASF substreams in the order determined by the
/// provided map file. When the end of the sequence is reached, enumeration
/// continues from the loop start position as specified by the map, enabling
/// seamless looping. The enumerator does not modify the underlying MUS or map
/// files and is not thread-safe.
/// </remarks>
public sealed class MusStitchEnumerator : IEnumerator<AsfFile>, IEnumerator
{
    // TODO: Support for interactive music maps (section, intensity, pursuit, crash)

    private readonly MapFile _map;
    private readonly MusFile _mus;
    private readonly int[] _indices;
    private readonly int _loopStart;
    private int _currentIndex = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="MusStitchEnumerator"/>
    /// class.
    /// </summary>
    /// <param name="mus">The music file to enumerate.</param>
    /// <param name="map">
    /// The map file to enumerate <paramref name="mus"/> with.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if either <paramref name="mus"/> or <paramref name="map"/> is 
    /// <see langword="null"/>.
    /// </exception>
    public MusStitchEnumerator(MusFile mus, MapFile map)
    {
        ArgumentNullException.ThrowIfNull(mus);
        ArgumentNullException.ThrowIfNull(map);

        _map = map;
        _mus = mus;
        (_indices, _loopStart) = MapStitcher.Stitch(map);
    }

    /// <inheritdoc/>
    public AsfFile Current => _mus.AsfSubStreams[_indices[_currentIndex]];

    object IEnumerator.Current => Current;

    /// <inheritdoc/>
    public void Dispose() { }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        if (_currentIndex < 0)
        {
            _currentIndex = 0;
            return true;
        }
        else if (_currentIndex < _indices.Length - 1)
        {
            _currentIndex++;
            return true;
        }
        else
        {
            _currentIndex = Array.IndexOf(_indices, _loopStart);
            return true;
        }
    }

    /// <inheritdoc/>
    public void Reset()
    {
        _currentIndex = -1;
    }
}
