namespace VhdLib;

public class OffsetStream : Stream
{
    private Stream _baseStream;
    private readonly long _streamOffset;


    public OffsetStream(Stream baseStream, long offset)
    {
        if (offset >= baseStream.Length)
        {
            throw new IndexOutOfRangeException("Offset is larger than stream contents");
        }
            
        _baseStream = baseStream;
        _streamOffset = offset;
        _baseStream.Seek(_streamOffset, SeekOrigin.Begin);
    }

    public override void Flush()
    {
        _baseStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return _baseStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return _baseStream.Seek(offset + _streamOffset, origin);
    }

    public override void SetLength(long value)
    {
        _baseStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _baseStream.Write(buffer, offset, count);
    }

    public override bool CanRead => _baseStream.CanRead;

    public override bool CanSeek => _baseStream.CanSeek;
    public override bool CanWrite => _baseStream.CanWrite;

    public override long Length => _baseStream.Length - _streamOffset;

    public override long Position
    {
        get => _baseStream.Position - _streamOffset;
        set => _baseStream.Position = value + _streamOffset;
    }
}