namespace ToolBox.Compression;

public static class Crc32Legacy
{
    public const uint MinBufferSize = 512;
    public const uint MaxBufferSize = 65536;
    public const uint DefaultBufferSize = 4096;
    public static uint BufferSize { get; set; } = DefaultBufferSize;

    private static uint[] Crc32Table { get; }

    static Crc32Legacy()
    {
        // Initialize the CRC32 table
        const uint dwPolynomial = 0xEDB88320;
        var crc = new uint[256];
        for (uint i = 0; i < 256; i++)
        {
            uint dwCrc = i;
            for (int j = 8; j > 0; j--)
            {
                if ((dwCrc & 1) == 1)
                {
                    dwCrc = dwCrc >> 1 ^ dwPolynomial;
                }
                else
                {
                    dwCrc >>= 1;
                }
            }
            crc[i] = dwCrc;
        }
        Crc32Table = crc;
    }

    public static async Task<uint> GetCrc32Async(Stream stream, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        unchecked
        {
            uint crc32Result = 0xFFFFFFFF;

            uint bufferSize = Math.Min(MaxBufferSize, Math.Max(BufferSize, MinBufferSize));
            byte[] buffer = new byte[bufferSize];

            int bytesRead = await stream.ReadAsync(buffer, cancellationToken);

            while (bytesRead > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    crc32Result = crc32Result >> 8 ^ Crc32Table[buffer[i] ^ crc32Result & 0x000000FF];
                }
                bytesRead = await stream.ReadAsync(buffer, cancellationToken);
            }

            return ~crc32Result;
        }
    }


}
