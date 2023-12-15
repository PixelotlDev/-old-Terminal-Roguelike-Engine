namespace AdventureGame
{
    internal class ItemEntity : GameObject
    {
        public Item Item { get; private set; }

        public ItemEntity(World world, int xPos, int yPos, Item item, Tags tag = Tags.Default)
            : base(world, xPos, yPos, tag, item.name)
        {
            Item = item;
            sprite = item.sprite;
        }
    }
}
