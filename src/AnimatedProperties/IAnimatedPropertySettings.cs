using BlazorCssTransitions.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedProperties;

public interface IAnimatedPropertySettings<TThis, TReadyThis, TRegistration>
	where TReadyThis : IAnimatedPropertyReadyToRegister<TRegistration>
{
	TReadyThis WithSpec(Spec spec);
	TThis WithIterationCount(int count);
	TThis WithDirection(AnimationDirection direction);
	TThis WithFillMode(AnimationFillMode fillMode);
	TReadyThis With(
		Spec spec,
		int iterCount = AnimatedProperty.DefaultIterationCount,
		AnimationDirection direction = AnimatedProperty.DefaultDirection,
		AnimationFillMode fillMode = AnimatedProperty.DefaultFillMode);
}
