namespace AdventureGame
{
    internal class InputHandler : UtilityObject
    {
        Player player;
        //MenuHandler menu;

        public InputHandler(World world, /*MenuHandler menu,*/ Tags tag = Tags.Default, string name = "")
            : base(world, tag, name)
        {
            player = (Player)world.FindEntityWithTag(Tags.Player);
            //this.menu = menu;
        }

        public override void Update()
        {
            ProcessInput();
        }

        private void ProcessInput()
        {
            while (Console.KeyAvailable) { Console.ReadKey(true); }
            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.NumPad1:
                    player.Move(-1, 1);
                    break;

                case ConsoleKey.NumPad2:
                    player.Move(0, 1);
                    break;

                case ConsoleKey.NumPad3:
                    player.Move(1, 1);
                    break;

                case ConsoleKey.NumPad4:
                    player.Move(-1, 0);
                    break;

                case ConsoleKey.NumPad6:
                    player.Move(1, 0);
                    break;

                case ConsoleKey.NumPad7:
                    player.Move(-1, -1);
                    break;

                case ConsoleKey.NumPad8:
                    player.Move(0, -1);
                    break;

                case ConsoleKey.NumPad9:
                    player.Move(1, -1);
                    break;

                case ConsoleKey.OemPeriod:
                    //menu.NextPage();
                    break;

                case ConsoleKey.OemComma:
                    //menu.PrevPage();
                    break;

                default:
                    break;
            }
        }
    }
}
