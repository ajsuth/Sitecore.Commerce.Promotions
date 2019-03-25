// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterPromotionsByBookAssociatedCatalogsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Promotions.Engine.Pipelines.Blocks
{
	using Feature.Promotions.Engine.Pipelines.Arguments;
	using Sitecore.Commerce.Core;
	using Sitecore.Commerce.Plugin.Promotions;
	using Sitecore.Framework.Pipelines;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	/// <summary>
	/// Defines a pipeline block
	/// </summary>
	/// <seealso>
	///     <cref>
	///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Core.PipelineArgument,
	///         Sitecore.Commerce.Core.PipelineArgument, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
	///     </cref>
	/// </seealso>
	[PipelineDisplayName("Change to <Project>Constants.Pipelines.Blocks.<Block Name>")]
    public class FilterPromotionsByBookAssociatedCatalogsBlock : PipelineBlock<IEnumerable<Promotion>, IEnumerable<Promotion>, CommercePipelineExecutionContext>
    {
		/// <summary>
		/// Gets or sets the commander.
		/// </summary>
		/// <value>
		/// The commander.
		/// </value>
		protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public FilterPromotionsByBookAssociatedCatalogsBlock(CommerceCommander commander)
		    : base(null)
		{
            this.Commander = commander;
        }

		/// <summary>
		/// The execute.
		/// </summary>
		/// <param name="arg">
		/// The pipeline argument.
		/// </param>
		/// <param name="context">
		/// The context.
		/// </param>
		/// <returns>
		/// The <see cref="PipelineArgument"/>.
		/// </returns>
		public override async Task<IEnumerable<Promotion>> Run(IEnumerable<Promotion> arg, CommercePipelineExecutionContext context)
		{
			if (arg == null)
			{
				return Enumerable.Empty<Promotion>();
			}

			var promotionArray = arg as Promotion[] ?? arg.ToArray();
			if (!((IEnumerable<Promotion>)promotionArray).Any())
			{
				return ((IEnumerable<Promotion>)promotionArray).AsEnumerable();
			}

			var promotionsArgument = context.CommerceContext.GetObject<DiscoverPromotionsArgument>() as DiscoverProductPromotionsArgument;
			var linesCatalogs = promotionsArgument.CatalogIds;
			if (!linesCatalogs.Any())
			{
				return ((IEnumerable<Promotion>)promotionArray).AsEnumerable();
			}

			var qualifyingPromotions = new List<Promotion>();
			foreach (var promotion in promotionArray)
			{
				var associatedCatalogs = (await Commander.Pipeline<IGetBookAssociatedCatalogsPipeline>().Run(new GetBookAssociateCatalogsArgument(promotion.Book.Name, true), context))
					.Where(catalogModel => !string.IsNullOrEmpty(catalogModel.Name))
					.ToList();

				if (associatedCatalogs.Select(c => c.Name).Intersect(linesCatalogs, StringComparer.OrdinalIgnoreCase).Any())
				{
					qualifyingPromotions.Add(promotion);
				}
			}

			return qualifyingPromotions.AsEnumerable();
		}
	}
}