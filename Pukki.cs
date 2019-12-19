using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pipo 
{
    class Pukki
    {
        const double SWING_TIME = 14;
        const string tag = "pukki";
        const float speed = 100;
        public const float radius = 70;

        public CircleFacer facer;
        public SpriteAnimater animater;
        private System.Timers.Timer swingTimer;

        public Pukki(Vector2 pos, Texture2D[] frames)
        {
            facer = new CircleFacer(tag, pos, Vector2.Zero, radius, true);
            //animater = new SpriteAnimater(frames, new int[]{ 0, 2,5,5,2 }, radius);
            animater = new SpriteAnimater(frames, new int[]{ 0, 2,5,5,2 }, new Vector2(98,122));
           // InitSwingtimer();
        }

        public void ApplySpeed()
        {
            facer.velocity *= speed;
        }

        //private void InitSwingtimer()
        //{
        //    swingTimer = new System.Timers.Timer(SWING_TIME/60*1000);
        //    // Hook up the Elapsed event for the timer. 
        //    swingTimer.Elapsed += (s, e) =>
        //    {
        //        if (animater.isAnimating) = false;
        //            
        //    };
        //    swingTimer.AutoReset = true;
        //    swingTimer.Enabled = false;
        //}
        
        public void Swing()
        {
            if (animater.isAnimating)
                return;

            animater.InitializeAnimation();
            animater.PlayOnce();
            //swingTimer.Start();
        }
    }
}
