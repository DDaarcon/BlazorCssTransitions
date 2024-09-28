﻿using BlazorCssTransitions.Help;
using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedVisibilityTransitions;

internal class ExpandEnterTransition : BaseSpecificEnterTransition
{
    public ExpandEnterTransition(Specification spec, float startScaleX, float startScaleY, float finishScaleX, float finishScaleY)
    {
        TransitionAllTimeStyle = $"""
            --start-expand-scale-x: {startScaleX.ToCss()};
            --start-expand-scale-y: {startScaleY.ToCss()};
            --finish-expand-scale-x: {finishScaleX.ToCss()};
            --finish-expand-scale-y: {finishScaleY.ToCss()};
            """;
        TransitionInitialClass = _initialClass;
        TransitionFinishClass = _finishClass;
        Specification = spec;
        TransitionedProperty = "scale";
    }

    private const string _initialClass = "expand-animation-start";
    private const string _finishClass = "expand-animation-finish";

    protected override BaseSpecificEnterTransition ShallowClone()
        => (BaseSpecificEnterTransition)MemberwiseClone();
}