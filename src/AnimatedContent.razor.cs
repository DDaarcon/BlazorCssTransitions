using Microsoft.AspNetCore.Components;

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
    public Func<StateChange, InterstateTransitions>? TransitionsProvider { get; set; }


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


    public readonly record struct StateChange(
        TState? Source,
        TState? Target);

    public class InterstateTransitions
    {
        public EnterTransition? TargetEnter { get; init; }
        public ExitTransition? SourceExit { get; init; }
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


    private readonly List<StateRecord> _pastStates = new();
    private int _currentStateKey = 0;
    private bool _hasInitialTargetStateBeenShown = false;
    private bool _hasInitialParametersBeenSet = false;

    protected override void OnParametersSet()
    {
        if (TargetState is not null
            && !EqualityComparer<TState>.Default.Equals(_targetState, TargetState))
        {
            if (_hasInitialParametersBeenSet)
            {
                _pastStates.Add(new StateRecord
                {
                    Key = _currentStateKey++,
                    State = _targetState
                });
            }

            _targetState = TargetState;
        }

        if (!_hasInitialParametersBeenSet)
            _hasInitialParametersBeenSet = true;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        _hasInitialTargetStateBeenShown = true;
    }


    private async Task OnTargetStateElementWasShown()
    {
        await OnContentChange.InvokeAsync(TargetState);
    }

    private void OnPastStateElementWasHidden(StateRecord pastState)
    {
        _pastStates.Remove(pastState);
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

    private AnimatedVisibilityParameters[] GetAnimatedVisibilityParametersForRender(Func<TState?, StateSwitchCase> getStateCase)
    {
        var paramsOfStatesToRender = new AnimatedVisibilityParameters[_pastStates.Count + 1];

        var targetStateCase = getStateCase(TargetState);
        var targetTransitions = TransitionsProvider?.Invoke(new StateChange(
            Source: default,
            Target: TargetState));

        paramsOfStatesToRender[0] = new AnimatedVisibilityParameters(
            Visible: true,
            Enter: targetTransitions?.TargetEnter ?? targetStateCase.Enter ?? SharedEnter,
            Exit: null, // exit is not relevant for target state, it will be set when becomes past state
            StartWithTransition: StartWithTransition || _hasInitialTargetStateBeenShown,
            OnHidden: default,
            OnShown: new EventCallback(this, () => OnTargetStateElementWasShown()),
            Key: _currentStateKey,
            Fragment: targetStateCase.Fragment
        );


        if (_pastStates.Count > 0)
        {
            var fromProcessedToNewerStateTransitions = TransitionsProvider?.Invoke(new StateChange(
                Source: _pastStates[^1].State,
                Target: TargetState));

            for (int pastStateIndexFromEnd = 1; pastStateIndexFromEnd <= _pastStates.Count; pastStateIndexFromEnd++)
            {
                var pastState = _pastStates[^pastStateIndexFromEnd];
                var pastStateCase = getStateCase(pastState.State);

                var fromOlderToProcessedStateTranstions = TransitionsProvider?.Invoke(new StateChange(
                    Source: pastStateIndexFromEnd + 1 <= _pastStates.Count ? _pastStates[^(pastStateIndexFromEnd + 1)].State : default,
                    Target: _pastStates[^pastStateIndexFromEnd].State));

                paramsOfStatesToRender[pastStateIndexFromEnd] = new AnimatedVisibilityParameters(
                    Visible: false,
                    Enter: fromOlderToProcessedStateTranstions?.TargetEnter ?? pastStateCase.Enter ?? SharedEnter,
                    Exit: fromProcessedToNewerStateTransitions?.SourceExit ?? pastStateCase.Exit ?? SharedExit,
                    StartWithTransition: false,
                    OnHidden: new EventCallback(this, () => OnPastStateElementWasHidden(pastState)),
                    OnShown: default,
                    Key: pastState.Key,
                    Fragment: pastStateCase.Fragment
                );

                // when processing next (older) state we can reuse already found transitions (older becomes processed, processed becomes newer)
                fromProcessedToNewerStateTransitions = fromOlderToProcessedStateTranstions;
            }
        }

        return paramsOfStatesToRender;
    }


    private record AnimatedVisibilityParameters(
        bool Visible,
        EnterTransition? Enter,
        ExitTransition? Exit,
        bool StartWithTransition,
        EventCallback OnHidden,
        EventCallback OnShown,
        int Key,
        RenderFragment Fragment
    );
}
