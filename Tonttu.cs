using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pipo;

namespace Joulurauhaa2019
{
    class Tonttu
    {
        public const float Radius = 32;
        public const float Speed = 200;
        public CircleFacer Facer;
        public readonly SpriteAnimater Animater;

        private readonly Vector2 pivot;
        private readonly int[] delays; 

        /// <summary>
        /// velocity should normally be unit in length, as it is scaled with 
        /// </summary>
        public Tonttu(Vector2 position, Vector2 velocity, Texture2D[] frames)
        {
            pivot = new Vector2( 32, 35 );
            delays = new int[]{ 0, 5,2,5,2 };

            Facer = new CircleFacer(Radius, position, Speed*velocity, true);
            Animater = new SpriteAnimater(frames.Length, pivot);

            Animater.SetFrames(frames, delays);
            Animater.Start(true);
        }
    }
}
