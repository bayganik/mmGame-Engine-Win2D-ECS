
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Microsoft.Graphics.Canvas;

using System.Numerics;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI;
using System.Diagnostics;
using System.Drawing;

namespace mmGameEngine
{
    //
    // IFrameworkView
    //   applicationView:
    //     The default view provided by the app object. You can use this instance in your
    //     implementation to obtain the CoreWindow created by the app object, and register
    //     callbacks for the all the event (key clicks, mouse move, etc)
    //
    // When you run without XAML, your swapchain is the only thing rendering graphics,
    // if screen is resized, we still keep the swapchain location the same, so mouse clicks
    // will register in the correct locations.
    //
    public class mmGameApp : IFrameworkView
    {
        internal static mmGameApp _instance;                                   //instance of game controller
        Scene _scene;
        Scene _nextScene;

        CanvasRenderTarget FrontBuffer { get { return accumulationBuffers[currentBuffer]; } }
        CanvasRenderTarget BackBuffer { get { return accumulationBuffers[(currentBuffer + 1) % 2]; } }
        CanvasRenderTarget[] accumulationBuffers = new CanvasRenderTarget[2];

        CanvasDevice canvasdevice;
        SwapChainManager swapChainManager;
        CoreWindow GameWindow;
        bool ExitProgram;
        int currentBuffer = 0;

        //----------------
        // Step 03
        //----------------
        //  This function is called when the mmGameApp class is first created. It gives us a chance to prepare our
        //  application before it really gets going. The parameter, CoreApplicationView, is an instance
        //  of a class that gives us access to some low-level application properties and notifications.
        //
        public void Initialize(CoreApplicationView applicationView)
        {
            //
            // Particular Scene has been initialized, and may have set the screen size
            //
            ApplicationView.PreferredLaunchViewSize = Global.ScreenSize;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            _instance = this;
            _scene = new Scene();
            _nextScene = null;
            CurrentScene = Global.CurrentScene;


            applicationView.Activated += applicationView_Activated;
            CoreApplication.Suspending += CoreApplication_Suspending;

            Global.HalfHeight   = (float)Global.ScreenSize.Height / 2;
            Global.HalfWidth    = (float)Global.ScreenSize.Width / 2;
            Global.WindowCenter = new Vector2(Global.HalfWidth, Global.HalfHeight);
            Input.KeyValue = "";
        }
        //----------------
        // Step 04
        //----------------
        // This function is called right after Initialize(), and it gives us the opportunity to set up window
        // notifications, such as keystrokes, mouse movements, and touchscreen interactions.
        //
        public void SetWindow(CoreWindow _window)
        {
            //
            // After initialize, we come here
            //
            GameWindow = _window;
            //
            // Event call backs
            //
            GameWindow.KeyDown          += Window_KeyDown;
            GameWindow.KeyUp            += Window_KeyReleased;              
            GameWindow.PointerPressed   += Window_PointerPressed;           //mouse click
            GameWindow.PointerMoved     += Window_PointerMoved;             //mouse move
            GameWindow.PointerReleased  += Window_PointerReleased;          //mouse release
            GameWindow.ResizeCompleted  += Window_Resize;

            canvasdevice = new CanvasDevice();
            swapChainManager = new SwapChainManager(GameWindow, canvasdevice);
            swapChainManager.EnsureMatchesWindow(GameWindow);
            Global.CanvasDeviceInUse = canvasdevice;
            Global.SwapChain = swapChainManager.SwapChain;

        }
        //----------------
        // Step 05
        //----------------
        // We can do things like load graphics and sound effects, and allocate memory.
        //
        public void Load(string entryPoint)
        {
            if (CurrentScene == null)
                return;

            CurrentScene.Begin();
            //
            // Load your resources here before "Run"
            // your scene will have "public override async Task Process()" method
            //
            CurrentScene.Process().AsAsyncAction();
        }
        //----------------
        // Step 06
        //----------------
        private void applicationView_Activated(CoreApplicationView sender, IActivatedEventArgs args)
        {

            CoreWindow.GetForCurrentThread().Activate();
        }
        //----------------
        // Step 07
        //----------------
        // Game loop is here
        //
        public void Run()
        {
            double FirstFrameTime = 0;
            double SecondFrameTime = 0;
            //double Counter = 0;
            var applicationView = ApplicationView.GetForCurrentView();
            applicationView.Title = "Game Window";

            ExitProgram = false;
            if(CurrentScene == null) ExitProgram = true;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            while (!ExitProgram)
            {
                CoreWindow.GetForCurrentThread().Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessAllIfPresent);
                //------------------------
                //       Update Scene 
                //------------------------
                Update();
                _scene.RemoveDeletedEntities();         // remove deleted/destroyed entities
                //------------------------
                //       Render Scene 
                //------------------------
                Render();

                TimeSpan ts = stopWatch.Elapsed;
                FirstFrameTime = ts.TotalMilliseconds;

                Global.DeltaTime = (FirstFrameTime - SecondFrameTime) / 1000;       //0.0166 is fps 60
                SecondFrameTime = FirstFrameTime;
                double fps = 1 / Global.DeltaTime;
            }
        }
        private void Update()
        {
            CurrentScene.Update();

        }
        private void Render()
        {
            //
            // Make the swapchain same size as entire window
            // Update the accumulation buffer
            //
            SwapAccumulationBuffers();
            EnsureCurrentBufferMatchesWindow();

            CurrentScene.Render();
        }

