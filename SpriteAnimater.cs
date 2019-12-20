using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pipo 
{
    /*
    * For rendering and animating square sprites :)
    * Frames[0]    == default sprite
    * Frames[1..n] == animation frames
    */
    //TODO Change name to EventedSpriter or EventfulAnimator for clarity ":D"
    class SpriteAnimater
    {
        //BAD BAD BAD
        public Action action;
        public Action postAction;
        public int actionFrame;

        //readonly int id;
        public bool IsAnimating;
        public readonly Vector2 Pivot;
        private Texture2D defaultSprite;
        public Texture2D[] Frames;

        private bool isLooping;
        private int frameCount;
        private int currentFrame;
        private int passedDelay;
        private int[] delays;

        public SpriteAnimater(int frameCount, Vector2 pivot)
        {
            this.frameCount = frameCount;
            this.Pivot = pivot;
            isLooping = false;
            IsAnimating = false;
            currentFrame = 0;
            passedDelay = 0;
            actionFrame = -1;
        }

        public void SetFrames(Texture2D[] frames, int[] delays)
        {
            this.Frames = frames;
            this.delays = delays;
            defaultSprite = Frames[0];
        }

        public void Start(bool looping)
        {
            IsAnimating = true;
            this.isLooping = looping;
            //currentDelay = 0;
        }
        
        public void Reset()
        {
            IsAnimating = false;
            isLooping = false;
            currentFrame = 0;
            passedDelay = 0;
        }

        public void SetDefaultSprite(Texture2D sprite)
        {
            defaultSprite = sprite;
        }

        public Texture2D GetCurrentFrame()
        {
            if (IsAnimating)
            {
                passedDelay++;
                if (passedDelay >= delays[currentFrame])
                {
                    currentFrame++;
                    currentFrame %= frameCount;
                    if (!isLooping)
                    {
                        if (actionFrame < -1)
                        {
                            Reset();
                            postAction();
                            actionFrame = -1;
                        }
                    }
                    passedDelay = 0;
                }
                if (currentFrame == actionFrame)
                {
                    action();
                    actionFrame = -2;
                }
                return Frames[currentFrame];
            }
            return defaultSprite;
        }
    }
}
