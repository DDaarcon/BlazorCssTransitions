using BlazorCssTransitions.AnimatedContentInternal;
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
    /// Provides enter transition for appearing content and exit transition for disappearing content. <br />
    /// It may be called even if transitons are already assigned and cached.
    /// </summary>
    [Parameter]
    public Func<StateChange, InterstateTransitions?>? TransitionsProvider { get; set; }

    /// <summary>
    /// Newer content should render above older one (later in DOM).
    /// </summary>
    [Parameter]
    public bool NewStateOnTop { get; init; }
    private bool _newStateOnTop;

    /// <summary>
    /// Whether the initial content should appear with enter transition.
    /// </summary>
    [Parameter]
    public bool StartWithTransition { get; init; }
    private bool _startWithTransition;
    

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
    public bool ReassignTransitionsOnEachUpdate { get; init; }
    private bool _reassignTransitionsOnEachUpdate;

    /// <summary>
    /// Hidden elements will stay rendered (their state will not be lost).
    /// Keeping too many elements might negatively impact performance.
    /// </summary>
    [Parameter]
    public bool PreserveHiddenElements { get; init; }
    private bool _preserveHiddenElements;

    /// <summary>
    /// States for transition.
    /// </summary>
    /// <param name="SourceOrDefault">Source state, from which content transitions to target state. Might contain default value.</param>
    /// <param name="Target">Target state, to which content transitions from source state.</param>
    /// <param name="IsSourcePresent">Whether source state had been applied. Especially useful when dealing with value types.</param>
    public readonly record struct StateChange(
        TState? SourceOrDefault,
        TState? Target,
        bool IsSourcePresent = true)
    {
        /// <summary>
        /// Checks whether source has been assigned and equals to the provided value.
        /// </summary>
        public bool SourceEquals(TState? state)
            => IsSourcePresent && CompareStates(SourceOrDefault, state);
    }

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

    // TODO 
    // it might be a good to add a lock or a cancelation of a previous creation request, when component's Parameters change once again
    private bool _hasInitialTargetStateBeenShown = false;
    private readonly StateElementsCollection<TState> _elementsCollection = new();

    protected override void OnInitialized()
    {
        _newStateOnTop = NewStateOnTop;
        _startWithTransition = StartWithTransition;
        _reassignTransitionsOnEachUpdate = ReassignTransitionsOnEachUpdate;
        _preserveHiddenElements = PreserveHiddenElements;
    }

    protected override void OnParametersSet()
    {
        EnsureReadonlyFieldsAreNotChanged();

        if (!_elementsCollection.EqualsCurrentTarget(TargetState))
        {
            ApplyNewTargetState();
        }

        _shouldRerender = true;

        ProcessElements();
    }
    private void EnsureReadonlyFieldsAreNotChanged()
    {
        const string message = "Changing value of {0} is not supported.";

        if (_newStateOnTop != NewStateOnTop)
            throw new NotSupportedException(string.Format(message, nameof(NewStateOnTop)));
        if (_startWithTransition != StartWithTransition)
            throw new NotSupportedException(string.Format(message, nameof(StartWithTransition)));
        if (_reassignTransitionsOnEachUpdate != ReassignTransitionsOnEachUpdate)
            throw new NotSupportedException(string.Format(message, nameof(ReassignTransitionsOnEachUpdate)));
        if (_preserveHiddenElements != PreserveHiddenElements)
            throw new NotSupportedException(string.Format(message, nameof(PreserveHiddenElements)));
    }

    private void ApplyNewTargetState()
    {
        if (PreserveHiddenElements)
        {
            _elementsCollection.AppendNewOrReuseTarget(TargetState);
        }
        else
        {
            _elementsCollection.AppendNewTarget(TargetState);
        }
    }

    private void ProcessElements()
    {
        InterstateTransitions? fromCurrentToNewerTransitions = null;
        InterstateTransitions? fromOlderToCurrentTransitions = null;

        foreach (var (older, current, newer) 
            in _elementsCollection.EnumerateForWrite(usingOrder: PreserveHiddenElements))
        {
            var stateCase = GetStateCase(current.State);

            fromOlderToCurrentTransitions = TransitionsProvider?.Invoke(new StateChange(
                SourceOrDefault: older is not null ? older.State : default,
                Target: current.State,
                IsSourcePresent: older is not null));

            bool isTarget = current == _elementsCollection.TargetElement;

            if (isTarget)
                PrepareTargetElement(current);
            else
                PrepareNonTargetElement(current);

            current.Fragment = stateCase.Fragment;

            fromCurrentToNewerTransitions = fromOlderToCurrentTransitions;


            void PrepareTargetElement(StateElementsCollection<TState>.IElementForWrite element)
            {
                current.SetEnterIfNotCached(
                    enter: fromOlderToCurrentTransitions?.TargetEnter ?? stateCase.Enter ?? SharedEnter,
                    cache: !ReassignTransitionsOnEachUpdate);

                current.OnStateChanged = new EventCallback<AnimatedVisibility.State>(this, OnTargetElementVisibilityStateChange);

                current.StartWithTransition = StartWithTransition || _hasInitialTargetStateBeenShown;
            }

            void PrepareNonTargetElement(StateElementsCollection<TState>.IElementForWrite element)
            {
                current.SetEnterIfNotCached(
                    enter: fromOlderToCurrentTransitions?.TargetEnter ?? stateCase.Enter ?? SharedEnter,
                    cache: !ReassignTransitionsOnEachUpdate);

                current.SetExitIfNotCached(
                    exit: fromCurrentToNewerTransitions?.SourceExit ?? stateCase.Exit ?? SharedExit,
                    cache: !ReassignTransitionsOnEachUpdate);

                current.OnStateChanged = new EventCallback<AnimatedVisibility.State>(this, (AnimatedVisibility.State state) => OnNonTargetElementVisibilityStateChange(state, current));
            }
        }
    }

    private StateSwitchCase GetStateCase(TState? state)
    {
        if (ChildContent is not null)
            return ChildContent(state);

        if (Switch is not null)
            return Switch(state);

        throw new ApplicationException("State case provider is not set");
    }


    private void OnNonTargetElementVisibilityStateChange(AnimatedVisibility.State visibilityState, StateElementsCollection<TState>.IElement element)
    {
        if (visibilityState is AnimatedVisibility.State.Hidden
            && !PreserveHiddenElements)
        {
            _elementsCollection.Remove(element);
            _shouldRerender = true;
        }
    }

    private async Task OnTargetElementVisibilityStateChange(AnimatedVisibility.State visibilityState)
    {
        switch (visibilityState)
        {
            case AnimatedVisibility.State.Shown:
                await OnTargetStateAppeared.InvokeAsync(TargetState);
                break;
            case AnimatedVisibility.State.Showing:
                await OnTargetStateAppearing.InvokeAsync(TargetState);
                break;
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

    private static bool CompareStates(TState? state1, TState? state2)
        => EqualityComparer<TState?>.Default.Equals(state1, state2);
}
