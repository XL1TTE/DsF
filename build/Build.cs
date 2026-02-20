using System.Diagnostics.CodeAnalysis;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[SuppressMessage("ReSharper", "AllUnderscoreLocalParameterName")]
[GitHubActions(
    "profiles_service",
    GitHubActionsImage.UbuntuLatest,
    On = new[] { GitHubActionsTrigger.Push },
    InvokedTargets = new[] { nameof(UserProfilesTesting) })]
class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution(GenerateProjects = true)] readonly Solution Solution;

    static readonly string Framework = "net10.0";

    Target Clean => _ => _
        .Executes(() =>
        {
            DotNetClean(_ =>
                            _.SetProject(Solution)
                       );
        });

    Target Restore => _ => _
                           .After(Clean)
                           .Executes(() =>
                           {
                               DotNetRestore(_ =>
                                                 _.SetProjectFile(Solution)
                                            );
                           });

    Target Compile => _ => _
                           .DependsOn(Restore)
                           .Executes(() =>
                           {
                               DotNetBuild(_ =>
                                               _.SetProjectFile(Solution)
                                                .SetConfiguration(Configuration)
                                          );
                           });

    Target AspireRun => _ => _
                             .DependsOn(Compile)
                             .Executes(() =>
                             {
                                 var process = ProcessTasks.StartProcess(
                                                                         "dotnet run --project",
                                                                         Solution.src.AppHost.Path
                                                                        );

                                 Log.Information($"Aspire started (pid {process.Id})");
                             });

    Target OpenAspireDashBoard => _ => _
                                       .After(AspireRun)
                                       .Executes(() => { });

    Target Test => _ => _
        .DependsOn(UserProfilesTesting);

    Target UserProfilesTesting => _ => _
        .DependsOn(Compile)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution.src.UserProfilesService_Testing)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetFramework(Framework));
        });
}
