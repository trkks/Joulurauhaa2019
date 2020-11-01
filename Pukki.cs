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

        public const float Radius = 70;
        public const float BottleOffset = 110;
        public const float HangingDistance = Radius * 1.2f;
        public const int SpriteWidth = 900/(225/(int)Radius);
        public const int SpriteHeight = 550/(225/(int)Radius);
        public const int MaxHangingElves = 16;
        public readonly SpriteAnimater Animater;
        public float Speed = 300;
        public bool GameOver;
        public CircleFacer Body;
        public CircleFacer Bottle;
        public int HangingElves = 0;

        private readonly Texture2D deathFrame;
        public readonly Vector2 pivot;
        private readonly int[] delays;
        private const float slowDownMultiplier = 0.90f;

        public Pukki(Vector2 position, Texture2D[] frames, Texture2D deathFrame)
        {
            this.deathFrame = deathFrame;
            pivot = new Vector2( 440, 260 );
            delays = new int[]{ 0,2,6,6 };

            Body = new CircleFacer(Radius, position, Vector2.Zero, true);
            Bottle = new CircleFacer(
                Radius*1.5f,
                position + BottleOffset*Vector2.UnitX,
                Vector2.Zero,
                false);
            Animater = new SpriteAnimater(frames.Length);
            
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
            Animater.actionFrame = 2;
            Animater.action = () => { Bottle.IsHitBoxActive = true; };
            Animater.postAnimationAction = () => { Animater.Reset(); Bottle.IsHitBoxActive = false; };
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
            Speed *= slowDownMultiplier;
            if (HangingElves >= MaxHangingElves)
                Die();
        }
    }
}
