namespace BlazorCssTransitions.UnitTests.Tests.AnimatedContent;

internal class SimpleElementsInitializationCounterService
{
    public int Count { get; private set; }

    public void OnInitialized()
        => Count++;
}
