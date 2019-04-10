// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseCartLineAction.cs" company="Sitecore Corporation">
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
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc />
    /// <summary>
    /// Defines the base cart line action
    /// </summary>
    [EntityIdentifier(nameof(BaseCartLineAction))]
    public abstract class BaseCartLineAction : ICartLineAction
    {
        /// <summary>The execute.</summary>
        /// <param name="context">The rule execution context.</param>
        public virtual void Execute(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();
            var cart = commerceContext?.GetObject<Cart>();
            var totals = commerceContext?.GetObject<CartTotals>();
            if (cart == null || !cart.Lines.Any() || totals == null || !totals.Lines.Any())
            {
                return;
            }

            if (!this.HasValidRuleValues(context))
            {
                return;
            }

            var lines = this.ApplicableLines(context);
            if (lines == null || !lines.Any())
            {
                return;
            }

            var propertiesModel = commerceContext.GetObject<PropertiesModel>();
            var discount = commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount;
            lines.ForEach(line => this.ApplyAdjustmentToLine(totals, line, propertiesModel, discount, commerceContext, context));
        }

        /// <summary>Validates custom rule values.</summary>
        /// <param name="context">The rule execution context.</param>
        /// <returns>Flag representing all custom rule values are valid</returns>
        public abstract bool HasValidRuleValues(IRuleExecutionContext context);

        /// <summary>Determines the applicable cart lines to apply the promotion to.</summary>
        /// <param name="context">The rule execution context.</param>
        /// <returns>A list of applicable <see cref="CartLineComponent"/> for the promotion.</returns>
        public abstract IEnumerable<CartLineComponent> ApplicableLines(IRuleExecutionContext context);

        /// <summary>Calculates the discount amount for the cart line.</summary>
        /// <param name="line">The cart line.</param>
        /// <param name="context">The rule execution context.</param>
        /// <returns>The discount amount.</returns>
        public abstract decimal CalculateLineDiscount(CartLineComponent line, IRuleExecutionContext context);

        /// <summary>Applies the adjustment to the cart line.</summary>
        /// <param name="totals">The cart totals.</param>
        /// <param name="line">The cart line.</param>
        /// <param name="propertiesModel">The properties model.</param>
        /// <param name="adjustmentType">The adjustment type.</param>
        /// <param name="commerceContext">The commerce context.</param>
        /// <param name="context">The rule execution context.</param>
        public virtual void ApplyAdjustmentToLine(
            CartTotals totals,
            CartLineComponent line,
            PropertiesModel propertiesModel,
            string adjustmentType,
            CommerceContext commerceContext,
            IRuleExecutionContext context)
        {
            if (!totals.Lines.ContainsKey(line.Id) || !line.HasPolicy<PurchaseOptionMoneyPolicy>())
            {
                return;
            }

            var amount = this.CalculateLineDiscount(line, context);
            amount = this.ToAdjustmentAmount(amount, commerceContext);

            var awardingBlock = this.GetType().Name;
            line.Adjustments.Add(new CartLineLevelAwardedAdjustment()
            {
                Name = (propertiesModel?.GetPropertyValue("PromotionText") as string ?? adjustmentType),
                DisplayName = (propertiesModel?.GetPropertyValue("PromotionCartText") as string ?? adjustmentType),
                Adjustment = new Money(commerceContext.CurrentCurrency(), amount),
                AdjustmentType = adjustmentType,
                IsTaxable = false,
                AwardingBlock = awardingBlock
            });

            totals.Lines[line.Id].SubTotal.Amount += amount;

            line.GetComponent<MessagesComponent>().AddMessage(
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