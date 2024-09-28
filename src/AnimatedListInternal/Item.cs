using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedListInternal;
internal readonly struct Item
{
    public required string Key { get; init; }
    public required bool ShouldStayVisible { get; init; }
    public required RenderFragment Fragment { get; init; }
}
