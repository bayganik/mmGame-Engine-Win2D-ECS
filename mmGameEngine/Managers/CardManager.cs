using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;
using Entitas;

namespace mmGameEngine
{
    public class CardDeckManager
    {
        public CanvasBitmap CardDeckTexture;
        public bool EndOfGame = false;                  //game stops
        public int Score = 0;                           //game score
        public Scene ActiveScene;                       //scene
        public int CardWidth = 72;                      //width of each card
        public int CardHeight = 100;                    //height of top showing (when fanned out)
        public int JockerLocation = 64;                 //jocker in the spritesheet
        public int XCard = 52;                          //X marks the card
        public int OCard = 53;                          //O marks the card
        public int CardBackBegin = 54;                  //card backs starting point (last row of cards)
        public bool CardsHaveCollider = true;           //each card dealt has a box collider

        //object syncRoot = new System.Object();       //object for locking
        //CardDeckManager _Instance;

        //
        // One deck of cards
        //
        // deckTotal gives the bounds of cardsInDeck
        // currentDeckNumber is zero based, so 0 is deck one
        // currentCardNumber is zero based, so 0 is 2 of hearts (in our image)
        //
        //private List<StacksItems> cardStacks;         // card stack definitions
        CardComponent[] cardDeck;                         // card objects
        Rect[] SourceFrames;

        int[] cardDeckPointer;                   // shuffeled card numbers for cardDeck
        int deckTotal = 1;                       // total number of decks
        Sprite _deckLocation;                    // object in the game that is location of deck of cards

        int currentDeckNumber;                   // current deck of cards (if only one then value 0)
        int currentCardNumber = 0;               // current card Number  in the deck 
        int currentCardBack = 6;                 // current back of a card
        float deckFanOut = 0.03f;                // value added to fan out the deck 
        //
        // All card images
        //
        CanvasBitmap[] cardFaces;
        CanvasBitmap[] cardBacks;
        CanvasBitmap JockerCard;
        CanvasBitmap EmptyCardHolder;

        //CanvasAnimatedControl cRenderer;
        public CardDeckManager(CanvasBitmap cardSheet, CanvasBitmap emptyCard)
        {
            Score = 0;
            EndOfGame = false;
            EmptyCardHolder = emptyCard;

            InitAllCards(cardSheet, 72, 100);
        }
        /// <summary>
        /// Initialize cards/images (in case the card spritesheet is different)
        /// </summary>
        /// <param name="_cardImage"></param>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public void InitAllCards(CanvasBitmap cardSheet, int _width = 72, int _height = 100)
        {

            CardWidth = _width;
            CardHeight = _height;
            cardFaces = new CanvasBitmap[52];             //card face images
            cardBacks = new CanvasBitmap[10];             //card back images

            int cols = 13;
            int rows = 5;
            CanvasBitmap[] cards = new CanvasBitmap[cols * rows];
            //
            // Find source Rects for each card
            //
            int ind = 0;

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    byte[] oneFrameImage = cardSheet.GetPixelBytes(x * CardWidth, y * CardHeight, CardWidth, CardHeight);
                    cards[ind] = CanvasBitmap.CreateFromBytes(Global.SwapChain, oneFrameImage, CardWidth, CardHeight, 
                        Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized);
                    ind++;

                }
            }

            //CanvasBitmap cardSheet = Raylib.LoadImage("AssetsEngine/Cards/CardDeck_72x100.png");
            CanvasBitmap temp;
            //
            // card image has 5 rows of 13 cards
            //
            for (int i = 0; i < 52; i++)            //first 52 rectanges are face cards
            {
                cardFaces[i] = cards[i];
            }
            //
            // back of cards 54-63
            //
            int cardBackIndx = 0;
            for (int i = 54; i < 63; i++)
            {
                cardBacks[cardBackIndx] = cards[i];
                cardBackIndx++;
            }
            //
            // Joker is card 64
            //
            JockerCard = cards[64];

