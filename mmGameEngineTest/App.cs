using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using mmGameEngine;

namespace mmGameEngineTest
{
    class App
    {
        //----------------
        // Step 01
        //----------------
        // Main entry into the game
        public static void Main(string[] args)
        {
            //
            //     Gets the main CoreApplicationView(IFrameworkViewSource)
            //     instance for all running apps that use this
            //     CoreApplication instance.
            //
            IFrameworkViewSource ivs = new ViewSource();
            //
            // Tell CoreApplication to run "ivs" using mmGameApp.cs (Step 03) which is IFrameworkView
            //
            CoreApplication.Run(ivs);


        }
    }
    class ViewSource : IFrameworkViewSource
    {
        //----------------
        // Step 02
        //----------------
        public IFrameworkView CreateView()
        {
            Global.CurrentScene = new GameScene();
            //Global.CurrentScene = new CardScene();
            return Global.CurrentScene;
        }
    }
}
