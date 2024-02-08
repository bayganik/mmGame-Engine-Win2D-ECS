using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml.Controls;
using System.Runtime.InteropServices.WindowsRuntime;
using Entitas;
using System.ComponentModel;

namespace mmGameEngine
{
    public static class Global
    {
        //
        // Game state
        //
        public static bool GameOver = false;
        public static GameState StateOfGame;
        public static int GameScore;
        public static Int64 FrameCount;
        //
        // Special Render Layers (lowest number first to render)
        //
        public const int SCROLLINGBACK_LAYER = -2000;
        public const int TILEMAP_LAYER = -1500;
        public const int BOXCOLLIDER_LAYER = 1000;
        public const int CURSOR_LAYER = 100000;
        public const int UI_LAYER = 99000;

        //public static Page MainGamePage;
        public static Matrix3x2 Camera;
        public static Scene CurrentScene;
        public static int TotalNumberOfPaths = 1000;
        public static int DesiredConnectionsPerPath = 250;
        public static bool DebugMode = false;
        public const double TwoPi = 2d * Math.PI;
        //
        // Canvas (to render)
        //
        public static CanvasDevice CanvasDeviceInUse;
        public static CanvasSwapChain SwapChain;
        public static CanvasDrawingSession CanvasDraw;
        public static CanvasSpriteBatch SpriteBatchDraw;
        //
        // Entitas ECS context for Game/Scene UI
        //
        public static Entitas.Context EntityContext;
        public static Entitas.Context SceneContext;
        //
        // Screen size 
        //
        public static Size ScreenSize = new Size(800, 600);
        public static Vector2 WindowCenter = new Vector2((float)ScreenSize.Width / 2, (float)ScreenSize.Height / 2);
        public static Vector2 WorldSize = new Vector2(400, 400);
        public static double DeltaTime;
        public static float HalfHeight = 200;
        public static float HalfWidth = 200;
        public static Vector2 ViewportCenter;   //always at 0,0 when starting

        public static bool DebugRenderEnabled = false;
        /// <summary>
        /// use method DestroyGameEntity() . Don't add directly
        /// </summary>
        public static Dictionary<Entity, bool> GameEntityToDestroy;
        public static Dictionary<Entity, bool> SceneEntityToDestroy;

        public static double[] DirectionAngles =
        {
            0.50 * Math.PI, // Down
            0.75 * Math.PI, // DownLeft
            1.00 * Math.PI, // Left
            1.25 * Math.PI, // UpLeft
            1.50 * Math.PI, // Up
            1.75 * Math.PI, // UpRight
            0.00 * Math.PI, // Right
            0.25 * Math.PI  // DownRight
        };
        public static List<Sprite> SpritesFromSheet(Entity entity, CanvasBitmap texture,
                                                    int cellWidth, int cellHeight,
                                                    int cellOffset = 0, int maxCellsToInclude = int.MaxValue)
        {
            var spritesList = new List<Sprite>();

            var cols = texture.Bounds.Width / cellWidth;
            var rows = texture.Bounds.Height / cellHeight;
            var i = 0;

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    // skip everything before the first cellOffset
                    if (i++ < cellOffset)
                        continue;
                    //
                    // Separate the image from the sheet
                    //
                    byte[] oneFrameImage = texture.GetPixelBytes(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                    CanvasBitmap _image = CanvasBitmap.CreateFromBytes(SwapChain, oneFrameImage, cellWidth, cellHeight, DirectXPixelFormat.B8G8R8A8UIntNormalized);

                    Sprite tmpSp = new Sprite(_image, new Rect(x * cellWidth, y * cellHeight, cellWidth, cellHeight));
                    tmpSp.SourceRect = new Rect(0, 0, cellWidth, cellHeight);

                    tmpSp.OwnerEntity = entity;
                    spritesList.Add(tmpSp);

                    // once we hit the max number of cells to include bail out. were done.
                    if (spritesList.Count == maxCellsToInclude)
                        return spritesList;
                }
            }

