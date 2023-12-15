namespace AdventureGame
{
    [Flags]
    enum DFlags
    {
        none = 0,
        left = 0b0001,
        right = 0b0010,
        up = 0b0100,
        down = 0b1000
    }

    internal class CharBuffer
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Tile[] TileBuffer { get; private set; }

        public CharTile[] CharTileBuffer { get; private set; }


        public CharBuffer(int width, int height)
        {
            Width = width;
            Height = height;

            int bufferSize = Width * Height;
            TileBuffer = new Tile[bufferSize];
            
            CharTileBuffer = new CharTile[bufferSize];
        }

        public void ClearAll()
        {
            int bufferSize = Width * Height;
            for (int i = 0; i < bufferSize; i++)
            {
                TileBuffer[i] = new(0b0000);
                CharTileBuffer[i] = new(' ');
            }
        }

        // replaces a tile entirely
        public void ReplaceTile(DFlags direction, int x, int y)
        {
            int pos = x + (y * Width);
            TileBuffer[pos].Directions = direction;
        }

        // edits only the direction of a tile, and leaves the rest of the byte intact
        public void EditTile(DFlags direction, int x, int y)
        {
            int pos = x + (y * Width);
            TileBuffer[pos].AddDirection(direction);
        }

        public void EditChar(char character, int x, int y)
        {
            int pos = x + (y * Width);
            CharTileBuffer[pos].Character = character;
        }

        public void EditTextColour(ConsoleColor colour, int x, int y)
        {
            int pos = x + (y * Width);
            CharTileBuffer[pos].TextColour = colour;
        }

        public void EditHighlight(ConsoleColor colour, int x, int y)
        {
            int pos = x + (y * Width);
            CharTileBuffer[pos].HighlightColour = colour;
        }

        // used by boxes, this method transfers the tiles given into TileBuffer, then connects adjacent tiles together,
        // and finally transfers those tiles over to the CharTileBuffer
        public void DrawTilesToBuffer(DFlags[] tiles, Dictionary<DFlags, char> tileDict, int x, int y, int width, int height)
        {
            for (int i = 0; i < tiles.Length; i++)
            {
                ReplaceTile(tiles[i], x + (i % width), y + (i / width));
            }
            ConnectTiles(x, y, width, height);
            ConstructTextFromTiles(tileDict, x, y, width, height);
        }

        // takes all the characters in CharTileBuffer, chucks them in an array, then prints that array
        // (much faster than just writing each character individually)
        public void DrawBuffer()
        {
            char[] rawChars = new char[CharTileBuffer.Length];
            int pos;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    pos = j + (i * Width);
                    rawChars[pos] = CharTileBuffer[pos].Character;
                }
            }

            Console.SetCursorPosition(0, 0);
            Console.ResetColor();
            Console.Write(rawChars);

            ApplyColours();
        }

        // connects adjacent tiles together (e.g. ┌───│ becomes  ┌───┤)
        private void ConnectTiles(int x, int y, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int pos = (j + x) + ((i + y) * Width);
                    // check left tile
                    // check to make sure the Tile we're looking at isn't on the left border of the screen
                    if (pos % Width != 0)
                    {
                        // if the tile to the left has a line going right...
                        if (TileBuffer[pos - 1].HasDirection(DFlags.right))
                        {
                            // then add a line going left to connect to it
                            TileBuffer[pos].AddDirection(DFlags.left);
                        }
                    }

                    // check right tile
                    // same as above, with different directions
                    if (pos % Width != Width - 1)
                    {
                        if (TileBuffer[pos + 1].HasDirection(DFlags.left))
                        {
                            TileBuffer[pos].AddDirection(DFlags.right);
                        }
                    }

                    // check up tile
                    if (pos / Width > 0)
                    {
                        if (TileBuffer[pos - Width].HasDirection(DFlags.down))
                        {
                            TileBuffer[pos].AddDirection(DFlags.up);
                        }
                    }

                    // check down tile
                    if (pos / Width < Height - 1)
                    {
                        if (TileBuffer[pos + Width].HasDirection(DFlags.up))
                        {
                            TileBuffer[pos].AddDirection(DFlags.down);
                        }
                    }
                }
            }
        }
        
        // transfers TileBuffer to CharTileBuffer by looking up directions in tileDict and recieving the corresponding char
        private void ConstructTextFromTiles(Dictionary<DFlags, char> tileDict, int x, int y, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int pos = (j + x) + ((i + y) * Width);
                    EditChar(tileDict[TileBuffer[pos].Directions], j+x, i+y);
                }
            }
        }

        private void ApplyColours()
        {
            int pos;

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    pos = j + (i * Width);

                    if (CharTileBuffer[pos].TextColour != ConsoleColor.White || CharTileBuffer[pos].HighlightColour != ConsoleColor.Black)
                    {
                        Console.SetCursorPosition(j, i);
                        CharTileBuffer[pos].DrawColouredCharTile();

                        Console.ResetColor();
                    }
                }
            }

        }
    }

    // struct to represent a 'tile', with flags for line direction
    internal struct Tile
    {
        // the last 4 bits are taken up by direction flags, but the first 4 are free to use for... something :3
        private byte bitmask;
        public DFlags Directions
        {
            get { return (DFlags)(bitmask & 0b00001111); }
            set
            {
                bitmask &= 0b11110000;
                bitmask |= (byte)value;
            }
        }


        public Tile(DFlags directions)
        {
            bitmask = (byte)(directions);
        }

        public void AddDirection(DFlags direction)
        {
            bitmask |= (byte)direction;
        }

        public bool HasDirection(DFlags direction)
        {
            return (Directions & direction) == direction;
        }
    }

    // struct to represent one screen tile (i.e. one character in the console) and any corresponding highlighting
    internal struct CharTile
    {
        public ConsoleColor TextColour { get; set; }
        public ConsoleColor HighlightColour { get; set; }

        public char Character { get; set; }

        public CharTile(char character, ConsoleColor textColour = ConsoleColor.White, ConsoleColor highlightColour = ConsoleColor.Black)
        {
            Character = character;

            TextColour = textColour;
            HighlightColour = highlightColour;
        }

        public void DrawColouredCharTile()
        {
            Console.ForegroundColor = TextColour;
            Console.BackgroundColor = HighlightColour;
            Console.Write(Character);
        }
    }
}
