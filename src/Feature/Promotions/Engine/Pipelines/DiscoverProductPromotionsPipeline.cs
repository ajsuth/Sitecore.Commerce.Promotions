// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscoverProductPromotionsPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Promotions.Engine.Pipelines
{
	using Feature.Promotions.Engine.Pipelines.Arguments;
	using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
	using Sitecore.Commerce.Plugin.Promotions;
	using Sitecore.Framework.Pipelines;
	using System.Collections.Generic;

	/// <inheritdoc />
	/// <summary>
	///  Defines the DiscoverProductPromotionsPipeline pipeline.
	/// </summary>
	/// <seealso>
	///     <cref>
	///         Sitecore.Commerce.Core.CommercePipeline{DiscoverProductPromotionsArgument,
	///         Namespace.PipelineArgumentOrEntity}
	///     </cref>
	/// </seealso>
	/// <seealso cref="T:Feature.Promotions.Engine.Pipelines.DiscoverProductPromotionsPipeline" />
	public class DiscoverProductPromotionsPipeline : CommercePipeline<DiscoverProductPromotionsArgument, IEnumerable<Promotion>>, IDiscoverProductPromotionsPipeline
	{
		/// <inheritdoc />
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Feature.Promotions.Engine.Pipelines.DiscoverProductPromotionsPipeline" /> class.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <param name="loggerFactory">The logger factory.</param>
		public DiscoverProductPromotionsPipeline(IPipelineConfiguration<IDiscoverProductPromotionsPipeline> configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory)
        {
        }
    }
}

