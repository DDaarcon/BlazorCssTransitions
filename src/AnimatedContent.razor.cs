using BlazorCssTransitions.AnimatedVisibilityTransitions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public partial class AnimatedContent<TState>
{
    [Parameter, EditorRequired]
    public required TState? TargetState { get; set; }
    private TState? _targetState;

    [Parameter]
    public Func<TState?, StateSwitchCase>? Switch { get; set; }

    [Parameter]
    public RenderFragment<TState?>? ChildContent { get; set; }
    [Parameter]
    public GetTransitions? TransitionsProvider { get; set; }

    [Parameter]
    public bool NewStateOnTop { get; set; }

    [Parameter]
    public bool StartWithTransition { get; set; }

    [Parameter]
    public EventCallback<TState?> OnContentChange { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public EnterTransition? SharedEnter { get; set; }
    [Parameter]
    public ExitTransition? SharedExit { get; set; }

    private List<StateRecord> PastStates { get; } = new();
    private int CurrentStateKey { get; set; } = 0;
    private bool HasInitialTargetStateBeenShown { get; set; } = false;

    private bool _hasInitialParametersBeenSet = false;

    protected override void OnParametersSet()
    {
        if (TargetState is not null
            && !EqualityComparer<TState>.Default.Equals(_targetState, TargetState))
        {
            if (_hasInitialParametersBeenSet)
            {
                PastStates.Add(new StateRecord
                {
                    Key = CurrentStateKey++,
                    State = _targetState
                });
            }

            _targetState = TargetState;
            _shouldRender = true;
        }

        if (!_hasInitialParametersBeenSet)
            _hasInitialParametersBeenSet = true;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        HasInitialTargetStateBeenShown = true;
    }


    private bool _shouldRender;
    protected override bool ShouldRender()
    {
        var shouldRender = _shouldRender;
        _shouldRender = false;
        return shouldRender;
    }


    private async Task OnTargetStateElementWasShown()
    {
        await OnContentChange.InvokeAsync(TargetState);
    }

    private void OnPastStateElementWasHidden(StateRecord pastState)
    {
        PastStates.Remove(pastState);
        _shouldRender = true;
    }

    private string ContainerStyles
        => Style ?? "";

    private const string _containerClass = "animated-content";
    private string ContainerClasses
    {
        get
        {
            IEnumerable<string> classes = [_containerClass];

            if (!String.IsNullOrEmpty(Class))
                classes = classes.Append(Class);

            return String.Join(" ", classes);
        }
    }

    private readonly struct StateRecord
    {
        public required int Key { get; init; }
        public required TState? State { get; init; }
    }

    public class StateSwitchCase
    {
        public required RenderFragment Fragment { get; init; }

        public EnterTransition? Enter { get; init; }
        public ExitTransition? Exit { get; init; }


        public static implicit operator StateSwitchCase(RenderFragment fragment)
            => new()
            {
                Fragment = fragment
            };
    }

    public class InterstateTransitions
    {
        public EnterTransition? FromPreviousStateEnter { get; set; }
        public ExitTransition? ToNextStateExit { get; set; }
    }

    public delegate InterstateTransitions? GetTransitions(TState? fromState, TState? toState);
}
