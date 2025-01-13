using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public interface IAnimatedListItemModel : IEquatable<IAnimatedListItemModel>
{
    public string Key { get; }
}

public abstract class AnimatedListItemModel : IAnimatedListItemModel
{
    public required string Key { get; init; }

    public bool Equals(IAnimatedListItemModel? other)
        => other?.Key == Key;
}
