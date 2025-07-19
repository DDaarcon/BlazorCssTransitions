using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.CssStylesValidation;
using BlazorCssTransitions.Shared.JsInterop;
using BlazorCssTransitions.Shared.SizeMeasurement;
using BlazorCssTransitions.Specifications;
using Microsoft.AspNetCore.Components;

namespace BlazorCssTransitions;

// TODO write tests for CollapseVertically and CollapseHorizontally
public partial class AnimatedSizeContainer : IAsyncDisposable
{
    [Inject]
    private JsSizeMeter _sizeMeter { get; init; } = default!;
    [Inject]
    private JsSizeObserver _sizeObserver { get; init; } = default!;
    [Inject]
    private ITimerService _timerService { get; init; } = default!;
    [Inject]
    private ICssStylesAppliedValidator _cssStylesAppliedValidator { get; init; } = default!;

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
    private bool _stopAnimating;

    /// <summary>
    /// Predicate returning whether should animate size changes.
    /// </summary>
    [Parameter]
    public Func<Dimensions, bool>? ShouldAnimate { get; set; }

    /// <summary>
    /// Whether contaienr height should transition to 0. Is ignored when <see cref="FillHeight"/> is <c>true</c>.
    /// </summary>
    [Parameter]
    public bool CollapseVertically { get; set; } = false;
    private bool _collapseVertically;

    /// <summary>
    /// Whether contaienr width should transition to 0. Is ignored when <see cref="FillWidth"/> is <c>true</c>.
    /// </summary>
    [Parameter]
    public bool CollapseHorizontally { get; set; } = false;
    private bool _collapseHorizontally;

    public readonly record struct Dimensions(
        double Height,
        double Width);

    [Parameter]
    public EventCallback OnResized { get; set; }

    /// <summary>
    /// Read-only property indicating whether the container is currently animating. <br />
    /// * This is useful for preventing multiple animations from being triggered at the same time.
    /// </summary>
    public bool IsAnimating { get; private set; }

    /// <summary>
    /// Forces recalculation
    /// </summary>
    public async Task Recalculate()
    {
        var maskSize = await _sizeMeter.MeasureElementScroll(MaskReference);

        UpdateContentSize(maskSize);
    }


    private ElementReference ContainerReference { get; set; }
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

    private double _targetHeightPx = 0d;
    private double _targetWidthPx = 0d;

    private string ContainerStyle => GetContainerStyle();

    private bool _isAfterFirstRender;
    // Some updates require the StopAnimaing change to be already applied to properly apply animations.
    private bool _isRespondToExternalSizeChangeRequestScheduledAfterRender;
    private bool _isWaitingForRequiredStylesToBeApplied;

    private IAsyncDisposable? _resizeListener;

    protected override void OnParametersSet()
    {
        _spec = Spec ?? _defaultSpec;

        bool shouldRespondToExternalSizeChangeRequestImmediately = true;

        if (_stopAnimating != StopAnimating)
        {
            if (!StopAnimating)
            {
                shouldRespondToExternalSizeChangeRequestImmediately = false;
                _isRespondToExternalSizeChangeRequestScheduledAfterRender = true;
            }
            _stopAnimating = StopAnimating;
        }

        if (shouldRespondToExternalSizeChangeRequestImmediately)
            RespondToExternalSizeChangeRequest();
    }

    private void RespondToExternalSizeChangeRequest()
    {
        if (_collapseHorizontally != CollapseHorizontally
            || _collapseVertically != CollapseVertically)
        {
            _collapseHorizontally = CollapseHorizontally;
            _collapseVertically = CollapseVertically;

            UpdateTargetSize();
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_isRespondToExternalSizeChangeRequestScheduledAfterRender)
        {
            _isRespondToExternalSizeChangeRequestScheduledAfterRender = false;

            _isWaitingForRequiredStylesToBeApplied = true;
            await _cssStylesAppliedValidator.EnsureStylesWereApplied(ContainerReference);
            _isWaitingForRequiredStylesToBeApplied = false;

            RespondToExternalSizeChangeRequest();
        }

        if (!firstRender)
            return;

        // only first render

        _resizeListener = await _sizeObserver.ListenForElementSizeChanges(MaskReference, async size =>
        {
            await InvokeAsync(() =>
            {
                UpdateContentSize(size);
            });
        });
        
        _isAfterFirstRender = true;
        await Recalculate();
    }

    protected override bool ShouldRender()
    {
        return !_isWaitingForRequiredStylesToBeApplied;
    }


    private double _contentHeightPx;
    private double _contentWidthPx;
    private void UpdateContentSize(DOMScrollRect maskSize)
    {
        if (_contentHeightPx == maskSize.Height
            && _contentWidthPx == maskSize.Width)
        {
            return;
        }

        _contentHeightPx = maskSize.Height;
        _contentWidthPx = maskSize.Width;

        UpdateTargetSize();
    }

    private void UpdateTargetSize()
    {
        bool hasTargetSizeBeenUpdated = false;
        if (!FillHeight)
        {
            if (_collapseVertically)
            {
                if (_targetHeightPx != 0)
                {
                    _targetHeightPx = 0;
                    hasTargetSizeBeenUpdated = true;
                }
            }
            else if (_contentHeightPx != _targetHeightPx)
            {
                _targetHeightPx = _contentHeightPx;
                hasTargetSizeBeenUpdated = true;
            }
        }

        if (!FillWidth)
        {
            if (_collapseHorizontally)
            {
                if (_targetWidthPx != 0)
                {
                    _targetWidthPx = 0;
                    hasTargetSizeBeenUpdated = true;
                }
            }
            else if (_contentWidthPx != _targetWidthPx)
            {
                _targetWidthPx = _contentWidthPx;
                hasTargetSizeBeenUpdated = true;
            }
        }

        if (hasTargetSizeBeenUpdated)
        {
            IsAnimating = true;
            NotifyOnResizeFinish();

            StateHasChanged();
        }
    }

    // TODO caching?
    private string GetContainerStyle()
    {
        IEnumerable<string> styles = [_spec.GetStyle(animatedProperty: "all")];

        var shouldAnimateByPredicate = ShouldAnimate?.Invoke(new Dimensions(_targetHeightPx, _targetWidthPx))
            ?? true;

        bool enableAdjustingSizeToMask = _isAfterFirstRender
            && !_stopAnimating
            && shouldAnimateByPredicate;

        if (!FillHeight
            && (CollapseVertically || enableAdjustingSizeToMask))
        {
            styles = styles.Append($"height: {_targetHeightPx.ToCss()}px;");
        }

        if (!FillWidth
            && (CollapseHorizontally || enableAdjustingSizeToMask))
        {
            styles = styles.Append($"width: {_targetWidthPx.ToCss()}px;");
        }

        if (!String.IsNullOrEmpty(Style))
            styles = styles.Append(Style);

        return String.Join(" ", styles);
    }

    private ITimerService.ITimerRegistration? _transitionTimer;

    private void NotifyOnResizeFinish()
    {
        _transitionTimer = _timerService.StartNew(_spec.GetTotalDuration(), 
            actionToExecute: () =>
            {
                InvokeAsync(() =>
                {
                    IsAnimating = false;
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
