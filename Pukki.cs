using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pipo 
{
    class Pukki
    {
        const int SWING_TIME = 14;
        const string tag = "pukki";
        const float speed = 100;
        public const float radius = 70;

        public CircleFacer facer;
        public SpriteAnimater animater;
        public int swingTimeStart;

        public Pukki(Vector2 pos, float radius, Texture2D[] frames)
        {
            facer = new CircleFacer(tag, pos, Vector2.Zero, radius, true);
            animater = new SpriteAnimater(frames, new int[]{ 0, 2,5,5,2 }, radius);
            swingTimeStart = int.MaxValue-SWING_TIME;
        }

        public void ApplySpeed()
        {
            facer.velocity *= speed;
        }
        
        public void Swing(int swingInputTime)
        {
            if (swingTimeStart + SWING_TIME < swingInputTime)
            {
                animater.isAnimating = false;
                return;
            }
            swingTimeStart = swingInputTime;
            animater.InitializeAnimation();
            animater.isAnimating = true;
        }
        
    }
}
