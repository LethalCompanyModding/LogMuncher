using dev.mamallama.checkrunnerlib.CheckRunners;
using LogMuncher.Checks;

namespace LogMuncher.CheckRunners;

internal class AllChecksRunner(string Source, string Content, string Level) : BaseCheckRunner
{
    public override string CheckID => "LCM Problem Matcher";

    public override ICheckRunner[] MyChecks => checks;
    private readonly ICheckRunner[] checks = [
            //new RegexSourceRunner(Source),
            new RegexContentRunner(Content),
            //new RegexLevelRunner(Level),
        ];

}
