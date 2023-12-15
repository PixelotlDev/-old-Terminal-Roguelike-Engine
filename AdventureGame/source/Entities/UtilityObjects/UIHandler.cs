using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AdventureGame
{
    internal class UIHandler : UtilityObject
    {
        public readonly Player player;

        private readonly Root gameRoot;
        private readonly Window window;
        private readonly ItemList itemList;
        private readonly SettingsList settingsList;

        string assetsPath = "assets/UI/";

        public UIHandler(World world, Tags tag = Tags.Default, string name = "")
            : base(world, tag, name)
        {
            player = (Player)world.FindEntityWithTag(Tags.Player);

            gameRoot = new();
            window = new(gameRoot, (BoxContainerStyle)0b01, Align.topCentre, new(0, 10), new(58, 11), 14, 1);

            // base-level containers
            BoxContainer screenBox = new(gameRoot, 0b00, Align.topLeft, new(3, 1), new(120, 31));

            // screen box
            BoxContainer leftSidebar = new(screenBox, 0b00, Align.topLeft, new(0, 0), new(13, 27));
            BoxContainer rightSidebar = new(screenBox, 0b00, Align.topRight, new(0, 0), new(13, 27));
            BoxContainer bottomBar = new(screenBox, 0b00, Align.bottomCentre, new(0, 0), new(120, 5));

            // left sidebar
            new Image(leftSidebar, GetImageFromFile(assetsPath + "LeftTitle.txt"), Align.topCentre, new(0, 1)); // left title

            // right sidebar
            new Image(rightSidebar, GetImageFromFile(assetsPath + "RightTitle.txt"), Align.topCentre, new(0, 1)); // right title

            // bottom bar
            new Label(bottomBar, "This will be a console someday", AlignText.centre, Align.centre, new(0, 0), 116); // console stand-in

            // window

            // main tab
            Tab mainTab = new(window, new(58, 11), "Main");

            Button backButton = new(mainTab, (BoxContainerStyle)0b10, Align.topCentre, new(0, 2), new(16, 3));
            new Label(backButton, "Back", AlignText.centre, Align.centre, new(0, 0), 14); //backButtonLabel

            Button quitButton = new(mainTab, (BoxContainerStyle)0b10, Align.bottomCentre, new(0, 2), new(16, 3));
            new Label(quitButton, "Quit", AlignText.centre, Align.centre, new(0, 0), 14); //quitButtonLabel

            // backpack tab
            Tab backpackTab = new(window, new(70, 17), "Backpack");

            itemList = new(backpackTab, player, (BoxContainerStyle)0b01, Align.centreLeft, new(16, 0), new(26, 17), new(17, 3));
            new ItemTab(itemList, "Accessories"); //itemTab1
            new ItemTab(itemList, "Armour");      //itemTab2
            new ItemTab(itemList, "Food");        //itemTab3
            new ItemTab(itemList, "Keys");        //itemTab4
            new ItemTab(itemList, "Materials");   //itemTab5
            new ItemTab(itemList, "Potions");     //itemTab6
            new ItemTab(itemList, "Scrolls");     //itemTab7
            new ItemTab(itemList, "Weapons");     //itemTab8

            // stats tab
            Tab statsTab = new(window, new(70, 19), "Statistics");

            BoxContainer statsBox = new BoxContainer(statsTab, (BoxContainerStyle)0b01, Align.centreLeft, new(0, 0), new(22, 19));
            new Label(statsBox, "[player name]", AlignText.centre, Align.topCentre, new(0, 1), 18);
            new Label(statsBox, "- Attack: " + player.Attack, AlignText.left, Align.topLeft, new(2, 3), 18);
            new Label(statsBox, "- Defence: " + player.Defence, AlignText.left, Align.topLeft, new(2, 5), 18);
            new Label(statsBox, "- Speed: " + player.Speed, AlignText.left, Align.topLeft, new(2, 7), 18);
            new Label(statsBox, "- Satiety: " + player.Satiety + "/100", AlignText.left, Align.bottomLeft, new(2, 2), 18);
            new Label(statsBox, "- Weight: " + player.inventory.CurrentWeight + "/" + player.inventory.WeightCap, AlignText.left, Align.bottomLeft, new(2, 1), 18);

            BoxContainer equipBox = new BoxContainer(statsTab, (BoxContainerStyle)0b01, Align.centreRight, new(0, 0), new(49, 19), -1);

            player.inventory.AddMoney(363899345745);
            string valueText = player.inventory.Money.ToString();
            if (player.inventory.Money > 999999999999) { valueText = "∞"; }
            else if (player.inventory.Money > 999999999) { valueText = valueText.Substring(0, valueText.Length - 9) + "." + valueText.Substring(valueText.Length - 9, 1) + "B"; }
            else if (player.inventory.Money > 999999) { valueText = valueText.Substring(0, valueText.Length - 6) + "." + valueText.Substring(valueText.Length - 6, 1) + "M"; }
            else if (player.inventory.Money > 999) { valueText = valueText.Substring(0, valueText.Length - 3) + "." + valueText.Substring(valueText.Length - 3, 1) + "K"; }
            new Label(equipBox, valueText + "☼", AlignText.right, Align.topRight, new(2, 1), 20, false);

            new Label(equipBox, "Health:", AlignText.centre, Align.topCentre, new(0, 1), 7);

            BoxContainer healthBox = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topCentre, new(0, 2), new(41, 3));
            player.Damage(60);
            string healthString = new('█', (int)Math.Round(39 * ((float)player.Health / player.MaxHealth)));
            healthString += new string('░', 39 - healthString.Length);
            new Label(healthBox, healthString, AlignText.centre, Align.centre, new(0, 0), 39);
            new Label(healthBox, player.Health + "/" + player.MaxHealth, AlignText.centre, Align.centre, new(0, 0), 39, zLayer: 1);

            new Label(equipBox, "Head:", AlignText.centre, Align.topLeft, new(7, 5), 5);
            BoxContainer headBox = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topLeft, new(5, 6), new(9, 5));
            new Image(headBox, GetImageFromFile("assets/items/armour/head/head.txt"), Align.centre, new(0, 0));

            new Label(equipBox, "Body:", AlignText.centre, Align.topLeft, new(17, 5), 5);
            BoxContainer bodyBox = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topLeft, new(15, 6), new(9, 5));
            new Image(bodyBox, GetImageFromFile("assets/items/armour/body/body.txt"), Align.centre, new(0, 0));

            new Label(equipBox, "Legs:", AlignText.centre, Align.topLeft, new(27, 5), 5);
            BoxContainer legsBox = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topLeft, new(25, 6), new(9, 5));
            new Image(legsBox, GetImageFromFile("assets/items/armour/legs/legs.txt"), Align.centre, new(0, 0));

            new Label(equipBox, "Feet:", AlignText.centre, Align.topLeft, new(37, 5), 5);
            BoxContainer feetBox = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topLeft, new(35, 6), new(9, 5));
            new Image(feetBox, GetImageFromFile("assets/items/armour/feet/feet.txt"), Align.centre, new(0, 0));

            new Label(equipBox, "Main:", AlignText.centre, Align.topLeft, new(7, 12), 5);
            BoxContainer mainHandBox = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topLeft, new(5, 13), new(9, 5));
            new Image(mainHandBox, GetImageFromFile("assets/items/weapons/leftHand.txt"), Align.centre, new(0, 0));

            new Label(equipBox, "Off-:", AlignText.centre, Align.topLeft, new(17, 12), 5);
            BoxContainer offHandBox = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topLeft, new(15, 13), new(9, 5));
            new Image(offHandBox, GetImageFromFile("assets/items/weapons/rightHand.txt"), Align.centre, new(0, 0));

            new Label(equipBox, "Acc1:", AlignText.centre, Align.topLeft, new(27, 12), 5);
            BoxContainer accessory1Box = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topLeft, new(25, 13), new(9, 5));
            new Image(accessory1Box, GetImageFromFile("assets/items/blankItem.txt"), Align.centre, new(0, 0));

            new Label(equipBox, "Acc2:", AlignText.centre, Align.topLeft, new(37, 12), 5);
            BoxContainer accessory2Box = new BoxContainer(equipBox, (BoxContainerStyle)0b01, Align.topLeft, new(35, 13), new(9, 5));
            new Image(accessory2Box, GetImageFromFile("assets/items/blankItem.txt"), Align.centre, new(0, 0));


            // settings tab
            Tab settingsTab = new(window, new(68, 17), "Settings");

            //new BoxContainer(settingsTab, 0b00, Align.topLeft, new(0, 2), new(5, 5)); //tabBoxTest
            settingsList = new(settingsTab, (BoxContainerStyle)0b01, Align.centreRight, new(0, 0), new(52, 17), new(17, 5), -1);

            SettingsTab generalTab = new(settingsList, "General");

            SettingsTab videoTab = new(settingsList, "Video");

            new Label(videoTab, "Window Mode", AlignText.centre, Align.topCentre, new(0, 2), 11);
            new SelectionBox(videoTab, "Borderless Windowed", 0, Align.topCentre, new(0, 3), new(36, 3));

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                new Label(videoTab, "Font Size", AlignText.centre, Align.topCentre, new(0, 7), 11);
                new SelectionBox(videoTab, "32pt", 0, Align.topCentre, new(0, 8), new(28, 3));
            }

            SettingsTab audioTab = new(settingsList, "Audio");

            SettingsTab inputTab = new(settingsList, "Input");

            window.WinInit();
            settingsList.ListInit();
        }

        public void RenderUI()
        {
            while (true)
            {
                gameRoot.Buffer.ClearAll();

                gameRoot.FillBuffer(gameRoot.Buffer);
                gameRoot.Buffer.DrawBuffer();

                switch(Console.ReadKey(true).Key)
                {
                    case ConsoleKey.LeftArrow:
                        window.PrevTab();
                        break;

                    case ConsoleKey.RightArrow:
                        window.NextTab();
                        break;

                    case ConsoleKey.UpArrow:
                        itemList.PrevTab();
                        break;

                    case ConsoleKey.DownArrow:
                        itemList.NextTab();
                        break;

                    case ConsoleKey.OemMinus:
                        itemList.PrevItem();
                        Debug.Print(itemList.GetCurrentItem().equipped.ToString());
                        break;

                    case ConsoleKey.OemPlus:
                        itemList.NextItem();
                        Debug.Print(itemList.GetCurrentItem().equipped.ToString());
                        break;

                    case ConsoleKey.Z:
                        Item item = itemList.GetCurrentItem();
                        if (item.equipped) { item.equipped = !player.inventory.Dequip(item); }
                        else { item.equipped = player.inventory.Equip(item); }
                        break;

                    case ConsoleKey.NumPad8:
                        settingsList.PrevTab();
                        break;

                    case ConsoleKey.NumPad2:
                        settingsList.NextTab();
                        break;
                }
            }
        }

        public static char[][] GetImageFromFile(string path)
        {
            string[] textImageString = { };
            try { textImageString = File.ReadAllLines(path); }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException || ex is DirectoryNotFoundException) { textImageString = File.ReadAllLines("assets/ImageNotFound.txt"); }
            }

            return textImageString.Select(item => item.ToArray()).ToArray();
        }
    }
}
