using nUpdate.Exceptions;

namespace nUpdate;

/// <summary>Every JSON document nUpdate writes starts with an integer <c>format</c>; this checks it before the rest is trusted.</summary>
internal static class FormatVersion
{
    /// <exception cref="UnsupportedFormatException">The document was written in another format than this nUpdate understands.</exception>
    public static void Check(int actual, int current, string documentName)
    {
        if (actual == current)
            return;
        var hint = actual > current ? "a newer version of nUpdate; update the library" : "an older version of nUpdate; open and migrate it with nUpdate Administration";
        throw new UnsupportedFormatException($"The {documentName} has format {actual}, but this nUpdate reads format {current}. It was written by {hint}.");
    }
}
