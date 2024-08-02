using dev.mamallama.checkrunnerlib.Checks;
using dev.mamallama.checkrunnerlib.CheckRunners;
using LogMuncher.Checks;

namespace LogMuncher.CheckRunners;

internal class AllChecksRunner(string Source, string Content, string Level) : BaseCheckRunner
{
    public override string CheckGroupID => "LCM Problem Matcher";

    protected override ICheck[] MyChecks => checks;
    private readonly ICheck[] checks = [
            //new RegexSourceRunner(Source),
            new RegexContentRunner(Content),
            //new RegexLevelRunner(Level),
        ];
}
