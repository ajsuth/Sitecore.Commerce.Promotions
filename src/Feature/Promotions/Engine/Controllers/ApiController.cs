// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Feature.Promotions.Engine.Controllers
{
	using Feature.Promotions.Engine.Commands;
	using Microsoft.AspNetCore.Mvc;
	using Newtonsoft.Json.Linq;
	using Sitecore.Commerce.Core;
    using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
    using System.Web.Http.OData;

    /// <inheritdoc />
    /// <summary>
    /// Defines an api controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    [Route("api")]
    public class ApiController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Feature.Promotions.Engine.Controllers.ApiController" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public ApiController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

		[HttpPut]
		[Route("GetBulkPromotionCallouts()")]
		public async Task<IActionResult> GetBulkPromotionCallouts([FromBody] ODataActionParameters value)
		{
			if (!ModelState.IsValid)
			{
				return new BadRequestObjectResult(ModelState);
			}

			if (!value.ContainsKey("itemIds") || !(value["itemIds"] is JArray))
			{
				return new BadRequestObjectResult(value);
			}

			var jarray = (JArray)value["itemIds"];
			var itemIds = jarray?.ToObject<IEnumerable<string>>();
			var result = await Command<GetBulkPromotionCalloutsCommand>().Process(CurrentContext, itemIds);

			return new ObjectResult(result);
		}
	}
}