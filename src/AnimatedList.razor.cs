using BlazorCssTransitions.AnimatedListInternal;
using BlazorCssTransitions.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions;

public partial class AnimatedList<TItem>
    where TItem : IAnimatedListItemModel
{
    [Parameter, EditorRequired]
    public required RenderFragment<TItem> Template { get; set; }

    [Parameter]
    public IEnumerable<TItem>? Items { get; set; }
    private IEnumerable<TItem>? _itemsBF;


    private readonly ItemsCollection<TItem> _itemsCollection = new(Rerender));

    private List<ItemState>? _itemsState;

    private class ItemState
    {
        public required TItem Model { get; init; }
        public double? Height { get; set; }
        public double? TopOffset { get; set; }
        public bool IsBeingRemoved { get; set; }
    }

    protected override Task OnParametersSetAsync()
    {
        if (_itemsBF != Items)
        {
            _itemsBF = Items;
        }
    }

    private async Task Rerender()
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    private ItemsModificationType DetermineItemsModificationType()
    {
        if ((_itemsBF is null || !_itemsBF.Any())
            && _itemsState is { Count: > 0 })
        {
            return ItemsModificationType.OnlyRemovals;
        }

        if (_itemsBF is not null && _itemsBF.Any()
            && _itemsState is not { Count: > 0 })
        {
            return ItemsModificationType.OnlyAdditions;
        }

        if ((_itemsBF is null || !_itemsBF.Any())
            && _itemsState is not { Count: > 0 })
        {
            return ItemsModificationType.NoChange;
        }

        var newItems = _itemsBF!.ToArray();
        var smallerAmountOfItems = Math.Min(newItems.Length, _itemsState!.Count);

        //TODO maybe construct dictionaries?

        bool isThereAnyReposition = false;
        bool isThereAnyAddition = false;
        bool isThereAnyDeletion = false;
        bool isThereAnyReplacement = false;

        for (int i = 0; i < smallerAmountOfItems; i++)
        {
            var modelForNewItem = newItems[i];
            var modelForOldItem = _itemsState[i].Model;

            if (modelForNewItem.Equals(modelForOldItem))
            {
                continue; // elements match, no change
            }

            var doesModelForNewItemExistInOldList = _itemsState.Any(x => x.Model.Equals(modelForNewItem));
            var doesModelForOldItemExistInNewList = newItems.Contains(modelForOldItem);

            if (doesModelForNewItemExistInOldList
                && doesModelForOldItemExistInNewList)
            {
                isThereAnyReposition = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
                continue;
            }

            if (doesModelForNewItemExistInOldList)
            {
                isThereAnyDeletion = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
                continue;
            }

            if (doesModelForOldItemExistInNewList)
            {
                isThereAnyAddition = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
                continue;
            }

            isThereAnyReplacement = true;
            if (IsComplexChange())
                return ItemsModificationType.ComplexChange;
        }

        foreach (var item 
            in newItems.Skip(smallerAmountOfItems)
                .Concat(_itemsState.Skip(smallerAmountOfItems).Select(x => x.Model)))
        {

        }

        if (smallerAmountOfItems < newItems.Length)
        {
            isThereAnyAddition = true;
            if (IsComplexChange())
                return ItemsModificationType.ComplexChange;
        }

        if (smallerAmountOfItems < _itemsState.Count)
        {
            isThereAnyDeletion = true;
            if (IsComplexChange())
                return ItemsModificationType.ComplexChange;
        }

        bool IsComplexChange()
            => isThereAnyAddition.ToInt() + isThereAnyDeletion.ToInt() + isThereAnyReplacement.ToInt() + isThereAnyReposition.ToInt() > 2;

        if (isThereAnyAddition)
            return ItemsModificationType.OnlyAdditions;
        if (isThereAnyDeletion)
            return ItemsModificationType.OnlyRemovals;
        if (isThereAnyReplacement)
            return ItemsModificationType.OnlyReplacements;
        if (isThereAnyReposition)
            return ItemsModificationType.OnlyReposition;

        return ItemsModificationType.NoChange;
    }

    private void AdjustItemsStateFromParameterChange()
    {
        if (_itemsBF is null)
            return;

        _itemsState ??= new List<ItemState>();

        foreach (var itemState in _itemsState)
        {
            if (!_itemsBF.Contains(itemState.Model))
            {

            }
        }

        foreach (var item in _itemsBF)
        {

        }
    }

    private enum ItemsModificationType
    {
        NoChange,
        ComplexChange,
        OnlyRemovals,
        OnlyAdditions,
        OnlyReposition,
        OnlyReplacements
    }
}
