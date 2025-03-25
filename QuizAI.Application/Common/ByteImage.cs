using Shipwreck.Phash.Imaging;

namespace QuizAI.Application.Common;

public class ByteImage : IByteImage
{
    public ByteImage(byte[] pixels, int width, int height)
    {
        Pixels = pixels ?? throw new ArgumentNullException(nameof(pixels));
        Width = width;
        Height = height;
    }

    public int Width { get; }
    public int Height { get; }
    private byte[] Pixels { get; }

    public byte GetPixel(int x, int y) => Pixels[y * Width + x];
    public byte this[int x, int y] => GetPixel(x, y);
}

