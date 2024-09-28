using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal abstract class BaseCombinedTransition<TTransition, TSpecificTransition> : BaseTransition
    where TTransition : IBaseTransition
    where TSpecificTransition : BaseSpecificTransition<TTransition>
{
    protected readonly ImmutableArray<TSpecificTransition> _transitions;

    public BaseCombinedTransition(IEnumerable<TSpecificTransition> transitions)
    {
        _transitions = transitions.ToImmutableArray();
    }

    public BaseCombinedTransition(TTransition firstTransition, TTransition secondTransition)
    {
        _transitions = GetTransitionsCollectionFromTransition(firstTransition)
            .Concat(GetTransitionsCollectionFromTransition(secondTransition))
            .ToImmutableArray();
    }

    protected static IEnumerable<TSpecificTransition> GetTransitionsCollectionFromTransition(TTransition transition)
        => transition switch
        {
            BaseCombinedTransition<TTransition, TSpecificTransition> anotherCombinedTransition => anotherCombinedTransition._transitions,
            TSpecificTransition anotherSpecificTransition => [anotherSpecificTransition],
            _ => []
        };

    internal override string GetFinishClasses()
        => String.Join(" ", _transitions.Select(x => x.GetFinishClasses()));

    internal override string GetFinishedStyle()
        => $"""
        {String.Join("\n", _transitions.Select(x => x.GetFinishStyleWithoutSpecification()))}
        """;

    internal override string GetFinishStyle()
        => $"""
        {String.Join("\n", _transitions.Select(x => x.GetFinishStyleWithoutSpecification()))}
        transition: {String.Join(", ", _transitions.Select(x => x.Specification.GetTransitionValue(x.TransitionedProperty)))};
        """;

    internal override string GetInitialClasses()
        => String.Join(" ", _transitions.Select(x => x.GetInitialClasses()));

    internal override string GetInitialStyle()
        => $"""
        {String.Join("\n", _transitions.Select(x => x.GetInitialStyleWithoutSpecification()))}
        transition: {String.Join(", ", _transitions.Select(x => x.Specification.GetTransitionValue(x.TransitionedProperty)))};
        """;

    internal override IEnumerable<Specification> GetSpecifications()
        => _transitions.Select(x => x.Specification);
}
