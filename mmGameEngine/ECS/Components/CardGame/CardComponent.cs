using System;
using System.Collections.Generic;
using System.Text;
using Entitas;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
namespace mmGameEngine
{
    public class CardComponent : RenderComponent
    {
        public CardPileComponent HoldingPile;                 //Card pile component holding this card
        public string CName = "Card";
        public Rect SourceRect;
        public Rect DestRect;
        //
        // Individual card in a deck
        //
        // The ranking depends on the card image that holds the faces of the cards
        // in our case of the card images wed have, it starts with cardindex = 0 -> 2 of hearts
        //
        public CanvasBitmap Texture;
        public CanvasBitmap CardFace;
        public CanvasBitmap CardBack;

        public int Index;                   // 0 - 51 e.g. cardfaces[faceIndex];
        public int FaceImage = 0;           // 0 two,.. 8 ten, 9 jack, 10 queen, 11 king, 12 Ace
        public int Suit = 0;                // 0 heart, 1 dimond, 2 clubs, 3 spade
        public bool IsFaceUp = true;       // card face showing?
        public bool IsRed = true;           // could be used in game like Solitair
        public int Rank = 2;               // 2 two, 10 ten, 10 jack, 10 queen, 10 king, 11 Ace 
        public int RankExtra = 0;          // 1 Ace can also be ONE in blackjack

        public int CardStack = 0;               // Stack of cards this belongs (pointer)
        //
        // entity moving on its own
        //
        public bool IsMoving;

        public CardComponent()
        {

        }
        public override void Update()
        {
            base.Update();
            if (OwnerEntity == null)
                return;
            if (!Enabled)
                return;
        }
        public override void Render()
        {
            if (OwnerEntity == null)
                return;
            if (!OwnerEntity.IsVisible)
                return;

            if (!Enabled)
                return;
            //
            // Card face or back
            //
            Vector2 position = OwnerEntity.Get<TransformComponent>().Position;
            if (IsFaceUp)
                Texture = CardFace;
            else
                Texture = CardBack;

                //Raylib.DrawTexture(CardBack, (int)position.X, (int)position.Y, Color.WHITE);

            var Origin = new Vector2((float)Texture.Size.Width * 0.5f, (float)Texture.Size.Height * 0.5f);

            SourceRect = new Rect(0, 0, Texture.Size.Width, Texture.Size.Height);
            DestRect = new Rect(Transform.Position.X, Transform.Position.Y,
                         Texture.Size.Width * Transform.Scale.X,
                         Texture.Size.Height * Transform.Scale.Y);


            Global.SpriteBatchDraw.DrawFromSpriteSheet(Texture,
                Transform.Position,
                SourceRect,
                Vector4.One,
                Origin,
                Transform.Rotation,
                Transform.Scale,
                Transform.Flip);
            //
            // Entity.Transform.Position + LocalOffset (Vector2) consider for minor adjustments
            //
            //Rectangle sRect = Texture.Bounds;
            //var Origin = new Vector2((float)sRect.Width * 0.5f, (float)sRect.Height * 0.5f);

            //DrawImage(Texture,
            //    new Rectangle(Entity.Position.X, Entity.Position.Y, sRect.Width, sRect.Height), sRect);
        }
        //
        // Now we use the session not spritebatch
        //
        //public void Render(CanvasSpriteBatch sb)
        //{
        //    CanvasBitmap Texture = CardFace;

        //    //
        //    // Card face or back
        //    //
        //    if (IsFaceUp && CardFace != null)
        //        Texture = CardFace;
        //    if (!IsFaceUp && CardBack != null)
        //        Texture = CardBack;
        //    //
        //    // Entity.Transform.Position + LocalOffset (Vector2) consider for minor adjustments
        //    //
        //    Rect sRect = Texture.Bounds;
        //    var Origin = new Vector2((float)sRect.Width * 0.5f, (float)sRect.Height * 0.5f);

        //    sb.DrawFromSpriteSheet(Texture, Entity.Transform.Position, sRect,
        //                           Vector4.One, Origin, Entity.Transform.Rotation, 
        //                           Entity.Transform.Scale, Entity.Flip);
        //}
    }
}
