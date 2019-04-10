// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConcreteCartAction.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Foundation.Carts.Engine.Actions
{
	using Sitecore.Framework.Rules;

	/// <inheritdoc />
	/// <summary>
	/// Defines the concrete cart action
	/// </summary>
	[EntityIdentifier(nameof(ConcreteCartAction))]
    public class ConcreteCartAction : BaseCartAction
    {
        /// <summary>Validates custom rule values.</summary>
        /// <param name="context">The rule execution context.</param>
        /// <returns>Flag representing all custom rule values are valid</returns>
        public override bool HasValidRuleValues(IRuleExecutionContext context)
        {
            return true;
        }
		
        /// <summary>Calculates the discount amount for the cart line.</summary>
        /// <param name="context">The rule execution context.</param>
        /// <returns>The discount amount.</returns>
        public override decimal CalculateDiscount(IRuleExecutionContext context)
        {
            return new decimal();
        }
    }
}