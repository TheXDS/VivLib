namespace TheXDS.Vivianne.Info;

/// <summary>
/// Unit tests for the <see cref="VersionIdentifier"/> helper class.
/// </summary>
[TestFixture]
internal class VersionIdentifierTests
{
    #region FceVersion(byte[])

    [Test]
    public void FceVersion_BinaryHeader_Nfs4_ReturnsNfs4()
    {
        var data = new byte[4];
        Array.Copy(BitConverter.GetBytes(0x00101014), data, 4);
        var result = VersionIdentifier.FceVersion(data);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs4));
    }

    [Test]
    public void FceVersion_BinaryHeader_Nfs3_ReturnsNfs3()
    {
        var data = new byte[4];
        Array.Copy(BitConverter.GetBytes(0x12345678), data, 4);
        var result = VersionIdentifier.FceVersion(data);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs3));
    }

    [Test]
    public void FceVersion_BinaryHeader_Mco_ReturnsMco()
    {
        var data = new byte[4];
        Array.Copy(BitConverter.GetBytes(0x00101015), data, 4);
        var result = VersionIdentifier.FceVersion(data);
        Assert.That(result, Is.EqualTo(NfsVersion.Mco));
    }

    #endregion

    #region FceVersion(Stream)

    [Test]
    public void FceVersion_SeekableStream_ReturnsCorrectVersion()
    {
        var data = new byte[4];
        Array.Copy(BitConverter.GetBytes(0x00101014), data, 4);
        using var stream = new MemoryStream(data);

        var result = VersionIdentifier.FceVersion(stream);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs4));
        Assert.That(stream.Position, Is.EqualTo(0), "Stream should be rewound after read");
    }

    [Test]
    public void FceVersion_NonSeekableStream_ReturnsUnknown()
    {
        var data = new byte[4];
        Array.Copy(BitConverter.GetBytes(0x00101014), data, 4);
        using var stream = new NonSeekableStream(data);

        var result = VersionIdentifier.FceVersion(stream);
        Assert.That(result, Is.EqualTo(NfsVersion.Unknown));
    }

    #endregion

    #region FceVersion(int)

    [Test]
    public void FceVersion_Int_Nfs4_ReturnsNfs4()
    {
        var result = VersionIdentifier.FceVersion(0x00101014);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs4));
    }

    [Test]
    public void FceVersion_Int_Nfs3_ReturnsNfs3()
    {
        var result = VersionIdentifier.FceVersion(0x12345678);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs3));
    }

    [Test]
    public void FceVersion_Int_Mco_ReturnsMco()
    {
        var result = VersionIdentifier.FceVersion(0x00101015);
        Assert.That(result, Is.EqualTo(NfsVersion.Mco));
    }

    #endregion

    #region FeDataVersion

    [Test]
    public void FeDataVersion_Nfs4Header_ReturnsNfs4()
    {
        var file = new byte[] { 4, 0, 0, 0 };
        var result = VersionIdentifier.FeDataVersion(file);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs4));
    }

    [Test]
    public void FeDataVersion_Nfs3Header_ReturnsNfs3()
    {
        var file = new byte[] { 3, 0, 0, 0 };
        var result = VersionIdentifier.FeDataVersion(file);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs3));
    }

    #endregion

    #region CarpVersion

    [Test]
    public void CarpVersion_Length356_ReturnsNfs2()
    {
        var file = new byte[356];
        var result = VersionIdentifier.CarpVersion(file);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs2));
    }

    [Test]
    public void CarpVersion_ContainsUndersteerGradient_ReturnsNfs4()
    {
        var content = "understeer gradient(80) some other text";
        var file = System.Text.Encoding.Latin1.GetBytes(content);
        var result = VersionIdentifier.CarpVersion(file);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs4));
    }

    [Test]
    public void CarpVersion_StartsWithSerial_ReturnsNfs3()
    {
        var content = "Serial some data";
        var file = System.Text.Encoding.Latin1.GetBytes(content);
        var result = VersionIdentifier.CarpVersion(file);
        Assert.That(result, Is.EqualTo(NfsVersion.Nfs3));
    }

    [Test]
    public void CarpVersion_Unknown_ReturnsUnknown()
    {
        var content = "Not matching any known format";
        var file = System.Text.Encoding.Latin1.GetBytes(content);
        var result = VersionIdentifier.CarpVersion(file);
        Assert.That(result, Is.EqualTo(NfsVersion.Unknown));
    }

    #endregion

    private sealed class NonSeekableStream : Stream
    {
        private readonly byte[] _buffer;
        private int _position;

        public NonSeekableStream(byte[] buffer) => (_buffer, _position) = (buffer, 0);

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _buffer.Length;
        public override long Position { get => _position; set => throw new NotSupportedException(); }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var toRead = Math.Min(count, _buffer.Length - _position);
            Array.Copy(_buffer, _position, buffer, offset, toRead);
            _position += toRead;
            return toRead;
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
    }
}