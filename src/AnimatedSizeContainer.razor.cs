using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.JsInterop;
using BlazorCssTransitions.Shared.SizeMeasurement;
using BlazorCssTransitions.Specifications;
using Microsoft.AspNetCore.Components;

namespace BlazorCssTransitions;

public partial class AnimatedSizeContainer : IAsyncDisposable
{
    [Inject]
    private JsSizeMeter _sizeMeter { get; init; } = default!;
    [Inject]
    private JsSizeObserver _sizeObserver { get; init; } = default!;
    [Inject]
    private ITimerService _timerService { get; init; } = default!;

    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    [Parameter]
    public Spec? Spec { get; set; }
    private Spec _spec = default!;
    private readonly static Spec _defaultSpec = Spec.Linear();

    [Parameter]
    public bool FillWidth { get; set; } = false;
    [Parameter]
    public bool FillHeight { get; set; } = false;

    [Parameter]
    public string? Class { get; set; }

    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Whether should stop animating size changes (instant changes).
    /// </summary>
    [Parameter]
    public bool StopAnimating { get; set; } = false;

    /// <summary>
    /// Predicate returning whether should animate size changes.
    /// </summary>
    [Parameter]
    public Func<Dimensions, bool>? ShouldAnimate { get; set; }

    public readonly record struct Dimensions(
        double Height,
        double Width);

    [Parameter]
    public EventCallback OnResized { get; set; }

    /// <summary>
    /// Forces recalculation
    /// </summary>
    public async Task Recalculate()
    {
        var maskSize = await _sizeMeter.MeasureElementScroll(MaskReference);

        UpdateTargetSize(maskSize);
    }


    private ElementReference MaskReference { get; set; }

    private string ContainerClass
    {
        get
        {
            IEnumerable<string> classes = [_containerClass];
            if (FillHeight)
                classes = classes.Append(_fillHeightClass);
            if (FillWidth)
                classes = classes.Append(_fillWidthClass);
            if (!String.IsNullOrEmpty(Class))
                classes = classes.Append(Class);

            return String.Join(" ", classes);
        }
    }
    private const string _containerClass = "animated-size-container";
    private const string _fillWidthClass = "fill-width";
    private const string _fillHeightClass = "fill-height";

    private string ContainerStyle
    {
        get
        {
            IEnumerable<string> styles = [_spec.GetStyle(animatedProperty: "all")];

            var shouldAnimateByPredicate = ShouldAnimate?.Invoke(new Dimensions(_contentHeight, _contentWidth))
                ?? true;

            if (!FillHeight && _afterFirstRender && !StopAnimating && shouldAnimateByPredicate)
                styles = styles.Append($"height: {_contentHeight.ToCss()}px;");
            if (!FillWidth && _afterFirstRender && !StopAnimating && shouldAnimateByPredicate)
                styles = styles.Append($"width: {_contentWidth.ToCss()}px;");
            if (!String.IsNullOrEmpty(Style))
                styles = styles.Append(Style);

            return String.Join(" ", styles);
        }
    }

    private bool _afterFirstRender;

    private IAsyncDisposable? _resizeListener;

    protected override void OnParametersSet()
    {
        _spec = Spec ?? _defaultSpec;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        _resizeListener = await _sizeObserver.ListenForElementSizeChanges(MaskReference, async size =>
        {
            await InvokeAsync(() =>
            {
                UpdateTargetSize(size);
            });
        });
        
        _afterFirstRender = true;
        await Recalculate();
    }


    private double _contentHeight;
    private double _contentWidth;
    private void UpdateTargetSize(DOMScrollRect maskSize)
    {
        if (_contentHeight == maskSize.Height
            && _contentWidth == maskSize.Width)
        {
            return;
        }

        _contentHeight = maskSize.Height;
        _contentWidth = maskSize.Width;
        StateHasChanged();

        NotifyOnResizeFinish();
    }


    private ITimerService.ITimerRegistration? _transitionTimer;

    private void NotifyOnResizeFinish()
    {
        _timerService.StartNew(_spec.GetTotalDuration(), 
            actionToExecute: () =>
            {
                InvokeAsync(() =>
                {
                    OnResized.InvokeAsync();
                });
            }, 
            caller: this,
            oldRegistration: _transitionTimer);
    }

    public async ValueTask DisposeAsync()
    {
        _transitionTimer?.Abort();

        if (_resizeListener is not null)
            await _resizeListener.DisposeAsync();
    }
}
