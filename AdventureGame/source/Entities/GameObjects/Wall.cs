namespace AdventureGame
{
    internal class Wall : GameObject
    {
        public Wall(World world, int xPos, int yPos, bool vertical, Tags tag = Tags.Default, string name = "")
            : base(world, xPos, yPos, tag, name)
        {
            if (vertical) { sprite = '|'; }
            else { sprite = '-'; }
        }
    }
}