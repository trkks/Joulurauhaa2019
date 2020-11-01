using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pipo;

namespace Joulurauhaa2019
{
    class Tonttu
    {
        public const float Radius = 32;
        public const float Speed = 200;
        public readonly SpriteAnimater Animater;
        public bool IsDead;
        public CircleFacer Facer;

        private readonly Texture2D deathFrame;
        public readonly Vector2 pivot;
        private readonly int[] delays; 

        private float lerp;

        /// <summary>
        /// velocity should normally be unit in length, as it is scaled with 
        /// </summary>
        public Tonttu(Vector2 position, Vector2 velocity, Texture2D[] frames, Texture2D deathFrame)
        {
            this.deathFrame = deathFrame;
            pivot = new Vector2( 32, 35 );
            delays = new int[]{ 7,2,7,2 };

            Facer = new CircleFacer(Radius, position, Speed*velocity, true);
            Animater = new SpriteAnimater(frames.Length);

            Animater.SetFrames(frames, delays);
            Animater.Start(true);

            IsDead = false;
        }

        /// <summary>
        /// "Fly *away* relative to target"
        /// TODO Move into CircleFacer, here replace with HandleBeingHit()
        /// </summary>
        public void SetTrajectoryFrom(Vector2 target)
        {
            Vector2 direction = Vector2.Normalize(Vector2.Subtract(Facer.Position, target));
            this.Facer.Velocity = Speed * direction;
        }

        public void ScaleVelocity(float factor)
        {
            this.Facer.Velocity = Vector2.Normalize(this.Facer.Velocity) * Speed * factor;
        }
            
        public void Die(Vector2 target)
        {
            SetTrajectoryFrom(target);
            ScaleVelocity(3);

            Animater.SetFrames(
                new Texture2D[] { deathFrame, deathFrame, deathFrame, deathFrame },
                new int[] { 0,0,0,0 } );
            IsDead = true;

            Animater.SetAction(
                () => { return; },
                () => { Facer.Velocity = Vector2.Zero; Facer.IsHitBoxActive = false; },
                0
            );
        }

        public void Deactivate(Texture2D newDefault)
        {
            Animater.Reset();
            Animater.SetDefaultSprite(newDefault);
            Facer.IsHitBoxActive = false;
        }

        public void Move(float deltaTime)
        {
            lerp += 0.25f;
            if (lerp >= 1)
                lerp = 0;

            if (!IsDead)
                Facer.Velocity = Vector2.Lerp(Facer.Velocity, Facer.Facing, lerp);
            Facer.Move(deltaTime);
        }
    }
}
