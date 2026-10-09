using System.IO;
using System.Linq;
using System.Text;
using MuncherLib.Muncher;
using MuncherLib.RuleDatabase;
using Xunit;

namespace MuncherLib.Tests;

public class AssetBundleLoadFailureTests
{
    // Pasted from https://github.com/Sylkadi/BrutalCompanyMinus/issues/17 (a Lethal Company BepInEx log).
    private const string RealLine = "[Error  : Unity Log] The AssetBundle 'IO.Stream' can't be loaded because another AssetBundle with the same files is already loaded.";

    static AssetBundleLoadFailureTests() => Rules.Init();

    private static LineData Munch(string log)
    {
        using var input = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(log)));
        using var output = new StreamWriter(new MemoryStream());
        using var muncher = new LogMuncher(input, output, HTMLOutput: false);
        return muncher.MunchLog(raw: true).Result.Single();
    }

    [Fact]
    public void AnAssetBundleThatUnityRefusesIsWeightedAsCritical()
    {
        var line = Munch(RealLine);

        // An error line is 10 on its own; rule 40001 adds the critical 15.
        Assert.Equal(25f, line.Weight);
        Assert.Contains("[[LCM40001]]", line.ToString());
    }

    [Fact]
    public void AnOrdinaryErrorLineStaysBelowIt()
    {
        var plain = Munch("[Error  : Unity Log] Something else went wrong.");

        Assert.True(plain.Weight < 25f);
    }
}
