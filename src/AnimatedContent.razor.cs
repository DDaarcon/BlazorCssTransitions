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

    [Parameter]
    public Func<StatesChange, CurrentTransitions?>? TransitionsProvider { get; set; }

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
    /// Current and surrounding states.
    /// </summary>
    /// <param name="Earlier">State, from which it is transitioned to the current. Further from the target.</param>
    /// <param name="Current">State, for which transitions are being set.</param>
    /// <param name="Later">Sate, to which it is transitioned from the current. Closer to the target.</param>
    public readonly record struct StatesChange(
        PossibleState Earlier,
        TState? Current,
        PossibleState Later);

    /// <param name="Value">Assigned value or default.</param>
    /// <param name="HasValue">Whether value has been assigned and do not contain default value.</param>
    public readonly record struct PossibleState(
        TState? Value,
        bool HasValue)
    {
        public static implicit operator TState?(PossibleState @this)
        {
            if (!@this.HasValue)
                throw new Exception("PossibleState structure does not have assigned state");
            return @this.Value;
        }
    }

    public class CurrentTransitions
    {
        public EnterTransition? Enter { get; init; }
        public ExitTransition? Exit { get; init; }
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
        foreach (var (older, current, newer) 
            in _elementsCollection.EnumerateForWrite(usingOrder: PreserveHiddenElements))
        {
            var currentTransitions = TransitionsProvider?.Invoke(new StatesChange(
                Earlier: new PossibleState(
                    older is not null ? older.State : default,
                    older is not null),
                Current: current.State,
                Later: new PossibleState(
                    newer is not null ? newer.State : default,
                    newer is not null)));

            var stateCase = GetStateCase(current.State);

            bool isTarget = current == _elementsCollection.TargetElement;

            if (isTarget)
                PrepareTargetElement(current);
            else
                PrepareNonTargetElement(current);

            current.Fragment = stateCase.Fragment;


            void PrepareTargetElement(StateElementsCollection<TState>.IElementForWrite element)
            {
                current.SetEnterIfNotCached(
                    enter: currentTransitions?.Enter ?? stateCase.Enter ?? SharedEnter,
                    cache: !ReassignTransitionsOnEachUpdate);

                current.OnStateChanged = new EventCallback<AnimatedVisibility.State>(this, OnTargetElementVisibilityStateChange);

                current.StartWithTransition = StartWithTransition || _hasInitialTargetStateBeenShown;
            }

            void PrepareNonTargetElement(StateElementsCollection<TState>.IElementForWrite element)
            {
                current.SetEnterIfNotCached(
                    enter: currentTransitions?.Enter ?? stateCase.Enter ?? SharedEnter,
                    cache: !ReassignTransitionsOnEachUpdate);

                current.SetExitIfNotCached(
                    exit: currentTransitions?.Exit ?? stateCase.Exit ?? SharedExit,
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
