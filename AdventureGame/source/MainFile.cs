using System.Text;
using System.Runtime.InteropServices;

namespace AdventureGame
{
    internal class MainFile
    {
        static void Main()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                ConsoleHelper.SetCurrentFont("Consolas", 32);
                ConsoleHelper.Fullscreen(true);
                Console.SetWindowSize(126, 33);
                Console.SetBufferSize(126, 33);
            }
            else
            {
                // the width is one bigger than it needs to be to avoid incorrect rendering on publish
                Console.SetBufferSize(126, 33);
                Console.SetWindowSize(126, 33);
            }

            Console.Title = "Rogue clone";
            Console.CursorVisible = false;
            Console.OutputEncoding = Encoding.Unicode;

            World world = new();
            while (true)
            {
                world.GlobalUpdate();
            }
        }
    }
}