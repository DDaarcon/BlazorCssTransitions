﻿using Microsoft.AspNetCore.Components;

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
    /// Provides enter transition for appearing content and exit transition for disappearing content. <br />
    /// It may be called even if transitons are already assigned and cached.
    /// </summary>
    [Parameter]
    public Func<StateChange, InterstateTransitions?>? TransitionsProvider { get; set; }

    /// <summary>
    /// Newer content should render above older one (later in DOM).
    /// </summary>
    [Parameter]
    public bool NewStateOnTop { get; set; }

    /// <summary>
    /// Initial content should appear with enter transition.
    /// </summary>
    [Parameter]
    public bool StartWithTransition { get; set; }
    

    /// <summary>
    /// Event triggered when target content is fully shown.
    /// </summary>
    [Parameter]
    public EventCallback<TState?> OnTargetStateAppeared { get; set; }

    /// <summary>
    /// Event triggered when target content starts appearing animation.
    /// </summary>
    [Parameter]
    public EventCallback<TState?> OnTargetStateAppearing { get; set; }


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

    /// <summary>
    /// Hidden elements will stay rendered (their state will not be lost).
    /// Keeping too many elements might negatively impact performance.
    /// </summary>
    [Parameter]
    public bool PreserveHiddenElements { get; set; }

    /// <summary>
    /// States for transition.
    /// </summary>
    /// <param name="Source">Source state, from which content transitions to target state.</param>
    /// <param name="Target">Target state, to which content transitions from source state.</param>
    /// <param name="IsSourcePresent">Whether source state had been applied. Especially useful when dealing with value types.</param>
    public readonly record struct StateChange(
        TState? Source,
        TState? Target,
        bool IsSourcePresent = true);

    public class InterstateTransitions
    {
        // TODO explanation
        public EnterTransition? TargetEnter { get; init; }
        // TODO explanation
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
    private int _currentStateKey = 1; // start counting from 1 to not use default value
    private bool _hasInitialTargetStateBeenShown = false;

    protected override void OnParametersSet()
    {
        if (!_targetStateData.HasValue || !EqualityComparer<TState>.Default.Equals(_targetStateData.Value.State, TargetState))
        {
            ApplyNewTargetState();
        }

        _shouldRerender = true;
    }

    private void ApplyNewTargetState()
    {
        if (_targetStateData.HasValue)
        {
            // add previous target to past states
            _pastStatesData.Add(_targetStateData.Value);
        }

        if (!PreserveHiddenElements)
        {
            CreateNewTargetStateData();
            return;
        }

        var indexInPastStates = _pastStatesData.FindIndex(x => EqualityComparer<TState>.Default.Equals(x.State, TargetState));

        if (indexInPastStates == -1)
        {
            CreateNewTargetStateData();
            return;
        }

        _targetStateData = _pastStatesData[indexInPastStates];
        _pastStatesData.RemoveAt(indexInPastStates);
        _targetStateData.Value.Reset();

        void CreateNewTargetStateData()
        {
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

    private bool _shouldRerender = false;
    protected override bool ShouldRender()
    {
        if (!_shouldRerender)
            return false;

        _shouldRerender = false;
        return true;
    }

    private void OnPastStateElementWasHidden(StateData pastState)
    {
        if (!PreserveHiddenElements)
        {
            _pastStatesData.Remove(pastState);
            _shouldRerender = true;
        }
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

        public void Reset()
        {
            CachedEnter = null;
            IsEnterCached = false;
            CachedExit = null;
            IsExitCached = false;
        }
    }

    private AnimatedVisibilityParameters[] GetAnimatedVisibilityParametersForRender()
    {
        if (!_targetStateData.HasValue)
            return [];

        var paramsOfStatesToRender = new AnimatedVisibilityParameters[_pastStatesData.Count + 1];

        var targetStateCase = GetStateCase(_targetStateData.Value.State);

        bool hasAnyPastStates = _pastStatesData.Count > 0;

        InterstateTransitions? targetTransitions = TransitionsProvider?.Invoke(new StateChange(
            Source: hasAnyPastStates ? _pastStatesData[^1].State : default,
            Target: _targetStateData.Value.State,
            IsSourcePresent: hasAnyPastStates));

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
            OnStateChanged: new EventCallback<AnimatedVisibility.State>(this, OnTargetStateVisibilityStateChange),
            Key: _targetStateData.Value.Key,
            Fragment: targetStateCase.Fragment
        );


        if (_pastStatesData.Count > 0)
        {
            InterstateTransitions? fromProcessedToNewerStateTransitions = targetTransitions;

            for (int pastStateIndexFromEnd = 1; pastStateIndexFromEnd <= _pastStatesData.Count; pastStateIndexFromEnd++)
            {
                var pastState = _pastStatesData[^pastStateIndexFromEnd];
                var pastStateCase = GetStateCase(pastState.State);

                bool hasEarlierState = pastStateIndexFromEnd + 1 <= _pastStatesData.Count;

                InterstateTransitions? fromOlderToProcessedStateTranstions = TransitionsProvider?.Invoke(new StateChange(
                    Source: hasEarlierState
                        ? _pastStatesData[^(pastStateIndexFromEnd + 1)].State
                        : default,
                    Target: _pastStatesData[^pastStateIndexFromEnd].State,
                    IsSourcePresent: hasEarlierState));

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
                    OnStateChanged: default,
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

    private async Task OnTargetStateVisibilityStateChange(AnimatedVisibility.State visibilityState)
    {
        switch (visibilityState)
        {
            case AnimatedVisibility.State.Shown:
                await OnTargetStateAppeared.InvokeAsync();
                break;
            case AnimatedVisibility.State.Showing:
                await OnTargetStateAppearing.InvokeAsync();
                break;
        }
    }

    private record AnimatedVisibilityParameters(
        bool Visible,
        EnterTransition? Enter,
        ExitTransition? Exit,
        bool StartWithTransition,
        EventCallback OnHidden,
        EventCallback<AnimatedVisibility.State> OnStateChanged,
        int Key,
        RenderFragment Fragment
    );
}
