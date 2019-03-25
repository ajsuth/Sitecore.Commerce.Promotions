using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Promotions;
using Sitecore.Commerce.Plugin.Rules;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using Sitecore.Framework.Rules;
using Sitecore.Framework.Rules.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Feature.Promotions.Engine.Pipelines.Blocks
{
	[PipelineDisplayName("Promotions.block.evaluatepromotions")]
	public class EvaluatePromotionsBlock : PipelineBlock<IEnumerable<Promotion>, IEnumerable<Promotion>, CommercePipelineExecutionContext>
	{
		private readonly IRuleMetadataMapper _ruleMapper;
		private readonly IRunRuleSetPipeline _runRuleSetPipeline;

		public EvaluatePromotionsBlock(IRuleMetadataMapper ruleMapper, IRunRuleSetPipeline runRuleSetPipeline)
		  : base((string)null)
		{
			this._ruleMapper = ruleMapper;
			this._runRuleSetPipeline = runRuleSetPipeline;
		}

		public override async Task<IEnumerable<Promotion>> Run(IEnumerable<Promotion> arg, CommercePipelineExecutionContext context)
		{
			var promotions = arg as List<Promotion> ?? arg.ToList();
			Condition.Requires(promotions).IsNotNull((Name) + ": The block argument cannot be null.");
			var qualifyingPromotions = new List<Promotion>();
			if (!promotions.Any())
			{
				return qualifyingPromotions.AsEnumerable();
			}

			context.Logger.LogDebug((Name) + ".EvaluatePromotions", Array.Empty<object>());
			foreach (var promotion in promotions)
			{
				if (!promotion.HasPolicy<PromotionQualificationsPolicy>() || !promotion.GetPolicy<PromotionQualificationsPolicy>().Qualifications.Any<ConditionModel>() || !promotion.HasComponent<PromotionRulesComponent>())
				{
					await context.CommerceContext.AddMessage(
						context.GetPolicy<KnownResultCodes>().Warning,
						"PromotionNotEvaluated",
						new object[1] { promotion.FriendlyId },
						$"Promotion {promotion.FriendlyId} is not going to be evaluated since it has no qualifications or benefits.");
				}
				else
				{
					var qualifyingRule = promotion.GetComponent<PromotionRulesComponent>().QualifyingRule as MappedRuleMetadata;
					if (qualifyingRule == null)
					{
						await context.CommerceContext.AddMessage(
							context.GetPolicy<KnownResultCodes>().Warning,
							"PromotionNotEvaluated", 
							new object[1] { promotion.FriendlyId },
							$"Promotion {promotion.FriendlyId} is not going to be evaluated since it has no benefits or qualifications.");
					}
					else
					{
						var rule = _ruleMapper.MapToRule(qualifyingRule);
						var ruleSet = new RuleSet()
						{
							Rules = new List<IRule>()
							{
								rule
							}
						};
						var promotionsArgument = context.CommerceContext.GetObjects<EvaluatePromotionsArgument>().FirstOrDefault();
						var objectList = promotionsArgument?.Facts.ToList();
						var propertiesModel = new PropertiesModel();
						propertiesModel.Properties.Add("PromotionId", promotion.Id);
						propertiesModel.Properties.Add("PromotionCartText", promotion.DisplayCartText);
						propertiesModel.Properties.Add("PromotionText", promotion.DisplayText);
						if (promotion.HasComponent<PromotionItemsComponent>())
						{
							propertiesModel.Properties.Add("PromotionItems", promotion.GetComponent<PromotionItemsComponent>());
						}

						objectList?.Add(propertiesModel);
						var runRuleSetArgument = new RunRuleSetArgument(ruleSet) { Facts = objectList };
						var source2 = await _runRuleSetPipeline.Run(runRuleSetArgument, context);
						if (source2.All(r => r))
						{
							qualifyingPromotions.Add(promotion);
						}
					}
				}
			}

			context.Logger.LogDebug((Name) + ".PromotionsEvaluated");

			return qualifyingPromotions.AsEnumerable();
		}
	}
}
