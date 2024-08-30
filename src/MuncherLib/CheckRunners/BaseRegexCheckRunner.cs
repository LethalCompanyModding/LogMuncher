using dev.mamallama.checkrunnerlib.CheckRunners;

namespace MuncherLib.CheckRunners;

internal abstract class BaseRegexCheckRunner(string Target) : BaseCheckRunner
{
    protected string Target = Target;
}
