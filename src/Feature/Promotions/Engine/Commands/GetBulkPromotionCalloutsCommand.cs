// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetBulkPromotionCalloutsCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Promotions.Engine.Commands
{
	using Feature.Promotions.Engine.Models;
	using Feature.Promotions.Engine.Pipelines;
	using Feature.Promotions.Engine.Pipelines.Arguments;
	using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
	using Sitecore.Commerce.Plugin.Carts;
	using Sitecore.Commerce.Plugin.Catalog;
	using Sitecore.Commerce.Plugin.Promotions;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

    /// <inheritdoc />
    /// <summary>
    /// Defines the GetBulkPromotionCalloutsCommand command.
    /// </summary>
    public class GetBulkPromotionCalloutsCommand : CommerceCommand
    {
        /// <summary>
        /// Gets or sets the commander.
        /// </summary>
        /// <value>
        /// The commander.
        /// </value>
        protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Feature.Promotions.Engine.Commands.GetBulkPromotionCalloutsCommand" /> class.
        /// </summary>
        /// <param name="pipeline">
        /// The pipeline.
        /// </param>
        /// <param name="serviceProvider">The service provider</param>
        public GetBulkPromotionCalloutsCommand(CommerceCommander commander, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            this.Commander = commander;
        }

		/// <summary>
		/// The process of the command
		/// </summary>
		/// <param name="commerceContext">
		/// The commerce context
		/// </param>
		/// <param name="parameter">
		/// The parameter for the command
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		public virtual async Task<IEnumerable<SellableItemPromotionCallout>> Process(CommerceContext commerceContext, IEnumerable<string> itemIds)
		{
			IEnumerable<SellableItemPromotionCallout> SellableItemPromotionCallouts;

			using (CommandActivity.Start(commerceContext, this))
			{
				var contextOptions = commerceContext.GetPipelineContextOptions();
				var items = new List<SellableItemPromotionCallout>();
				var catalogIds = new List<string>();
				var productArguments = new List<ProductArgument>();
				foreach (var itemId in itemIds)
				{
					var productArgument = ProductArgument.FromItemId(itemId);
					if (!productArgument.IsValid())
					{
						await contextOptions.CommerceContext.AddMessage(
							commerceContext.GetPolicy<KnownResultCodes>().Error,
							"ItemIdIncorrectFormat",
							new object[1] { itemId },
							$"Expecting a CatalogId and a ProductId in the ItemId: {itemId}.");

						break;
					}

					productArguments.Add(productArgument);
					catalogIds.Add(productArgument.CatalogName);
				}

				catalogIds = catalogIds.Distinct().ToList();
				var arg = new DiscoverProductPromotionsArgument(new Cart())
				{
					CatalogIds = catalogIds
				};

				var promotions = await Commander.Pipeline<DiscoverProductPromotionsPipeline>().Run(arg, contextOptions);

				foreach (var productArgument in productArguments)
				{
					var sellableItem = await Commander.Pipeline<IGetSellableItemPipeline>().Run(productArgument, contextOptions);
					if (sellableItem != null)
					{
						items.Add(this.TranslateToPromotionOverlay(sellableItem, promotions));
					}
				}
				SellableItemPromotionCallouts = items.AsEnumerable();
			}
			return SellableItemPromotionCallouts;
		}

		private SellableItemPromotionCallout TranslateToPromotionOverlay(SellableItem sellableItem, IEnumerable<Promotion> promotions)
		{
			var SellableItemPromotionCallout = new SellableItemPromotionCallout(sellableItem.FriendlyId);

			//foreach (var promotion in promotions)
			//{
			//	var overlayName = GetOverlayImageName(promotion, sellableItem);
			//	if (overlayName != null)
			//	{
			//		SellableItemPromotionCallout.ImageName = overlayName;
			//		break;
			//	}
			//}

			return SellableItemPromotionCallout;
		}

		private string GetCalloutImageName(Promotion promotion, SellableItem sellableItem)
		{
			var qualificationsPolicy = promotion.GetPolicy<PromotionQualificationsPolicy>();
			//foreach (var qualification in qualificationsPolicy.Qualifications)
			//{
			//	var targetItemId = qualification.GetProperty("TargetItemId");
			//	if (targetItemId != null && targetItemId.Value.Equals(sellableItem.FriendlyId))
			//	{
			//		return promotion.GetComponent<ImageComponent>().OverlayImageName;
			//	}
			//}

			return null;
		}
	}
}