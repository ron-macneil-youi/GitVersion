using GitVersion.Extensions;
using GitVersion.Helpers;
using GitVersion.Logging;
using GitVersion.OutputVariables;

namespace GitVersion.BuildAgents
{
    public class TeamCity : BuildAgentBase
    {
        public TeamCity(IEnvironment environment, ILog log) : base(environment, log)
        {
        }

        public const string EnvironmentVariableName = "TEAMCITY_VERSION";

        protected override string EnvironmentVariable { get; } = EnvironmentVariableName;

        public override string? GetCurrentBranch(bool usingDynamicRepos)
        {
            var branchName = Environment.GetEnvironmentVariable("Git_Branch");

            if (branchName.IsNullOrEmpty())
            {
                if (!usingDynamicRepos)
                {
                    WriteBranchEnvVariableWarning();
                }

                return base.GetCurrentBranch(usingDynamicRepos);
            }

            return branchName;
        }

        private void WriteBranchEnvVariableWarning() => this.Log.Warning(@"TeamCity doesn't make the current branch available through environmental variables.
Depending on your authentication and transport setup of your git VCS root things may work. In that case, ignore this warning.
In your TeamCity build configuration, add a parameter called `env.Git_Branch` with value %teamcity.build.vcs.branch.<vcsid>%
See https://gitversion.net/docs/reference/build-servers/teamcity for more info");

        public override bool PreventFetch() => !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("Git_Branch"));

        public override string[] GenerateSetParameterMessage(string name, string value) => new[]
            {
                $"##teamcity[setParameter name='GitVersion.{name}' value='{ServiceMessageEscapeHelper.EscapeValue(value)}']",
                $"##teamcity[setParameter name='system.GitVersion.{name}' value='{ServiceMessageEscapeHelper.EscapeValue(value)}']"
            };

        public override string GenerateSetVersionMessage(VersionVariables variables) => $"##teamcity[buildNumber '{ServiceMessageEscapeHelper.EscapeValue(variables.FullSemVer)}']";
    }
}
