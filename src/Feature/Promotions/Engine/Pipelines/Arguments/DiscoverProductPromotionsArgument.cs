// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiscoverProductPromotionsArgument.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Promotions.Engine.Pipelines.Arguments
{
    using Sitecore.Commerce.Core;
	using Sitecore.Commerce.Plugin.Promotions;
	using System.Collections.Generic;

	/// <inheritdoc />
	/// <summary>
	/// The DiscoverProductPromotionsArgument.
	/// </summary>
	public class DiscoverProductPromotionsArgument : DiscoverPromotionsArgument
	{
		public DiscoverProductPromotionsArgument(CommerceEntity entity)
			: base(entity)
		{
		}

		public IEnumerable<string> CatalogIds { get; set; }
	}
}