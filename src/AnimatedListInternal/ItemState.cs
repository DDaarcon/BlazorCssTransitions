using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedListInternal;

public class ItemState<TItem>
    where TItem : IAnimatedListItemModel
{
    public required TItem Model { get; init; }
    public double? Height { get; set; }
    public double? TopOffset { get; set; }
    public bool IsBeingRemoved { get; set; }
    public TItem? ReplacementTarget { get; set; }
}
