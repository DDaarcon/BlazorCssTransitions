using BlazorCssTransitions.Shared.JsInterop;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedListInternal;

public partial class AnimatedListItem<TItem>
    where TItem : IAnimatedListItemModel
{
    [Inject]
    private JsSizeMeter _jsSizeMeter { get; init; } = default!;

    [CascadingParameter]
    private ItemsCollection<TItem> Collection { get; set; } = default!;

    [Parameter, EditorRequired]
    public required ItemState<TItem> Model { get; init; }

    private bool IsFirstRender { get; set; } = true;

    private ElementReference ItemContainer { get; set; }

    protected override void OnInitialized()
    {
        ItemContainer.
        if (Collection is null)
        {
            throw new Exception("Component AnimatedListItem should only be used internally inside AnimatedList");
        }
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            IsFirstRender = false;
        }
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task MeasureSize()
    {
        var size = await _jsSizeMeter.MeasureElementScroll(ItemContainer);
        size.
    }
}
