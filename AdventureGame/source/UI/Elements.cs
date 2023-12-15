using System.Drawing;

namespace AdventureGame
{
    // centre does not allow for padding, and will ignore it (i.e. leftCentre can be padded horizontally but not vertically)
    enum Align
    {
        topLeft,
        centreLeft,
        bottomLeft,
        topCentre,
        centre,
        bottomCentre,
        topRight,
        centreRight,
        bottomRight
    }

    internal abstract class Element
    {
        // (0, 0) is top left corner
        public Point Position { get; protected set; }
        public Size Dimensions { get; protected set; }

        public int ZLayer { get; protected set; }

        public bool Enabled { get; set; }

        protected Container? Parent { get; private set; }
        protected Tab? ParentTab { get; private set; }

        protected readonly Size minDimensions;

        protected Element(Container? parent, Point position, Size dimensions, Size minDimensions, int zLayer, bool enabled)
        {
            Enabled = enabled;

            Dimensions = dimensions;
            this.minDimensions = minDimensions;

            if (parent is not null)
            {
                // if the object is a tab, then we want all of its children (other than Name, setup in the constructor)
                // to render themselves in the window
                if (parent is Tab tab && tab.Name is not null) { Parent = tab.Parent; ParentTab = tab; }
                else if(parent is ItemTab itemTab && itemTab.Name is not null) { Parent = itemTab.Parent; }
                else if(parent is SettingsTab settingsTab && settingsTab.Name is not null) { Parent = settingsTab.Parent; }
                else { Parent = parent; }

                parent.AddChild(this);

                if (Dimensions.Width > Parent.Dimensions.Width || Dimensions.Height > Parent.Dimensions.Height)
                {
                    // log some kind of error
                    throw new ArgumentOutOfRangeException(nameof(Dimensions),
                                                          Dimensions,
                                                          "Argument cannot be larger than parent dimensions");
                }
                else if (Dimensions.Width == Parent.Dimensions.Width && Dimensions.Height == Parent.Dimensions.Height)
                {
                    // log some kind of warning about dimensions being equal to dimensions of parent
                }
            }
            else if (this is not Root)
            {
                throw new ArgumentNullException(nameof(parent), "parent cannot be null unless type is Root");
            }

            if (Dimensions.Width < minDimensions.Width || Dimensions.Height < minDimensions.Height)
            {
                // log some kind of error
                throw new ArgumentOutOfRangeException(nameof(Dimensions),
                                                      Dimensions,
                                                      "Argument cannot be smaller than minDimensions");
            }

            ZLayer = zLayer;
            Position = position;
        }

        public virtual void DrawTiles(CharBuffer buffer) { return; /* log that DrawTilesToBuffer has not been implimented */ }

        public virtual void DrawText(CharBuffer buffer) { return; /* log that GenerateDisplayText has not been implimented */ }

        public virtual void FillBuffer(CharBuffer buffer)
        {
            if (Enabled)
            {
                if (this is Container container)
                {
                    Element[] sortedChildren = container.GetChildren().OrderBy(c => c.ZLayer).ToArray();
                    foreach (Element childElement in sortedChildren)
                    {
                        if (childElement.ZLayer < 0) { childElement.FillBuffer(buffer); }
                    }

                    container.DrawTiles(buffer);

                    foreach (Element childElement in sortedChildren)
                    {
                        if (childElement.ZLayer >= 0) { childElement.FillBuffer(buffer); }
                    }
                }

                DrawText(buffer);
            }
        }

        public Container? GetParent() { return Parent; }

        public virtual void MoveElement(Align align, Size padding) { Position = FindPositionFromParent(align, padding); }

        public virtual void Destroy()
        {
            Parent.RemoveChild(this);
            if(ParentTab is not null) { ParentTab.RemoveChild(this); }
            Parent = null;
        }

        protected Point FindPositionFromParent(Align align, Size padding)
        {
            if (Parent is null)
            {
                // log some kind of error
                throw new NullReferenceException("Parameter 'parent' must be given a value");
            }

            // all of these need to throw a different ArgumentOutOfRangeException, so they're in different statements
            if (padding.Width < 0 || padding.Height < 0)
            {
                // log some kind of error
                throw new ArgumentOutOfRangeException(nameof(padding),
                                                       padding,
                                                      "Parameter cannot be set to a negative value.");
            }

            if (padding.Width > (Parent.Dimensions.Width - Dimensions.Width) ||
                padding.Height > (Parent.Dimensions.Height - Dimensions.Height))
            {
                // log some kind of error
                throw new ArgumentOutOfRangeException(nameof(padding),
                                                       padding,
                                                      "Parameter would cause element to fall out of bounds of its parent");
            }

            Point thisPosition = new();
            switch ((int)align % 3)
            {
                // top
                case 0:
                    thisPosition.Y = Parent.Position.Y + padding.Height;
                    break;

                // centre
                case 1:
                    thisPosition.Y = Parent.Position.Y + ((int)Math.Ceiling(Parent.Dimensions.Height / 2f)) - ((int)Math.Ceiling(Dimensions.Height / 2f));
                    break;

                // bottom
                case 2:
                    thisPosition.Y = (Parent.Position.Y + Parent.Dimensions.Height) - Dimensions.Height - padding.Height;
                    break;
            }

            switch ((int)align / 3)
            {
                // left
                case 0:
                    thisPosition.X = Parent.Position.X + padding.Width;
                    break;

                // centre
                case 1:
                    thisPosition.X = Parent.Position.X + ((int)Math.Ceiling(Parent.Dimensions.Width / 2f)) - ((int)Math.Ceiling(Dimensions.Width / 2f));
                    break;

                // right
                case 2:
                    thisPosition.X = (Parent.Position.X + Parent.Dimensions.Width) - Dimensions.Width - padding.Width;
                    break;
            }

            return thisPosition;
        }

