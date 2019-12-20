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
        }

        public void SetFrames(Texture2D[] frames, int[] delays)
        {
            this.Frames = frames;
            this.delays = delays;
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
            Frames[0] = sprite;
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
                    if (currentFrame == 0)
                        if (isLooping)
                            currentFrame = 1;
                        else
                        {
                            postAction();
                            Reset();
                        }
                    passedDelay = 0;
                }
                if (currentFrame == actionFrame)
                {
                    action();
                    actionFrame = -1;
                }
            }
            return Frames[currentFrame];
        }
    }
}