            return spritesList;
        }
        public static List<CanvasBitmap> ImagesFromSheet(CanvasBitmap texture,
                                            int cellWidth, int cellHeight,
                                            int cellOffset = 0, int maxCellsToInclude = int.MaxValue)
        {
            var imagesList = new List<CanvasBitmap>();

            var cols = texture.Bounds.Width / cellWidth;
            var rows = texture.Bounds.Height / cellHeight;
            var i = 0;

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    // skip everything before the first cellOffset
                    if (i++ < cellOffset)
                        continue;
                    //
                    // Separate the image from the sheet
                    //
                    byte[] oneFrameImage = texture.GetPixelBytes(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                    CanvasBitmap _image = CanvasBitmap.CreateFromBytes(SwapChain, oneFrameImage, cellWidth, cellHeight, DirectXPixelFormat.B8G8R8A8UIntNormalized);
                    imagesList.Add(_image);

                    // once we hit the max number of cells to include bail out. were done.
                    if (imagesList.Count == maxCellsToInclude)
                        return imagesList;
                }
            }

            return imagesList;
        }

        public static float RotateToFace(Vector2 _pos, Vector2 _focus)
        {
            //
            // Rotation returned as degrees to turn
            //
            Vector2 direction = new Vector2();
            double angle = 0;
            float rotation = 0f;


            direction.X = _focus.X - _pos.X;
            direction.Y = _focus.Y - _pos.Y;

            double radianstodegrees = 180 / Math.PI;
            angle = Math.Atan2(direction.Y, direction.X) * radianstodegrees;
            angle += 90;

            return rotation = (float)angle;

        }
        public static Direction GetDirection(double angle)
        {
            while (angle < 0)
                angle += Math.PI * 2;

            double bestDirectionScore = double.MaxValue;
            Direction bestDirection = Direction.Down;

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
            {
                var directionAngle = DirectionAngles[(int)direction];

                var score = Math.Abs(directionAngle - angle);

                if (score < bestDirectionScore)
                {
                    bestDirectionScore = score;
                    bestDirection = direction;
                }
            }

            return bestDirection;
        }
        //
        // Add an entity/children to be destroyed after Scene Update/Render is done
        //
        public static void DestroyGameEntity(Entity entity)
        {
            //
            // All children added first (to be removed)
            //
            if (entity.Get<TransformComponent>().ChildCount > 0)
            {
                foreach (Component child in entity.Get<TransformComponent>().Children)
                {
                    GameEntityToDestroy.TryAdd(child.OwnerEntity, true);
                    SceneColliderManager.RemoveCollider(child.OwnerEntity);
                }
            }
            //
            // Add entity to be removed
            //
            GameEntityToDestroy.TryAdd(entity, true);
            SceneColliderManager.RemoveCollider(entity);
        }
        #region  // Scene/Game Entity Create \\
        //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
        //               Create Entity (Node} in this Scene
        //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
        public static Entity CreateGameEntity(string name, Vector2 initPosition, float initScale = 1.0f)
        {
            Entity ent = EntityContext.CreateEntity();
            ent.EntityType = 0;
            ent.Name = name;
            ent.IsVisible = true;

            //
            // Add a Transform component
            //
            TransformComponent trm = new TransformComponent();
            trm.Position = initPosition;
            trm.Scale = new Vector2(initScale, initScale);
            trm.Rotation = 0;

            ent.Add(trm);

            return ent;
        }
        public static Entity CreateGameEntity(Vector2 initPosition, float initScale = 1.0f)
        {
            return CreateGameEntity("gameEnt", initPosition, initScale);
        }
        public static Entity CreateGameEntity(string name = "")
        {
            if (string.IsNullOrEmpty(name))
                name = "gameEnt";
            return CreateGameEntity(name, Vector2.One, 1.0f);
        }
        /// <summary>
        /// Create a Scene Entity (drawn on top of game scene e.g. UI entity)
        /// </summary>
        /// <returns></returns>
        public static Entity CreateSceneEntity(string name = "", float initScale = 1.0f)
        {
            Entity ent = CreateGameEntity(name, Vector2.Zero, initScale);
            ent.EntityType = 1;
            return ent;
        }
        /// <summary>
        /// Create a Scene Entity (drawn on top of game scene e.g. UI entity)
        /// </summary>
        /// <param name="initPosition"></param>
        /// <param name="initScale"></param>
        /// <returns></returns>
        public static Entity CreateSceneEntity(Vector2 initPosition)
        {
            Entity ent = CreateGameEntity("sceneEnt", initPosition, 1.0f);
            ent.EntityType = 1;
            return ent;
        }

        #endregion
        #region Map Creation Data
        public static bool TmxMapAvailable = false;
        public static int MapWidthInPixels = 1920;
        public static int MapHeightInPixels = 1080;

        public static int PixelScale = 10;
        public static int Padding = 0;
        public static Vector2 MapPosition = Vector2.Zero;
        public static int MaxRoomConnections = 3;
        public static int MapResolution = 16;
        public static int MaxPathConnectionAttempts = 5;

        public static Dictionary<string, int> DirectionalStringToInt = new Dictionary<string, int>();
        public static Dictionary<int, string> IntToDirectionalString = new Dictionary<int, string>();
        public static Dictionary<int, string> IntToClockwiseDirectionalString = new Dictionary<int, string>();

        // probability that region will continue to try to expand past minimum size
        // calculated once for each tile added
        // e.g. a tile that has just met minimum size requirements has an n% chance of trying to add an additional tile (will fail if attempted add is already occupied),
        //  then an n% chance of attempting to add another tile after that, and so on
        public static int ProbabilityOfExpansion = 0;
        public static int MinimumRegionSize = 100;
        public static int MinimumCaveSize = 100;
        public static int MergeThreshold = 500;
        #endregion

        #region Mouse Position
        public static Vector2 ScreenToWorld(Vector2 _pos)
        {
            Matrix3x2 _inverseTransformMatrix = Matrix3x2.Identity;
            //
            // following matrix when moving to 0,0 location
            //
            Matrix3x2 tempMat = Matrix3x2.CreateTranslation(Vector2.Zero);

            Global.Camera = Matrix3x2.Multiply(Global.Camera, tempMat);
            //
            // Get current Camera matrix and fine inverse tranform matrix
            //
            bool transform = Matrix3x2.Invert(Global.Camera, out _inverseTransformMatrix);
            //
            // determine the current position in this world
            //
            Vector2 tempVector = Vector2.Transform(_pos, _inverseTransformMatrix);
            return tempVector;
        }
        #endregion

        #region Canvas Layout
        public static int CanvasWidth;
        public static int CanvasHeight;
        #endregion

        #region Fonts / Text Formats
        public static CanvasTextFormat FontSmall = new CanvasTextFormat();
        public static CanvasTextFormat FontMedium = new CanvasTextFormat();
        public static CanvasTextFormat FontLarge = new CanvasTextFormat();
        public static CanvasTextFormat FontExtraLarge = new CanvasTextFormat();

        public static CanvasTextFormat DefaultFont;
        public static CanvasTextFormat DefaultFontNoWrap;

        public static CanvasTextLayout UpArrow;

        public static CanvasTextLayout DoubleUpArrow;
        public static CanvasTextLayout DownArrow;
        public static CanvasTextLayout DoubleDownArrow;

        private static Dictionary<char, double> CharacterWidthDictionary = new Dictionary<char, double>();
        #endregion

        #region Region Naming
        public static string[] RegionTypes = {
            "Forest",
            "Plains",
            "Desert",
            "Mountain",
            "Plateau",
            "Steppes",
            "Volcano",
            "Highlands",
            "Canyon",
            "Valley",
            "Marsh",
            "Bog",
            "Swamp",
            "Drylands",
            "Wetlands",
            "Jungle",
            "Hills"
        };

        public static string[] RegionNames = {
            "Cornelia",
            "Pravoka",
            "Elfheim",
            "Duergar",
            "Melmond",
            "Onrac",
            "Lufenia",
            "Gaia",

            "Altair",
            "Gatrea",
            "Paloom",
            "Poft",
            "Salamand",
            "Bafsk",
            "Fynn",
            "Mysidia",
            "Machanon",

            "Ur",
            "Kazus",
            "Canaan",
            "Tozus",
            "Tokkul",
            "Gysahl",
            "Amur",
            "Replito",
            "Duster",
            "Saronia",
            "Falgabard",

            "Baron",
            "Kaipo",
            "Fabul",
            "Troia",
            "Mist",
            "Mythril",
            "Agart",
            "Eblan",
            "Tomra",

            "Tule",
            "Carwen",
            "Walse",
            "Karnak",
            "Crescent",
            "Jachol",
            "Istory",
            "Lix",
            "Regole",
            "Quelb",
            "Surgate",
            "Moore",

            "Narshe",
            "Figaro",
            "Mobliz",
            "Nikeah",
            "Kohlingen",
            "Jidoor",
            "Zozo",
            "Maranda",
            "Tzen",
            "Albrook",
            "Vector",
            "Thamasa"
        };

        //public static string[] CaveNames = {
        //    ""
        //};

        public static string[] CaveNameStyles =
        {
            "Cave of /1",
            "/1 Cave",
            "Caverns of /1",
            "/1 Caverns"
        };

        public static string RandomRegionName()
        {
            return RegionNames.RandomArrayItem();
        }

        public static string RandomCaveName()
        {
            // TODO: using region names for now; replace with cave names
            string strCaveName = RegionNames.RandomArrayItem();
            return CaveNameStyles.RandomArrayItem().Replace("/1", strCaveName);
        }

        public static string RandomRegionType()
        {
            string strRegionType = RegionTypes.RandomArrayItem();
            string strRegionName = RegionNames.RandomArrayItem();

            switch (Random.Next(2))
            {
                case 0:
                    // use region type as prefix
                    return strRegionType + " of " + strRegionName;
                case 1:
                    // use region type as suffix
                    return strRegionName + " " + strRegionType;
                default:
                    return string.Empty;
            }
        }
        #endregion

        #region Random
        public static Random Random = new Random(DateTime.Now.Millisecond);
        public static Color RandomColor()
        {
            int red = 20 + Random.Next(235);
            int green = 20 + Random.Next(235);
            int blue = 20 + Random.Next(235);

            return Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
        }
        internal static Color RandomCaveColor()
        {
            int red = 150 + Random.Next(75);
            int green = red;
            int blue = red;

            return Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);
        }
        public static T RandomListItem<T>(this List<T> list)
        {
            return list[Random.Next(list.Count)];
        }
        public static T RandomArrayItem<T>(this T[] array)
        {
            return array[Random.Next(array.Length)];
        }
        #endregion

        #region HitTest
        public static bool HitTestRect(Rect rect, Point point)
        {
            if (point.X < rect.X) { return false; }
            if (point.X >= rect.X + rect.Width) { return false; }
            if (point.Y < rect.Y) { return false; }
            if (point.Y >= rect.Y + rect.Height) { return false; }

            return true;
        }
        #endregion

        #region VirtualKeyToString
        [DllImport("user32.dll")]
        public static extern int ToUnicode(uint virtualKeyCode, uint scanCode,
            byte[] keyboardState,
            [Out, MarshalAs(UnmanagedType.LPWStr, SizeConst = 64)]
            StringBuilder receivingBuffer,
            int bufferSize, uint flags);

        public static string VirtualKeyToString(VirtualKey keys, bool shift = false, bool altGr = false)
        {
            var buf = new StringBuilder(256);
            var keyboardState = new byte[256];
            if (shift)
                keyboardState[(int)VirtualKey.Shift] = 0xff;
            if (altGr)
            {
                keyboardState[(int)VirtualKey.Control] = 0xff;
                keyboardState[(int)VirtualKey.Menu] = 0xff;
            }
            ToUnicode((uint)keys, 0, keyboardState, buf, 256, 0);
            return buf.ToString();
        }
        #endregion

        #region Initialization
        static Global()
        {
            //
            // This gets executed, first time Global is used (mmGame.cs)
            //
            FontSmall.FontFamily = "Old English Text MT";
            FontSmall.FontSize = 18;
            FontSmall.WordWrapping = CanvasWordWrapping.NoWrap;

            FontMedium.FontFamily = "Old English Text MT";
            FontMedium.FontSize = 24;
            FontMedium.WordWrapping = CanvasWordWrapping.NoWrap;

            FontLarge.FontFamily = "Old English Text MT";
            FontLarge.FontSize = 32;
            FontLarge.WordWrapping = CanvasWordWrapping.NoWrap;

            FontExtraLarge.FontFamily = "Old English Text MT";
            FontExtraLarge.FontSize = 48;
            FontExtraLarge.WordWrapping = CanvasWordWrapping.NoWrap;

            DefaultFont = new CanvasTextFormat();
            DefaultFont.FontFamily = "Arial";
            DefaultFont.FontSize = 14;
            DefaultFont.WordWrapping = CanvasWordWrapping.Wrap; //.NoWrap;

            DefaultFontNoWrap = new CanvasTextFormat();
            DefaultFontNoWrap.FontFamily = "Arial";
            DefaultFontNoWrap.FontSize = 14;
            DefaultFontNoWrap.WordWrapping = CanvasWordWrapping.NoWrap; //.NoWrap;

            LoadDirectionalStringToInt();
            LoadIntToDirectionalString();
            LoadIntToClockwiseDirectionalString();
        }
        private static void LoadDirectionalStringToInt()
        {
            DirectionalStringToInt.Add("nw", 0);
            DirectionalStringToInt.Add("n", 1);
            DirectionalStringToInt.Add("ne", 2);
            DirectionalStringToInt.Add("w", 3);
            DirectionalStringToInt.Add("o", 4);
            DirectionalStringToInt.Add("e", 5);
            DirectionalStringToInt.Add("sw", 6);
            DirectionalStringToInt.Add("s", 7);
            DirectionalStringToInt.Add("se", 8);
        }
        public static int SortRoomConnections(string strDirection1, string strDirection2)
        {
            int n1 = -1;
            DirectionalStringToInt.TryGetValue(strDirection1, out n1);

            int n2 = -2;
            DirectionalStringToInt.TryGetValue(strDirection2, out n2);

            // DirectionalStringToInt[x].CompareTo(Statics.DirectionalStringToInt[y]))
            return n1.CompareTo(n2);
        }
        private static void LoadIntToDirectionalString()
        {
            IntToDirectionalString.Add(0, "nw");
            IntToDirectionalString.Add(1, "n");
            IntToDirectionalString.Add(2, "ne");
            IntToDirectionalString.Add(3, "w");
            IntToDirectionalString.Add(4, "o");
            IntToDirectionalString.Add(5, "e");
            IntToDirectionalString.Add(6, "sw");
            IntToDirectionalString.Add(7, "s");
            IntToDirectionalString.Add(8, "se");
        }
        private static void LoadIntToClockwiseDirectionalString()
        {
            IntToClockwiseDirectionalString.Add(0, "nw");
            IntToClockwiseDirectionalString.Add(1, "n");
            IntToClockwiseDirectionalString.Add(2, "ne");
            IntToClockwiseDirectionalString.Add(3, "e");
            IntToClockwiseDirectionalString.Add(4, "se");
            IntToClockwiseDirectionalString.Add(5, "s");
            IntToClockwiseDirectionalString.Add(6, "sw");
            IntToClockwiseDirectionalString.Add(7, "w");
        }
        public static void Initialize(CanvasDevice device)
        {
            LoadCharacterWidths(device);

            UpArrow = new CanvasTextLayout(device, "\u2191", DefaultFontNoWrap, 0, 0);
            DoubleUpArrow = new CanvasTextLayout(device, "\u219f", DefaultFontNoWrap, 0, 0);
            DownArrow = new CanvasTextLayout(device, "\u2193", DefaultFontNoWrap, 0, 0);
            DoubleDownArrow = new CanvasTextLayout(device, "\u21a1", DefaultFontNoWrap, 0, 0);
        }
        private static void LoadCharacterWidths(CanvasDevice device)
        {
            string str = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            str += @"abcdefghijklmnopqrstuvwxyz";

            str += @"1234567890";
            str += @"!@#$%^&*()";

            str += @"`~,<.>/?\|[{]}=+-_";

            foreach (char c in str)
            {
                CanvasTextLayout l = new CanvasTextLayout(device, c.ToString(), DefaultFontNoWrap, 0, 0);
                CharacterWidthDictionary.Add(c, l.LayoutBounds.Width);
            }
        }
        #endregion

        #region Character/String Width

        public static double StringWidth(string str)
        {
            double dWidth = 0;

            foreach (char c in str.Replace(' ', '.'))
            {
                dWidth += CharacterWidthDictionary[c];
            }

            return dWidth;
        }
        #endregion
        /// <summary>
        /// swaps the two object types
        /// </summary>
        /// <param name="first">First.</param>
        /// <param name="second">Second.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void Swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }
        public static string RandomString(int size = 38)
        {
            var builder = new StringBuilder();

            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
        public static string GetOppositeDirection(string strDirection)
        {
            switch (strDirection)
            {
                case "nw": return "se";
                case "n": return "s";
                case "ne": return "sw";
                case "w": return "e";
                case "e": return "w";
                case "sw": return "ne";
                case "s": return "n";
                case "se": return "nw";
            }

            return string.Empty;
        }

        public static List<T> GetEnumAsList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        public static Matrix3x2 GetDisplayTransform(Vector2 outputSize, Vector2 sourceSize)
        {
            // Scale the display to fill the control.
            var scale = outputSize / sourceSize;
            var offset = Vector2.Zero;

            // Letterbox or pillarbox to preserve aspect ratio.
            if (scale.X > scale.Y)
            {
                scale.X = scale.Y;
                offset.X = (outputSize.X - sourceSize.X * scale.X) / 2;
            }
            else
            {
                scale.Y = scale.X;
                offset.Y = (outputSize.Y - sourceSize.Y * scale.Y) / 2;
            }

            return Matrix3x2.CreateScale(scale) *
                   Matrix3x2.CreateTranslation(offset);
        }

        public static CanvasGeometry CreateStarGeometry(ICanvasResourceCreator resourceCreator, float scale, Vector2 center)
        {
            Vector2[] points =
            {
                new Vector2(-0.24f, -0.24f),
                new Vector2(0, -1),
                new Vector2(0.24f, -0.24f),
                new Vector2(1, -0.2f),
                new Vector2(0.4f, 0.2f),
                new Vector2(0.6f, 1),
                new Vector2(0, 0.56f),
                new Vector2(-0.6f, 1),
                new Vector2(-0.4f, 0.2f),
                new Vector2(-1, -0.2f),
            };

            var transformedPoints = from point in points
                                    select point * scale + center;

            return CanvasGeometry.CreatePolygon(resourceCreator, transformedPoints.ToArray());
        }

        public static float DegreesToRadians(float angle)
        {
            return angle * (float)Math.PI / 180;
        }

        static readonly Random random = new Random();

        //public static Random Random
        //{
        //    get { return random; }
        //}

        public static float RandomBetween(float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }

        public static async Task<byte[]> ReadAllBytes(string filename)
        {
            var uri = new Uri("ms-appx:///" + filename);
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            var buffer = await FileIO.ReadBufferAsync(file);

            return buffer.ToArray();
        }

        public static async Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)))
            {
                return await task;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        public struct WordBoundary { public int Start; public int Length; }

        public static List<WordBoundary> GetEveryOtherWord(string str)
        {
            List<WordBoundary> result = new List<WordBoundary>();

            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == ' ')
                {
                    int nextSpace = str.IndexOf(' ', i + 1);
                    int limit = nextSpace == -1 ? str.Length : nextSpace;

                    WordBoundary wb = new WordBoundary();
                    wb.Start = i + 1;
                    wb.Length = limit - i - 1;
                    result.Add(wb);
                    i = limit;
                }
            }
            return result;
        }
    }
}
