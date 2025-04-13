using BlazorCssTransitions.AnimatedVisibilityTransitions;
using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.CssStylesValidation;
using BlazorCssTransitions.Specifications;
using Microsoft.AspNetCore.Components;

namespace BlazorCssTransitions;

public partial class AnimatedVisibility : IDisposable, IHandleEvent
{
    [Inject]
    private ICssStylesAppliedValidator _cssStylesAppliedValidator { get; init; } = default!;
    [Inject]
    private ITimerService _timerService { get; init; } = default!;

    /// <summary>
    /// Visibility status
    /// </summary>
    [Parameter, EditorRequired]
    public required bool Visible { get; set; }

    /// <summary>
    /// Whether the initial render should trigger showing/hiding transition
    /// </summary>
    [Parameter]
    public bool StartWithTransition { get; set; }

    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    /// <summary>
    /// Style of container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Css classes of container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Enter transition.
    /// </summary>
    [Parameter]
    public EnterTransition? Enter { get; set; }
    private BaseTransition _enter = default!;
    internal static readonly EnterTransition _defaultEnter = EnterTransition.FadeIn();

    /// <summary>
    /// Exit transition.
    /// </summary>
    [Parameter]
    public ExitTransition? Exit { get; set; }
    private BaseTransition _exit = default!;
    internal static readonly ExitTransition _defaultExit = ExitTransition.FadeOut();

    /// <summary>
    /// Event triggered when enter transition finishes.
    /// </summary>
    [Parameter]
    public EventCallback OnShown { get; set; }

    /// <summary>
    /// Event triggered when exit transition finishes.
    /// </summary>
    [Parameter]
    public EventCallback OnHidden { get; set; }

    /// <summary>
    /// Event triggered when visibility state changes.
    /// </summary>
    /// <remarks>
    /// When <see cref="RemoveFromDOMWhenHidden"/> is enabled, state <see cref="State.Showing"/> is set when element is added to DOM.<br/>
    /// When <see cref="DisappearWhenHidden"/> is enabled, state <see cref="State.Showing"/> is set when element appears.
    /// </remarks>
    [Parameter]
    public EventCallback<State> OnStateChanged { get; set; }

    /// <summary>
    /// Whether container with its content should be removed from DOM when state is equal to <see cref="State.Hidden"/>.
    /// Keep in mind that removed elements will be disposed (their state will be lost).
    /// </summary>
    [Parameter]
    public bool RemoveFromDOMWhenHidden { get; set; }

    /// <summary>
    /// Whether container should disappear (display: none) when state is equal to <see cref="State.Hidden"/>.
    /// Has lower importance than <see cref="RemoveFromDOMWhenHidden"/>, so if both parameters are present, container will be removed rather than disappeared.
    /// </summary>
    [Parameter]
    public bool DisappearWhenHidden { get; set; }

    public enum State
    {
        Hidden,
        Showing,
        Shown,
        Hiding
    }



    private State _currentState;
    private bool _isAfterInitialParametersSet;
    private bool _isCurrentRenderAddingContainerToDOM;
    private bool _isApperingRequested;

    private bool ShouldNotRenderAnything { get; set; }
    private bool ShouldRenderDisappeared { get; set; }

    private ElementReference _containerElement { get; set; }

    protected override void OnInitialized()
    {
        _enter = (BaseTransition)(Enter ?? _defaultEnter);
        _exit = (BaseTransition)(Exit ?? _defaultExit);

        _currentState = (Visible, StartWithTransition) switch
        {
            (true, false) => State.Shown,
            (true, true) => State.Hidden,
            (false, false) => State.Hidden,
            (false, true) => State.Shown,
        };

        _isAfterInitialParametersSet = false;

        _isCurrentRenderAddingContainerToDOM =
            // always start with rendering if RemoveFromDOMWhenHidden is disabled
            !RemoveFromDOMWhenHidden
            // even if RFDWH is true, visible should be rendered
            || Visible
            // even if not visible, render when starts from transition to be hidden
            || StartWithTransition;

        ShouldNotRenderAnything = !_isCurrentRenderAddingContainerToDOM;

        ShouldRenderDisappeared =
            // ignore everything else if RemoveFromDOMWhenHidden is enabled (that option takes precedence)
            !RemoveFromDOMWhenHidden
            // render disabled only if that functionality is enabled
            && DisappearWhenHidden
            // applicable only if element should be hidden
            && !Visible
            // element will start as visible, even if Visible is false, when starting from transition to visible
            && !StartWithTransition;

    }

