using dev.mamallama.checkrunnerlib.CheckRunners;

namespace LogMuncher.CheckRunners;

internal class AllChecksRunner(string Source, string Content) : BaseCheckRunner
{
    public override string CheckID => "LCM Problem Matcher";

    public override ICheckRunner[] MyChecks => checks;
    private readonly ICheckRunner[] checks = [
            new RegexSourceRunner(Source),
            new RegexContentRunner(Content),
        ];

}
