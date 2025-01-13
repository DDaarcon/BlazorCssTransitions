using BlazorCssTransitions.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedListInternal;

internal class ItemsCollection<TItem>(Func<Task> onItemsChanged)
    where TItem : IAnimatedListItemModel
{
    private readonly Func<Task> _onItemsChanged = onItemsChanged;

    public IEnumerable<ItemState<TItem>>? ItemsState => _itemsState;
    private List<ItemState<TItem>>? _itemsState;

    public async Task ProcessItemModels(IEnumerable<TItem>? items)
    {
        var itemsListed = items?.ToList();

        EnsureNoDuplicateKeys(itemsListed);

    }


    private static void EnsureNoDuplicateKeys(IList<TItem>? items)
    {
        if (items is null)
            return;

        for (int i = 1; i < items.Count; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (items[i].Key == items[j].Key)
                    throw new Exception($"There is a duplicate key \"{items[i].Key}\" in AnimatedList items");
            }
        }
    }


    private void AdjustItemsState(IList<TItem>? items)
    {
        var modificationType = DetermineItemsModificationType(items);

        if (modificationType is ItemsModificationType.NoChange)
            return;

        if (modificationType is ItemsModificationType.OnlyAdditions)
        {

        }

        _itemsState ??= new List<ItemState<TItem>>();

        foreach (var itemState in _itemsState)
        {
            if (!items.Contains(itemState.Model))
            {

            }
        }

        foreach (var item in items)
        {

        }
    }


    private ItemsModificationType DetermineItemsModificationType(IList<TItem>? items)
    {
        if (items is not { Count: > 0 }
            && _itemsState is { Count: > 0 })
        {
            return ItemsModificationType.OnlyRemovals;
        }

        if (items is { Count: > 0 }
            && _itemsState is not { Count: > 0 })
        {
            return ItemsModificationType.OnlyAdditions;
        }

        if (items is not { Count: > 0 }
            && _itemsState is not { Count: > 0 })
        {
            return ItemsModificationType.NoChange;
        }

        var smallerAmountOfItems = Math.Min(items!.Count, _itemsState!.Count);

        //TODO maybe construct dictionaries?

        bool isThereAnyReposition = false;
        bool isThereAnyAddition = false;
        bool isThereAnyDeletion = false;
        bool isThereAnyReplacement = false;

        for (int i = 0; i < smallerAmountOfItems; i++)
        {
            var modelForNewItem = items[i];
            var modelForOldItem = _itemsState[i].Model;

            if (modelForNewItem.Equals(modelForOldItem))
            {
                continue; // elements match, no change
            }

            bool doesModelForNewItemExistInOldList = _itemsState.Any(x => x.Model.Equals(modelForNewItem));
            bool doesModelForOldItemExistInNewList = items.Contains(modelForOldItem);

            if (doesModelForNewItemExistInOldList
                && doesModelForOldItemExistInNewList)
            {
                isThereAnyReposition = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
            }
            else if (doesModelForNewItemExistInOldList)
            {
                isThereAnyDeletion = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
            }
            else if (doesModelForOldItemExistInNewList)
            {
                isThereAnyAddition = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
            }
            else
            {
                isThereAnyReplacement = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange; 
            }
        }

        foreach (var item in items.Skip(smallerAmountOfItems))
        {
            bool doesModelForNewItemExistInOldList = _itemsState.Any(x => x.Model.Equals(item));

            if (doesModelForNewItemExistInOldList)
            {
                isThereAnyReposition = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
            }
            else
            {
                isThereAnyAddition = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
            }
        }

        foreach (var item in _itemsState.Skip(smallerAmountOfItems).Select(x => x.Model))
        {
            bool doesModelForOldItemExistInNewList = items.Contains(item);

            if (doesModelForOldItemExistInNewList)
            {
                isThereAnyReposition = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
            }
            else
            {
                isThereAnyDeletion = true;
                if (IsComplexChange())
                    return ItemsModificationType.ComplexChange;
            }
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

    private class DifferenceAnalysisResult
    {
        public required ItemsModificationType ModificationType { get; init; }
        public Index[]? OnlyRemovalIndexes { get; init; }
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
