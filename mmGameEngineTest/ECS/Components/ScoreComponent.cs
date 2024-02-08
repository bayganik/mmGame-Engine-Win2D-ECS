using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Entitas;
using mmGameEngine;
using Transform = mmGameEngine.TransformComponent;


namespace mmGameEngineTest
{
    public class ScoreComponent : Component
    {
        public string ScoreWords = "Score : ";
        public int Score = 0;
        public ScoreComponent()
        {
            Score = 0;
        }
    }

}
