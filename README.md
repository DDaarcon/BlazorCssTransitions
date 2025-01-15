# BlazorCssTransitions
### Work in progress
## Introduction
Library simplifies adding animations to blazor apps. Using C#/Blazor code create animations of visibility, content change, size change and other (TBA).

The API was inspired by Jetpack Compose's (kotlin) animations API.

## Usage
- Transition descriptors

To compose a enter/exit transition you should use interfaces `EnterTransition` and `ExitTransition`. Transition's timing is defined with `Spec`.

Example:
```c#
private readonly static EnterTransition _myElementEnterTrans
  = EnterTransition.SlideInVertically(Spec.EaseOut(TimeSpan.FromMilliseconds(200)), initialY: "20px")
    + EnterTransition.FadeIn(Spec.EaseIn(TimeSpan.FromMilliseconds(200)));

private readonly static ExitTransition _myElementExitTrans
  = ExitTransition.SlideOutHorizontally(Spec.EaseIn(TimeSpan.FromMilliseconds(200), delay: TimeSpan.FromMilliseconds(300)), finishX: "-20px")
    + ExitTransition.FadeOut(Spec.EaseIn(TimeSpan.FromMilliseconds(200), delay: TimeSpan.FromMilliseconds(300)));
```

- Animated visibility

Blazor component `AnimatedVisibility` can be used to animate entrance and exit of content.

Example:
```razor
<AnimatedVisibility Visible=@(true)
                    Enter=@_myElementEnterTrans
                    Exit=@_myElementExitTrans
                    StartWithTransition
                    OnHidden=@(() => {})
                    OnShown=@(() => {})
                    Style="height: 100%"
                    Class="process-animation">
    <div> MY ELEMENT </div>
</AnimatedVisibility>
```

- Animated content

Blazor component `AnimatedContent` can be used to animate changes of content.

Example:
```razor
<AnimatedContent TState="bool"
                 TargetState=@(true)
                 TransitionsProvider=@GetTransitions>
    @if (context)
    {
       <span> YES! </span>
    }


    else
    {
        <span> NO! </span>
    }
</AnimatedContent>

@code {
    private AnimatedContent<bool>.InterstateTransitions? GetTransitions(bool fromState, bool toState)
    {
        if (fromState)
            return GetTransitionsWithOffset("40px", "-40px");
        if (!fromState)
            return GetTransitionsWithOffset("-40px", "40px");

        return null; // null can be returned when no transitions should be applied

        static AnimatedContent<bool>.InterstateTransitions GetTransitionsWithOffset(CssLength offsetEnter, CssLength offsetExit)
        {
            return new AnimatedContent<bool>.InterstateTransitions
            {
                FromPreviousStateEnter = EnterTransition.FadeIn(Spec.EaseOut(TimeSpan.FromMilliseconds(150)))
                    + EnterTransition.SlideInHorizontally(Spec.EaseOut(TimeSpan.FromMilliseconds(150)), initialX: offsetEnter),
                ToNextStateExit = ExitTransition.FadeOut(Spec.EaseIn(TimeSpan.FromMilliseconds(150)))
                    + ExitTransition.SlideOutHorizontally(Spec.EaseIn(TimeSpan.FromMilliseconds(150)), finishX: offsetExit)
            };
        }
    }
}
```

- Animated size container

Blazor component `AnimatedSizeContainer` can be used to animate size changes.

Example:
```razor
<AnimatedSizeContainer Spec=@Spec.EaseInOut(TimeSpan.FromMilliseconds(100))
                       FillWidth
                       OnResized=@(() => {})>
  <span> Element that can change it's content </span>
</AnimatedSizeContainer>
```
