using Microsoft.Xna.Framework;
using Pipo;
using Joulurauhaa2019;

namespace Joulurauhaa2019
{
    class RectangleBoard
    {
        public float width;
        public float height;

        public RectangleBoard(float w, float h)
        {
            width = w;
            height = h;
        }

        public Bounds Colliding(ref CircleFacer target)
        {
            if (target.Position.Y - target.Radius <= 0)
                return Bounds.Top;
            else if (target.Position.Y + target.Radius >  height)
                return Bounds.Bottom;
            if (target.Position.X - target.Radius <= 0)
                return Bounds.Left;
            else if (target.Position.X + target.Radius > width)
                return Bounds.Right;

            return Bounds.Inside;
        }

        public bool Inside(ref CircleFacer target)
        {
            return (target.Position.Y - target.Radius > 0)
                && (target.Position.Y + target.Radius < height)
                && (target.Position.X - target.Radius > 0)
                && (target.Position.X + target.Radius < width);
                
        }
    }
}
