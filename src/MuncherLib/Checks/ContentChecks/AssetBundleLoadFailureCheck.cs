using dev.mamallama.checkrunnerlib.Checks;
using System.Text.RegularExpressions;
using MuncherLib.Muncher;

namespace MuncherLib.Checks.ContentChecks;

internal class AssetBundleLoadFailureCheck(string Target) : BaseViolationCheck(Target, SpecificViolation)
{
    private readonly static Violation SpecificViolation = new(
        new("""The AssetBundle '.*' can't be loaded because another AssetBundle with the same files is already loaded""", RegexOptions.IgnoreCase | RegexOptions.Compiled, new(0, 0, 1)),
        40001
    );
    protected override CheckStatus ViolationLevel => CheckStatus.Warning;
}
