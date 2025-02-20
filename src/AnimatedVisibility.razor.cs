using BlazorCssTransitions.AnimatedVisibilityTransitions;
using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.CssStylesValidation;
using BlazorCssTransitions.Specifications;
using Microsoft.AspNetCore.Components;

namespace BlazorCssTransitions;

public partial class AnimatedVisibility : IDisposable
{
    [Inject]
    private ICssStylesAppliedValidator _cssStylesAppliedValidator { get; init; } = default!;
    [Inject]
    private ITimerService _timerService { get; init; } = default!;


    [Parameter, EditorRequired]
    public required bool Visible { get; set; }

    [Parameter]
    public bool StartWithTransition { get; set; }

    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public EnterTransition? Enter { get; set; }
    private BaseTransition _enter = default!;
    private static readonly EnterTransition _defaultEnter = EnterTransition.FadeIn();

    [Parameter]
    public ExitTransition? Exit { get; set; }
    private BaseTransition _exit = default!;
    private static readonly ExitTransition _defaultExit = ExitTransition.FadeOut();

    [Parameter]
    public EventCallback OnShown { get; set; }

    [Parameter]
    public EventCallback OnHidden { get; set; }

    [Parameter]
    public EventCallback<State> OnStateChanged { get; set; }

    [Parameter]
    public bool RemoveFromDOMWhenHidden { get; set; }


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

    private bool ShouldNotRenderAnything { get; set; }

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
            !RemoveFromDOMWhenHidden // always start with rendering if RemoveFromDOMWhenHidden is false
            || Visible // even if RFDWH is true, visible should be rendered
            || StartWithTransition; // even if not visible, render when starts from transition to be hidden

        ShouldNotRenderAnything = !_isCurrentRenderAddingContainerToDOM;
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isAfterInitialParametersSet)
        {
            _isAfterInitialParametersSet = true;
            await NotifyAboutStateChange();
            return;
        }

        if (ShouldNotRenderAnything
            && Visible)
        {
            // when element removed from DOM should become visible
            // state should be Hidden
            ShouldNotRenderAnything = false;
            _isCurrentRenderAddingContainerToDOM = true;
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

        await SetIntermediateState();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (ShouldRerenderAfterAddingContainerToDOM(firstRender))
        {
            _isCurrentRenderAddingContainerToDOM = false;
            await _cssStylesAppliedValidator.EnsureStylesWereApplied(_containerElement);

            await SetIntermediateState();
            StateHasChanged();
        }
    }

    private bool ShouldRerenderAfterAddingContainerToDOM(bool firstRender)
    {
        if (!_isCurrentRenderAddingContainerToDOM)
            return false;

        return (firstRender && StartWithTransition)
            || RemoveFromDOMWhenHidden;
    }

    private async Task SetIntermediateState()
    {
        if ((Visible && _currentState is State.Showing or State.Shown)
            || (!Visible && _currentState is State.Hiding or State.Hidden))
        {
            return;
        }

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

                await NotifyAboutStateChange();
                StateHasChanged();
            });
        }, oldRegistration: _transitionTimer);
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

    private string GetContainerClasses()
    {
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

        return String.Join(" ", classes);
    }

    public void Dispose()
    {
        _transitionTimer?.Abort();
    }
}
