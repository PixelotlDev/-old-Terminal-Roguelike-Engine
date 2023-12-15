// windows-only collection of functions made to manipulate the console in ways exclusive to the windows console

using System.Runtime.InteropServices;

namespace AdventureGame
{
    public static class ConsoleHelper
    {
        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);


        private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfo
        {
            internal int cbSize;
            internal int FontIndex;
            internal short FontWidth;
            public short FontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.wc, SizeConst = 32)]
            public string FontName;
        }

        // changes the font and font size of a console window
        // from: https://stackoverflow.com/a/62763047
        public static FontInfo[] SetCurrentFont(string font, short fontSize = 0)
        {
            FontInfo before = new FontInfo
            {
                cbSize = Marshal.SizeOf<FontInfo>()
            };

            if (GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before))
            {

                FontInfo set = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>(),
                    FontIndex = 0,
                    FontFamily = FixedWidthTrueType,
                    FontName = font,
                    FontWeight = 400,
                    FontSize = fontSize > 0 ? fontSize : before.FontSize
                };

                // Get some settings from current font.
                if (!SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set))
                {
                    var ex = Marshal.GetLastWin32Error();
                    Console.WriteLine("Set error " + ex);
                    throw new System.ComponentModel.Win32Exception(ex);
                }

                FontInfo after = new FontInfo
                {
                    cbSize = Marshal.SizeOf<FontInfo>()
                };
                GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);

                return new[] { before, set, after };
            }
            else
            {
                var er = Marshal.GetLastWin32Error();
                Console.WriteLine("Get error " + er);
                throw new System.ComponentModel.Win32Exception(er);
            }
        }

        // makes the console window fill the screen, as if the user had pressed f11
        // from: https://stackoverflow.com/a/70329738

        // for ShowWindow
        private const int RESET = 1;
        private const int MAXIMISE = 3;

        // for PostMessage
        private const int F11 = 0x7A;

        private const uint KEYDOWN = 0x100;

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private static readonly IntPtr consoleWindow = GetConsoleWindow();
        
        public static void Fullscreen(bool on)
        {
            // though we check the height (or width, either or) of the window, if it is maximised, it will be at the maximum height,
            // and so this function won't run and we will stay maximised and not 'fullscreen' (borderless windowed)
            // to fix this, we basically just reset the window to normal proportions, since this will have no effect
            // if the window is in 'fullscreen' mode
            ShowWindow(consoleWindow, RESET);
            if ((Console.WindowHeight == Console.LargestWindowHeight) != on) { PostMessage(consoleWindow, KEYDOWN, (IntPtr)F11, IntPtr.Zero); }
        }

        public static void Maximise()
        {
            ShowWindow(consoleWindow, MAXIMISE);

        }
    }
}