        protected int GetNumOfParents()
        {
            if (Parent is not null)
            {
                return Parent.GetNumOfParents() + 1;
            }
            else { return 0; }
        }
    }

    // containers
    internal abstract class Container : Element
    {
        protected List<Element> children = new();

        protected bool Scrollable;

        public Container(Container? parent, Point position, Size dimensions, Size minDimensions, int zLayer, bool enabled, bool scrollable)
            : base(parent, position, dimensions, minDimensions, zLayer, enabled)
        {
            Scrollable = scrollable;
        }

        public virtual void AddChild(Element element)
        {
            children.Add(element);
        }

        public virtual void RemoveChild(Element element)
        {
            children.Remove(element);
        }

        public List<Element> GetChildren()
        {
            return children;
        }

        public override void Destroy()
        {
            base.Destroy();

            List<Element> tempChildren = new(children);
            foreach (Element element in tempChildren)
            {
                element.Destroy();
            }
        }
    }

    internal class Tab : Container
    {
        public Label? Name { get; private set; }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;

                foreach(Element element in children)
                {
                    if (element != Name) { element.Enabled = value; }
                }

                if (value)
                {
                    ((Window)Parent).ResizeWindow(WinDimensions);
                }
            }
        }

        private Size WinDimensions { get; set; }

        public Tab(Window parent, Size winDimensions, string name, bool enabled = true)
            : base(parent, new(0, 0), new(3, 3), new(3, 3), 0, enabled, false)
        {
            Dimensions = new(parent.TabWidth, 3);
            Position = new(parent.Position.X + ((parent.Dimensions.Width / 2) - (parent.GetChildren().Count * Dimensions.Width / 2)) + (parent.GetChildren().IndexOf(this) * Dimensions.Width),
                           parent.Position.Y - 2);

            WinDimensions = winDimensions;
            // we only do this here because otherwise, the elements that we enter will be going off the wrong size,
            // and might trigger a custom error by being out of bounds
            ((Window)Parent).ResizeWindow(WinDimensions);

            Name = new(this, name, AlignText.centre, Align.centre, new(0, 0), Dimensions.Width - 2);
        }

        protected virtual DFlags[] GenerateTabTiles()
        {
            List<List<DFlags>> boxComponents;
            if (Selected)
            {
                boxComponents = new()
                {
                    new() { (DFlags)0b1010, (DFlags)0b1001 },
                    new() { (DFlags)0b1100, (DFlags)0b1100 },
                    new() { (DFlags)0b0101, (DFlags)0b0110 }
                };
            }
            else
            {
                boxComponents = new()
                {
                    new() { (DFlags)0b1010, (DFlags)0b1001 },
                    new() { (DFlags)0b1100, (DFlags)0b1100 }
                };
            }

            int height = boxComponents.Count;
            int width = Dimensions.Width;

            for (int i = 0; i < height; i++)
            {
                if (i == 0)
                {
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[i].Insert(1, (DFlags)0b0011);
                    }
                }
                else
                {
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[i].Insert(1, 0b0000);
                    }
                }
            }

            DFlags[] boxTileArray = new DFlags[width + ((height - 1) * width)];
            for (int i = 0; i < boxTileArray.Length; i++)
            {
                boxTileArray[i] = boxComponents[i / width][i % width];
            }

            return boxTileArray;
        }

        public override void DrawTiles(CharBuffer buffer)
        {
            buffer.DrawTilesToBuffer(GenerateTabTiles(), ((Window)Parent).tileDict, Position.X, Position.Y, Dimensions.Width, Dimensions.Height);
        }

        public override void AddChild(Element element)
        {
            base.AddChild(element);
            if (Name is not null)
            {
                element.Enabled = Selected;
            }
        }

        public void UpdateTab()
        {
            Window parentWindow = (Window)Parent;

            Dimensions = new(parentWindow.TabWidth, 3);
            Position = new(parentWindow.Position.X + ((parentWindow.Dimensions.Width / 2) - (parentWindow.GetChildren().Count * parentWindow.TabWidth / 2)) + (parentWindow.GetChildren().IndexOf(this) * Dimensions.Width),
                           parentWindow.Position.Y - 2);

            Name.MoveElement(Align.centre, new(0, 0));
        }
    }

    internal class SettingsTab : Container
    {
        public Label? Name { get; private set; }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;

                foreach (Element element in children)
                {
                    if (element != Name) { element.Enabled = value; }
                }
            }
        }

        public SettingsTab(SettingsList parent, string name, bool enabled = true)
            : base(parent, new(0, 0), new(3, 3), new(3, 3), 0, enabled, false)
        {
            Dimensions = parent.TabSize;
            Position = new(parent.Position.X - (parent.TabSize.Width - 1),
                           parent.Position.Y + ((parent.GetChildren().IndexOf(this) * parent.TabSize.Height) - parent.GetChildren().IndexOf(this)));

            Name = new(this, name, AlignText.centre, Align.centre, new(0, 0), Dimensions.Width - 2);
        }

        protected virtual DFlags[] GenerateTabTiles()
        {
            bool atTop = Parent.GetChildren().IndexOf(this) == 0;
            bool atBottom = Parent.GetChildren().IndexOf(this) == Parent.GetChildren().Count - 1;

            List<List<DFlags>> boxComponents;
            if (Selected)
            {
                boxComponents = new()
                {
                    new() { (DFlags)0b1010, (DFlags)0b0101 },
                    new() { (DFlags)0b1100, 0b0000 },
                    new() { (DFlags)0b1100, 0b0000 },
                    new() { (DFlags)0b1100, 0b0000 },
                    new() { (DFlags)0b0110, (DFlags)0b1001 }
                };
            }
            else
            {
                boxComponents = new()
                {
                    new() { (DFlags)0b1010, (DFlags)0b1001 },
                    new() { (DFlags)0b1100, (DFlags)0b1100 },
                    new() { (DFlags)0b1100, (DFlags)0b1100 },
                    new() { (DFlags)0b1100, (DFlags)0b1100 },
                    new() { (DFlags)0b0110, (DFlags)0b0101 }
                };
            }

            if (atTop)
            {
                boxComponents[0][1] &= (DFlags)0b1011;
            }
            if (atBottom)
            {
                boxComponents[4][1] &= (DFlags)0b0111;
            }

            int height = boxComponents.Count;
            int width = Dimensions.Width;

            for (int i = 0; i < height; i++)
            {
                if (i == 0 || i == height - 1)
                {
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[i].Insert(1, (DFlags)0b0011);
                    }
                }
                else
                {
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[i].Insert(1, 0b0000);
                    }
                }
            }

            DFlags[] boxTileArray = new DFlags[width + ((height - 1) * width)];
            for (int i = 0; i < boxTileArray.Length; i++)
            {
                boxTileArray[i] = boxComponents[i / width][i % width];
            }

            return boxTileArray;
        }

        public override void DrawTiles(CharBuffer buffer)
        {
            buffer.DrawTilesToBuffer(GenerateTabTiles(), ((SettingsList)Parent).tileDict, Position.X, Position.Y, Dimensions.Width, Dimensions.Height);
        }

        public override void AddChild(Element element)
        {
            base.AddChild(element);
            if (Name is not null)
            {
                element.Enabled = Selected;
            }
        }

        public void UpdateTab()
        {
            SettingsList parentList = (SettingsList)Parent;

            Dimensions = parentList.TabSize;
            Position = new(parentList.Position.X - (parentList.TabSize.Width - 1),
                           parentList.Position.Y + ((parentList.GetChildren().IndexOf(this) * parentList.TabSize.Height) - parentList.GetChildren().IndexOf(this)));

            Name.MoveElement(Align.centre, new(0, 0));
        }
    }

    internal class ItemTab : Container
    {
        public Label? Name { get; private set; }

        private bool selected;
        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;

                foreach (Element element in children)
                {
                    if (element != Name) { element.Enabled = value; }
                }
            }
        }

        public ItemTab(ItemList parent, string name, bool enabled = true)
            : base(parent, new(0, 0), new(3, 3), new(3, 3), 0, enabled, false)
        {
            Dimensions = parent.TabSize;
            Position = new(parent.Position.X - (parent.TabSize.Width - 1),
                           parent.Position.Y + ((parent.GetChildren().IndexOf(this) * parent.TabSize.Height) - parent.GetChildren().IndexOf(this)));

            Name = new(this, name, AlignText.left, Align.centreLeft, new(2, 0), Dimensions.Width - 2);
        }

        protected DFlags[] GenerateTabTiles()
        {
            bool atTop = Parent.GetChildren().IndexOf(this) == 0;
            bool atBottom = Parent.GetChildren().IndexOf(this) == Parent.GetChildren().Count - 1;

            List<List<DFlags>> boxComponents;
            if (Selected)
            {
                boxComponents = new()
                {
                    new() { (DFlags)0b1010, (DFlags)0b0101 },
                    new() { (DFlags)0b1100,         0b0000 },
                    new() { (DFlags)0b0110, (DFlags)0b1001 }
                };
            }
            else
            {
                boxComponents = new()
                {
                    new() { (DFlags)0b1010, (DFlags)0b1001 },
                    new() { (DFlags)0b1100, (DFlags)0b1100 },
                    new() { (DFlags)0b0110, (DFlags)0b0101 }
                };
            }

            if (atTop)
            {
                boxComponents[0][1] &= (DFlags)0b1011;
            }
            if (atBottom)
            {
                boxComponents[2][1] &= (DFlags)0b0111;
            }

            int height = boxComponents.Count;
            int width = Dimensions.Width;

            for (int i = 0; i < height; i++)
            {
                if (i == 0 || i == height - 1)
                {
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[i].Insert(1, (DFlags)0b0011);
                    }
                }
                else
                {
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[i].Insert(1, 0b0000);
                    }
                }
            }

            DFlags[] boxTileArray = new DFlags[width + ((height - 1) * width)];
            for (int i = 0; i < boxTileArray.Length; i++)
            {
                boxTileArray[i] = boxComponents[i / width][i % width];
            }

            return boxTileArray;
        }

        public override void DrawTiles(CharBuffer buffer)
        {
            buffer.DrawTilesToBuffer(GenerateTabTiles(), ((ItemList)Parent).tileDict, Position.X, Position.Y, Dimensions.Width, Dimensions.Height);
        }

        public override void AddChild(Element element)
        {
            base.AddChild(element);
            if (Name is not null) { element.Enabled = Selected; }
        }

        public void UpdateTab()
        {
            ItemList parentList = (ItemList)Parent;

            Dimensions = parentList.TabSize;
            Position = new(parentList.Position.X - (parentList.TabSize.Width - 1),
                           parentList.Position.Y + ((parentList.GetChildren().IndexOf(this) * parentList.TabSize.Height) - parentList.GetChildren().IndexOf(this)));

            Name.MoveElement(Align.centreLeft, new(2, 0));
        }
    }

    internal class Root : Container
    {
        public CharBuffer Buffer { get; private set; }

        public Root()
            : base(null, new(0, 0), new(Console.BufferWidth, Console.BufferHeight), new(Console.BufferWidth, Console.BufferHeight), 0, true, false)
        {
            Buffer = new(Dimensions.Width, Dimensions.Height - 1);
        }
    }

    [Flags]
    enum BoxContainerStyle
    {
        softCorners,
        doubleEdges
    }

    internal class BoxContainer : Container
    {
        protected readonly DFlags[] tileMask;
        protected readonly BoxContainerStyle type;

        public readonly Dictionary<DFlags, char> tileDict;

        public BoxContainer(Container parent, BoxContainerStyle type, Align align, Size padding, Size dimensions, int zLayer = 0, bool enabled = true, bool scrollable = false)
            : base(parent, new(0, 0), dimensions, new(3, 3), zLayer, enabled, scrollable)
        {
            Position = FindPositionFromParent(align, padding);

            tileMask = new DFlags[Dimensions.Width + (Dimensions.Height * Dimensions.Width)];

            this.type = type;

            switch ((int)type & 0b11)
            {
                case 0b00:
                    tileDict = new Dictionary<DFlags, char>
                    {
                        { 0b0000, ' ' },
                        { (DFlags)0b0001, '╴' },
                        { (DFlags)0b0010, '╶' },
                        { (DFlags)0b0100, '╵' },
                        { (DFlags)0b1000, '╷' },

                        { (DFlags)0b0011, '─' },
                        { (DFlags)0b1100, '│' },

                        { (DFlags)0b0101, '┘' },
                        { (DFlags)0b1001, '┐' },
                        { (DFlags)0b0110, '└' },
                        { (DFlags)0b1010, '┌' },

                        { (DFlags)0b0111, '┴' },
                        { (DFlags)0b1011, '┬' },
                        { (DFlags)0b1101, '┤' },
                        { (DFlags)0b1110, '├' },

                        { (DFlags)0b1111, '┼' }
                    };
                    break;

                case 0b01:
                    tileDict = new Dictionary<DFlags, char>
                    {
                        { 0b0000, ' ' },
                        { (DFlags)0b0001, '╴' },
                        { (DFlags)0b0010, '╶' },
                        { (DFlags)0b0100, '╵' },
                        { (DFlags)0b1000, '╷' },

                        { (DFlags)0b0011, '─' },
                        { (DFlags)0b1100, '│' },

                        { (DFlags)0b0101, '╯' },
                        { (DFlags)0b1001, '╮' },
                        { (DFlags)0b0110, '╰' },
                        { (DFlags)0b1010, '╭' },

                        { (DFlags)0b0111, '┴' },
                        { (DFlags)0b1011, '┬' },
                        { (DFlags)0b1101, '┤' },
                        { (DFlags)0b1110, '├' },

                        { (DFlags)0b1111, '┼' }
                    };
                    break;

                case 0b10:
                    tileDict = new Dictionary<DFlags, char>
                    {
                        { 0b0000, ' ' },
                        { (DFlags)0b0001, '╴' },
                        { (DFlags)0b0010, '╶' },
                        { (DFlags)0b0100, '╵' },
                        { (DFlags)0b1000, '╷' },

                        { (DFlags)0b0011, '═' },
                        { (DFlags)0b1100, '║' },

                        { (DFlags)0b0101, '╝' },
                        { (DFlags)0b1001, '╗' },
                        { (DFlags)0b0110, '╚' },
                        { (DFlags)0b1010, '╔' },

                        { (DFlags)0b0111, '╩' },
                        { (DFlags)0b1011, '╦' },
                        { (DFlags)0b1101, '╣' },
                        { (DFlags)0b1110, '╠' },

                        { (DFlags)0b1111, '╬' }
                    };
                    break;

                case 0b11:
                    tileDict = new Dictionary<DFlags, char>
                    {
                        { 0b0000, ' ' },
                        { (DFlags)0b0001, '╴' },
                        { (DFlags)0b0010, '╶' },
                        { (DFlags)0b0100, '╵' },
                        { (DFlags)0b1000, '╷' },

                        { (DFlags)0b0011, '═' },
                        { (DFlags)0b1100, '║' },

                        { (DFlags)0b0101, '╯' },
                        { (DFlags)0b1001, '╮' },
                        { (DFlags)0b0110, '╰' },
                        { (DFlags)0b1010, '╭' },

                        { (DFlags)0b0111, '╩' },
                        { (DFlags)0b1011, '╦' },
                        { (DFlags)0b1101, '╣' },
                        { (DFlags)0b1110, '╠' },

                        { (DFlags)0b1111, '╬' }
                    };
                    break;

                default:
                    throw new ArgumentException("type not valid",
                                                nameof(type));
            }
        }

        protected virtual DFlags[] GenerateBoxTiles()
        {
            List<List<DFlags>> boxComponents = new() { new() { (DFlags)0b1010, (DFlags)0b1001 },
                                                       new() { (DFlags)0b0110, (DFlags)0b0101 } };

            int height = Dimensions.Height;
            int width = Dimensions.Width;

            for (int i = 0; i < height; i++)
            {
                if (i == 0 || i == height - 1)
                {
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[i].Insert(1, (DFlags)0b0011);
                    }
                }
                else
                {
                    boxComponents.Insert(1, new() { (DFlags)0b1100, (DFlags)0b1100 });
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[1].Insert(1, 0b0000);
                    }
                }
            }

            DFlags[] boxTileArray = new DFlags[width + ((height - 1) * width)];
            for (int i = 0; i < boxTileArray.Length; i++)
            {
                boxTileArray[i] = boxComponents[i / width][i % width];
            }

            return boxTileArray;
        }

        public override void DrawTiles(CharBuffer buffer)
        {
            buffer.DrawTilesToBuffer(GenerateBoxTiles(), tileDict, Position.X, Position.Y, Dimensions.Width, Dimensions.Height);
        }
    }

    // have the menu resize to fit its contents - maybe tab should contain dimension info
    enum MenuTabSelected
    {
        Main,
        Backpack,
        Statistics,
        Settings
    }
    internal class Window : BoxContainer
    {
        public int TabWidth { get; private set; }

        public int Selected { get; private set; }

        public Window(Container parent, BoxContainerStyle type, Align align, Size padding, Size dimensions, int tabWidth, int zLayer = 0, bool enabled = true)
            : base(parent, type, align, padding, dimensions, zLayer, enabled, true)
        {
            TabWidth = tabWidth;

            Selected = 0;
        }

        public void NextTab()
        {
            ((Tab)children[Selected]).Selected = false;

            Selected++;

            int maxValue = ((int)Enum.GetValues(typeof(MenuTabSelected)).Cast<ItemTypes>().Max());
            if (Selected > maxValue) { Selected = maxValue; }

            ((Tab)children[Selected]).Selected = true;
        }

        public void PrevTab()
        {
            ((Tab)children[Selected]).Selected = false;

            Selected--;
            if (Selected < 0) { Selected = 0; }

            ((Tab)children[Selected]).Selected = true;
        }

        public void WinInit()
        {
            for (int i = 0; i < children.Count; i++)
            {
                if(i == (int)Selected) { ((Tab)children[i]).Selected = true; continue; }
                ((Tab)children[i]).Selected = false;
            }
        }

        public override void AddChild(Element element)
        {
            if(element is Tab tab) { AddTab(tab); }
            else { throw new ArgumentException("argument must be of type Tab", nameof(element)); }
        }

        public void ResizeWindow(Size newDim)
        {
            Position = new(Position.X - ((newDim.Width - Dimensions.Width) / 2),
                           Position.Y - ((newDim.Height - Dimensions.Height) / 2));

            Dimensions = newDim;

            ReloadTabs();
        }

        private void AddTab(Tab tab)
        {
            children.Add(tab);
            ReloadTabs();
        }

        private void ReloadTabs()
        {
            for (int i = 0; i < children.Count; i++)
            {
                Tab tab = (Tab)children[i];
                if (tab.Name is not null) { tab.UpdateTab(); }
            }
        }
    }

    internal class SettingsList : BoxContainer
    {
        public Size TabSize { get; private set; }

        public int Selected { get; private set; }

        public SettingsList(Container parent, BoxContainerStyle type, Align align, Size padding, Size dimensions, Size tabSize, int zLayer = 0, bool enabled = true)
            : base(parent, type, align, padding, dimensions, zLayer, enabled, true)
        {
            TabSize = tabSize;

            Selected = 0;
        }

        public void NextTab()
        {
            ((SettingsTab)children[Selected]).Selected = false;

            Selected++;

            int maxValue = ((int)Enum.GetValues(typeof(MenuTabSelected)).Cast<ItemTypes>().Max());
            if (Selected > maxValue) { Selected = maxValue; }

            ((SettingsTab)children[Selected]).Selected = true;
        }

        public void PrevTab()
        {
            ((SettingsTab)children[Selected]).Selected = false;

            Selected--;
            if (Selected < 0) { Selected = 0; }

            ((SettingsTab)children[Selected]).Selected = true;
        }

        public void ListInit()
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (i == (int)Selected) { ((SettingsTab)children[i]).Selected = true; continue; }
                ((SettingsTab)children[i]).Selected = false;
            }
        }

        public override void AddChild(Element element)
        {
            if (element is SettingsTab tab) { AddTab(tab); }
            else { throw new ArgumentException("argument must be of type Tab", nameof(element)); }
        }

        public void ResizeWindow(Size newDim)
        {
            Position = new(Position.X - ((newDim.Width - Dimensions.Width) / 2),
                           Position.Y - ((newDim.Height - Dimensions.Height) / 2));

            Dimensions = newDim;

            ReloadTabs();
        }

        private void AddTab(SettingsTab tab)
        {
            children.Add(tab);
            ReloadTabs();
        }

        private void ReloadTabs()
        {
            for (int i = 0; i < children.Count; i++)
            {
                SettingsTab tab = (SettingsTab)children[i];
                if (tab.Name is not null) { tab.UpdateTab(); }
            }
        }
    }


    internal class Button : BoxContainer
    {
        public Button(Container parent, BoxContainerStyle type, Align align, Size padding, Size dimensions, int zLayer = 0, bool enabled = true)
            : base(parent, type, align, padding, dimensions, zLayer, enabled, false)
        {
            if (Dimensions.Width == Parent.Dimensions.Width || Dimensions.Height == Parent.Dimensions.Height)
            {
                // log some kind of error
                throw new ArgumentOutOfRangeException(nameof(Dimensions),
                                                      Dimensions,
                                                      "Argument cannot be as large as parent dimensions");
            }
        }
    }

    internal class SelectionBox : BoxContainer
    {
        public SelectionBox(Container parent, string text, BoxContainerStyle type, Align align, Size padding, Size dimensions, int zLayer = 0, bool enabled = true)
            : base(parent, type, align, padding, dimensions, zLayer, enabled, false)
        {
            if (Dimensions.Width == Parent.Dimensions.Width || Dimensions.Height == Parent.Dimensions.Height)
            {
                // log some kind of error
                throw new ArgumentOutOfRangeException(nameof(Dimensions),
                                                      Dimensions,
                                                      "Argument cannot be as large as parent dimensions");
            }

            new Label(this, "◂", AlignText.left, Align.centreLeft, new(2, 0), 1);
            new Label(this, text, AlignText.centre, Align.centre, new(0, 0), text.Length);
            new Label(this, "▸", AlignText.right, Align.centreRight, new(2, 0), 1);
        }
    }


    enum ItemTabSelected
    {
        Accessory,
        Armour,
        Food,
        Key,
        Material,
        Potion,
        Scroll,
        Weapon
    }
    internal class ItemList : BoxContainer
    {
        public Size TabSize { get; private set; }

        private int tabSelected;
        private int itemSelected;

        private int listScroll;

        private BoxContainer itemBox;

        private Player player;

        private List<Item> itemList = new();

        public ItemList(Container parent, Player player, BoxContainerStyle type, Align align, Size padding, Size dimensions, Size tabSize, int zLayer = 0, bool enabled = true, bool scrollable = true)
            : base(parent, type, align, padding, dimensions, zLayer, enabled, scrollable)
        {
            TabSize = tabSize;

            tabSelected = 0;
            itemSelected = 0;

            this.player = player;

            player.inventory.AddItem(ItemDefinitions.steelHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironHelmet.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.strawHat.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.strawHat.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelBreastplate.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.chainmailShirt.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.chainmailShirt.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.chainmailShirt.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.chainmailShirt.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.tornShirt.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.tornShirt.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelLeggings.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelLeggings.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelLeggings.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelLeggings.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.skirtOfSpinning.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.raggedPants.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.raggedPants.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelGreaves.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.steelGreaves.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironGreaves.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironGreaves.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironGreaves.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironGreaves.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.ironGreaves.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.leatherShoes.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.leatherShoes.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.strawSandals.InstantiateNew());
            
            player.inventory.AddItem(ItemDefinitions.smolSword.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.smolSword.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.tallSword.InstantiateNew());
            
            player.inventory.AddItem(ItemDefinitions.bread.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.bread.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.bread.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.bread.InstantiateNew());
            
            player.inventory.AddItem(ItemDefinitions.strengthPotion.InstantiateNew());
            player.inventory.AddItem(ItemDefinitions.strengthPotion.InstantiateNew());

            itemBox = new((Container)Parent.GetChildren()[1], type, Align.centreRight, new(0, 0), new(Parent.Dimensions.Width - (Dimensions.Width + TabSize.Width - 2), Parent.Dimensions.Height));
            ReloadItems();
        }

        public void NextTab()
        {
            ((ItemTab)children[tabSelected]).Selected = false;

            tabSelected++;

            int maxValue = ((int)Enum.GetValues(typeof(ItemTypes)).Cast<ItemTypes>().Max()) - 1;
            if (tabSelected > maxValue) { tabSelected = maxValue; }
            else
            {
                itemSelected = 0;
                listScroll = 0;
            }

            ((ItemTab)children[tabSelected]).Selected = true;

            ReloadItems();
            ReloadItemBox();
        }

        public void PrevTab()
        {
            ((ItemTab)children[tabSelected]).Selected = false;

            tabSelected--;
            if(tabSelected < 0) { tabSelected = 0; }
            else
            {
                itemSelected = 0;
                listScroll = 0;
            }

            ((ItemTab)children[tabSelected]).Selected = true;

            ReloadItems();
            ReloadItemBox();
        }

        public void NextItem()
        {
            itemSelected++;
            if (itemSelected >= itemList.Count) { itemSelected = itemList.Count - 1; }

            if(itemSelected >= Dimensions.Height - 2 + listScroll) { listScroll++; }

            ReloadItemBox();
        }

        public void PrevItem()
        {
            itemSelected--;
            if (itemSelected < 0) { itemSelected = 0; }

            if (itemSelected < listScroll) { listScroll--; }

            ReloadItemBox();
        }

        public Item GetCurrentItem()
        {
            return itemList[itemSelected];
        }

        public override void AddChild(Element element)
        {
            if (element is ItemTab tab) { AddTab(tab); }
            else { throw new ArgumentException("argument must be of type ItemTab", nameof(element)); }
        }

        public override void DrawText(CharBuffer buffer)
        {
            string[] itemText = new string[itemList.Count];
            bool[] selected = new bool[itemText.Length];
            for (int i = 0; i < itemList.Count; i++)
            {
                itemText[i] = " " + itemList[i].sprite + " " + itemList[i].name.Truncate(Dimensions.Width - 2 - 6);

                if(i == listScroll && listScroll > 0)
                {
                    int itemTextLen = itemText[i].Length;
                    for (int j = 0; j < Dimensions.Width - 4 - itemTextLen; j++)
                    {
                        itemText[i] = itemText[i].Insert(itemText[i].Length, " ");
                    }
                    itemText[i] = itemText[i].Insert(itemText[i].Length, "▲");
                }
                else if (i == itemList.Count - (itemList.Count - (Dimensions.Height - 2) - listScroll) - 1 && listScroll < itemList.Count - (Dimensions.Height - 2))
                {
                    int itemTextLen = itemText[i].Length;
                    for (int j = 0; j < Dimensions.Width - 4 - itemTextLen; j++)
                    {
                        itemText[i] = itemText[i].Insert(itemText[i].Length, " ");
                    }
                    itemText[i] = itemText[i].Insert(itemText[i].Length, "▼");
                }
                // TODO: figure out why this isnt working
                else if (itemList[i].equipped)
                {
                    int itemTextLen = itemText[i].Length;
                    for (int j = 0; j < Dimensions.Width - 4 - itemTextLen; j++)
                    {
                        itemText[i] = itemText[i].Insert(itemText[i].Length, " ");
                    }
                    itemText[i] = itemText[i].Insert(itemText[i].Length, "*");
                }
                if (i == itemSelected) { selected[i] = true; }
            }

            int x = Position.X + 1;
            int y = Position.Y + 1;
            for (int i = 0; i < itemText.Length; i++)
            {
                if (y + i < Position.Y + Dimensions.Height - 1)
                {
                    int textIndex = i + listScroll;
                    for (int j = 0; j < itemText[textIndex].Length; j++)
                    {
                        buffer.EditChar(itemText[textIndex][j], x + j, y + i);
                        if (selected[textIndex])
                        {
                            buffer.EditTextColour(ConsoleColor.Black, x + j, y + i);
                            buffer.EditHighlight(ConsoleColor.White, x + j, y + i);
                        }
                    }
                }
            }
        }

        protected override DFlags[] GenerateBoxTiles()
        {
            List<List<DFlags>> boxComponents = new()
            {
                new() { (DFlags)0b1010, (DFlags)0b1001 },
                new() { (DFlags)0b0110, (DFlags)0b0101 }
            };

            int height = Dimensions.Height;
            int width = Dimensions.Width;

            for (int i = 0; i < height; i++)
            {
                if (i == 0)
                {
                    for (int j = 2; j < width; j++)
                    {
                        if (j == 8) { boxComponents[i].Insert(1, (DFlags)0b0110); continue; }
                        if (j > 8 && j < 21) { boxComponents[i].Insert(1, (DFlags)0b0000); continue; }
                        if (j == 21) { boxComponents[i].Insert(1, (DFlags)0b0101); continue; }
                        boxComponents[i].Insert(1, (DFlags)0b0011);
                    }
                }
                else if (i == height - 1)
                {
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[i].Insert(1, (DFlags)0b0011);
                    }
                }
                else
                {
                    boxComponents.Insert(1, new() { (DFlags)0b1100, (DFlags)0b1100 });
                    for (int j = 2; j < width; j++)
                    {
                        boxComponents[1].Insert(1, 0b0000);
                    }
                }
            }

            DFlags[] boxTileArray = new DFlags[width + ((height - 1) * width)];
            for (int i = 0; i < boxTileArray.Length; i++)
            {
                boxTileArray[i] = boxComponents[i / width][i % width];
            }

            return boxTileArray;
        }

        private void AddTab(ItemTab tab)
        {
            children.Add(tab);
            ReloadTabs();

            if (children.IndexOf(tab) == tabSelected) { tab.Selected = true; }
            else { tab.Selected = false; }
        }

        private void ReloadTabs()
        {
            for (int i = 0; i < children.Count - 1; i++)
            {
                ((ItemTab)children[i]).UpdateTab();
            }
        }

        private void ReloadItems()
        {
            itemList = new();
            foreach (Item item in player.inventory.Items)
            {
                if (item.type == (ItemTypes)(tabSelected + 1))
                {
                    itemList.Add(item);
                }
            }
        }

        private void ReloadItemBox()
        {
            itemBox.Destroy();
            itemBox = new((Container)Parent.GetChildren()[1], type, Align.centreRight, new(0, 0), new(Parent.Dimensions.Width - (Dimensions.Width + TabSize.Width - 2), Parent.Dimensions.Height));
            if (itemList.Count > 0) { GenerateItemInfo(itemList[itemSelected]); }
        }

        private void GenerateItemInfo(Item item)
        {
            // name
            new Label(itemBox, item.name, AlignText.centre, Align.topCentre, new(0, 1), 17); 

            // value
            string valueText = item.value.ToString();
            if (item.value > 999999999999) { valueText = "∞"; }
            else if(item.value > 999999999) { valueText = valueText.Substring(0, valueText.Length - 9) + "B"; }
            else if(item.value > 999999) { valueText = valueText.Substring(0, valueText.Length - 6) + "M"; }
            else if(item.value > 999) { valueText = valueText.Substring(0, valueText.Length - 3) + "K"; }
            else { valueText = item.value.ToString(); }
            new Label(itemBox, valueText + "☼", AlignText.right, Align.topRight, new(2, 1), 5, false);

            // image
            new Image(itemBox, UIHandler.GetImageFromFile(item.imageLocation), Align.topCentre, new(0, 3));

            // description
            new Label(itemBox, item.description, AlignText.centre, Align.topCentre, new(0, 5), 23, false, true);

            // stats
            int statCounter = (int)Math.Floor(item.description.Length / 23f);
            if (item.damage is not null) { new Label(itemBox, "- Damage: " + item.damage, AlignText.left, Align.topLeft, new(1, 9 + statCounter), 25, false); statCounter++;}
            if (item.attack is not null) { new Label(itemBox, "- Attack + " + item.attack, AlignText.left, Align.topLeft, new(1, 9 + statCounter), 25, false); statCounter++; }
            if (item.defence is not null) { new Label(itemBox, "- Defence + " + item.defence, AlignText.left, Align.topLeft, new(1, 9 + statCounter), 25, false); statCounter++; }
            if (item.speed is not null) { new Label(itemBox, "- Speed + " + item.speed, AlignText.left, Align.topLeft, new(1, 9 + statCounter), 25, false); statCounter++; }
            if (item.sustenance is not null) { new Label(itemBox, "- Sustenance + " + item.sustenance, AlignText.left, Align.topLeft, new(1, 9 + statCounter), 25, false); statCounter++; }

            // weight
            new Label(itemBox, item.weight + "҂", AlignText.right, Align.bottomRight, new(2, 1), 5, false);
        }
    }

    // text
    internal class Image : Element
    {
        public char[][] TextImage { get; private set; }

        public Image(Container parent, char[][] textImage, Align align, Size padding, int zLayer = 0, bool enabled = true)
            : base(parent, new(0, 0), new(textImage[0].Length, textImage.Length), new(1, 1), zLayer, enabled)
        {
            if (Dimensions.Width == Parent.Dimensions.Width || Dimensions.Height == Parent.Dimensions.Height)
            {
                // log some kind of error
                throw new ArgumentOutOfRangeException(nameof(Dimensions),
                                                      Dimensions,
                                                      "Argument cannot be as large as parent dimensions");
            }

            TextImage = textImage;

            Position = FindPositionFromParent(align, padding);
        }

        public override void DrawText(CharBuffer buffer)
        {
            for (int i = 0; i < TextImage.Length; i++)
            {
                for (int j = 0; j < TextImage[i].Length; j++)
                {
                    buffer.EditChar(TextImage[i][j], Position.X + j, Position.Y + i);
                }
            }
        }
    }

    enum AlignText
    {
        left,
        centre,
        right
    }

    // impliment wrappable text
    internal class Label : Element
    {
        public string Text { get; set; }

        private AlignText TextAlign { get; set; }

        public bool Selected { get; set; }
        private bool Truncatable { get; set; }
        private bool Wrappable { get; set; }

        public Label(Container parent, string text, AlignText textAlign, Align align, Size padding, int maxTextSize, bool truncatable = true, bool wrappable = false, int zLayer = 0, bool enabled = true)
            : base(parent, new(0, 0), new(maxTextSize, 1), new(1, 1), zLayer, enabled)
        {
            if (Dimensions.Width == Parent.Dimensions.Width || Dimensions.Height == Parent.Dimensions.Height)
            {
                // log some kind of error
                throw new ArgumentOutOfRangeException(nameof(Dimensions),
                                                      Dimensions,
                                                      "Argument cannot be as large as parent dimensions");
            }

            Selected = false;
            Truncatable = truncatable;
            Wrappable = wrappable;

            if (Truncatable && Wrappable)
            {
                throw new ArgumentException("Truncatable and Wrappable cannot both be true");
            }

            Text = text;

            if (!Truncatable && !Wrappable && Text.Length > Dimensions.Width)
            {
                throw new ArgumentOutOfRangeException(nameof(Text),
                                                      Text.Length,
                                                      "Text cannot be longer than element unless truncatable or wrappable");
                // log some error
            }

            TextAlign = textAlign;

            Position = FindPositionFromParent(align, padding);
        }

        public override void DrawText(CharBuffer buffer)
        {
            string text;
            if (Truncatable && Text.Length > Dimensions.Width) { text = Text.Truncate(Dimensions.Width); }
            else { text = Text; }

            int x;
            switch (TextAlign)
            {
                // left-aligned
                case AlignText.left:
                    x = Position.X;
                    break;

                // centre-aligned
                case AlignText.centre:
                    if (text.Length > Dimensions.Width) { x = Position.X; }
                    else { x = Position.X + ((Dimensions.Width / 2) - (text.Length / 2)); }
                    break;

                // right-aligned
                case AlignText.right:
                    if (text.Length > Dimensions.Width) { x = Position.X; }
                    else { x = Position.X + (Dimensions.Width - text.Length); }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(TextAlign),
                                                          TextAlign,
                                                          "Argument did not divide correctly");
            }

            if (Wrappable)
            {
                string wholeText = text;
                for (int i = 0; i < Math.Ceiling((float)text.Length / Dimensions.Width); i++)
                {
                    string tempText = "";
                    bool tooBig = false;
                    while (!tooBig)
                    {
                        string wordToAdd = wholeText.Split(' ')[0];
                        if (tempText.Length + wordToAdd.Length + (tempText.Length == 0 ? 0 : 1) > Dimensions.Width)
                        {
                            if (tempText.Length == 0)
                            {
                                tempText = wholeText.Substring(0, Dimensions.Width);
                                wholeText = wholeText.Remove(0, Dimensions.Width);
                            }
                            tooBig = true;
                        }
                        else
                        {
                            // if there's a word before this one, we want to add a space
                            if (tempText.Length > 0) { tempText += " "; }
                            tempText += wordToAdd;

                            wholeText = wholeText.Remove(0, wordToAdd.Length);
                            // and if there's more words to come, then we want to remove the leading space
                            if(wholeText.Length > 0) { wholeText = wholeText.Remove(0, 1); }
                        }
                        if(wholeText.Length == 0) { tooBig = true; }
                    }

                    if (TextAlign == AlignText.centre) { x = Position.X + ((Dimensions.Width / 2) - (tempText.Length / 2)); }

                    for (int j = 0; j < tempText.Length; j++)
                    {
                        buffer.EditChar(tempText[j], x + j, Position.Y + i + 2);
                        if (Selected)
                        {
                            buffer.EditTextColour(ConsoleColor.Black, x + j, Position.Y + i + 2);
                            buffer.EditHighlight(ConsoleColor.White, x + j, Position.Y + i + 2);
                        }
                    }
                }
            }

            else
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (x + i > Parent.Position.X && x + i < Parent.Position.X + (Parent.Dimensions.Width - 1))
                    {
                        buffer.EditChar(text[i], x + i, Position.Y);
                        if (Selected)
                        {
                            buffer.EditTextColour(ConsoleColor.Black, x + i, Position.Y);
                            buffer.EditHighlight(ConsoleColor.White, x + i, Position.Y);
                        }
                    }
                }
            }
        }
    }
}
