using Microsoft.Xna.Framework.Graphics;

namespace Pipo 
{
    /*
    * index 0 is the default sprite
    */
    class SpriteAnimater
    {
        //readonly int id;
        public readonly Texture2D[] frames;
        public readonly int frameCount;
        public int currentFrame;
        public bool isAnimating;
        //float rotation;

        public SpriteAnimater(Texture2D[] frames0)
        {
            frames = frames0;
            frameCount = frames.Length;
            currentFrame = 0;
        }
        
        public Texture2D ToggleAnimation()
        {
            isAnimating = !isAnimating;
            currentFrame = 0;
            return frames[currentFrame];
        }

        public Texture2D getNext()
        {
            if (isAnimating)
            {
                currentFrame = (currentFrame + 1) % frameCount;
            }
            return frames[currentFrame];
        }
    }
}
