﻿using BannerKings.Managers.Education;
using BannerKings.Managers.Skills;
using HarmonyLib;
using Helpers;
using SandBox.Tournaments.MissionLogics;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.TournamentGames;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace BannerKings.Behaviours
{
    public class BKTournamentBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.TournamentFinished.AddNonSerializedListener(this, OnTournamentFinished);
        }

        public override void SyncData(IDataStore dataStore)
        {
        }

        private void OnTournamentFinished(CharacterObject winner, MBReadOnlyList<CharacterObject> participants, Town town,
            ItemObject prize)
        {
            if (BannerKingsConfig.Instance.PopulationManager == null)
            {
                return;
            }

            if (participants.Contains(Hero.MainHero.CharacterObject))
            {
                EducationData education = BannerKingsConfig.Instance.EducationManager.GetHeroEducation(Hero.MainHero);
                if (education.HasPerk(BKPerks.Instance.GladiatorTourDeCalradia))
                {
                    Town resultTown = SettlementHelper.FindNearestTown((Settlement s) => 
                    { 
                        return s.Town.HasTournament; 
                    }, 
                    null
                    ).Town;

                    TournamentGame game = Campaign.Current.TournamentManager.GetTournamentGame(resultTown);
                    if (resultTown != null)
                    {
                        InformationManager.ShowTextInquiry(new TextInquiryData(
                            new TextObject("{=!}Nearest Tournament").ToString(),
                            new TextObject("{=!}As a known gladiator, you are informed that {TOWN} holds the nearest tournament match. It's prize is {PRIZE}")
                            .SetTextVariable("TOWN", resultTown.Name)
                            .SetTextVariable("PRIZE", game.Prize.Name)
                            .ToString(),
                            true,
                            false,
                            GameTexts.FindText("str_ok").ToString(),
                            string.Empty,
                            null,
                            null
                            ));
                    }
                }

                if (winner == Hero.MainHero.CharacterObject && education.HasPerk(BKPerks.Instance.GladiatorPromisingAthlete))
                {
                    Hero notable = town.Settlement.Notables.GetRandomElement();
                    ChangeRelationAction.ApplyPlayerRelation(notable, 2);
                }
            }

            var data = BannerKingsConfig.Instance.PopulationManager.GetPopData(town.Settlement);
            var tournament = data.TournamentData;
            if (tournament is {Active: true})
            {
                float price = town.MarketData.GetPrice(prize);
                var renown = -10f;
                if (price <= 10000)
                {
                    renown += price / 1000f;
                }
                else
                {
                    renown += price / 10000f;
                }

                GainRenownAction.Apply(Hero.MainHero, renown, true);
                InformationManager.DisplayMessage(new InformationMessage(string
                    .Format("Your prize of choice for the tournament at {0} has awarded you {1} renown", renown,
                        town.Name)));
                tournament.Active = false;
            }
        }
    }

    namespace Patches
    {
        [HarmonyPatch(typeof(TournamentBehavior), "GetExpectedDenarsForBet")]
        internal class GetExpectedDenarsForBetlPatch
        {
            private static void Postfix(ref int result, int bet)
            {
                EducationData education = BannerKingsConfig.Instance.EducationManager.GetHeroEducation(Hero.MainHero);
                if (education.HasPerk(BKPerks.Instance.GladiatorPromisingAthlete))
                {
                    int baseResult = result;
                    result = (int)(baseResult * 1.3f);
                }
            }
        }

        [HarmonyPatch(typeof(TournamentBehavior), "GetMaximumBet")]
        internal class GetMaximumBetlPatch
        {
            private static void Postfix(ref int result)
            {
                EducationData education = BannerKingsConfig.Instance.EducationManager.GetHeroEducation(Hero.MainHero);
                if (education.HasPerk(BKPerks.Instance.GladiatorTourDeCalradia))
                {
                    result += 150;
                }
            }
        }
    }
}