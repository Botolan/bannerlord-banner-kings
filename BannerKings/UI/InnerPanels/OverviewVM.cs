﻿using BannerKings.Models;
using BannerKings.Populations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using static BannerKings.Managers.PopulationManager;

namespace BannerKings.UI
{
    public class OverviewVM : ViewModel
    {
        private MBBindingList<PopulationInfoVM> _popInfo;
        private MBBindingList<InformationElement> _satisfactionInfo;
        private MBBindingList<InformationElement> _statsInfo;
        private MBBindingList<InformationElement> _foodInfo;
        private MBBindingList<InformationElement> _productionInfo;
        private MBBindingList<InformationElement> _defenseInfo;
        private Settlement _settlement;
        private bool _isSelected;
        private PopulationData data;

        public OverviewVM(Settlement _settlement, bool _isSelected)
        {
            _defenseInfo = new MBBindingList<InformationElement>();
            _popInfo = new MBBindingList<PopulationInfoVM>();
            _satisfactionInfo = new MBBindingList<InformationElement>();
            _statsInfo = new MBBindingList<InformationElement>();
            _foodInfo = new MBBindingList<InformationElement>();
            _productionInfo = new MBBindingList<InformationElement>();
            this._settlement = _settlement;
            this._isSelected = _isSelected;
            this.RefreshValues();
        }

        public override void RefreshValues()
        {
            base.RefreshValues();
            PopulationData data = BannerKingsConfig.Instance.PopulationManager.GetPopData(_settlement);
            this.data = data;
            PopInfo.Clear();
            SatisfactionInfo.Clear();
            StatsInfo.Clear();
            FoodInfo.Clear();
            ProductionInfo.Clear();
            DefenseInfo.Clear();
            if (data != null && data.Classes != null)
            {
                data.Classes.ForEach(popClass => PopInfo
                .Add(new PopulationInfoVM(Helpers.Helpers.GetClassName(popClass.type, _settlement.Culture).ToString(), popClass.count,
                    Helpers.Helpers.GetClassHint(popClass.type, _settlement.Culture))));

              

                StatsInfo.Add(new InformationElement("Stability:", FormatValue(data.Stability),
                    "The overall stability of this settlement, affected by security, loyalty, assimilation and whether you are legally entitled to the settlement. Stability is the basis of economic prosperity"));
                StatsInfo.Add(new InformationElement("Population Growth:", new BKGrowthModel().CalculateEffect(_settlement, data).ResultNumber.ToString(), 
                    "The population growth of your settlement on a daily basis, distributed among the classes"));
                StatsInfo.Add(new InformationElement("Administrative Cost:", FormatValue(new AdministrativeModel().CalculateAdministrativeCost(_settlement)),
                    "Costs associated with the settlement administration, including those of active policies and decisions, deducted on tax revenue"));
                StatsInfo.Add(new InformationElement("Cultural Assimilation:", FormatValue(data.CultureData.GetAssimilation(Hero.MainHero.Culture)),
                    "Percentage of the population that shares culture with you. Assimilating foreign settlements requires a competent governor that shares your culture"));

                FoodInfo.Add(new InformationElement("Storage Limit:", _settlement.Town.FoodStocksUpperLimit().ToString(), 
                    "The amount of food this settlement is capable of storing"));
                FoodInfo.Add(new InformationElement("Estimated Holdout:", string.Format("{0} Days", new BKFoodModel().GetFoodEstimate(_settlement.Town, true, _settlement.Town.FoodStocksUpperLimit())),
                    "How long this settlement will take to start starving in case of a siege"));

               
                //ProductionInfo.Add(new InformationElement("Population Cap:", new GrowthModel().CalculateSettlementCap(_settlement).ToString(),
                //    "The maximum capacity of people this settlement can naturally support"));


                DefenseInfo.Add(new InformationElement("Militia Cap:", new BKMilitiaModel().GetMilitiaLimit(data, _settlement.IsCastle).ToString(),
                    "The maximum number of militiamen this settlement can support, based on it's population"));
                DefenseInfo.Add(new InformationElement("Militia Quality:", FormatValue(new BKMilitiaModel().CalculateEliteMilitiaSpawnChance(_settlement)),
                        "Chance of militiamen being spawned as veterans instead of recruits"));
            } 
        }

        private string FormatValue(float value) => (value * 100f).ToString("0.00") + '%';

        [DataSourceProperty]
        public bool IsSelected
        {
            get => this._isSelected;
            set
            {
                if (value != this._isSelected)
                {
                    this._isSelected = value;
                    if (value) this.RefreshValues();
                    base.OnPropertyChangedWithValue(value, "IsSelected");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<PopulationInfoVM> PopInfo
        {
            get => _popInfo;
            set
            {
                if (value != _popInfo)
                {
                    _popInfo = value;
                    base.OnPropertyChangedWithValue(value, "PopInfo");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<InformationElement> DefenseInfo
        {
            get => _defenseInfo;
            set
            {
                if (value != _defenseInfo)
                {
                    _defenseInfo = value;
                    base.OnPropertyChangedWithValue(value, "DefenseInfo");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<InformationElement> ProductionInfo
        {
            get => _productionInfo;
            set
            {
                if (value != _productionInfo)
                {
                    _productionInfo = value;
                    base.OnPropertyChangedWithValue(value, "ProductionInfo");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<InformationElement> FoodInfo
        {
            get => _foodInfo;
            set
            {
                if (value != _foodInfo)
                {
                    _foodInfo = value;
                    base.OnPropertyChangedWithValue(value, "FoodInfo");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<InformationElement> SatisfactionInfo
        {
            get => _satisfactionInfo;
            set
            {
                if (value != _satisfactionInfo)
                {
                    _satisfactionInfo = value;
                    base.OnPropertyChangedWithValue(value, "SatisfactionInfo");
                }
            }
        }

        [DataSourceProperty]
        public MBBindingList<InformationElement> StatsInfo
        {
            get => _statsInfo;
            set
            {
                if (value != _statsInfo)
                {
                    _statsInfo = value;
                    base.OnPropertyChangedWithValue(value, "StatsInfo");
                }
            }
        }

        [DataSourceProperty]
        public string AdministrativeCost
        {
            get
            {
                float cost = new AdministrativeModel().CalculateAdministrativeCost(_settlement);
                return FormatValue(cost);
            }
        }

        [DataSourceProperty]
        public string PopGrowth
        {
            get
            {
                int growth = (int)new BKGrowthModel().CalculateEffect(_settlement, data).ResultNumber;
                return growth.ToString() + " (Daily)";
            }
        }

        [DataSourceProperty]
        public string Assimilation
        {
            get
            {
                float result = BannerKingsConfig.Instance.PopulationManager.GetPopData(_settlement).CultureData.GetAssimilation(Hero.MainHero.Culture);
                return (result * 100f).ToString() + '%';
            }
        }
    }
}
