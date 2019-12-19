using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pipo 
{
    /*
    * For rendering square sprites :)
    * index 0 is the default sprite
    */
    class SpriteAnimater
    {
        //readonly int id;
        public readonly Texture2D[] frames;
        public readonly int[] frameTimes;
        public readonly int frameCount;
        public int currentFrame;
        public int frameTimer;
        public bool isAnimating;
        public bool looping;
        public Vector2 bodyOffset;
        //float rotation;

        //frames0 must equal times in length
        public SpriteAnimater(Texture2D[] frames0, int[] times, float bodyRadius)
        {
            frames = frames0;
            if (times.Length != frames.Length)
                frameTimes = new int[frames.Length];
            else
                frameTimes = times;

            frameCount = frames.Length;
            bodyOffset = new Vector2(bodyRadius,bodyRadius);

            isAnimating = false;
            looping = true;
            currentFrame = 1;
            frameTimer = 0;
        }

        public SpriteAnimater(Texture2D[] frames0, int[] times, Vector2 bodyOffset0)
        {
            frames = frames0;
            if (times.Length != frames.Length)
                frameTimes = new int[frames.Length];
            else
                frameTimes = times;

            frameCount = frames.Length;
            bodyOffset = bodyOffset0;

            isAnimating = false;
            currentFrame = 1;
            frameTimer = 0;
        } 

        public void InitializeAnimation()
        {
            isAnimating = false;
            looping = true;
            currentFrame = 1;
            frameTimer = 0;
        }

        public void PlayOnce()
        {
            isAnimating = true;
            looping = false;
        }

        public Texture2D getFrame()
        {
            if (!isAnimating)
                return frames[0];

            if (currentFrame+1 == frameCount && !looping)
            {
                isAnimating = false;
                return frames[0];
            }

            frameTimer++;
            if (frameTimer >= frameTimes[currentFrame])
            {
                currentFrame = (currentFrame + 1) % frameCount;
                frameTimer = 0;
            }

            return frames[currentFrame];
        }

        public void SetDefaultSprite(Texture2D sprite)
        {
            frames[0] = sprite;
        }
    }
}
