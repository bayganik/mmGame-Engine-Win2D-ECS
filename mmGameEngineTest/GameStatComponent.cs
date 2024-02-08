using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mmGameEngine;

namespace mmGameEngineTest
{
    public class GameStatComponent : Component
    {
        public string ScoreWords = "Score : ";
        public int Score = 0;
        public bool EndOfGame = false;
        public GameStatComponent()
        {

        }
    }
}
