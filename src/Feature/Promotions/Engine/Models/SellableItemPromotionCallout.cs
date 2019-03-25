// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SellableItemPromotionCallout.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Promotions.Engine.Models
{
    using Sitecore.Commerce.Core;

    /// <inheritdoc />
    /// <summary>
    /// The SellableItemPromotionCallout.
    /// </summary>
    public class SellableItemPromotionCallout : Model
    {
		public string ItemId { get; set; }
		public string DisplayText { get; set; }

		public SellableItemPromotionCallout(string itemId)
		{
			ItemId = itemId;
		}
	}
}