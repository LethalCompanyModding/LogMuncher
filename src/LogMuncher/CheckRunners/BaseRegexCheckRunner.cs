using dev.mamallama.checkrunnerlib.CheckRunners;

namespace LogMuncher.CheckRunners;

internal abstract class BaseRegexCheckRunner(string Target) : BaseCheckRunner
{
    protected string Target = Target;
}
