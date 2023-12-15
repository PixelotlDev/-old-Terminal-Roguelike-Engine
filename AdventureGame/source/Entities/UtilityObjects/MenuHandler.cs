//TODO: have some data be filled in via json

/*namespace AdventureGame
{
    enum Menus
    {
        Pause,
        Inventory,
        Options
    }

    enum InventoryPages
    {
        Weapons,
        Armour,
        Potions,
        Food
    }

    enum OptionsPages
    {
        General,
        Audio,
        Display,
        Controls
    }

    class PauseMenu
    {
        public string menuName;

        public PauseMenu(string name)
        {
            menuName = name;
        }
    }


    class PauseMenu<T> : PauseMenu where T : Enum
    {
        public T currentPage;

        public PauseMenu(T page, string name)
            : base(name)
        {
            currentPage = page;
        }
    }

    internal class MenuHandler : UtilityObject
    {
        private readonly Player player;
        private readonly UIHandler UI;

        private readonly PauseMenu[] pauseMenus;

        private readonly Menus currentMenu = 0;

        private readonly ItemTypes[] weaponTypes = new ItemTypes[] { ItemTypes.StrWeapon, ItemTypes.DexWeapon, ItemTypes.AimWeapon };
        private readonly ItemTypes[] armourTypes = new ItemTypes[] { ItemTypes.Armour };
        private readonly ItemTypes[] potionTypes = new ItemTypes[] { ItemTypes.Potion };
        private readonly ItemTypes[] foodTypes = new ItemTypes[] { ItemTypes.Food };

        private readonly ItemTypes[][] itemTypeArrays;

        public MenuHandler(World world, Player player, UIHandler UI, Tags tag = Tags.Default, string name = "")
            : base(world, tag, name)
        {
            this.player = player;
            this.UI = UI;

            pauseMenus = new PauseMenu[] { new PauseMenu("Pause"), new PauseMenu<InventoryPages>(0, "Backpack"), new PauseMenu<OptionsPages>(0, "Options") };
            itemTypeArrays = new ItemTypes[][] { weaponTypes, armourTypes, potionTypes, foodTypes };
        }

        public override void RenderUpdate()
        {
            UpdateUILabels();
        }

        public void NextPage()
        {
            if (pauseMenus[(int)currentMenu].GetType().IsSubclassOf(typeof(PauseMenu)))
            {
                pauseMenus[(int)currentMenu] = pauseMenus[(int)currentMenu].Next();

                int currentPage = Convert.ToInt32(pauseMenus[(int)currentMenu]);
                UI.itemNameArrays[currentPage] = GetNamesOfItems(player.inventory.GetItemsByType(itemTypeArrays[currentPage]));
            }
            
        }

        public void PrevPage()
        {
            currentMenuPage = currentMenuPage.Prev();
        }

        public void UpdateUILabels()
        {
            // update UI headers
            // if this menu ever changes (e.g if you pause and the pause menu appears in the left sidebar) we can add
            // something to change this value, but for now it can just be "BACKPACK"
            UI.LeftSidebarHeader = "BACKPACK";
            // later, this will change to the name of an item whose stats and description you're looking at
            UI.RightSidebarHeader = "STATISTICS";

            // update UI labels
            UI.NavigatorLabelMiddle = currentMenuPage.ToString().Truncate(12);
            UI.NavigatorLabelLeft = currentMenuPage.Prev().ToString().Truncate(UI.smallNavigatorDimensions.Width);
            UI.NavigatorLabelRight = currentMenuPage.Next().ToString().Truncate(UI.smallNavigatorDimensions.Width);
        }

        public string[] GetNamesOfItems(Item[] items)
        {
            string[] itemNames = new string[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                if (!items[i].Equals(new Item())) { itemNames[i] = items[i].name; }
            }

            return itemNames;
        }
    }
}*/
