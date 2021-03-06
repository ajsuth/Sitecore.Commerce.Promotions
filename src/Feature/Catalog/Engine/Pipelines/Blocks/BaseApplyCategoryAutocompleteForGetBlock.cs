﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplyCategoryAutocompleteBaseBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace SamplePromotions.Feature.Catalog.Engine.Pipelines.Blocks
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Plugin.Search;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using CatalogConstants = Feature.Catalog.Engine.CatalogConstants;

    /// <summary>Replaces the TargetCategorySitecoreId field with a CategoryId autocomplete field</summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(CatalogConstants.Pipelines.Blocks.BaseApplyCategoryAutocompleteForGet)]
    public abstract class BaseApplyCategoryAutocompleteForGetBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>Gets or sets the commander.</summary>
        /// <value>The commander.</value>
        protected readonly CommerceCommander Commander;

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public BaseApplyCategoryAutocompleteForGetBlock(CommerceCommander commander)
          : base(null)
        {
            this.Commander = commander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The <see cref="EntityView"/>.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            var entityViewArgument = context.CommerceContext.GetObject<EntityViewArgument>();
            if (string.IsNullOrEmpty(entityViewArgument?.ViewName)
                    || !entityViewArgument.ViewName.Equals(this.GetEntityViewName(context), StringComparison.OrdinalIgnoreCase))
            {
                return await Task.FromResult(entityView);
            }

            var isEditAction = entityViewArgument.ForAction.Equals(this.GetActionName(context), StringComparison.OrdinalIgnoreCase);
            if (!(entityViewArgument.Entity is Promotion) || !isEditAction)
            {
                return await Task.FromResult(entityView);
            }

            var entity = (Promotion)entityViewArgument.Entity;
            var targetCategorySitecoreId = entityView.GetProperty("TargetCategorySitecoreId");
            if (targetCategorySitecoreId == null)
            {
                return await Task.FromResult(entityView);
            }

            targetCategorySitecoreId.IsHidden = true;
            PopulateItemDetails(entityView, context);

            return await Task.FromResult(entityView);
        }

        /// <summary>Sets the CategoryId view property to autocomplete.</summary>
        /// <param name="view">The <see cref="EntityView"/>.</param>
        /// <param name="context">The context.</param>
        protected virtual void PopulateItemDetails(EntityView view, CommercePipelineExecutionContext context)
        {
            if (view == null)
            {
                return;
            }

            var categoryId = view.GetProperty("CategoryId");
            if (categoryId == null)
            {
                return;
            }
            
            var policyByType = SearchScopePolicy.GetPolicyByType(
                context.CommerceContext,
                context.CommerceContext.Environment,
                typeof(Category));
            if (policyByType != null)
            {
                var policy = new Policy()
                {
                    PolicyId = "EntityType",
                    Models = new List<Model>()
                    {
                        new Model() { Name = "Category" }
                    }
                };
                categoryId.UiType = "Autocomplete";
                categoryId.Policies.Add(policy);
                categoryId.Policies.Add(policyByType);
            }
        }

        /// <summary>Gets the action name.</summary>
        /// <param name="context">The context.</param
        /// <returns>The action name.</returns>
        protected abstract string GetActionName(CommercePipelineExecutionContext context);

        /// <summary>Gets the entity view name.</summary>
        /// <param name="context">The context.</param
        /// <returns>The entity view name.</returns>
        protected abstract string GetEntityViewName(CommercePipelineExecutionContext context);
    }
}