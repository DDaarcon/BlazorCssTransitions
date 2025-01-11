using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;
internal abstract class BaseSpecificTransition<TTransition> : BaseTransition
    where TTransition : IBaseTransition
{
    protected BaseSpecificTransition() { }

    public static implicit operator TTransition(BaseSpecificTransition<TTransition> @this)
        => (TTransition)@this;


    private string? _transitionedProperty;
    internal string TransitionedProperty
    {
        get => Assertions.AssertNotNullAndGet(_transitionedProperty, "Transitioned property must be set before transition is used.");
        private protected set => _transitionedProperty = value;
    }

    private Spec? _specification;
    internal Spec Specification
    {
        get => Assertions.AssertNotNullAndGet(_specification, "Animation specification must be set before transition is used.");
        private protected set => _specification = value;
    }


    internal string? TransitionAllTimeStyle { get; private protected set; }

    internal string? TransitionInitialStyle { get; private protected set; }

    internal override string GetInitialStyle()
    {
        return $"""
            {TransitionAllTimeStyle ?? ""}
            {TransitionInitialStyle ?? ""}
            {Specification.GetStyle(TransitionedProperty)}
            """;
    }


    internal string? TransitionInitialClass { get; private protected set; }

    internal override string GetInitialClasses()
    {
        return $"{TransitionInitialClass ?? ""}";
    }



    internal string? TransitionFinishStyle { get; private protected set; }

    internal override string GetFinishStyle()
    {
        return $"""
            {TransitionAllTimeStyle ?? ""}
            {TransitionFinishStyle ?? ""}
            {Specification.GetStyle(TransitionedProperty)}
            """;
    }

    internal override string GetFinishedStyle()
    {
        return $"""
            {TransitionAllTimeStyle ?? ""}
            {TransitionFinishStyle ?? ""}
            """;
    }


    internal string? TransitionFinishClass { get; private protected set; }

    internal override string GetFinishClasses()
    {
        return $"{TransitionFinishClass ?? ""}";
    }

    internal string GetInitialStyleWithoutSpecification()
    {
        return $"""
            {TransitionAllTimeStyle ?? ""}
            {TransitionInitialStyle ?? ""}
            """;
    }

    internal string GetFinishStyleWithoutSpecification()
    {
        return $"""
            {TransitionAllTimeStyle ?? ""}
            {TransitionFinishStyle ?? ""}
            """;
    }

    internal override IEnumerable<Spec> GetSpecifications()
        => [Specification];
}
