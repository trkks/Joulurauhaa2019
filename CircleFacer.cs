using Microsoft.Xna.Framework;
using System;

namespace Pipo 
{
    //TODO Add sprite data
    //EDIT sprite into own class -> recognition with id -> ECS??
    //Anchor/pivot is center
    class CircleFacer
    {
        //readonly int id; //static readonly?
        public readonly string tag; 

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 facing; //TODO int rotation?
        public float size; //radius
        public bool isHitBoxActive;

        public CircleFacer(string tag0, 
                        Vector2 pos, Vector2 velo, float size0, bool active)
        {
            tag = tag0;

            position = pos;
            velocity = velo;
            size = size0;
            isHitBoxActive = active;

            facing = new Vector2(0,1);
        }

        public void Move(float delta)
        {
            position += velocity * delta;
        }

        public float faceToRotation()
        {
            return (float)Math.Acos(Vector2.Dot(Vector2.UnitY, facing)/facing.Length());
        }

        public bool CircleCollision(CircleFacer target)
        {
            return Vector2.Distance(position, target.position) < (size + target.size);
        }

        //Change this to copy to RectangleBoard?
        //public bool SquareCollision(CircleFacer target)
        //{
        //    return (position.X + size > target.position.X 
        //         && position.X < target.position.X + target.size
        //         && position.Y + size > target.position.Y 
        //         && position.Y < target.position.Y + target.size);
        //}

        // sure this works
        public static void Bounce(ref CircleFacer cf1, ref CircleFacer cf2)
        {
            Vector2 tmpVelo1 = cf1.velocity;
            Vector2 tmpVelo2 = cf2.velocity;
            cf1.velocity = Vector2.Reflect(tmpVelo1, Vector2.Normalize(Vector2.Subtract(cf1.position, cf2.position)));
            cf2.velocity = Vector2.Reflect(tmpVelo2, Vector2.Normalize(Vector2.Subtract(cf2.position, cf1.position)));
        }
    }
}
