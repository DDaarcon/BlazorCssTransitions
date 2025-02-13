using Microsoft.AspNetCore.Components;

namespace BlazorCssTransitions;

public partial class AnimatedContent<TState>
{
    /// <summary>
    /// State, for which content should appear.
    /// </summary>
    [Parameter, EditorRequired]
    public required TState? TargetState { get; set; }

    /// <summary>
    /// Provides content for state, and enter and exit transitions for that content.
    /// </summary>
    [Parameter]
    public Func<TState?, StateSwitchCase>? Switch { get; set; }

    [Parameter]
    public RenderFragment<TState?>? ChildContent { get; set; }
    /// <summary>
    /// Provides enter transition for appearing content and exit transition for disappearing content.
    /// </summary>
    [Parameter]
    public Func<StateChange, InterstateTransitions>? TransitionsProvider { get; set; }

    /// <summary>
    /// Newer content should render above older one.
    /// </summary>
    [Parameter]
    public bool NewStateOnTop { get; set; }

    /// <summary>
    /// Initial content should appear with enter transition.
    /// </summary>
    [Parameter]
    public bool StartWithTransition { get; set; }

    /// <summary>
    /// Callback for when target content is fully shown.
    /// </summary>
    [Parameter]
    public EventCallback<TState?> OnContentChanged { get; set; }

    /// <summary>
    /// Css styles for content container.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }
    /// <summary>
    /// Css classes for content container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Prevents content from exceeding bounds of the container.
    /// </summary>
    [Parameter]
    public bool KeepContentInBounds { get; set; }

    /// <summary>
    /// Shared enter transition. Is be used when <see cref="TransitionsProvider"/> and <see cref="Switch"/> return <c>null</c> for enter transition.
    /// </summary>
    [Parameter]
    public EnterTransition? SharedEnter { get; set; }
    /// <summary>
    /// Shared exit transition. Is be used when <see cref="TransitionsProvider"/> and <see cref="Switch"/> return <c>null</c> for exit transition.
    /// </summary>
    [Parameter]
    public ExitTransition? SharedExit { get; set; }

    /// <summary>
    /// Transitions for each state will not be cached.
    /// They will be recalculated (from <see cref="TransitionsProvider"/>, <see cref="Switch"/> or shared transitions) on each update.
    /// </summary>
    [Parameter]
    public bool ReassignTransitionsOnEachUpdate { get; set; }


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

    private StateData? _targetStateData;
    private readonly List<StateData> _pastStatesData = new();
    private int _currentStateKey = 0;
    private bool _hasInitialTargetStateBeenShown = false;

    protected override void OnParametersSet()
    {
        if (!_targetStateData.HasValue || !EqualityComparer<TState>.Default.Equals(_targetStateData.Value.State, TargetState))
        {
            if (_targetStateData.HasValue)
            {
                // add previous target to past states
                _pastStatesData.Add(_targetStateData.Value);
            }

            _targetStateData = new StateData
            {
                Key = _currentStateKey++,
                State = TargetState
            };
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        _hasInitialTargetStateBeenShown = true;
    }


    private async Task OnTargetStateElementWasShown()
    {
        await OnContentChanged.InvokeAsync(_targetStateData!.Value.State);
    }

    private void OnPastStateElementWasHidden(StateData pastState)
    {
        _pastStatesData.Remove(pastState);
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


    private struct StateData
    {
        public required int Key { get; init; }
        public required TState? State { get; init; }

        public bool IsEnterCached { get; private set; }
        public EnterTransition? CachedEnter { get; private set; }
        public void CacheEnter(EnterTransition? enter)
        {
            IsEnterCached = true;
            CachedEnter = enter;
        }

        public bool IsExitCached { get; private set; }
        public ExitTransition? CachedExit { get; private set; }
        public void CacheExit(ExitTransition? exit)
        {
            IsExitCached = true;
            CachedExit = exit;
        }
    }

    private AnimatedVisibilityParameters[] GetAnimatedVisibilityParametersForRender()
    {
        if (!_targetStateData.HasValue)
            return [];

        var paramsOfStatesToRender = new AnimatedVisibilityParameters[_pastStatesData.Count + 1];

        var targetStateCase = GetStateCase(_targetStateData.Value.State);
        var targetTransitions = TransitionsProvider?.Invoke(new StateChange(
            Source: default,
            Target: _targetStateData.Value.State));

        EnterTransition? enter = null;
        ExitTransition? exit = null;

        enter = targetTransitions?.TargetEnter ?? targetStateCase.Enter ?? SharedEnter;

        if (!ReassignTransitionsOnEachUpdate)
        {
            _targetStateData.Value.CacheEnter(enter);
        }

        paramsOfStatesToRender[0] = new AnimatedVisibilityParameters(
            Visible: true,
            Enter: enter,
            Exit: null, // exit is not relevant for target state, it will be set when becomes past state
            StartWithTransition: StartWithTransition || _hasInitialTargetStateBeenShown,
            OnHidden: default,
            OnShown: new EventCallback(this, () => OnTargetStateElementWasShown()),
            Key: _targetStateData.Value.Key,
            Fragment: targetStateCase.Fragment
        );


        if (_pastStatesData.Count > 0)
        {
            var fromProcessedToNewerStateTransitions = TransitionsProvider?.Invoke(new StateChange(
                Source: _pastStatesData[^1].State,
                Target: _targetStateData.Value.State));

            for (int pastStateIndexFromEnd = 1; pastStateIndexFromEnd <= _pastStatesData.Count; pastStateIndexFromEnd++)
            {
                var pastState = _pastStatesData[^pastStateIndexFromEnd];
                var pastStateCase = GetStateCase(pastState.State);

                var fromOlderToProcessedStateTranstions = TransitionsProvider?.Invoke(new StateChange(
                    Source: pastStateIndexFromEnd + 1 <= _pastStatesData.Count 
                        ? _pastStatesData[^(pastStateIndexFromEnd + 1)].State
                        : default,
                    Target: _pastStatesData[^pastStateIndexFromEnd].State));

                (enter, exit) = GetProperTransitionsForPastStateAndCacheIfRequired(
                    state: pastState,
                    calculatedEnter: fromOlderToProcessedStateTranstions?.TargetEnter ?? pastStateCase.Enter ?? SharedEnter,
                    calculatedExit: fromProcessedToNewerStateTransitions?.SourceExit ?? pastStateCase.Exit ?? SharedExit);

                paramsOfStatesToRender[pastStateIndexFromEnd] = new AnimatedVisibilityParameters(
                    Visible: false,
                    Enter: enter,
                    Exit: exit,
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

    private StateSwitchCase GetStateCase(TState? state)
    {
        if (ChildContent is not null)
            return ChildContent(state);

        if (Switch is not null)
            return Switch(state);

        throw new ApplicationException("State case provider is not set");
    }

    private (EnterTransition?, ExitTransition?) GetProperTransitionsForPastStateAndCacheIfRequired(
        StateData state,
        EnterTransition? calculatedEnter,
        ExitTransition? calculatedExit)
    {
        if (ReassignTransitionsOnEachUpdate)
            return (calculatedEnter, calculatedExit);

        if (!state.IsEnterCached)
            state.CacheEnter(calculatedEnter);

        if (!state.IsExitCached)
            state.CacheExit(calculatedExit);

        return (state.CachedEnter, state.CachedExit);
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
