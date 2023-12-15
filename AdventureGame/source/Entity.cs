namespace AdventureGame
{
    internal abstract class Entity
    {
        protected World world;

        public Tags tag;
        public string name;

        public Entity(World world, Tags tag, string name)
        {
            this.world = world;
            this.tag = tag;
            this.name = name;
        }
        public virtual void Update() { }

        public virtual void RenderUpdate() { }

        public virtual void Destroy()
        {
            world.AllEntities.Remove(this);
        }
    }
}
