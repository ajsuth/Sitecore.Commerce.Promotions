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
    public class BaseCartLineActionFixture
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

                var action = Substitute.ForPartsOf<BaseCartLineAction>();

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

                var action = Substitute.ForPartsOf<BaseCartLineAction>();

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

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                
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
            public void Execute_04_NoCartTotals(
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

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                
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
            public void Execute_05_NoCartTotalLines(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());
                cartTotals.Lines.Clear();

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var action = Substitute.ForPartsOf<BaseCartLineAction>();

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
            public void Execute_06_NoValidRuleRules(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
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
                action.DidNotReceiveWithAnyArgs().ApplicableLines(context);
                cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
                cart.Adjustments.Should().BeEmpty();
            }

            [Theory, AutoNSubstituteData]
            public void Execute_07_NoMatchingLines(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
                action.ApplicableLines(context).ReturnsForAnyArgs((IEnumerable<CartLineComponent>)null);

                /**********************************************
                 * Act
                 **********************************************/
                Action executeAction = () => action.Execute(context);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                action.Received().HasValidRuleValues(context);
                action.Received().ApplicableLines(context);
                action.DidNotReceiveWithAnyArgs().ApplyAdjustmentToLine(
                    cartTotals,
                    new CartLineComponent(),
                    commerceContext.GetObject<PropertiesModel>(),
                    commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount,
                    commerceContext,
                    context);
                cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
                cart.Adjustments.Should().BeEmpty();
            }

            [Theory, AutoNSubstituteData]
            public void Execute_08_EmptyMatchingLines(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
                action.ApplicableLines(context).ReturnsForAnyArgs(Enumerable.Empty<CartLineComponent>());

                /**********************************************
                 * Act
                 **********************************************/
                Action executeAction = () => action.Execute(context);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                action.Received().HasValidRuleValues(context);
                action.Received().ApplicableLines(context);
                action.DidNotReceiveWithAnyArgs().ApplyAdjustmentToLine(
                    cartTotals,
                    cart.Lines[0],
                    commerceContext.GetObject<PropertiesModel>(),
                    commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount,
                    commerceContext,
                    context);
                cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
                cart.Adjustments.Should().BeEmpty();
            }

            [Theory, AutoNSubstituteData]
            public void Execute_09_NoLineIdInTotalsLines(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());
                while (cart.Lines.Count > 1)
                {
                    cart.Lines.RemoveAt(0);
                }

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
                action.ApplicableLines(context).ReturnsForAnyArgs(cart.Lines);

                /**********************************************
                 * Act
                 **********************************************/
                Action executeAction = () => action.Execute(context);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                action.Received().HasValidRuleValues(context);
                action.Received().ApplicableLines(context);
                action.Received(1).ApplyAdjustmentToLine(
                    cartTotals,
                    cart.Lines[0],
                    commerceContext.GetObject<PropertiesModel>(),
                    commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount,
                    commerceContext,
                    context);
                action.DidNotReceiveWithAnyArgs().CalculateLineDiscount(cart.Lines[0], context);
                cart.Lines.SelectMany(l => l.Adjustments).Should().BeEmpty();
                cart.Adjustments.Should().BeEmpty();
            }

            [Theory, AutoNSubstituteData]
            public void Execute_10_LineHasNoPurchaseOptionMoneyPolicy(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals,
                Totals totals)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());
                while (cart.Lines.Count > 1)
                {
                    cart.Lines.RemoveAt(0);
                }
                var cartLine = cart.Lines[0];
                cartTotals.Lines.Add(cartLine.Id, totals);

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
                action.ApplicableLines(context).ReturnsForAnyArgs(cart.Lines);

                /**********************************************
                 * Act
                 **********************************************/
                Action executeAction = () => action.Execute(context);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                action.Received().HasValidRuleValues(context);
                action.Received().ApplicableLines(context);
                action.Received(1).ApplyAdjustmentToLine(
                    cartTotals,
                    cart.Lines[0],
                    commerceContext.GetObject<PropertiesModel>(),
                    commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount,
                    commerceContext,
                    context);
                action.DidNotReceiveWithAnyArgs().CalculateLineDiscount(cart.Lines[0], context);
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
                Cart cart,
                CartTotals cartTotals,
                Totals totals)
            {
                /**********************************************
               * Arrange
               **********************************************/
                const decimal AMOUNT_OFF = 0.006m;
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());
                while (cart.Lines.Count > 1)
                {
                    cart.Lines.RemoveAt(0);
                }
                var cartLine = cart.Lines[0];
                cartTotals.Lines.Add(cartLine.Id, totals);
                cartLine.SetPolicy(new PurchaseOptionMoneyPolicy());

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var globalPricingPolicy = commerceContext.GetPolicy<GlobalPricingPolicy>();
                globalPricingPolicy.ShouldRoundPriceCalc = false;

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
                action.ApplicableLines(context).ReturnsForAnyArgs(cart.Lines);
                action.CalculateLineDiscount(cart.Lines[0], context).ReturnsForAnyArgs(AMOUNT_OFF);
                //action.WhenForAnyArgs(x => x.ProcessLine(cartTotals, cartLine, new PropertiesModel(), string.Empty, commerceContext, context)).

                /**********************************************
                 * Act
                 **********************************************/
                Action executeAction = () => action.Execute(context);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                action.Received().HasValidRuleValues(context);
                action.Received().ApplicableLines(context);
                action.Received(1).ApplyAdjustmentToLine(
                    cartTotals,
                    cart.Lines[0],
                    commerceContext.GetObject<PropertiesModel>(),
                    commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount,
                    commerceContext,
                    context);
                action.Received().CalculateLineDiscount(cart.Lines[0], context);
                action.Received().ToAdjustmentAmount(AMOUNT_OFF, commerceContext);
                cart.Adjustments.Should().BeEmpty();
                cart.Lines.SelectMany(l => l.Adjustments).Should().HaveCount(1);
                cartLine.Adjustments.FirstOrDefault().Should().NotBeNull();
                cartLine.Adjustments.FirstOrDefault().Should().BeOfType<CartLineLevelAwardedAdjustment>();
                cartLine.Adjustments.FirstOrDefault().Name.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
                cartLine.Adjustments.FirstOrDefault().DisplayName.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
                cartLine.Adjustments.FirstOrDefault().AdjustmentType.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
                cartLine.Adjustments.FirstOrDefault().AwardingBlock.Should().StartWith(nameof(BaseCartLineAction));
                cartLine.Adjustments.FirstOrDefault().IsTaxable.Should().BeFalse();
                cartLine.Adjustments.FirstOrDefault().Adjustment.CurrencyCode.Should().Be(commerceContext.CurrentCurrency());
                cartLine.Adjustments.FirstOrDefault().Adjustment.Amount.Should().Be(-AMOUNT_OFF);
                cartLine.HasComponent<MessagesComponent>().Should().BeTrue();
            }

            [Theory, AutoNSubstituteData]
            public void Execute_ShouldRoundPriceCalc_True(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals,
                Totals totals)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                const decimal AMOUNT_OFF = 0.006m;
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());
                while (cart.Lines.Count > 1)
                {
                    cart.Lines.RemoveAt(0);
                }
                var cartLine = cart.Lines[0];
                cartTotals.Lines.Add(cartLine.Id, totals);
                cartLine.SetPolicy(new PurchaseOptionMoneyPolicy());

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
                action.ApplicableLines(context).ReturnsForAnyArgs(cart.Lines);
                action.CalculateLineDiscount(cart.Lines[0], context).ReturnsForAnyArgs(AMOUNT_OFF);

                /**********************************************
                 * Act
                 **********************************************/
                Action executeAction = () => action.Execute(context);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                action.Received().HasValidRuleValues(context);
                action.Received().ApplicableLines(context);
                action.Received(1).ApplyAdjustmentToLine(
                    cartTotals,
                    cart.Lines[0],
                    commerceContext.GetObject<PropertiesModel>(),
                    commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount,
                    commerceContext,
                    context);
                action.Received().CalculateLineDiscount(cart.Lines[0], context);
                action.Received().ToAdjustmentAmount(AMOUNT_OFF, commerceContext);
                cart.Adjustments.Should().BeEmpty();
                cart.Lines.SelectMany(l => l.Adjustments).Should().HaveCount(1);
                cartLine.Adjustments.FirstOrDefault().Should().NotBeNull();
                cartLine.Adjustments.FirstOrDefault().Should().BeOfType<CartLineLevelAwardedAdjustment>();
                cartLine.Adjustments.FirstOrDefault().Name.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
                cartLine.Adjustments.FirstOrDefault().DisplayName.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
                cartLine.Adjustments.FirstOrDefault().AdjustmentType.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
                cartLine.Adjustments.FirstOrDefault().AwardingBlock.Should().StartWith(nameof(BaseCartLineAction));
                cartLine.Adjustments.FirstOrDefault().IsTaxable.Should().BeFalse();
                cartLine.Adjustments.FirstOrDefault().Adjustment.CurrencyCode.Should().Be(commerceContext.CurrentCurrency());
                cartLine.Adjustments.FirstOrDefault().Adjustment.Amount.Should().Be(-MINIMUM_AMOUNT_OFF);
                cartLine.HasComponent<MessagesComponent>().Should().BeTrue();
            }

            [Theory, AutoNSubstituteData]
            public void Execute_WithProperties(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals,
                Totals totals,
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
                while (cart.Lines.Count > 1)
                {
                    cart.Lines.RemoveAt(0);
                }
                var cartLine = cart.Lines[0];
                cartTotals.Lines.Add(cartLine.Id, totals);
                cartLine.SetPolicy(new PurchaseOptionMoneyPolicy());

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var propertiesModel = new PropertiesModel();
                propertiesModel.Properties.Add("PromotionId", promotionId);
                propertiesModel.Properties.Add("PromotionCartText", promotionCartText);
                propertiesModel.Properties.Add("PromotionText", promotionText);
                commerceContext.AddObject(propertiesModel);

                var action = Substitute.ForPartsOf<BaseCartLineAction>();
                action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
                action.ApplicableLines(context).ReturnsForAnyArgs(cart.Lines);
                action.CalculateLineDiscount(cart.Lines[0], context).ReturnsForAnyArgs(AMOUNT_OFF);

                /**********************************************
                 * Act
                 **********************************************/
                Action executeAction = () => action.Execute(context);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                action.Received().HasValidRuleValues(context);
                action.Received().ApplicableLines(context);
                action.Received(1).ApplyAdjustmentToLine(
                    cartTotals,
                    cart.Lines[0],
                    commerceContext.GetObject<PropertiesModel>(),
                    commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount,
                    commerceContext,
                    context);
                action.Received().CalculateLineDiscount(cart.Lines[0], context);
                action.Received().ToAdjustmentAmount(AMOUNT_OFF, commerceContext);
                cart.Adjustments.Should().BeEmpty();
                cart.Lines.SelectMany(l => l.Adjustments).Should().HaveCount(1);
                cartLine.Adjustments.FirstOrDefault().Should().NotBeNull();
                cartLine.Adjustments.FirstOrDefault().Should().BeOfType<CartLineLevelAwardedAdjustment>();
                cartLine.Adjustments.FirstOrDefault().Name.Should().Be(promotionText);
                cartLine.Adjustments.FirstOrDefault().DisplayName.Should().Be(promotionCartText);
                cartLine.Adjustments.FirstOrDefault().AdjustmentType.Should().Be(commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount);
                cartLine.Adjustments.FirstOrDefault().AwardingBlock.Should().StartWith(nameof(BaseCartLineAction));
                cartLine.Adjustments.FirstOrDefault().IsTaxable.Should().BeFalse();
                cartLine.Adjustments.FirstOrDefault().Adjustment.CurrencyCode.Should().Be(commerceContext.CurrentCurrency());
                cartLine.Adjustments.FirstOrDefault().Adjustment.Amount.Should().Be(-MINIMUM_AMOUNT_OFF);
                cartLine.HasComponent<MessagesComponent>().Should().BeTrue();
                cartLine.GetComponent<MessagesComponent>().Messages.FirstOrDefault(m => m.Text.Contains(promotionId));
            }

            [Theory, AutoNSubstituteData]
            public void Execute_WithConcreteClass(
                IRuleExecutionContext context,
                CommerceContext commerceContext,
                Cart cart,
                CartTotals cartTotals,
                Totals totals)
            {
                /**********************************************
                 * Arrange
                 **********************************************/
                const decimal AMOUNT_OFF = 0.006m;
                cart.Adjustments.Clear();
                cart.Lines.ForEach(l => l.Adjustments.Clear());
                while (cart.Lines.Count > 1)
                {
                    cart.Lines.RemoveAt(0);
                }
                var cartLine = cart.Lines[0];
                cartTotals.Lines.Add(cartLine.Id, totals);
                cartLine.SetPolicy(new PurchaseOptionMoneyPolicy());

                context.Fact<CommerceContext>().ReturnsForAnyArgs(commerceContext);
                commerceContext.AddObject(cart);
                commerceContext.AddObject(cartTotals);

                var action = Substitute.ForPartsOf<ConcreteCartLineAction>();
                action.HasValidRuleValues(context).ReturnsForAnyArgs(true);
                action.ApplicableLines(context).ReturnsForAnyArgs(cart.Lines);
                action.CalculateLineDiscount(cart.Lines[0], context).ReturnsForAnyArgs(AMOUNT_OFF);

                /**********************************************
                 * Act
                 **********************************************/
                Action executeAction = () => action.Execute(context);

                /**********************************************
                 * Assert
                 **********************************************/
                executeAction.Should().NotThrow<Exception>();
                action.Received().HasValidRuleValues(context);
                action.Received().ApplicableLines(context);
                action.Received(1).ApplyAdjustmentToLine(
                    cartTotals,
                    cart.Lines[0],
                    commerceContext.GetObject<PropertiesModel>(),
                    commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount,
                    commerceContext,
                    context);
                action.Received().CalculateLineDiscount(cart.Lines[0], context);
                action.Received().ToAdjustmentAmount(AMOUNT_OFF, commerceContext);
                cart.Adjustments.Should().BeEmpty();
                cart.Lines.SelectMany(l => l.Adjustments).Should().HaveCount(1);
                cartLine.Adjustments.FirstOrDefault().Should().NotBeNull();
                cartLine.Adjustments.FirstOrDefault().Should().BeOfType<CartLineLevelAwardedAdjustment>();
                cartLine.Adjustments.FirstOrDefault().AwardingBlock.Should().StartWith(nameof(ConcreteCartLineAction));
            }
        }
    }
}