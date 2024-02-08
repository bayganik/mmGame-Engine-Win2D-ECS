## Feb 08, 2024

The engine and a sample scenes with sprites, animated sprites, compound sprites + simple card game, 

https://github.com/rocwood/Entitas-Lite        (I've included Entitas here, since I made some minor additions)

Entity Component System (ECS) is used to allow for separation of concern when coding (Entitas) 

Scene is the base of the game.  Inside the Scene you add Entities that have Components.  Then you add a System to act on those entities.  If you don't add a system, you all you get is a scene displaying a bunch of things (like the Card Scene).  

There are components that have special meaning.  

    Each game is a scene holding Entities.
        
        * Game Entity (All the pieces used to do the game)
        
        * Scene Entity (typically UI elements that are drawn on top of all Game Entities)
        
    Transform component gets added to all entities when scene creates them.
    
    Sprite component is used to display images
    
    SpriteAnimation component adds an animated sprite using spritesheet
    
    Tiled map component adds a TmxMap that allows you to access all of its levels & objects
    
    Text compoenent addes a text and will follow the entity on the screen 
    
    BoxCollider component allows the Entity to collide with other entities that have a collider
    
Systems do the guts of the logic of the game.  They are executed once every frame and process all entities that match certain components (that we give them).

Below examples have "Debug" flag on.  F9 will flip "Debug" off/on.  The tank will move using arrow keys.  The red boxes are BoxColliders drawn as debug guide.

## To Start a VS2022 project:

    Start with Net 7.0 console app
    
    Using Dependencies add Raylib-cs Nuget package
    
    Add references to Entitas-Lite , mmGameEngine and Sanford.MIDI
 
## Your App.cs

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


## Game Scene

![game image](GameScene.jpg)









## Card Game Scene

![game image](CardScene.jpg)




