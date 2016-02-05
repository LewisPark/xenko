﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System.Collections.Generic;

namespace SiliconStudio.ActionStack
{
    /// <summary>
    /// Base interface of aggregate of action items.
    /// </summary>
    public interface IAggregateActionItem : IActionItem
    {
        /// <summary>
        /// Gets the aggregated action items.
        /// </summary>
        IReadOnlyCollection<IActionItem> ActionItems { get; }

        /// <summary>
        /// Gets or sets whether the order of contained action items should be reversed when undoing this action. 
        /// </summary>
        /// <remarks>The default value must be <c>true</c> in implementations.</remarks>
        /// <remarks>In some advanced use cases it is possible that the actions must be undone in the same order that they were done in the first place.</remarks>
        bool ReverseOrderOnUndo { get; set; }

        /// <summary>
        /// Gets whether the given action item is contained in this aggregate.
        /// </summary>
        /// <param name="actionItem">The action item to look for.</param>
        /// <returns><c>true</c> if this aggregate contains the given action item, <c>false</c> otherwise.</returns>
        bool ContainsAction(IActionItem actionItem);

        /// <summary>
        /// Gets all action items contained in this aggregate (including itself), recursively.
        /// </summary>
        /// <returns>An enumeration of all action items contained in this aggregate (including itself), recursively.</returns>
        IEnumerable<IActionItem> GetInnerActionItems();
    }
}