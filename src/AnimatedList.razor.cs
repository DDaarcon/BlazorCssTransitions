using BlazorCssTransitions.AnimatedListInternal;
using Microsoft.AspNetCore.Components;

namespace BlazorCssTransitions;

public partial class AnimatedList
{
    [Inject]
    internal ItemsCollection _itemsCollection { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public EnterTransition? ItemEnter { get; set; }
    [Parameter]
    public ExitTransition? ItemExit { get; set; }


    private IEnumerable<Item> Items { get; set; } = [];


    protected override async Task OnParametersSetAsync()
    {
        Items = await _itemsCollection.ProcessItems(ChildContentWrapper);
    }

    private void OnElementHided(string key)
    {
        Items = _itemsCollection.ProcessItemDeletionByKey(key);
    }
}
