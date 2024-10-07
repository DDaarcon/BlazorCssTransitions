﻿using BlazorCssTransitions.AnimatedVisibilityTransitions;
using BlazorCssTransitions.Shared.JsInterop;
using BlazorCssTransitions.Specifications;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public partial class AnimatedVisibility
{
    [Inject]
    internal JsSizeMeter _sizeMeter { get; set; } = default!;

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


    private ElementReference Container { get; set; }

    private State _currentState;

    private bool _isFirstRender;
    private bool _isFirstTransition;

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

        _isFirstRender = true;
        _isFirstTransition = true;
    }

    protected override void OnParametersSet()
    {
        if (_isFirstRender)
        {
            _isFirstRender = false;
            OnStateChanged.InvokeAsync(_currentState);
            return;
        }

        _enter = (BaseTransition?)Enter ?? _enter;
        _exit = (BaseTransition?)Exit ?? _exit;
        SetNewStateAfterParametersSet();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && StartWithTransition)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(20)); // TODO find a better way

            SetNewStateAfterParametersSet();
            StateHasChanged();
        }
    }



    internal async ValueTask<DOMScrollRect> GetScrollRect()
    {
        return await _sizeMeter.MeasureElementScroll(Container);
    }

    internal async ValueTask<DOMRect> GetRect()
    {
        return await _sizeMeter.MeasureElement(Container);
    }



    private void SetNewStateAfterParametersSet()
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
        OnStateChanged.InvokeAsync(_currentState);
        OnSetIntermediateState();
    }


    private System.Timers.Timer? _transitionTimer;

    private void OnSetIntermediateState()
    {
        var longestDuration = _currentState switch
        {
            State.Showing => _enter.GetSpecifications().GetLongestTotalDuration(),
            State.Hiding => _exit.GetSpecifications().GetLongestTotalDuration(),
            _ => throw new Exception($"State {_currentState} is not intermediate")
        };

        _transitionTimer?.Dispose();
        _transitionTimer = new(longestDuration);

        _transitionTimer.Elapsed += (sender, args) =>
        {
            _transitionTimer.Dispose();
            InvokeAsync(() =>
            {
                switch (_currentState)
                {
                    case State.Showing:
                        _currentState = State.Shown;
                        OnShown.InvokeAsync();
                        break;
                    case State.Hiding:
                        _currentState = State.Hidden;
                        OnHidden.InvokeAsync();
                        break;
                    default:
                        throw new Exception($"State {_currentState} is not intermediate");
                }

                OnStateChanged.InvokeAsync(_currentState);
                StateHasChanged();
            });
        };
        _transitionTimer.Start();
    }

    private static TimeSpan GetLongestDurationFromSpecifications(IEnumerable<Specification> specifications)
        => specifications.Select(x => x.Duration + x.Delay).Max();


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


    public enum State
    {
        Hidden,
        Showing,
        Shown,
        Hiding
    }


    public void Dispose()
    {
        _transitionTimer?.Dispose();
    }
}
