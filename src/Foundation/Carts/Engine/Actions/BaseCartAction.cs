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
    /// Defines a cart action
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
        public void Execute(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();
            var cart = commerceContext?.GetObject<Cart>();
            if (cart == null || !cart.Lines.Any())
            {
                return;
            }

            if (this.HasValidRuleValues(context))
            {
                return;
            }

            var amount = this.CalculateDiscount(context);
            amount = ToAdjustmentAmount(amount, commerceContext);
            if (amount == Decimal.Zero)
            {
                return;
            }
            
            this.ApplyAdjustmentToCart(cart, amount, commerceContext, context);
        }

        protected abstract bool HasValidRuleValues(IRuleExecutionContext context);
        
        protected abstract decimal CalculateDiscount(IRuleExecutionContext context);

        protected virtual void ApplyAdjustmentToCart(
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

        protected virtual decimal ToAdjustmentAmount(decimal amount, CommerceContext commerceContext)
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