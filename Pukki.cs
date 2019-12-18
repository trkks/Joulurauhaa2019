using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pipo;

namespace Pipo 
{
    class Pukki
    {
        const string tag = "pukki";
        const float speed = 200;

        public SpatialFacer spaFac;
        public SpriteAnimater sprAnim;

        public Pukki(Vector2 pos, float size, Texture2D[] frames)
        {
            spaFac = new SpatialFacer(tag, pos, new Vector2(speed, speed), size, true);
            sprAnim = new SpriteAnimater(frames);
        }

        
    }
}