        void SwapAccumulationBuffers()
        {
            currentBuffer = (currentBuffer + 1) % 2;
        }
        void EnsureCurrentBufferMatchesWindow()
        {
            var bounds = GameWindow.Bounds;
            Windows.Foundation.Size windowSize = new Windows.Foundation.Size(bounds.Width, bounds.Height);
            float dpi = DisplayInformation.GetForCurrentView().LogicalDpi;

            var buffer = accumulationBuffers[currentBuffer];

            if (buffer == null || !(SwapChainManager.SizeEqualsWithTolerance(buffer.Size, windowSize)) || buffer.Dpi != dpi)
            {
                if (buffer != null)
                    buffer.Dispose();

                buffer = new CanvasRenderTarget(canvasdevice, (float)windowSize.Width, (float)windowSize.Height, dpi);
                accumulationBuffers[currentBuffer] = buffer;
            }
        }
        //----------------
        // Step 08
        //----------------
        private void CoreApplication_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {

        }
        //----------------
        // Step Last
        //----------------
        // This is where app exists
        //
        public void Uninitialize()
        {
            //Game Exits here
        }
        public static Scene CurrentScene
        {
            get => _instance._scene;
            set
            {
                _instance._scene = value;
                Global.CurrentScene = value;
            }
        }
        private void Window_Resize(CoreWindow sender, object e)
        {
            //
            // Make sure swap chain maintains it location & display
            // elements hold their position on screen (they don't get elongated)
            //
            swapChainManager.EnsureMatchesWindow(GameWindow);
        }
        private void Window_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            //
            // check to see if previous key press was processed yet?
            //
            if (Input.KeyPressed) 
                return;
            
            args.Handled = true;    //Flag that keypress has been handled (to stop it repeating forever)
            //---------------------------------
            // Escape key exists the game
            //---------------------------------
            if (args.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                ExitProgram = true;
                return;
            }

            Input.KeyReleased = false;
            Input.KeyValue = args.VirtualKey.ToString();
            Input.KeyPressed = true;
            if (Input.KeyValue == "F11")
            {
                Global.DebugRenderEnabled = !Global.DebugRenderEnabled;
                return;
            }
        }
        private void Window_KeyReleased(CoreWindow sender, KeyEventArgs args)
        {
            args.Handled = true;    //Flag that keypress has been handled (to stop it repeating forever)
            Input.KeyReleased = true;
            Input.KeyValue = args.VirtualKey.ToString(); 
            Input.KeyPressed = false;

        }
        void Window_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            Input.MousePosition = args.CurrentPoint.Position.ToVector2();
            Input.LeftMousePressed = args.CurrentPoint.Properties.IsLeftButtonPressed;
            Input.RightMousePressed = args.CurrentPoint.Properties.IsRightButtonPressed;
            Input.LeftMouseReleased = false;
            Input.RightMouseReleased = false;
        }

        void Window_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            Input.MousePosition = args.CurrentPoint.Position.ToVector2();
        }

        void Window_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            Input.MousePosition = args.CurrentPoint.Position.ToVector2();
            Input.LeftMousePressed = false;
            Input.RightMousePressed = false;
            Input.LeftMouseReleased = true;
            Input.RightMouseReleased = true;
        }
    }
}
