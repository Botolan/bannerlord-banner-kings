﻿using BannerKings.Populations;
using BannerKings.UI.Panels;
using SandBox.View.Map;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace BannerKings.UI.Windows
{
    public class BannerKingsMapView : MapView
    {
        private BannerKingsViewModel datasource;
        private GauntletLayer layer;
        public string id;

        public BannerKingsMapView(string id)
        {
            this.id = id;
            this.CreateLayout();
        }

        protected override void CreateLayout()
        {
            base.CreateLayout();
            layer = new GauntletLayer(550, "GauntletLayer", false);
            ValueTuple<BannerKingsViewModel, string> tuple = this.GetVM(this.id);
            datasource = tuple.Item1;
            layer.LoadMovie(tuple.Item2, datasource);

            layer.InputRestrictions.SetInputRestrictions(false, InputUsageMask.All);
            MapScreen.Instance.AddLayer(layer);
            ScreenManager.TrySetFocus(layer);
        }

        private (BannerKingsViewModel, string) GetVM(string id)
        {
            PopulationData data = BannerKingsConfig.Instance.PopulationManager.GetPopData(Settlement.CurrentSettlement);
            if (id == "population")
                return (new PopulationVM(data), "PopulationWindow");
            else if (id == "guild")
                return (new GuildVM(data), "GuildWindow");
            else if (id == "vilage_project")
                return (new VillageProjectVM(data), "VillageProjectWindow");
            else if (id == "court")
                return (new CourtVM(data), "CourtWindow");
            else if (id == "titles")
                return (new TitleWindowVM(data), "TitlesWindow");
            else if (id == "religions")
                return (new ReligionVM(data), "ReligionWindow");
            else return (new PopulationVM(data), "PopulationWindow");
        }

        public void Close() => MapScreen.Instance.RemoveLayer(layer);
        public void Refresh() => this.datasource.RefreshValues();
    }
}
