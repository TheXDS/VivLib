using TheXDS.Vivianne.Models.Viv;
using TheXDS.Vivianne.Serializers;
using Fe3 = TheXDS.Vivianne.Models.Fe.Nfs3.FeData;
using Fe4 = TheXDS.Vivianne.Models.Fe.Nfs4.FeData;
using Fs3 = TheXDS.Vivianne.Serializers.Fe.Nfs3.FeDataSerializer;
using Fs4 = TheXDS.Vivianne.Serializers.Fe.Nfs4.FeDataSerializer;

namespace TheXDS.Vivianne.Extensions;

internal class VivExtensionsTests
{
    private static readonly IInSerializer<Fe3> f3 = new Fs3();
    private static readonly IInSerializer<Fe4> f4 = new Fs4();

    [Test]
    public void GetFriendlyName_nfs3_returns_valid_data()
    {
        VivFile viv = new()
        {
            {"fedata.eng", f3.Serialize(new Fe3() { CarName = "Test car NFS3"}) }
        };
        Assert.That(viv.GetFriendlyName(), Is.EqualTo("Test car NFS3"));
    }


    [Test]
    public void GetFriendlyName_nfs4_returns_valid_data()
    {
        VivFile viv = new()
        {
            {"fedata.eng", f4.Serialize(new Fe4() { CarName = "Test car NFS4"}) }
        };
        Assert.That(viv.GetFriendlyName(), Is.EqualTo("Test car NFS4"));
    }

    [Test]
    public void GetFriendlyName_no_fedata_returns_null()
    {
        VivFile viv = [];
        Assert.That(viv.GetFriendlyName(), Is.Null);
    }

    [Test]
    public void GetFriendlyName_invalid_fedata_returns_null()
    {
        VivFile viv = new()
        {
            {"fedata.eng", new byte[] { 0x00, 0x01, 0x02 } }
        };
        Assert.That(viv.GetFriendlyName(), Is.Null);
    }
}