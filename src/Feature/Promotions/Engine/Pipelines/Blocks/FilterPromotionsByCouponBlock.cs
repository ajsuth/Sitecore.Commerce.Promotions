// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterPromotionsByCouponBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Promotions.Engine.Pipelines.Blocks
{
	using Sitecore.Commerce.Core;
	using Sitecore.Commerce.Plugin.Coupons;
	using Sitecore.Commerce.Plugin.Promotions;
	using Sitecore.Framework.Pipelines;
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
    public class FilterPromotionsByCouponBlock : PipelineBlock<IEnumerable<Promotion>, IEnumerable<Promotion>, CommercePipelineExecutionContext>
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
        public FilterPromotionsByCouponBlock(CommerceCommander commander)
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
		public override Task<IEnumerable<Promotion>> Run(IEnumerable<Promotion> arg, CommercePipelineExecutionContext context)
		{
			var promotionArray = arg as Promotion[] ?? arg.ToArray();

			if (!((IEnumerable<Promotion>)promotionArray).Any())
			{
				return Task.FromResult(((IEnumerable<Promotion>)promotionArray).AsEnumerable());
			}

			// TODO: Display without coupon, if custom flag set against promotion

			var qualifyingPromotions = new List<Promotion>();
			((IEnumerable<Promotion>)promotionArray).ForEach(promotion =>
			{
				if (!promotion.HasPolicy<CouponRequiredPolicy>())
				{
					qualifyingPromotions.Add(promotion);
				}
			});

			return Task.FromResult(qualifyingPromotions.AsEnumerable());
		}
	}
}