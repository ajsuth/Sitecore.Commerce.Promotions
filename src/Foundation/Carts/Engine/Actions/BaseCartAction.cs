// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseCartAction.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Foundation.Carts.Engine.Actions
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Rules;
    using System;
    using System.Linq;

    /// <inheritdoc />
    /// <summary>
    /// Defines the base cart action
    /// </summary>
    [EntityIdentifier(nameof(BaseCartAction))]
    public abstract class BaseCartAction : ICartAction
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="context">
        /// The rule execution context.
        /// </param>
        public virtual void Execute(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();
            var cart = commerceContext?.GetObject<Cart>();
            if (cart == null || !cart.Lines.Any())
            {
                return;
            }

            if (!this.HasValidRuleValues(context))
            {
                return;
            }

            var amount = this.CalculateDiscount(context);
            amount = this.ToAdjustmentAmount(amount, commerceContext);
            if (amount == Decimal.Zero)
            {
                return;
            }
            
            this.ApplyAdjustmentToCart(cart, amount, commerceContext, context);
        }

		/// <summary>Validates custom rule values.</summary>
		/// <param name="context">The rule execution context.</param>
		/// <returns>Flag representing all custom rule values are valid</returns>
		public abstract bool HasValidRuleValues(IRuleExecutionContext context);

		/// <summary>Calculates the discount amount for the cart.</summary>
		/// <param name="context">The rule execution context.</param>
		/// <returns>The discount amount.</returns>
		public abstract decimal CalculateDiscount(IRuleExecutionContext context);

		/// <summary>Applies the adjustment to the cart.</summary>
		/// <param name="cart">The cart.</param>
		/// <param name="commerceContext">The commerce context.</param>
		/// <param name="context">The rule execution context.</param>
		public virtual void ApplyAdjustmentToCart(
            Cart cart,
            decimal amount,
            CommerceContext commerceContext,
            IRuleExecutionContext context)
        {
            var adjustmentType = commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount;
            var propertiesModel = commerceContext.GetObject<PropertiesModel>();
            var awardingBlock = this.GetType().Name;
            cart.Adjustments.Add(new CartLevelAwardedAdjustment()
            {
                Name = propertiesModel?.GetPropertyValue("PromotionText") as string ?? adjustmentType,
                DisplayName = propertiesModel?.GetPropertyValue("PromotionCartText") as string ?? adjustmentType,
                Adjustment = new Money(commerceContext.CurrentCurrency(), amount),
                AdjustmentType = adjustmentType,
                IsTaxable = false,
                AwardingBlock = awardingBlock
            });

            cart.GetComponent<MessagesComponent>().AddMessage(
                commerceContext.GetPolicy<KnownMessageCodePolicy>().Promotions,
                $"PromotionApplied: {propertiesModel?.GetPropertyValue("PromotionId") ?? awardingBlock}");
        }

		/// <summary>Converts the discount amount to the adjustment amount.</summary>
		/// <param name="amount">The discount amount.</param>
		/// <param name="commerceContext">The commerce context.</param>
		/// <returns>The adjustment amount.</returns>
		public virtual decimal ToAdjustmentAmount(decimal amount, CommerceContext commerceContext)
        {
            var globalPricingPolicy = commerceContext.GetPolicy<GlobalPricingPolicy>();
            if (globalPricingPolicy.ShouldRoundPriceCalc)
            {
                amount = decimal.Round(amount,
                    globalPricingPolicy.RoundDigits,
                    globalPricingPolicy.MidPointRoundUp
                        ? MidpointRounding.AwayFromZero
                        : MidpointRounding.ToEven);
            }

            return amount *= decimal.MinusOne;
        }
    }
}