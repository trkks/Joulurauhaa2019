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
        public bool isActive;
        public CircleFacer Facer;

        private readonly Texture2D deathFrame;
        private readonly Vector2 pivot;
        private readonly int[] delays; 

        /// <summary>
        /// velocity should normally be unit in length, as it is scaled with 
        /// </summary>
        public Tonttu(Vector2 position, Vector2 velocity, Texture2D[] frames, Texture2D deathFrame)
        {
            this.deathFrame = deathFrame;
            pivot = new Vector2( 32, 35 );
            delays = new int[]{ 5,2,5,2 };

            Facer = new CircleFacer(Radius, position, Speed*velocity, true);
            Animater = new SpriteAnimater(frames.Length, pivot);

            Animater.SetFrames(frames, delays);
            Animater.Start(true);

            isActive = true;
        }

        /// <summary>
        /// "Fly *away* relative to target"
        /// TODO Move into CircleFacer, here replace with HandleBeingHit()
        /// </summary>
        public void SetTrajectoryFrom(Vector2 target)
        {
            Vector2 direction = Vector2.Normalize(Vector2.Subtract(Facer.Position, target));
            //TODO Remove hardcode?
            this.Facer.Velocity = 4 * Speed * direction;
        }

        public void Die()
        {
            //TODO Memory cleanup
            Animater.Reset();
            Animater.SetDefaultSprite(deathFrame);
            isActive = false;
        }
    }
}
