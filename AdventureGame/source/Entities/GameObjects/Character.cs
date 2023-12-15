using System.Drawing;

namespace AdventureGame
{
    internal abstract class Character : GameObject
    {
        protected Point previousPos;

        protected int health, attack, defence, speed;

        protected Character(World world, int xPos, int yPos, Tags tag, string name) : base(world, xPos, yPos, tag, name) { previousPos = pos; }

        // forces any derivitives of Character to do something before running their own code
        public void MoveCollisionHandler(GameObject[] collisions)
        {
            bool hasMoved = pos != previousPos;
            pos = previousPos;

            if (hasMoved) { MoveCollisionHandler_v(collisions); }
        }
        protected virtual void MoveCollisionHandler_v(GameObject[] collisions) { }

        public void Damage(int damage)
        {
            // damage recieved stat
            health -= damage;
            if(health < 0) { health = 0; }
        }

        public void Heal(int damage)
        {
            // damage healed stat
            health += damage;
        }

        public bool IsAlive()
        {
            return health > 0;
        }

        public void Move(int moveX, int moveY)
        {
            previousPos = pos;
            pos.X += moveX;
            pos.Y += moveY;
        }

    }
}
