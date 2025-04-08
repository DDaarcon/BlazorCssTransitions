using BlazorCssTransitions.AnimatedVisibilityTransitions;
using BlazorCssTransitions.UnitTests.Other;
using BlazorCssTransitions.UnitTests.Tests.AnimatedContent;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace BlazorCssTransitions.UnitTests.Tests;

public partial class AnimatedContentTests
{

    internal static EnterTransition _enter = EnterTransition.FadeIn(Spec.Linear100ms);
    internal static BaseTransition EnterBase => (BaseTransition)_enter;
    
    internal static ExitTransition _exit = ExitTransition.FadeOut(Spec.EaseIn200ms);
    internal static BaseTransition ExitBase => (BaseTransition)_exit;
    
    
    internal static EnterTransition _enter2 = EnterTransition.FadeIn(Spec.Linear200ms);
    internal static BaseTransition Enter2Base => (BaseTransition)_enter2;
    
    internal static ExitTransition _exit2 = ExitTransition.FadeOut(Spec.EaseIn500ms);
    internal static BaseTransition Exit2Base => (BaseTransition)_exit2;



    internal static BaseTransition DefaultAnimatedVisibilityEnterBase => (BaseTransition)AnimatedVisibility._defaultEnter;
    internal static BaseTransition DefaultAnimatedVisibilityExitBase => (BaseTransition)AnimatedVisibility._defaultExit;

    public enum States
    {
        Zero,
        One,
        Two
    }

    protected override Task OnTestSetup()
    {
        Services.AddSingleton<SimpleElementsInitializationCounterService>();

        return base.OnTestSetup();
    }

    private SimpleElementsInitializationCounterService SimpleElementsInitializationCounter => Services.GetRequiredService<SimpleElementsInitializationCounterService>();

