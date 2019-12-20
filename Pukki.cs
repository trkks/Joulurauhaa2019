using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pipo;

namespace Joulurauhaa2019 
{
    class Pukki
    {
        public enum Collision
        {
            Body,
            Bottle,
            None
        }

        //const double SWING_TIME = 14;
        //const string tag = "pukki";
        public const float Radius = 70;
        public const float Speed = 100;
        public const float BottleOffset = 85;
        public readonly SpriteAnimater Animater;
        public bool GameOver;
        public CircleFacer Body;
        public CircleFacer Bottle;
        public int HangingElves = 0;

        private readonly Texture2D deathFrame;
        private readonly Vector2 pivot;
        private readonly int[] delays;

        public Pukki(Vector2 position, Texture2D[] frames, Texture2D deathFrame)
        {
            this.deathFrame = deathFrame;
            pivot = new Vector2( 135, 125 );
            delays = new int[]{ 0,2,6,4 };

            Body = new CircleFacer(Radius, position, Vector2.Zero, true);
            Bottle = new CircleFacer(
                0.75f*Radius, 
                position + BottleOffset*Vector2.UnitX,
                Vector2.Zero,
                false);
            Animater = new SpriteAnimater(frames.Length, pivot);
            
            Animater.SetFrames(frames, delays);
        }

        public void ApplySpeed()
        {
            Body.Velocity *= Speed;
        }

        public void Move(float deltaTime)
        {
            Body.Move(deltaTime);
            Bottle.Position = Body.Position + BottleOffset * Vector2.Normalize(Body.Facing);
        }

        public void Swing()
        {
            if (Animater.IsAnimating)
                return;

            Animater.Start(false);
            Animater.actionFrame = 3;
            Animater.action = () => { Bottle.IsHitBoxActive = true; };
            Animater.postAction = () => { Bottle.IsHitBoxActive = false; };
        }

        public Collision CollidingWith(CircleFacer target)
        {
            if (Body.CollidingWith(target))
                return Collision.Body;
            if (Animater.IsAnimating && Bottle.CollidingWith(target)) //TODO activate hitbox on correct animation frame
                return Collision.Bottle;

            return Collision.None;
        }

        public void Die()
        {
            GameOver = true;
            Animater.SetDefaultSprite(deathFrame);
        }

        public void AddElf()
        {
            HangingElves++;
            if (HangingElves>= 8)
                Die();
        }
    }
}
