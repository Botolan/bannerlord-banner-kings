﻿using BannerKings.Managers.Court;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.TownManagement;
using TaleWorlds.Library;

namespace BannerKings.UI.Items
{
    public class CouncilVM : SettlementGovernorSelectionVM
    {
        private Action<Hero> onDone;
        private CouncilData council;
        private List<Hero> courtMembers;
        public CouncilPosition Position { get; set; }

        public CouncilVM(Action<Hero> onDone, CouncilData council, CouncilPosition position, List<Hero> courtMembers) : base(null, onDone)
        {
            this.onDone = onDone;
            this.council = council;
            this.Position = position;
            this.courtMembers = courtMembers;
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            List<Hero> currentCouncil = council.GetMembers();
            CouncilMember currentMember = council.GetCouncilMember(Position);
            MBBindingList<SettlementGovernorSelectionItemVM> newList = new MBBindingList<SettlementGovernorSelectionItemVM>();
            newList.Add(this.AvailableGovernors[0]);
            foreach (Hero hero in this.courtMembers)
                if (!currentCouncil.Contains(hero) && hero.IsAlive && !hero.IsChild && currentMember.IsValidCandidate(hero))
                    newList.Add(new CouncilMemberVM(hero, new Action<SettlementGovernorSelectionItemVM>(this.OnSelection),
                                    Position, council.GetCompetence(hero, Position)));

            this.AvailableGovernors = newList;
        }

        private void OnSelection(SettlementGovernorSelectionItemVM item)
        {
            onDone(item.Governor);
        }
    }
}