            currentCardBack = 6;
            currentCardNumber = 0;
            currentDeckNumber = 0;
            cards = null;
            //temp = new Image();

        }
        /// <summary>
        /// Re-Initialize an entire deck of cards & get ready for a new game
        /// </summary>
        /// <param name="_shuffle"></param>
        public void CreateDeckOfCards(bool _shuffle = true)
        {

            cardDeckPointer = new int[52];
            cardDeck = new CardComponent[52];
            Score = 0;
            //
            // Create 52 CardComponents
            //
            for (int i = 0; i < 52; i++)
            {
                CardComponent card = new CardComponent();
                //
                // get face and back images
                //
                card.CardFace = cardFaces[i];
                card.CardBack = cardBacks[currentCardBack];

                card.Index = i;
                card.IsFaceUp = true;
                //
                // 0 heart, 1 dimond, 2 clubs, 3 spade
                //
                string tempSuit = "";
                switch (i)
                {
                    case int n when (n <= 12):
                        card.IsRed = true;                //hearts
                        card.Suit = 0;
                        tempSuit = "_Heart";
                        break;
                    case int n when (n >= 13 && n <= 25):
                        card.IsRed = true;                //diamond
                        card.Suit = 1;
                        tempSuit = "_Diamond";
                        break;
                    case int n when (n >= 26 && n <= 38):
                        card.IsRed = false;                //clubs
                        card.Suit = 2;
                        tempSuit = "_Club";
                        break;
                    case int n when (n >= 39):
                        card.IsRed = false;                //spades
                        card.Suit = 3;
                        tempSuit = "_Spade";
                        break;
                }
                //
                // This ranking depends on the card image that holds the face of the cards
                // 0 two, 1 three, 2 four, 3 five,... 8 ten, 9 jack, 10 queen, 11 king, 12 Ace
                //
                //
                card.FaceImage = i % 13;
                //
                // If this is a blackjack game the jack,queen,king = 10 points
                // all number cards are their values, except Ace to be 1 or 11
                //

                if (card.FaceImage <= 8)
                {
                    card.Rank = card.FaceImage + 2;
                    card.FaceImage = card.FaceImage + 2;
                }
                else if (card.FaceImage > 8 && card.FaceImage < 12)   //face cards
                {
                    card.Rank = 10;
                    card.FaceImage = card.FaceImage + 2;
                }
                else
                {
                    card.FaceImage = 1;              //Ace
                    card.Rank = 1;                   //Ace is one
                    card.RankExtra = 11;             //Ace is also 11
                }
                //
                // Card name will be "Cxx_Club" or "Cxx_Heart"
                //
                card.CName = "C" + card.FaceImage.ToString("00") + tempSuit;

                cardDeckPointer[i] = i;
                cardDeck[i] = card;
            }

            currentCardNumber = 0;
            if (_shuffle)
                Shuffle();
            else
                NoShuffle();
        }
        public void NoShuffle()
        {
            //
            // cardDeckPoint is in order 
            //
            int count = 51;
            for (int j = count; j > 1; j--)
            {
                int temp = cardDeckPointer[j];
                int Number = cardDeckPointer[j];

                cardDeckPointer[j] = cardDeckPointer[Number];
                cardDeckPointer[Number] = temp;
            }

            currentCardNumber = 0;
        }
        public void Shuffle()
        {
            Random rnd = new Random();
            //
            // cardDeckPoint is shuffled and first card number = 0
            //
            int count = 51;
            for (int j = count; j > 1; j--)
            {
                int temp = cardDeckPointer[j];
                int Number = rnd.Next(j + 1);           //next number less that max given

                cardDeckPointer[j] = cardDeckPointer[Number];
                cardDeckPointer[Number] = temp;
            }

            currentCardNumber = 0;
        }
        /// <summary>
        /// Get the next card in the deck (-1 if end of deck)
        /// </summary>
        /// <returns></returns>
        public int GetACard()
        {
            if (currentCardNumber > 51)
                return -1;

            //
            // 5/16/2017 there is only ONE deck of cards
            //
            int cardPTR = cardDeckPointer[currentCardNumber];

            if (currentCardNumber > 51)
                cardPTR = -1;

            currentCardNumber += 1;
            return cardPTR;
        }
        /// <summary>
        /// This is how you get a card entity (image)
        /// </summary>
        /// <param name="_faceup"></param>
        /// <returns></returns>
        public Entity DealACard(bool _enabled = false, bool _faceup = true)
        {
            //
            // There are no colliders on cards, pile of Cards have colliders
            //
            int cardnum = GetACard();
            if (cardnum < 0)
                return null;

            CardComponent cardComp = cardDeck[cardnum];
            cardComp.IsFaceUp = _faceup;
            //
            // Create a card entity with proper components
            //
            Entity _cardEnt = Global.CreateGameEntity();
            _cardEnt.Name = cardComp.CName;
            _cardEnt.Get<TransformComponent>().Enabled = _enabled;
            _cardEnt.Add(cardComp);
            if (CardsHaveCollider)
            {
                BoxCollider bx = new BoxCollider(CardWidth, CardHeight);
                _cardEnt.Add(bx);
            }

            _cardEnt.Tag = cardComp.Index * -1;                          //tag to identify this entity as a card
            return _cardEnt;
        }
        public Entity DealEmptyHolder()
        {
            CardComponent cardComp = new CardComponent();
            cardComp.CName = "EmptyHolder";
            cardComp.IsFaceUp = true;
            cardComp.RenderLayer = -1000;
            //
            // Create a Jockere entity with proper components
            //
            Entity _cardEnt = Global.CreateGameEntity();
            _cardEnt.Name = cardComp.CName;
            _cardEnt.Get<TransformComponent>().Enabled = true;
            cardComp.CardFace = EmptyCardHolder;
            cardComp.CardBack = cardBacks[currentCardBack];
            //
            // Empty card holders always have a collider
            //
            BoxCollider bx = new BoxCollider(CardWidth, CardHeight);
            bx.RenderLayer = 1;
            _cardEnt.Add(bx);

            _cardEnt.Add(cardComp);
            _cardEnt.Tag = -1;                          //tag to identify this entity as a card
            return _cardEnt;
        }
        public CanvasBitmap GetEmptyHolderTexture()
        {
            return EmptyCardHolder;
        }
        public Entity DealAJoker(bool _enabled = false, bool _faceup = true)
        {
            CardComponent cardComp = new CardComponent();
            cardComp.CName = "joker";
            cardComp.IsFaceUp = _faceup;
            //
            // Create a Jockere entity with proper components
            //
            Entity _cardEnt = Global.CreateGameEntity();
            _cardEnt.Name = cardComp.CName;
            _cardEnt.Get<TransformComponent>().Enabled = _enabled;
            cardComp.CardFace = JockerCard;
            cardComp.CardBack = cardBacks[currentCardBack];
            if (CardsHaveCollider)
            {
                BoxCollider bx = new BoxCollider(CardWidth, CardHeight);
                _cardEnt.Add(bx);
            }

            _cardEnt.Add(cardComp);
            _cardEnt.Tag = -1;                          //tag to identify this entity as a card
            return _cardEnt;
        }
        public CardComponent GetCardComponent(int cardPTR)
        {
            //
            // The CardComponent of the card is returned
            //
            CardComponent cardObj = cardDeck[cardPTR];
            return cardObj;
        }
        //public Sprite GetCardFace(int cardPTR)
        //{
        //    //
        //    // The face image of the card is returned
        //    //
        //    Sprite cardObj = cardFaces[cardPTR];
        //    return cardObj;
        //}
        //public Sprite GetCardBack()
        //{
        //    //
        //    // The back image of the card is returned
        //    //
        //    Sprite cardObj = cardBacks[currentCardBack];
        //    return cardObj;
        //}
    }
}
