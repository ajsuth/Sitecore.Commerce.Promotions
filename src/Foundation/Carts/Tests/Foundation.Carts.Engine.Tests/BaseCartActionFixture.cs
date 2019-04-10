using FluentAssertions;
using Foundation.Carts.Engine.Actions;
using NSubstitute;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Foundation.Carts.Engine.Tests
{
    public class BaseCartActionFixture
    {
		const decimal MINIMUM_AMOUNT_OFF = 0.01m;

		public class Boundary
        {
            [Theory, AutoNSubstituteData]
            public void Execute_01_NoCommerceContext(
				IRuleExecutionContext context,
				Cart cart)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				cart.Adjustments.Clear();
				cart.Lines.ForEach(l => l.Adjustments.Clear());

				context.Fact<CommerceContext>().ReturnsForAnyArgs((CommerceContext)null);

				var action = Substitute.ForPartsOf<BaseCartAction>();

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.DidNotReceiveWithAnyArgs().HasValidRuleValues(context);
				cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
				cart.Adjustments.Should().BeEmpty();
			}

			[Theory, AutoNSubstituteData]
            public void Execute_02_NoCart(
				IRuleExecutionContext context,
				CommerceContext commerceContext)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);

				var action = Substitute.ForPartsOf<BaseCartAction>();

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.DidNotReceiveWithAnyArgs().HasValidRuleValues(context);
			}

			[Theory, AutoNSubstituteData]
			public void Execute_03_NoCartLines(
				IRuleExecutionContext context,
				CommerceContext commerceContext,
				Cart cart)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				cart.Adjustments.Clear();
				cart.Lines.Clear();

				context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
				commerceContext.AddObject(cart);

				var action = Substitute.ForPartsOf<BaseCartAction>();

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.DidNotReceiveWithAnyArgs().HasValidRuleValues(context);
				cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
				cart.Adjustments.Should().BeEmpty();
			}

			[Theory, AutoNSubstituteData]
			public void Execute_04_NoValidRuleRules(
				IRuleExecutionContext context,
				CommerceContext commerceContext,
				Cart cart)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				cart.Adjustments.Clear();
				cart.Lines.ForEach(l => l.Adjustments.Clear());

				context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
				commerceContext.AddObject(cart);

				var action = Substitute.ForPartsOf<BaseCartAction>();
				action.HasValidRuleValues(context).ReturnsForAnyArgs(false);

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.Received().HasValidRuleValues(context);
				action.DidNotReceiveWithAnyArgs().CalculateDiscount(context);
				cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
				cart.Adjustments.Should().BeEmpty();
			}

			[Theory, AutoNSubstituteData]
			public void Execute_05_ZeroDiscount(
				IRuleExecutionContext context,
				CommerceContext commerceContext,
				Cart cart)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				cart.Adjustments.Clear();
				cart.Lines.ForEach(l => l.Adjustments.Clear());

				context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
				commerceContext.AddObject(cart);

				var action = Substitute.ForPartsOf<BaseCartAction>();
				action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
				action.CalculateDiscount(context).ReturnsForAnyArgs(decimal.Zero);

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.Received().HasValidRuleValues(context);
				action.Received().CalculateDiscount(context);
				action.ReceivedWithAnyArgs().ToAdjustmentAmount(new decimal(), commerceContext);
				cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
				cart.Adjustments.Should().BeEmpty();
			}

		}
		public class Functional
		{
			[Theory, AutoNSubstituteData]
			public void Execute_ShouldRoundPriceCalc_False(
				IRuleExecutionContext context,
				CommerceContext commerceContext,
				Cart cart)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				const decimal AMOUNT_OFF = 0.006m;
				cart.Adjustments.Clear();
				cart.Lines.ForEach(l => l.Adjustments.Clear());

				context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
				commerceContext.AddObject(cart);

				var globalPricingPolicy = commerceContext.GetPolicy<GlobalPricingPolicy>();
				globalPricingPolicy.ShouldRoundPriceCalc = false;

				var action = Substitute.ForPartsOf<BaseCartAction>();
				action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
				action.CalculateDiscount(context).ReturnsForAnyArgs(AMOUNT_OFF);

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.Received().HasValidRuleValues(context);
				action.Received().CalculateDiscount(context);
				action.Received().ToAdjustmentAmount(AMOUNT_OFF, commerceContext);
				cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
				cart.Adjustments.Should().HaveCount(1);
				cart.Adjustments.FirstOrDefault().Should().NotBeNull();
				cart.Adjustments.FirstOrDefault().Should().BeOfType<CartLevelAwardedAdjustment>();
				cart.Adjustments.FirstOrDefault().Name.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
				cart.Adjustments.FirstOrDefault().DisplayName.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
				cart.Adjustments.FirstOrDefault().AdjustmentType.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
				cart.Adjustments.FirstOrDefault().AwardingBlock.Should().StartWith(nameof(BaseCartAction));
				cart.Adjustments.FirstOrDefault().IsTaxable.Should().BeFalse();
				cart.Adjustments.FirstOrDefault().Adjustment.CurrencyCode.Should().Be(commerceContext.CurrentCurrency());
				cart.Adjustments.FirstOrDefault().Adjustment.Amount.Should().Be(-AMOUNT_OFF);
				cart.HasComponent<MessagesComponent>().Should().BeTrue();
			}

			[Theory, AutoNSubstituteData]
			public void Execute_ShouldRoundPriceCalc_True(
				IRuleExecutionContext context,
				CommerceContext commerceContext,
				Cart cart)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				const decimal AMOUNT_OFF = 0.006m;
				cart.Adjustments.Clear();
				cart.Lines.ForEach(l => l.Adjustments.Clear());

				context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
				commerceContext.AddObject(cart);
				
				var action = Substitute.ForPartsOf<BaseCartAction>();
				action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
				action.CalculateDiscount(context).ReturnsForAnyArgs(AMOUNT_OFF);

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.Received().HasValidRuleValues(context);
				action.Received().CalculateDiscount(context);
				action.Received().ToAdjustmentAmount(AMOUNT_OFF, commerceContext);
				cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
				cart.Adjustments.Should().HaveCount(1);
				cart.Adjustments.FirstOrDefault().Should().NotBeNull();
				cart.Adjustments.FirstOrDefault().Should().BeOfType<CartLevelAwardedAdjustment>();
				cart.Adjustments.FirstOrDefault().Name.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
				cart.Adjustments.FirstOrDefault().DisplayName.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
				cart.Adjustments.FirstOrDefault().AdjustmentType.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
				cart.Adjustments.FirstOrDefault().AwardingBlock.Should().StartWith(nameof(BaseCartAction));
				cart.Adjustments.FirstOrDefault().IsTaxable.Should().BeFalse();
				cart.Adjustments.FirstOrDefault().Adjustment.CurrencyCode.Should().Be(commerceContext.CurrentCurrency());
				cart.Adjustments.FirstOrDefault().Adjustment.Amount.Should().Be(-MINIMUM_AMOUNT_OFF);
				cart.HasComponent<MessagesComponent>().Should().BeTrue();
			}

			[Theory, AutoNSubstituteData]
			public void Execute_WithProperties(
				IRuleExecutionContext context,
				CommerceContext commerceContext,
				Cart cart,
				string promotionId,
				string promotionCartText,
				string promotionText)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				const decimal AMOUNT_OFF = 0.006m;
				cart.Adjustments.Clear();
				cart.Lines.ForEach(l => l.Adjustments.Clear());

				context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
				commerceContext.AddObject(cart);

				var propertiesModel = new PropertiesModel();
				propertiesModel.Properties.Add("PromotionId", promotionId);
				propertiesModel.Properties.Add("PromotionCartText", promotionCartText);
				propertiesModel.Properties.Add("PromotionText", promotionText);
				commerceContext.AddObject(propertiesModel);

				var action = Substitute.ForPartsOf<BaseCartAction>();
				action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
				action.CalculateDiscount(context).ReturnsForAnyArgs(AMOUNT_OFF);

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.Received().HasValidRuleValues(context);
				action.Received().CalculateDiscount(context);
				action.Received().ToAdjustmentAmount(AMOUNT_OFF, commerceContext);
				cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
				cart.Adjustments.Should().HaveCount(1);
				cart.Adjustments.FirstOrDefault().Should().NotBeNull();
				cart.Adjustments.FirstOrDefault().Should().BeOfType<CartLevelAwardedAdjustment>();
				cart.Adjustments.FirstOrDefault().Name.Should().Be(promotionText);
				cart.Adjustments.FirstOrDefault().DisplayName.Should().Be(promotionCartText);
				cart.Adjustments.FirstOrDefault().AdjustmentType.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
				cart.Adjustments.FirstOrDefault().AwardingBlock.Should().StartWith(nameof(BaseCartAction));
				cart.Adjustments.FirstOrDefault().IsTaxable.Should().BeFalse();
				cart.Adjustments.FirstOrDefault().Adjustment.CurrencyCode.Should().Be(commerceContext.CurrentCurrency());
				cart.Adjustments.FirstOrDefault().Adjustment.Amount.Should().Be(-MINIMUM_AMOUNT_OFF);
				cart.HasComponent<MessagesComponent>().Should().BeTrue();
				cart.GetComponent<MessagesComponent>().Messages.FirstOrDefault(m => m.Text.Contains(promotionId));
			}

			[Theory, AutoNSubstituteData]
			public void Execute_WithConcreteClass(
				IRuleExecutionContext context,
				CommerceContext commerceContext,
				Cart cart)
			{
				/**********************************************
                 * Arrange
                 **********************************************/
				const decimal AMOUNT_OFF = 0.006m;
				cart.Adjustments.Clear();
				cart.Lines.ForEach(l => l.Adjustments.Clear());

				context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
				commerceContext.AddObject(cart);
				
				var action = Substitute.ForPartsOf<ConcreteCartAction>();
				action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
				action.CalculateDiscount(context).ReturnsForAnyArgs(AMOUNT_OFF);

				/**********************************************
                 * Act
                 **********************************************/
				Action executeAction = () => action.Execute(context);

				/**********************************************
                 * Assert
                 **********************************************/
				executeAction.Should().NotThrow<Exception>();
				action.Received().HasValidRuleValues(context);
				action.Received().CalculateDiscount(context);
				action.Received().ToAdjustmentAmount(AMOUNT_OFF, commerceContext);
				cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
				cart.Adjustments.Should().HaveCount(1);
				cart.Adjustments.FirstOrDefault().Should().NotBeNull();
				cart.Adjustments.FirstOrDefault().Should().BeOfType<CartLevelAwardedAdjustment>();
				cart.Adjustments.FirstOrDefault().AwardingBlock.Should().StartWith(nameof(ConcreteCartAction));
			}
		}
	}
}
