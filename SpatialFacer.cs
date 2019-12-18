using Microsoft.Xna.Framework;
using System;

namespace Pipo 
{
    //TODO Add sprite data
    //EDIT sprite into own class -> recognition with id -> ECS??
    class SpatialFacer
    {
        //readonly int id; //static readonly?
        public readonly string tag; 

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 facing; //TODO int rotation?
        public float size;
        public bool isHitBoxActive;

        public SpatialFacer(string tag0, 
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
            return (float)Math.Acos(Vector2.Dot(new Vector2(0,1), facing)/facing.Length());
        }

        public bool CircleCollision(SpatialFacer target)
        {
            return Vector2.Distance(position, target.position) < (size + target.size);
        }

        public bool SquareCollision(SpatialFacer target)
        {
            return (position.X + size > target.position.X 
                 && position.X < target.position.X + target.size
                 && position.Y + size > target.position.Y 
                 && position.Y < target.position.Y + target.size);
        }
    }
}
