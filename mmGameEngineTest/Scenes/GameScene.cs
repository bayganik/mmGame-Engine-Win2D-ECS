using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using mmGameEngine;
using Entitas;
using Microsoft.Graphics.Canvas;
using System.Reflection;
using System.Net.Sockets;
using Windows.UI.Xaml.Media;

namespace mmGameEngineTest
{
    public class GameScene : Scene
    {
        CanvasBitmap TankBase;
        CanvasBitmap TankTurret;
        CanvasBitmap Missile;
        CanvasBitmap CHair;
        CanvasBitmap ChrImg;
        CanvasBitmap Background;
        CanvasBitmap ExplosionImg;


        public Entity LabelEnt;
        public Entity ExplosionEnt;
        public Entity MissleEnt;
        public Entity ButtonEnt;
        private Entity turretEnt;


        Entity MsgEnt;
        public GameScene() 
        {
            Global.ScreenSize = new Windows.Foundation.Size(800, 650);
            Global.DebugRenderEnabled = true;
        }
        public override async Task Process()
        {
            //----------------------------------
            // Get images (Texture2D)
            //----------------------------------
            ExplosionImg = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Sprite.Explode);
            TankBase    = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Sprite.Tankbase);
            TankTurret  = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Sprite.Turret);
            Missile     = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Sprite.Missile);
            CHair       = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Sprite.Crosshair);
            ChrImg      = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Sprite.PlayerSprite);
            Background  = await CanvasBitmap.LoadAsync(Global.SwapChain, Content.Sprite.GameBackground);
            //--------------------------------
            // Cursor with bulls eye image
            //--------------------------------
            ImageCursor chc = new ImageCursor(CHair);

            //--------------------------------
            // Label
            //--------------------------------
            LabelEnt = Global.CreateSceneEntity(new Vector2(650, 0));

            var size = Global.SwapChain.Size;
            var message = string.Format("{0:00}x{1:00} @{2:00}dpi", size.Width, size.Height, Global.SwapChain.Dpi);
            LabelComponent sc = new LabelComponent(message, 200, 50);
            LabelEnt.Add(sc);


            AddBoyAnimation();
            ShowWelcomeMsgBox();
            AddUI();
            AddTank();
            //AddMovingBackground();
            //-------------------------------
            //  System to run every frame
            //-------------------------------
            AddSystem(new TurretMovementSystem());
            AddSystem(new TankMovementSystem());
            AddSystem(new BulletMoveSystem());
            AddSystem(new CursorMoveSystem());
            AddSystem(new ExplosionSystem());
        }
        private void AddMovingBackground()
        {
            Entity bgEnt = Global.CreateGameEntity("background", new Vector2(0, 0));
            ScrollingImage si = new ScrollingImage(Background);
            //si.RenderLayer = -1000000;
            bgEnt.Add(si);
        }
        private void AddBoyAnimation()
        {
            //--------------------------------
            // Boy with animation
            //--------------------------------
            Entity chr = Global.CreateGameEntity("char");
            chr.Get<TransformComponent>().Position = new Vector2(100, 400);
            chr.Get<TransformComponent>().Scale = new Vector2(1f, 1f);

            SpriteAnimation sprAnime = new SpriteAnimation(ChrImg, 64, 64);
            sprAnime.AddAnimation("clap", "13,14,15,16,17,18,19");
            sprAnime.AddAnimation("idle", "13,14,15,14,14");
            sprAnime.Play("clap", true);
            chr.Add(sprAnime);

            BoxCollider bx = new BoxCollider(64, 64);
            chr.Add(bx);
        }
        private void AddTank()
        {
            //
            // Tank base
            //
            Entity tankEnt = Global.CreateGameEntity(new Vector2(100, 600), 0.25f);
            tankEnt.Name = "tank";
            tankEnt.Tag = 1000000;            //need this for collision detection
            tankEnt.Add(new TankComponent());
            Sprite ts = new Sprite(TankBase);
            ts.RenderLayer = 0;
            //
            // add a collider
            //
            BoxCollider bx = new BoxCollider((float)TankBase.Size.Width * 0.25f, (float)TankBase.Size.Height * 0.25f);
            
            tankEnt.Add(bx);
            tankEnt.Add(ts);
            //tankEnt.Add(new EntityCapturedComponent());
            //--------------------------------------------------------------------
            // Add turret to tank (his initial position doesn't matter)
            //--------------------------------------------------------------------
            turretEnt = Global.CreateGameEntity("turret");
            turretEnt.Tag = 1000000;            //need this for collision detection
            turretEnt.Get<TransformComponent>().Rotation = 0;
            Sprite tts = new Sprite(TankTurret);
            tts.RenderLayer = 10;
            tts.Origin = new Vector2(145, 500);             //when you want a very specific origin point (not center)

            turretEnt.Add(tts);
            turretEnt.Add(new TurretComponent());
            turretEnt.Get<TransformComponent>().Parent = tankEnt.Get<TransformComponent>();
            //
            // Add a txt to tank
            //
            Entity textEnt = Global.CreateGameEntity(new Vector2(200, 200));
            TextComponent txt = new TextComponent("Stay with tank!");
            txt.SetNewFont("Algerian", 12);
            txt.TextColor = Windows.UI.Colors.Black;
            txt.RenderLayer = 15;
            textEnt.Add(txt);

            textEnt.Get<TransformComponent>().Parent = tankEnt.Get<TransformComponent>();

        }
        //
        // Firing a shot from tip of Turret
        //
        public void AddMissile()
        {
            //
            // Add missle on tip of turret
            //
            MissleEnt = Global.CreateGameEntity(new Vector2(0, 0));
            MissleEnt.Tag = 1000000;            //need this for collision detection
            MissleEnt.Name = "Missle";
            MissleEnt.IsVisible = false;

            Sprite tts = new Sprite(Missile);
            tts.RenderLayer = 0;
            tts.ScaleOverRide = new Vector2(.80f, .80f);

            MissleEnt.Add(tts);

            EntityMover projtileComp = new EntityMover();
            projtileComp.Speed = 200;
            projtileComp.Enabled = false;
            projtileComp.IsMoving = false;

            MissleEnt.Add(projtileComp);                  //auto mover


            //
            // add a collider
            //
            BoxCollider bx = new BoxCollider((float)Missile.Size.Width * 0.5f, (float)Missile.Size.Height * 0.5f);
            MissleEnt.Add(bx);
            //MissleEnt.IsVisible = true;                              //may not be visible
           

        }
        public void ExpoldeMissile(Vector2 _location)
        {

            ExplosionEnt = Global.CreateGameEntity("explosion");

            ExplosionEnt.Get<TransformComponent>().Position = _location;
            ExplosionEnt.Scale = new Vector2(.5f, .5f);

            SpriteAnimation rocketAnime = new SpriteAnimation(ExplosionImg, 128, 128);
            rocketAnime.AddAnimation("blowup", "all");
            rocketAnime.Play("blowup", false);
            ExplosionEnt.Add(rocketAnime);
            //
            // This allows us to check see if explosion is done animating
            //
            ExplosionEnt.Add(new ExplosionComponent());
        }
        public void FireShot(Vector2 moveTo)
        {
            AddMissile(); 
            //
            // starting point & rotation is the Turret
            //
            {
                EntityMover em = MissleEnt.Get<EntityMover>();
                em.Enabled = true;
                em.MoveStart = turretEnt.Get<TransformComponent>().Position;
                em.MoveEnd = moveTo;
                em.IsMoving = true;
            }
            MissleEnt.Get<TransformComponent>().Scale = new Vector2(1, 1);
            MissleEnt.Get<TransformComponent>().Rotation = turretEnt.Get<TransformComponent>().Rotation; ;
            MissleEnt.Get<TransformComponent>().Enabled = true;
            MissleEnt.IsVisible = true;
        }
        private void ShowWelcomeMsgBox()
        {
            MsgEnt = Global.CreateSceneEntity(new Vector2(10, 10));
            MsgBoxComponent msg = new MsgBoxComponent("Please press the Ok button", 200, 100);
            MsgEnt.Add(msg);

            msg.MsgButton.Click += MsgButton_Click;
        }

        private void MsgButton_Click(object obj)
        {
            MsgEnt.IsVisible = false;
        }

        private void AddUI()
        {
            //--------------------------------
            // Panel
            //--------------------------------
            Entity PanEnt = Global.CreateSceneEntity(new Vector2(300, 100));
            PanelComponent pan = new PanelComponent(300, 200);

            //--------------------------------
            // Button
            //--------------------------------
            ButtonEnt = Global.CreateSceneEntity(new Vector2(10, 10));
            ButtonComponent bt = new ButtonComponent("OK", 75, 50);
            bt.Click += Ok_Clicked;
            ButtonEnt.AddComponent(bt);
            //--------------------------------
            // Progressbar 
            //--------------------------------
            Entity pgbEnt = Global.CreateSceneEntity(new Vector2(10, 85));
            ProgressBarComponent bc = new ProgressBarComponent(60, 10, "Progressbar", 0, 100, 50);
            pgbEnt.AddComponent(bc);

            //
            // add them to Panel
            //
            pan.AddEntity(ButtonEnt);
            pan.AddEntity(pgbEnt);


            PanEnt.AddComponent(pan);
        }

        public void Ok_Clicked(object obj)
        {

        }
    }
}
