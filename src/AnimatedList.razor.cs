using BlazorCssTransitions.AnimatedListInternal;
using BlazorCssTransitions.AnimatedVisibilityTransitions;
using BlazorCssTransitions.Help;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
