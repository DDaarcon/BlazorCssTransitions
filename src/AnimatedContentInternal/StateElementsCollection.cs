using Microsoft.AspNetCore.Components;

namespace BlazorCssTransitions.AnimatedContentInternal;

internal class StateElementsCollection<TState>
{
    private readonly List<Element> _elementsCollection = [];
    private Element? _targetElement;
    private int _currentKeyCounter = 0;
    private int _currentOrderCounter = 0;

    internal IElementForRead? TargetElement => _targetElement;
    internal bool EqualsCurrentTarget(TState? state)
    {
        if (_targetElement is null)
            return false;

        return CompareStates(_targetElement.State, state);
    }

    internal void AppendNewTarget(TState? targetState)
    {
        var newTarget = new Element
        {
            Key = _currentKeyCounter++,
            State = targetState,
            Order = _currentOrderCounter++
        };
        _elementsCollection.Add(newTarget);
        _targetElement = newTarget;
    }

    internal void AppendNewOrReuseTarget(TState? targetState)
    {
        var existingElement = _elementsCollection.FirstOrDefault(x => CompareStates(x.State, targetState));

        if (existingElement is null)
        {
            AppendNewTarget(targetState);
            return;
        }

        existingElement.Order = _currentOrderCounter++;
        _targetElement = existingElement;
    }

    internal void Remove(IElement elementToRemove)
    {
        if (elementToRemove is not Element castedElementToRemove)
            throw new Exception("Mistake. Element of a type different that internal Element can not be used.");

        if (castedElementToRemove == _targetElement)
            throw new Exception("Target element can not be removed");

        _elementsCollection.Remove(castedElementToRemove);
    }

    internal IEnumerable<IElementForRead> ListForRead(bool fromTarget)
    {
        return fromTarget
            ? _elementsCollection.AsEnumerable().Reverse()
            : _elementsCollection;
    }

    internal int GetZIndexForElement(IElement element, bool descendingFromTarget)
    {
        if (element is not Element castedElement)
            throw new Exception("Mistake. Element of different type that internal Element can not be used.");

        if (castedElement.Order is null)
            throw new Exception("Order has not been assigned.");

        return descendingFromTarget
            ? castedElement.Order.Value
            : _currentOrderCounter - castedElement.Order.Value;
    }

    internal IEnumerable<EditionGroup> EnumerateForWrite(bool usingOrder)
    {
        var orderedElements = usingOrder
            ? _elementsCollection.OrderByDescending(x => x.Order).ToArray()
            : _elementsCollection.AsEnumerable().Reverse().ToArray();

        for (int index = 0; index < orderedElements.Length; index++)
        {
            var current = orderedElements[index];

            current.Visible = current == _targetElement;

            yield return new EditionGroup(
                Newer: index + 1 >= orderedElements.Length ? null : orderedElements[index + 1],
                Current: current,
                Older: index <= 0 ? null : orderedElements[index - 1]
            );
        }
    }

    /// <param name="Newer">Closer to the target</param>
    /// <param name="Current"></param>
    /// <param name="Older">Further from the target</param>
    internal record EditionGroup(
        IElementForWrite? Newer,
        IElementForWrite Current,
        IElementForWrite? Older);

    internal interface IElement { }

    internal interface IElementForRead : IElement
    {
        int Key { get; }
        TState? State { get; }
        bool Visible { get; }
        EnterTransition? Enter { get; }
        ExitTransition? Exit { get; }
        RenderFragment? Fragment { get; }
        EventCallback<AnimatedVisibility.State> OnStateChanged { get; }
        bool StartWithTransition { get; }
    }

    internal interface IElementForWrite : IElement
    {
        int Key { get; }
        TState? State { get; }
        /// <summary> Managed internally by the collection </summary>
        bool Visible { get; }
        /// <summary> Managed internally by the collection </summary>
        int? Order { get; }
        EnterTransition? Enter { get; }
        bool IsEnterCached { get; }
        void SetEnterIfNotCached(EnterTransition? enter, bool cache);
        ExitTransition? Exit { get; }
        bool IsExitCached { get; }
        void SetExitIfNotCached(ExitTransition? exit, bool cache);
        RenderFragment? Fragment { get; set; }
        EventCallback<AnimatedVisibility.State> OnStateChanged { get; set; }
        bool StartWithTransition { get; set; }
    }

    private class Element : IElementForRead, IElementForWrite
    {
        public required int Key { get; init; }
        public required TState? State { get; init; }
        public bool Visible { get; set; }
        public EnterTransition? Enter { get; set; }
        public bool IsEnterCached { get; set; }
        public ExitTransition? Exit { get; set; }
        public bool IsExitCached { get; set; }
        public bool StartWithTransition { get; set; }
        public EventCallback<AnimatedVisibility.State> OnStateChanged { get; set; }
        public RenderFragment? Fragment { get; set; }
        public int? Order { get; set; }

        public void SetEnterIfNotCached(EnterTransition? enter, bool cache)
        {
            if (IsEnterCached)
                return;
            Enter = enter;
            IsEnterCached = cache;
        }

        public void SetExitIfNotCached(ExitTransition? exit, bool cache)
        {
            if (IsExitCached)
                return;
            Exit = exit;
            IsExitCached = cache;
        }
    }


    private static bool CompareStates(TState? state1, TState? state2)
        => EqualityComparer<TState?>.Default.Equals(state1, state2);
}
