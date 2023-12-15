namespace AdventureGame
{
    internal class Enemy : Character
    {
        public Enemy(World world, int xPos, int yPos, int hp, int at, int df, int sp, Tags tag = Tags.Default, string name = "")
            : base(world, xPos, yPos, tag, name)
        {
            sprite = 'E';

            health = hp;
            attack = at;
            defence = df;
            speed = sp;
        }

        protected override void MoveCollisionHandler_v(GameObject[] collisions)
        {
            foreach (GameObject collision in collisions)
            {
                if (collision is Player)
                {

                }
            }
        }
    }
}
