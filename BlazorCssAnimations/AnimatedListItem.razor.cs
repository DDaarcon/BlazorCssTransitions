using BlazorCssAnimations.AnimatedListInternal;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssAnimations;

public partial class AnimatedListItem : ComponentBase
{
    [Parameter, EditorRequired]
    public required string Key { get; set; }

    [Parameter, EditorRequired]
    public required RenderFragment ChildContent { get; set; }

    [CascadingParameter]
    internal ItemsCollection? Colleciton { get; set; }

    protected override void OnParametersSet()
    {
        if (Colleciton is null)
        {
            throw new Exception("AnimatedListItem must be rendered as a child of AnimatedList");
        }

        Colleciton.AddItem(Key, ChildContent);
    }
}
