using BlazorCssTransitions.AnimatedVisibilityTransitions;
using BlazorCssTransitions.UnitTests.Fakes;
using Bunit;

namespace BlazorCssTransitions.UnitTests.Tests;

public partial class AnimatedVisibilityTests
{
    private EnterTransition _enter = EnterTransition.FadeIn(Spec.Linear100ms);
    private BaseTransition EnterBase => (BaseTransition)_enter;

    private ExitTransition _exit = ExitTransition.FadeOut(Spec.EaseIn200ms);
    private BaseTransition ExitBase => (BaseTransition)_exit;

    [Fact]
    public async Task When_VisibleElementWithStartWithTransitionIsRendered_Then_ShouldRenderInSpecificWay()
    {
        var waitForCompletion = CreateCompletionAwaiter();
        int markupChangesCount = 0;

        IRenderedComponent<ArtificalContainer> container = null!;
        (container, var cut) = RenderComponentInArtificalContainerForImmediateRerendersAnalysis<AnimatedVisibility>(
            parameters => parameters
                .Add(x => x.Visible, true)
                .Add(x => x.StartWithTransition, true)
                .Add(x => x.Enter, _enter)
                .Add(x => x.Exit, _exit)
                .AddChildContent(SampleContent()),
            onMarkupUpdated: (container, cut) =>
            {
                switch (markupChangesCount++)
                {
                    case 0:
                        // initial render - setting up as hidden
                        container.MarkupMatches($"""
                            <div class="animated-visibility {EnterBase.GetInitialClasses()}" style="{EnterBase.GetInitialStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        break;
                    case 1:
                        // start for fade in transition
                        container.MarkupMatches($"""
                            <div class="animated-visibility {EnterBase.GetFinishClasses()}" style="{EnterBase.GetFinishStyle()}">
                                {SampleContentText}
                            </div>
                            """);

                        TimerService.SetResultForAllAwaitingTimers(FakeTimerService.TimerAction.Act);
                        break;
                    case 2:
                        // finish of fade in transition
                        container.MarkupMatches($"""
                            <div class="animated-visibility {ExitBase.GetInitialClasses()}" style="{ExitBase.GetInitialStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        waitForCompletion.MarkAsFinished();
                        break;
                }
            });

        await waitForCompletion.WaitForFinish();
    }

    [Fact]
    public async Task When_VisibleElementWithRemoveFromDOMWhenHiddenIsRendered_Then_ShouldRenderInSpecificWay()
    {
        var waitForCompletion = CreateCompletionAwaiter();
        int markupChangesCount = 0;

        IRenderedComponent<ArtificalContainer> container = null!;
        (container, var cut) = RenderComponentInArtificalContainerForImmediateRerendersAnalysis<AnimatedVisibility>(
            parameters => parameters
                .Add(x => x.Visible, false)
                .Add(x => x.RemoveFromDOMWhenHidden, true)
                .Add(x => x.Enter, _enter)
                .Add(x => x.Exit, _exit)
                .AddChildContent(SampleContent()),
            onMarkupUpdated: (container, cut) =>
            {
                switch (markupChangesCount++)
                {
                    case 0:
                        // initial render - do not render anything
                        container.MarkupMatches("");

                        cut!.SetParametersAndRender(parameters => parameters
                            .Add(x => x.Visible, true));
                        break;
                    case 1:
                        // first actual render - set up as hidden
                        container.MarkupMatches($"""
                            <div class="animated-visibility {EnterBase.GetInitialClasses()}" style="{EnterBase.GetInitialStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        break;
                    case 2:
                        // start of fade in transition
                        container.MarkupMatches($"""
                            <div class="animated-visibility {EnterBase.GetFinishClasses()}" style="{EnterBase.GetFinishStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        TimerService.SetResultForAllAwaitingTimers(FakeTimerService.TimerAction.Act);
                        break;
                    case 3:
                        // finish of fade in transition
                        container.MarkupMatches($"""
                            <div class="animated-visibility {ExitBase.GetInitialClasses()}" style="{ExitBase.GetInitialStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        cut!.SetParametersAndRender(
                            parameters => parameters
                                .Add(x => x.Visible, false));
                        break;
                    case 4:
                        // start of fade out transition
                        container.MarkupMatches($"""
                            <div class="animated-visibility {ExitBase.GetFinishClasses()}" style="{ExitBase.GetFinishStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        TimerService.SetResultForAllAwaitingTimers(FakeTimerService.TimerAction.Act);
                        break;
                    case 5:
                        // finish of fade out transition - do not render anything
                        container.MarkupMatches("");
                        waitForCompletion.MarkAsFinished();
                        break;
                }
            });

        await waitForCompletion.WaitForFinish();
    }

    [Fact]
    public async Task When_VisibleElementWithDisappearWhenHiddenIsRendered_Then_ShouldRenderInSpecificWay()
    {
        int markupChangesCount = 0;
        var waitForCompletion = CreateCompletionAwaiter(
            onTimeout: () => Assert.Fail($"Failed on {markupChangesCount}"));

        IRenderedComponent<ArtificalContainer> container = null!;
        (container, var cut) = RenderComponentInArtificalContainerForImmediateRerendersAnalysis<AnimatedVisibility>(
            parameters => parameters
                .Add(x => x.Visible, false)
                .Add(x => x.DisappearWhenHidden, true)
                .Add(x => x.Enter, _enter)
                .Add(x => x.Exit, _exit)
                .AddChildContent(SampleContent()),
            onMarkupUpdated: (container, cut) =>
            {
                switch (markupChangesCount++)
                {
                    case 0:
                        // initial render - render disappeared
                        container.MarkupMatches($"""
                            <div class="animated-visibility disappeared {EnterBase.GetInitialClasses()}" style="{EnterBase.GetInitialStyle()}">
                                {SampleContentText}
                            </div>
                            """);

                        cut!.SetParametersAndRender(parameters => parameters
                            .Add(x => x.Visible, true));
                        break;
                    case 1:
                        // first render of appeared container - set up as hidden
                        container.MarkupMatches($"""
                            <div class="animated-visibility {EnterBase.GetInitialClasses()}" style="{EnterBase.GetInitialStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        break;
                    case 2:
                        // start of fade in transition
                        container.MarkupMatches($"""
                            <div class="animated-visibility {EnterBase.GetFinishClasses()}" style="{EnterBase.GetFinishStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        TimerService.SetResultForAllAwaitingTimers(FakeTimerService.TimerAction.Act);
                        break;
                    case 3:
                        // finish of fade in transition
                        container.MarkupMatches($"""
                            <div class="animated-visibility {ExitBase.GetInitialClasses()}" style="{ExitBase.GetInitialStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        cut!.SetParametersAndRender(
                            parameters => parameters
                                .Add(x => x.Visible, false));
                        break;
                    case 4:
                        // start of fade out transition
                        container.MarkupMatches($"""
                            <div class="animated-visibility {ExitBase.GetFinishClasses()}" style="{ExitBase.GetFinishStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        TimerService.SetResultForAllAwaitingTimers(FakeTimerService.TimerAction.Act);
                        break;
                    case 5:
                        // finish of fade out transition - render disappeared
                        container.MarkupMatches($"""
                            <div class="animated-visibility disappeared {EnterBase.GetInitialClasses()}" style="{EnterBase.GetInitialStyle()}">
                                {SampleContentText}
                            </div>
                            """);
                        waitForCompletion.MarkAsFinished();
                        break;
                }
            });

        await waitForCompletion.WaitForFinish();
    }
}
