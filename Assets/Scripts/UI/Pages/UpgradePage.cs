using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
namespace DarkJimmy.UI
{
    public class UpgradePage : TabGenerator<TabButton, CharacterData>
    {
        [Header("Upgrade Page")]
        [SerializeField]
        private StatsUISlider statSlider;
        [SerializeField]
        private TMP_Text descriptionText;

        [SerializeField]
        private Image mainPriceIcon;
        [SerializeField]
        private TMP_Text mainPriceText;
        [SerializeField]
        private Image stoneIcon;
        [SerializeField]
        private TMP_Text stonePriceText;
        [SerializeField]
        private Image philosopyIcon;
        [SerializeField]
        private TMP_Text philosophyPriceText;
        [SerializeField]
        private BaseButton upgradeButton;
        [SerializeField]
        private TMP_Text currentValueText;
        [SerializeField]
        private TMP_Text nextValueText;
        [SerializeField]
        private TMP_Text upgradePayText;

        [SerializeField]
        private GameObject pricePanel;
        [SerializeField]
        private GameObject maxPanel;
        [SerializeField]
        private TMP_Text maxInfoText;

        [SerializeField]
        private string philoColor;
        [SerializeField]
        private string moonColor;
        [SerializeField]
        private string rockColor;
        [SerializeField]
        private string powerColor;
        private HorizontalLayoutGroup layout;
        private SystemManager system;
        private CloudSaveManager csm;

        private int mainPrice;
        private int stonePrice;
        private int philosophyPrice;
        private int nextValue;
        private bool isMaxValue = false;
        private int currentSkillLevel;

        private CharacterProperty CurrentProperty { get; set; }
    
        private  void Start()
        {
            system = SystemManager.Instance;
            csm = CloudSaveManager.Instance;
            globalData = csm.GetCurrentCharacterData();
            layout = container.GetComponent<HorizontalLayoutGroup>();
            Generate();
            upgradePayText.text = $"{LanguageManager.GetText("UpgradePay")}:";
            maxInfoText.text = LanguageManager.GetText("MaxSkillLevel");
            upgradeButton.OnClick(OnUpgrade);

            system.priceCalculate += PriceCalculate;
        }
        public override void Generate()
        {
            int count = Enum.GetNames(typeof(CharacterProperty)).Length;

            for (int i = 0; i < count; i++)
            {
                CharacterProperty property= (CharacterProperty)i;

                TabButton tab = Instantiate(prefab, container);
                tab.SetTabButtonName(property.ToString());

                tab.SetTabIcon(system.GetChracterPropSprite(property));
                tab.OnClick(i, OnSelect);
                tabs.Add(tab);
            }

            OnSelect(NextIndex);
        }

        public override void OnSelect(int index)
        {
            base.OnSelect(index);

            CurrentProperty = (CharacterProperty)NextIndex;
            TabButton prevTab = GetTab(PreviousIndex);
            TabButton nextTab = GetTab(NextIndex);

            prevTab.SetTabButton(false);
            nextTab.SetTabButton(true);

            SetStatus(CurrentProperty);
            PriceAndSkillUpgradeValueCalculate(CurrentProperty);

            layout.enabled = false;
            layout.enabled = true;
            Canvas.ForceUpdateCanvases();
        }

        private void SetStatus(CharacterProperty property)
        {
            if (Enum.TryParse(property.ToString(), out Stats stats))
            {
                statSlider.Stats = stats;
                statSlider.SetStatName(stats.ToString());
                statSlider.SetStatsIcon();
               // statSlider.SetSliderValues(globalData.GetCurrentCharacterProperty(property), globalData.GetMaxCapacity(property));
                // 
               system.updateStatsMax(stats, globalData.GetCurrentCharacterProperty(property), globalData.GetMaxCapacity(property));
            }
        }

        private void PriceCalculate()
        {
            PriceAndSkillUpgradeValueCalculate(CurrentProperty);
        }