    // TODO verify if is required
    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem item, object? arg)
    {
        // do not handle any events
        return Task.CompletedTask;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isAfterInitialParametersSet)
        {
            _isAfterInitialParametersSet = true;
            await NotifyAboutStateChange();
            return;
        }

        if (Enter is null
            || _enter != Enter)
        {
            _enter = (BaseTransition?)Enter ?? _enter;
        }
        if (Exit is null
            || _exit != Exit)
        {
            _exit = (BaseTransition?)Exit ?? _exit;
        }

        if (ShouldNotRenderAnything
            && Visible)
        {
            // when element removed from DOM should become visible
            // we expect _currentState to be Hidden
            ShouldNotRenderAnything = false;
            _isCurrentRenderAddingContainerToDOM = true;
            return;
        }

        if (ShouldRenderDisappeared
            && Visible)
        {
            // when disappeared element should become visible
            // we expect _currentState to be Hidden
            ShouldRenderDisappeared = false;
            _isApperingRequested = true;
            return;
        }

        await SetIntermediateState();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (CheckIfShouldRerender(firstRender))
        {
            _isCurrentRenderAddingContainerToDOM = false;
            _isApperingRequested = false;
            await _cssStylesAppliedValidator.EnsureStylesWereApplied(_containerElement);

            await SetIntermediateState();
            StateHasChanged();
        }
    }

    private bool CheckIfShouldRerender(bool firstRender)
    {
        bool isEitherJustAddedOrJustAppeared
            = _isCurrentRenderAddingContainerToDOM || _isApperingRequested;

        if (!isEitherJustAddedOrJustAppeared)
            return false;

        return (firstRender && StartWithTransition)
            || RemoveFromDOMWhenHidden
            || DisappearWhenHidden;
    }

    private async Task SetIntermediateState()
    {
        if ((Visible && _currentState is State.Showing or State.Shown)
            || (!Visible && _currentState is State.Hiding or State.Hidden))
        {
            return;
        }

        // abort right away, to prevent race conditions when OnSetIntermediateState expects intermediate state
        // but timer finishes and sets state to final
        _transitionTimer?.Abort();

        var newState = Visible
            ? State.Showing
            : State.Hiding;

        _currentState = newState;

        await NotifyAboutStateChange();
        OnSetIntermediateState();
    }


    private ITimerService.ITimerRegistration? _transitionTimer;

    private void OnSetIntermediateState()
    {
        var longestDuration = _currentState switch
        {
            State.Showing => _enter.GetSpecifications().GetLongestTotalDuration(),
            State.Hiding => _exit.GetSpecifications().GetLongestTotalDuration(),
            _ => throw new Exception($"State {_currentState} is not intermediate")
        };

        _transitionTimer = _timerService.StartNew(longestDuration, () =>
        {
            InvokeAsync(async () =>
            {
                _currentState = _currentState switch
                {
                    State.Showing => State.Shown,
                    State.Hiding => State.Hidden,
                    _ => _currentState
                };

                if (_currentState is State.Hidden
                    && RemoveFromDOMWhenHidden)
                {
                    ShouldNotRenderAnything = true;
                }
                else if (_currentState is State.Hidden
                    && DisappearWhenHidden)
                {
                    ShouldRenderDisappeared = true;
                }

                await NotifyAboutStateChange();
                StateHasChanged();
            });
        },
        caller: this,
        oldRegistration: _transitionTimer /* should have been aborted just before, but it doesn't harm to do it again */);
    }

    private async Task NotifyAboutStateChange()
    {
        switch (_currentState)
        {
            case State.Hidden:
                await OnHidden.InvokeAsync();
                break;
            case State.Shown:
                await OnShown.InvokeAsync();
                break;
        }

        await OnStateChanged.InvokeAsync(_currentState);
    }

    private string GetContainerStyle()
    {
        IEnumerable<string> styles = [
            _currentState switch
            {
                State.Hidden => _enter.GetInitialStyle(),
                State.Showing => _enter.GetFinishStyle(),
                State.Shown => _exit.GetInitialStyle(),
                State.Hiding => _exit.GetFinishStyle(),
                _ => throw new Exception($"State {_currentState} is not valid")
            }
        ];

        if (!String.IsNullOrEmpty(Style))
            styles = styles.Append(Style);

        return String.Join(" ", styles);
    }


    private const string _containerClass = "animated-visibility";
    private const string _disappearedContainerClass = "disappeared";

    private string GetContainerClasses()
    {
        // TODO benchmark
        IEnumerable<string> classes = [
            _containerClass,
            _currentState switch
            {
                State.Hidden => _enter.GetInitialClasses(),
                State.Showing => _enter.GetFinishClasses(),
                State.Shown => _exit.GetInitialClasses(),
                State.Hiding => _exit.GetFinishClasses(),
                _ => ""
            }
        ];

        if (!String.IsNullOrEmpty(Class))
            classes = classes.Append(Class);

        if (ShouldRenderDisappeared)
            classes = classes.Append(_disappearedContainerClass);

        return String.Join(" ", classes);
    }

    public void Dispose()
    {
        _transitionTimer?.Abort();
    }

#if UNITTESTS
    private static int _instanceCounter = 0;
    private int _instanceNumber = _instanceCounter++;
    public override string ToString()
    {
        return $"{nameof(AnimatedVisibility)}-{_instanceNumber}";
    }
#endif
}
