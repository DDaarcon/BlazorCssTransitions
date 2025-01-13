using BlazorCssTransitions.AnimatedListInternal;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public partial class AnimatedListItem : ComponentBase
{
    [Parameter, EditorRequired]
    public required string Key { get; set; }

    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    [CascadingParameter]
    internal ItemsCollection2? Colleciton { get; set; }

    protected override void OnParametersSet()
    {
        if (Colleciton is null)
        {
            throw new Exception("AnimatedListItem must be rendered as a child of AnimatedList");
        }

        Colleciton.AddItem(Key, ChildContent);
    }
}
