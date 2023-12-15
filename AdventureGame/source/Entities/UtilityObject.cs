namespace AdventureGame
{
    internal abstract class UtilityObject : Entity
    {
        public UtilityObject(World world, Tags tag, string name) : base(world, tag, name) { }

        public override void Destroy()
        {
            base.Destroy();
            world.UtilityObjects.Remove(this);
        }
    }
}