        private void PriceAndSkillUpgradeValueCalculate(CharacterProperty property)
        {
            int currentValue = globalData.GetCurrentCharacterProperty(property);
            isMaxValue = currentValue >= globalData.GetMaxCapacity(property);
            currentSkillLevel = globalData.GetCurrentSkillLevel(property);
            nextValue = isMaxValue ? currentValue : globalData.GetCharacterProperty(property,currentSkillLevel +1);

            string name = isMaxValue ? "Max" : "Upgrade";
            maxPanel.SetActive(isMaxValue);
            pricePanel.SetActive(!isMaxValue);
            upgradeButton.SetTabButtonName(name);
            upgradeButton.button.interactable = !isMaxValue;

            if (Enum.TryParse(GetStone(property).ToString(),out Stats stats))
                 stoneIcon.sprite = system.GetStatsIcon(stats);

            currentValueText.text = $"{currentValue}/{globalData.GetMaxCapacity(property)}";
            nextValueText.text = $"{nextValue}/{globalData.GetMaxCapacity(property)}";
          
            mainPrice = (currentSkillLevel+1) * 250;
            int _stonePrice = (currentSkillLevel+1) * 150;

            philosophyPrice = csm.GetStoneCount(GetStone(property))>= _stonePrice ? 0 : _stonePrice - csm.GetStoneCount(GetStone(property));

            stonePrice = philosophyPrice > 0 ? csm.GetStoneCount(GetStone(property)) : _stonePrice;

            mainPriceText.text = $"x{mainPrice}";

            string _stonPriceText = philosophyPrice > 0 && csm.GetStoneCount(Stones.Philosophy)>=philosophyPrice ? $"<color={GetColor(GetStone(property))}>x{csm.GetStoneCount(GetStone(property))}</color> + <color={GetColor(Stones.Philosophy)}>x{philosophyPrice}</color> / <color={GetColor(GetStone(property))}>{_stonePrice}</color>" : 
                
                
                $"<color={GetColor(GetStone(property))}>x{csm.GetStoneCount(GetStone(property))}</color> / <color={GetColor(GetStone(property))}>{_stonePrice}</color>";

            stonePriceText.text = _stonPriceText;
            philosophyPriceText.text = $"x{philosophyPrice}";

            //<color=green>{emblemCount}/{price}</color>
        }


        private void OnUpgrade()
        {
            if (isMaxValue)
                return;

            if (csm.CanSpendGem(GemType.Gold,mainPrice))
            {
                if (philosophyPrice > 0)
                {
                    if (!csm.CanSpendStone(GetStone(CurrentProperty), csm.GetStoneCount(GetStone(CurrentProperty))) || !csm.CanSpendStone(Stones.Philosophy, philosophyPrice))
                    {
                        UIManager.Instance.PageIndex = 2;
                        UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);
                        return;
                    }

                    csm.SpendGem(GemType.Gold, mainPrice);
                    csm.SpendStones(Stones.Philosophy, philosophyPrice);
                    csm.SpendStones(GetStone(CurrentProperty), stonePrice);
                }
                else
                {
  
                    if (!csm.CanSpendStone(GetStone(CurrentProperty), csm.GetStoneCount(GetStone(CurrentProperty))))
                    {
                        UIManager.Instance.PageIndex = 2;
                        UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);
                        return;
                    }

                    csm.SpendGem(GemType.Gold, mainPrice);
                    csm.SpendStones(GetStone(CurrentProperty), stonePrice);
                }

                globalData.GetCurrentSkillLevel(CurrentProperty) += 1;

                csm.SetCharacterData(csm.GetCurrentCharacterIndex(),globalData);

                OnSelect(NextIndex);
            }
            else
            {
                UIManager.Instance.PageIndex = 0;
                UIManager.Instance.OpenMenu(Menu.Menus.ShopOrientation);           
            }
        }
        private Stones GetStone(CharacterProperty property)
        {
            return property switch
            {
                CharacterProperty.Mana => Stones.PowerCrystal,
                CharacterProperty.Speed => Stones.Moonstone,
                _ => Stones.LifeCrystal,
            };
        }
        private Stats GetStoneStats(Stones stone)
        {
            return stone switch
            {
                Stones.PowerCrystal => Stats.Mana,
                Stones.Moonstone => Stats.Speed,
                Stones.Philosophy=>Stats.Philosophy,
                _ => Stats.HP,
            };
        }
        private string GetColor(Stones stone)
        {
            return stone switch
            {
                Stones.PowerCrystal => $"#{powerColor}",
                Stones.Moonstone => $"#{moonColor}",
                Stones.LifeCrystal=> $"#{rockColor}",
                _ => $"#{philoColor}",
            };
        }

        private void OnDestroy()
        {
            system.priceCalculate -= PriceCalculate;
        }

    }


}

