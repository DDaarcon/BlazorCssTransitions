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

    private readonly Dictionary<string, Item> _itemsByKey = new();
    /// <summary>
    /// Current keys in order that they have to be displayed
    /// </summary>
    private readonly List<string> _keysInOrder = new();

    /// <summary>
    /// Keys in order, before the update
    /// </summary>
    private string[]? _previousKeysInOrder;
    /// <summary>
    /// Keys gathered when items are added, in order
    /// </summary>
    private readonly List<string> _keysOfNewItems = new();
    /// <summary>
    /// Keys that were previously not present in the dictionary
    /// </summary>
    private readonly List<string> _keysOfNewlyAddedNewItems = new();

    public async Task<IEnumerable<Item>> ProcessItems(RenderFragment fragmentWithItemsToProcess)
    {
        _keysOfNewItems.Clear();
        _keysOfNewlyAddedNewItems.Clear();
        _previousKeysInOrder = _keysInOrder.ToArray();

        await _externalRenderer.Render(fragmentWithItemsToProcess);

        InitiateDisappearanceOfNoLongerPresentItems();

        SetKeysOrder();

        return GetItemsOrdered();
    }

    public IEnumerable<Item> ProcessItemDeletionByKey(string key)
    {
        if (_itemsByKey.Remove(key))
        {
            _keysInOrder.Remove(key);
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

    private void InitiateDisappearanceOfNoLongerPresentItems()
    {
        if (_previousKeysInOrder is null)
        {
            return;
        }

        var removedElementKeys = _previousKeysInOrder.Except(_keysOfNewItems);

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

    private void SetKeysOrder()
    {
        if (_keysInOrder.Count == 0)
        {
            _keysInOrder.AddRange(_keysOfNewItems);
            return;
        }

        int offsetInFinalKeys = 0;
        for (int newKeyIndex = 0; newKeyIndex < _keysOfNewItems.Count; newKeyIndex++)
        {

        }
    }

    private List<Item> GetItemsOrdered()
    {
        var items = new List<Item>();

        foreach (var key in _keysInOrder)
        {
            items.Add(_itemsByKey[key]);
        }

        return items;
    }
}
