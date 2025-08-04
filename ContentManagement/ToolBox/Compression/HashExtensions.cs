using System.Security.Cryptography;

namespace ToolBox.Compression;


/// <summary>Helper Methods for Computing a Hash</summary>
public static class HashExtensions
{

    public static string ToHashSha1Base64(this Stream stream) => Convert.ToBase64String(SHA1.HashData(stream));

    public static string ToHashSha1Base64(this byte[] buffer) => Convert.ToBase64String(SHA1.HashData(buffer));

    public static string ToHashSha1Hex(this Stream stream) => Convert.ToHexString(SHA1.HashData(stream));

    public static string ToHashSha1Hex(this byte[] buffer) => Convert.ToHexString(SHA1.HashData(buffer));

    public static string ToBase64(this byte[] bytes) => Convert.ToBase64String(bytes);

    /// <summary>Converts an array of 8-bit unsigned integers to its equivalent string representation that is encoded with uppercase hex characters.</summary>
    public static string ToHexString(this byte[] bytes) => Convert.ToHexString(bytes);

    public static byte[] ToHashSha1(this byte[] buffer) => SHA1.HashData(buffer);
    public static byte[] ToHashSha1(this Stream stream) => SHA1.HashData(stream);

    public static Task<byte[]> ToHashSha1Async(this Stream input) => SHA1.Create().ComputeHashAsync(input);

    [Obsolete("Use uint.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out uint value) instead")]
    public static uint ParseHexToUInt32(this string hex)
    {
        if (string.IsNullOrEmpty(hex)) return default;
        if (hex.Length > 8 || !hex.All(char.IsAsciiHexDigit)) throw new ArgumentOutOfRangeException(nameof(hex), "value can only contain up to 8 hexadecimal digits");

        uint value = 0;
        for (int i = 0; i < hex.Length; i++)
        {
            value <<= 4;
            value += Convert.ToByte($"{hex[i]}", 16);
        }
        return value;
    }

    /// <summary>Calc a CRC32 value compaible with PKZip <see cref="IO.Hashing.Crc32"/></summary>
    public static uint ToHashCrc32(this byte[] buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        var hasher = new System.IO.Hashing.Crc32();
        hasher.Append(buffer);
        return hasher.GetCurrentHashAsUInt32();
    }

    /// <summary>Calc a CRC32 value compaible with PKZip <see cref="System.IO.Hashing.Crc32"/></summary>
    public static uint ToHashCrc32(this ReadOnlySpan<byte> buffer)
    {
        var hasher = new System.IO.Hashing.Crc32();
        hasher.Append(buffer);
        return hasher.GetCurrentHashAsUInt32();
    }

    /// <summary>Calc a CRC32 value compaible with PKZip <see cref="System.IO.Hashing.Crc32"/></summary>
    public static uint ToHashCrc32(this FileInfo file)
    {
        if (!file.Exists) return default;

        using FileStream stream = file.OpenRead();
        var crc32 = stream.ToHashCrc32();
        stream.Close();
        return crc32;
    }

    /// <summary>Calc a CRC32 value compaible with PKZip <see cref="System.IO.Hashing.Crc32"/></summary>
    public static async Task<uint> ToHashCrc32Async(this FileInfo file, CancellationToken cancellationToken = default)
    {
        if (!file.Exists) return default;

        using FileStream stream = file.OpenRead();
        var crc32 = await ToHashCrc32Async(stream, cancellationToken);
        stream.Close();
        return crc32;
    }

    /// <summary>Calc a CRC32 value compaible with PKZip <see cref="IO.Hashing.Crc32"/></summary>
    public static uint ToHashCrc32(this Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var hasher = new System.IO.Hashing.Crc32();
        hasher.Append(stream);
        return hasher.GetCurrentHashAsUInt32();
    }

    /// <summary>Calcualate the CRC32 of a stream</summary>
    /// <remarks>Make sure Postion == 0 before calling</remarks>
    /// <returns></returns>
    public static async Task<uint> ToHashCrc32Async(this Stream stream, CancellationToken cancellationToken = default)
    {
        var crcHasher = new System.IO.Hashing.Crc32();
        await crcHasher.AppendAsync(stream, cancellationToken);
        return crcHasher.GetCurrentHashAsUInt32();
    }


}