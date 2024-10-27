using BlazorCssTransitions.Shared;
using BlazorCssTransitions.Shared.JsInterop;
using BlazorCssTransitions.Shared.SizeMeasurement;
using BlazorCssTransitions.Specifications;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public partial class AnimatedSizeContainer : IAsyncDisposable
{
    [Inject]
    private JsSizeMeter _sizeMeter { get; init; } = default!;
    [Inject]
    private JsSizeObserver _sizeObserver { get; init; } = default!;

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

    [Parameter]
    public bool StopResizing { get; set; } = false;

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
            if (!FillHeight && _afterFirstRender && !StopResizing)
                styles = styles.Append($"height: {_containerHeight.ToCss()}px;");
            if (!FillWidth && _afterFirstRender && !StopResizing)
                styles = styles.Append($"width: {_containerWidth.ToCss()}px;");
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


    private double _containerHeight;
    private double _containerWidth;
    private void UpdateTargetSize(DOMScrollRect maskSize)
    {
        if (_containerHeight == maskSize.Height
            && _containerWidth == maskSize.Width)
        {
            return;
        }

        _containerHeight = maskSize.Height;
        _containerWidth = maskSize.Width;
        StateHasChanged();

        NotifyOnResizeFinish();
    }


    private System.Timers.Timer? _transitionTimer;

    private void NotifyOnResizeFinish()
    {
        _transitionTimer?.Dispose();
        _transitionTimer = TimerHelper.StartNewOneTimeTimer(_spec.GetTotalDuration(), () =>
        {
            InvokeAsync(() =>
            {
                OnResized.InvokeAsync();
            });
        });
    }

    public void Dispose()
    {
        _transitionTimer?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        _transitionTimer?.Dispose();

        if (_resizeListener is not null)
            await _resizeListener.DisposeAsync();
    }
}
