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
    public required TState TargetState { get; set; }
    private TState? _targetState;

    [Parameter]
    public Func<TState, StateSwitchCase>? Switch { get; set; }
    [Parameter]
    public RenderFragment<TState>? ChildContent { get; set; }

    [Parameter]
    public bool NewStateOnTop { get; set; }

    [Parameter]
    public bool StartWithTransition { get; set; }

    [Parameter]
    public EventCallback<TState> OnContentChange { get; set; }

    private List<StateRecord> PastStates { get; } = new();
    private int CurrentStateKey { get; set; } = 0;
    private bool HasInitialTargetStateBeenShown { get; set; } = false;

    protected override void OnParametersSet()
    {
        if (TargetState is not null
            && !EqualityComparer<TState>.Default.Equals(_targetState, TargetState))
        {
            if (_targetState is not null)
            {
                PastStates.Add(new StateRecord
                {
                    Key = CurrentStateKey++,
                    State = _targetState
                });
            }

            _targetState = TargetState;
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        HasInitialTargetStateBeenShown = true;
    }

    private async Task OnTargetStateElementWasShown()
    {
        await OnContentChange.InvokeAsync(TargetState);
    }

    private void OnPastStateElementWasHidden(StateRecord pastState)
    {
        PastStates.Remove(pastState);
    }

    private readonly struct StateRecord
    {
        public required int Key { get; init; }
        public required TState State { get; init; }
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
}
