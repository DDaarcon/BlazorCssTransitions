using BlazorCssTransitions.Help;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorCssTransitions.AnimatedListInternal;
internal class ItemsCollection(
    ExternalRenderer externalRenderer)
{
    private readonly ExternalRenderer _externalRenderer = externalRenderer;

    private readonly Dictionary<string, Item> _itemsByKey = [];
    /// <summary>
    /// Current keys in order that they have to be displayed
    /// </summary>
    private readonly List<string> _currentKeysInOrder = new();

    /// <summary>
    /// Keys gathered when items are added, in order they appeared in fragment
    /// </summary>
    private readonly List<string> _keysOfNewItems = new();
    /// <summary>
    /// Keys that were previously not present in the dictionary, in order they appeared in fragment
    /// </summary>
    private readonly List<string> _keysOfNewlyAddedNewItems = new();

    public async Task<IEnumerable<Item>> ProcessItems(RenderFragment fragmentWithItemsToProcess)
    {
        _keysOfNewItems.Clear();
        _keysOfNewlyAddedNewItems.Clear();
        string[] previousKeysInOrder = [.. _currentKeysInOrder];

        await _externalRenderer.Render(fragmentWithItemsToProcess);

        InitiateDisappearanceOfNoLongerPresentItems(previousKeysInOrder);

        SetKeysInOrder(previousKeysInOrder);

        return GetItemsOrdered();
    }

    public IEnumerable<Item> ProcessItemDeletionByKey(string key)
    {
        if (_itemsByKey.Remove(key))
        {
            _currentKeysInOrder.Remove(key);
        }

        return GetItemsOrdered();
    }


    internal void AddItem(string key, RenderFragment fragment)
    {
        _keysOfNewItems.Add(key);

        if (!_itemsByKey.ContainsKey(key))
            _keysOfNewlyAddedNewItems.Add(key);

        _itemsByKey[key] = new Item
        {
            Key = key,
            ShouldStayVisible = true,
            Fragment = fragment
        };
    }

    private void InitiateDisappearanceOfNoLongerPresentItems(string[] previousKeysInOrder)
    {
        if (previousKeysInOrder is not { Length: > 0})
        {
            return;
        }

        var removedElementKeys = previousKeysInOrder.Except(_keysOfNewItems);

        foreach (var key in removedElementKeys)
        {
            if (_itemsByKey.TryGetValue(key, out var item))
            {
                _itemsByKey[key] = new Item
                {
                    Key = item.Key,
                    ShouldStayVisible = false,
                    Fragment = item.Fragment
                };
            }
        }
    }

    private void SetKeysInOrder(string[] previousKeysInOrder)
    {
        if (_currentKeysInOrder.Count == 0)
        {
            _currentKeysInOrder.AddRange(_keysOfNewItems);
            return;
        }

        // TODO currently supported is mainly just insertion in the end and deletion
        _currentKeysInOrder.AddRange(_keysOfNewlyAddedNewItems);

        //int offsetInFinalKeys = 0;
        //int[] indexesOfNewlyInsertedKeys = [];
        //for (int newKeyIndex = 0; newKeyIndex < _keysOfNewItems.Count; newKeyIndex++)
        //{
        //    var key = _keysOfNewItems[newKeyIndex];
        //    int existsInUpdatedOrder = _currentKeysInOrder.IndexOf(key);

        //    if (existsInUpdatedOrder > 0)
        //    {
        //        offsetInFinalKeys = 
        //        continue;
        //    }

        //    _currentKeysInOrder.Insert(newKeyIndex)

        //    if (existsInUpdatedOrder > 0)
        //    {
        //        // key exists in the previous order
        //        _currentKeysInOrder.Insert()
        //    }
        //}
    }

    private List<Item> GetItemsOrdered()
    {
        var items = new List<Item>();

        foreach (var key in _currentKeysInOrder)
        {
            items.Add(_itemsByKey[key]);
        }

        return items;
    }
}
