using System.Drawing;
using System.Text;

namespace AdventureGame
{
    internal class Viewport : UtilityObject
    {
        private readonly Size viewSize = new(80, 20);
        private readonly Point viewCorner = new(20, 6);
        public Point viewCenter;

        private char[,] viewportMap;

        private Player? player;

        public Viewport(World world, Tags tag = Tags.Default, string name = "")
            : base(world, tag, name)
        {
            try { player = (Player?)world.FindEntityWithTag(Tags.Player); }
            catch (InvalidCastException) { /*some kind of log action*/ }

            viewportMap = new char[viewSize.Width, viewSize.Height];
        }

        public override void RenderUpdate()
        {
            // center on the player
            if (player != null) { viewCenter = player.GetPos(); }
            else { /*some kind of log action*/ }

            ConstructViewport();
        }

        private void ConstructViewport()
        {
            for (int y = 0; y < viewSize.Height; y++)
            {
                for (int x = 0; x < viewSize.Width; x++)
                {
                    viewportMap[x, y] = ' ';
                }
            }

            foreach (GameObject gObject in world.GameObjects)
            {
                Point objectPos = gObject.GetPos();
                // we use >= here because we want to draw things at screen position 0, at the very top and left of the screen
                if (objectPos.X >= viewCenter.X - Math.Floor(viewSize.Width / 2f) && objectPos.X < viewCenter.X + Math.Ceiling(viewSize.Width / 2f))
                {
                    // why we don't have to do the same for the second condition of both if statements is a mystery to me
                    if (objectPos.Y >= viewCenter.Y - Math.Floor(viewSize.Height / 2f) && objectPos.Y < viewCenter.Y + Math.Ceiling(viewSize.Height / 2f))
                    {
                        // -viewCenter + ((viewSize / 2) + objectPos) comes from linear equation y = -x + a + b
                        // where: y = screen location of the object, x = world location of the camera, a =  half of the view size, b = world location of the object
                        viewportMap[(-viewCenter.X + ((viewSize.Width / 2) + objectPos.X)), (-viewCenter.Y + ((viewSize.Height / 2) + objectPos.Y))] = gObject.sprite;
                    }

                }
            }
        }

        public void RenderViewport()
        {
            Console.OutputEncoding = Encoding.UTF8;
            ConstructViewport();

            Console.SetCursorPosition(viewCorner.X, viewCorner.Y);
            for (int y = 0; y < viewSize.Height; y++)
            {
                for (int x = 0; x < viewSize.Width; x++)
                {
                    Console.Write(viewportMap[x, y]);
                }
                Console.WriteLine();
                Console.SetCursorPosition((int)viewCorner.X, Console.GetCursorPosition().Top);
            }
        }
    }
}
