// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcreteCartLineAction.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Foundation.Carts.Engine.Actions
{
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Framework.Rules;
    using System.Collections.Generic;
    using System.Linq;

    /// <inheritdoc />
    /// <summary>
    /// Defines the concrete cart line action
    /// </summary>
    [EntityIdentifier(nameof(ConcreteCartLineAction))]
    public class ConcreteCartLineAction : BaseCartLineAction
    {
        /// <summary>Validates custom rule values.</summary>
        /// <param name="context">The rule execution context.</param>
        /// <returns>Flag representing all custom rule values are valid</returns>
        public override bool HasValidRuleValues(IRuleExecutionContext context)
        {
            return true;
        }

        /// <summary>Determines the applicable cart lines to apply the promotion to.</summary>
        /// <param name="context">The rule execution context.</param>
        /// <returns>A list of applicable <see cref="CartLineComponent"/> for the promotion.</returns>
        public override IEnumerable<CartLineComponent> ApplicableLines(IRuleExecutionContext context)
        {
            return Enumerable.Empty<CartLineComponent>();
        }

        /// <summary>Calculates the discount amount for the cart line.</summary>
        /// <param name="line">The cart line.</param>
        /// <param name="context">The rule execution context.</param>
        /// <returns>The discount amount.</returns>
        public override decimal CalculateLineDiscount(CartLineComponent line, IRuleExecutionContext context)
        {
            return new decimal();
        }
    }
}