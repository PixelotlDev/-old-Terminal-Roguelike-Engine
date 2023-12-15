using System.Drawing;

namespace AdventureGame
{
    internal abstract class GameObject : Entity
    {
        public Point pos;

        public char sprite;

        protected GameObject(World world, int xPos, int yPos, Tags tag, string name)
            : base(world, tag, name)
        {
            SetPos(xPos, yPos);
        }

        public void SetPos(int x, int y)
        {
            pos = new Point(x, y);
        }

        public Point GetPos() { return pos; }

        public override void Destroy()
        {
            base.Destroy();
            world.GameObjects.Remove(this);
        }
    }
}