    [Fact]
    public async Task When_NewStateOnTopIsFalse_Then_ShouldRenderCorrectly()
    {
        var cut = RenderComponent<AnimatedContent<States>>(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero)
                .Add(x => x.NewStateOnTop, false)
                .Add(x => x.ChildContent, SampleContent()));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()}">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);

        cut.SetParametersAndRender(parameters => parameters.Add(x => x.TargetState, States.One));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityEnterBase.GetFinishClasses()}"
                        style="{DefaultAnimatedVisibilityEnterBase.GetFinishStyle()}">
                    {SampleContentText(States.One)}
                </div>
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetFinishClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetFinishStyle()}">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);

        var waitForRerender = TestCompletionAwaiter.Create()
            .ScheduleCallbackAfterNextMarkupUpdateOfElement(
                element: cut,
                actionCausingUpdate: () => TimerService.SetResultForAllAwaitingTimers(),
                onMarkupChange: () =>
                {
                    cut.MarkupMatches($"""
                        <div class="animated-content" style="">
                            <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                                    style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()}">
                                {SampleContentText(States.One)}
                            </div>
                        </div>
                        """);
                });
        await waitForRerender.WaitForFinish();
    }

    [Fact]
    public async Task When_NewStateOnTopIsTrue_Then_ShouldRenderCorrectly()
    {
        var cut = RenderComponent<AnimatedContent<States>>(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero)
                .Add(x => x.NewStateOnTop, true)
                .Add(x => x.ChildContent, SampleContent()));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()}">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);

        cut.SetParametersAndRender(parameters => parameters.Add(x => x.TargetState, States.One));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetFinishClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetFinishStyle()}">
                    {SampleContentText(States.Zero)}
                </div>
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityEnterBase.GetFinishClasses()}"
                        style="{DefaultAnimatedVisibilityEnterBase.GetFinishStyle()}">
                    {SampleContentText(States.One)}
                </div>
            </div>
            """);
    }

    [Fact]
    public async Task When_EventsAreSet_Then_TheyShouldBeInvokedOnRightMoments()
    {
        // 1
        var awaiter = TestCompletionAwaiter.Create(
            onTimeout: () => Assert.Fail("Initial render should invoke TargetAppeared"));
        var onAppear = () =>
        {
            awaiter.MarkAsFinished();
        };
        var onAppearing = () => { };
        void OnAppear()
        {
            onAppear();
        }
        void OnAppearing()
        {
            onAppearing();
        }

        var cut = RenderComponent<AnimatedContent<States>>(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero)
                .Add(x => x.OnTargetStateAppeared, OnAppear)
                .Add(x => x.OnTargetStateAppearing, OnAppearing)
                .Add(x => x.ChildContent, SampleContent()));

        await awaiter.WaitForFinish();



        // 2
        awaiter = TestCompletionAwaiter.Create(
            onTimeout: () => Assert.Fail("Changing state should invoke TargetAppearing"));
        onAppear = () => { };
        onAppearing = () =>
        {
            awaiter.MarkAsFinished();
        };

        cut.SetParametersAndRender(parameters => parameters.Add(x => x.TargetState, States.One));

        await awaiter.WaitForFinish();



        // 3
        awaiter = TestCompletionAwaiter.Create(
            onTimeout: () => Assert.Fail("TargetAppeared should have been invoked when target's appearing animation finishes"));
        onAppear = () =>
        {
            awaiter.MarkAsFinished();
        };
        onAppearing = () => { };

        TimerService.SetResultForAllAwaitingTimers();

        await awaiter.WaitForFinish();
    }

    
    [Fact]
    public async Task When_StartWithTransitionAndOnAppearingEventIsSet_Then_ItShouldBeInvokedOnAtStart()
    {
        var awaiter = TestCompletionAwaiter.Create(
            onTimeout: () => Assert.Fail("Initial render should invoke TargetAppearing"));
        var onAppearing = () =>
        {
            awaiter.MarkAsFinished();
        };
        void OnAppearing()
        {
            onAppearing();
        }

        var cut = RenderComponent<AnimatedContent<States>>(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero)
                .Add(x => x.StartWithTransition, true)
                .Add(x => x.OnTargetStateAppearing, OnAppearing)
                .Add(x => x.ChildContent, SampleContent()));

        await awaiter.WaitForFinish();
    }



    [Fact]
    public async Task When_KeepContentInBoundsIsTrue_Then_ShouldRenderCorrectly()
    {
        var cut = RenderComponent<AnimatedContent<States>>(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero)
                .Add(x => x.KeepContentInBounds, true)
                .Add(x => x.ChildContent, SampleContent()));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()} min-height: 0;">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);

        cut.SetParametersAndRender(parameters => parameters.Add(x => x.TargetState, States.One));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityEnterBase.GetFinishClasses()}"
                        style="{DefaultAnimatedVisibilityEnterBase.GetFinishStyle()} min-height: 0;">
                    {SampleContentText(States.One)}
                </div>
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetFinishClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetFinishStyle()} min-height: 0;">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);
    }


    [Fact]
    public async Task When_StateSwitches_Then_ShouldRenderInSpecificWay()
    {
        var counters = new Counters();

        Func<AnimatedContent<States>.StateChange, AnimatedContent<States>.InterstateTransitions> transitionProvider = (stateChange) =>
        {
            switch (counters.Renders)
            {
                case 0:
                    counters.TransitionsCalcsThisRender.ShouldBeLessThanOrEqualTo(0);
                    stateChange.SourceOrDefault.ShouldBe((States)0);
                    stateChange.IsSourcePresent.ShouldBeFalse();
                    stateChange.Target.ShouldBe(States.Zero);
                    break;
                case 1:
                    counters.TransitionsCalcsThisRender.ShouldBeLessThanOrEqualTo(1);
                    switch (counters.GetAndIncreaseTransitionsCalcsThisRender())
                    {
                        case 0:
                            stateChange.SourceOrDefault.ShouldBe(States.Zero);
                            stateChange.IsSourcePresent.ShouldBeTrue();
                            stateChange.Target.ShouldBe(States.One);
                            break;
                        case 1:
                            stateChange.IsSourcePresent.ShouldBeFalse();
                            stateChange.Target.ShouldBe(States.Zero);
                            break;

                    }
                    break;
            }

            return new AnimatedContent<States>.InterstateTransitions
            {
                SourceExit = _exit,
                TargetEnter = _enter
            };
        };

        var cut = RenderComponent<AnimatedContent<States>>(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero)
                .Add(x => x.TransitionsProvider, transitionProvider)
                .Add(x => x.ChildContent, SampleContent()));

        counters.GetAndIncreaseRenders();

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()}">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);

        cut!.SetParametersAndRender(parameters =>
            parameters.Add(x => x.TargetState, States.One));

        var contents = cut.FindComponents<AnimatedVisibility>();

        contents.Count.ShouldBe(2);

        var oldContent = contents[1];

        counters.GetAndIncreaseRenders();
        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {EnterBase.GetFinishClasses()}"
                        style="{EnterBase.GetFinishStyle()}">
                    {SampleContentText(States.One)}
                </div>
                <div class="animated-visibility animated-content-item {ExitBase.GetFinishClasses()}"
                        style="{ExitBase.GetFinishStyle()}">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);

        var waitForRerender = TestCompletionAwaiter.Create()
            .ScheduleCallbackAfterNextMarkupUpdateOfElement(
                element: cut,
                actionCausingUpdate: () => TimerService.SetResultForAwaitingTimers(oldContent.Instance, Fakes.FakeTimerService.TimerAction.Act),
                onMarkupChange: () =>
                {
                    cut.MarkupMatches($"""
                        <div class="animated-content" style="">
                            <div class="animated-visibility animated-content-item {EnterBase.GetFinishClasses()}"
                                    style="{EnterBase.GetFinishStyle()}">
                                {SampleContentText(States.One)}
                            </div>
                        </div>
                        """);
                });
        await waitForRerender.WaitForFinish();
    }


    [Fact]
    public async Task When_StatesHaveSpecificTransition_Then_ShouldExpectThoseTransitions()
    {
        var cut = RenderComponent<AnimatedContent<States>>(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero)
                .Add(x => x.NewStateOnTop, true)
                .Add(x => x.TransitionsProvider, TransitionsProvider)
                .Add(x => x.ChildContent, SampleContent()));

        AnimatedContent<States>.InterstateTransitions? TransitionsProvider(AnimatedContent<States>.StateChange stateChange)
        {
            if (stateChange.SourceEquals(States.Zero) || stateChange.Target is States.One)
                return new AnimatedContent<States>.InterstateTransitions
                {
                    TargetEnter = _enter2,
                    SourceExit = _exit
                };
            if (stateChange.SourceEquals(States.One) || stateChange.Target is States.Zero)
                return new AnimatedContent<States>.InterstateTransitions
                {
                    TargetEnter = _enter,
                    SourceExit = _exit2
                };
            return null;
        }

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()}">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);

        cut.SetParametersAndRender(
            parameters => parameters
                .Add(x => x.TargetState, States.One));

        var contents = cut.FindComponents<AnimatedVisibility>();

        contents.Count.ShouldBe(2);

        var oldContent = contents[0];

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {ExitBase.GetFinishClasses()}"
                        style="{ExitBase.GetFinishStyle()}">
                    {SampleContentText(States.Zero)}
                </div>
                <div class="animated-visibility animated-content-item {Enter2Base.GetFinishClasses()}"
                        style="{Enter2Base.GetFinishStyle()}">
                    {SampleContentText(States.One)}
                </div>
            </div>
            """);


        var waitForRerender = TestCompletionAwaiter.Create()
            .ScheduleCallbackAfterNextMarkupUpdateOfElement(
                element: cut,
                actionCausingUpdate: () => TimerService.SetResultForAwaitingTimers(oldContent.Instance, Fakes.FakeTimerService.TimerAction.Act),
                onMarkupChange: () =>
                {
                    cut.MarkupMatches($"""
                        <div class="animated-content" style="">
                            <div class="animated-visibility animated-content-item {Enter2Base.GetFinishClasses()}"
                                    style="{Enter2Base.GetFinishStyle()}">
                                {SampleContentText(States.One)}
                            </div>
                        </div>
                        """);
                });
        await waitForRerender.WaitForFinish();
    }

    /// <summary>
    /// Check to make sure manual rendering control doesn't break basic functionality
    /// </summary>
    [Fact]
    public void When_ContentUpdates_Then_ComponentShouldRerender()
    {
        var testComponent = RenderComponent<AnimatedContent.ContentUpdatesCase>(
            parameters => parameters
                .Add(x => x.InnerContent, "start"));

        var cut = testComponent.FindComponent<AnimatedContent<States>>();

        cut.ShouldNotBeNull();

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()}">
                    start
                </div>
            </div>
            """);

        testComponent.SetParametersAndRender(
            parameters => parameters
                .Add(x => x.InnerContent, "end"));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()}">
                    end
                </div>
            </div>
            """);
    }



    [Fact]
    public async Task When_PreserveHiddenElementsIsSet_Then_ShouldKeepHiddenElementsRendered()
    {
        var cut = RenderComponent<AnimatedContent<States>>(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero)
                .Add(x => x.NewStateOnTop, true)
                .Add(x => x.PreserveHiddenElements, true)
                .Add(x => x.SharedEnter, _enter)
                .Add(x => x.SharedExit, _exit)
                .Add(x => x.ChildContent, SampleCountableContent()));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {DefaultAnimatedVisibilityExitBase.GetInitialClasses()}"
                        style="{DefaultAnimatedVisibilityExitBase.GetInitialStyle()} z-index: 0;">
                    {SampleContentText(States.Zero)}
                </div>
            </div>
            """);

        SimpleElementsInitializationCounter.Count.ShouldBe(1);

        cut.SetParametersAndRender(
            parameters => parameters
                .Add(x => x.TargetState, States.One));

        var contents = cut.FindComponents<AnimatedVisibility>();

        contents.Count.ShouldBe(2);

        var zeroContent = contents[0];
        var oneContent = contents[1];

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {ExitBase.GetFinishClasses()}"
                        style="{ExitBase.GetFinishStyle()} z-index: 0;">
                    {SampleContentText(States.Zero)}
                </div>
                <div class="animated-visibility animated-content-item {EnterBase.GetFinishClasses()}"
                        style="{EnterBase.GetFinishStyle()} z-index: 1;">
                    {SampleContentText(States.One)}
                </div>
            </div>
            """);

        SimpleElementsInitializationCounter.Count.ShouldBe(2);


        await TestCompletionAwaiter.Create()
            .ScheduleCallbackAfterNextMarkupUpdateOfElement(
                element: cut,
                actionCausingUpdate: () => TimerService.SetResultForAwaitingTimers(zeroContent.Instance),
                onMarkupChange: () =>
                {
                    cut.MarkupMatches($"""
                        <div class="animated-content" style="">
                            <div class="animated-visibility disappeared animated-content-item {EnterBase.GetInitialClasses()}"
                                    style="{EnterBase.GetInitialStyle()} z-index: 0;">
                                {SampleContentText(States.Zero)}
                            </div>
                            <div class="animated-visibility animated-content-item {EnterBase.GetFinishClasses()}"
                                    style="{EnterBase.GetFinishStyle()} z-index: 1;">
                                {SampleContentText(States.One)}
                            </div>
                        </div>
                        """);
                })
            .WaitForFinish();

        SimpleElementsInitializationCounter.Count.ShouldBe(2);


        cut.SetParametersAndRender(
            parameters => parameters
                .Add(x => x.TargetState, States.Zero));

        cut.MarkupMatches($"""
            <div class="animated-content" style="">
                <div class="animated-visibility animated-content-item {EnterBase.GetFinishClasses()}"
                        style="{EnterBase.GetFinishStyle()} z-index: 2;">
                    {SampleContentText(States.Zero)}
                </div>
                <div class="animated-visibility animated-content-item {ExitBase.GetFinishClasses()}"
                        style="{ExitBase.GetFinishStyle()} z-index: 1;">
                    {SampleContentText(States.One)}
                </div>
            </div>
            """);

        SimpleElementsInitializationCounter.Count.ShouldBe(2);
    }

    private class Counters
    {
        public int Renders { get; private set; } = 0;
        public int TransitionsCalcsThisRender { get; private set; } = 0;

        public int GetAndIncreaseRenders()
        {
            TransitionsCalcsThisRender = 0;
            return Renders++;
        }

        public int GetAndIncreaseTransitionsCalcsThisRender()
            => TransitionsCalcsThisRender++;
    }
}
