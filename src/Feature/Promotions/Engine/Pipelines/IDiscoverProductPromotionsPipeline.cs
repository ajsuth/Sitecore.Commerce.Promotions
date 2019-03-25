// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDiscoverProductPromotionsPipeline.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Promotions.Engine.Pipelines
{
	using Feature.Promotions.Engine.Pipelines.Arguments;
	using Sitecore.Commerce.Core;
	using Sitecore.Commerce.Plugin.Promotions;
	using Sitecore.Framework.Pipelines;
	using System.Collections.Generic;

	/// <summary>
	/// Defines the IDiscoverProductPromotionsPipeline interface
	/// </summary>
	/// <seealso>
	///     <cref>
	///         Sitecore.Framework.Pipelines.IPipeline{DiscoverProductPromotionsArgument,
	///         PipelineArgumentOrEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
	///     </cref>
	/// </seealso>
	[PipelineDisplayName("[Insert Project Name].Pipeline.IDiscoverProductPromotionsPipeline")]
    public interface IDiscoverProductPromotionsPipeline : IPipeline<DiscoverProductPromotionsArgument, IEnumerable<Promotion>, CommercePipelineExecutionContext>
    {
    }
}
