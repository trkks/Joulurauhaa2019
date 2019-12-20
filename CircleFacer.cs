using Microsoft.Xna.Framework;
using System;

namespace Pipo 
{
    //TODO sprite into own class -> recognition with id -> ECS??
    class CircleFacer
    {
        //public readonly int id; //static readonly?
        //public readonly string Tag; 

        public bool IsHitBoxActive; //TODO protection level :thinking:
        public Vector2 Position;
        public Vector2 Facing;
        public Vector2 Velocity;
        public readonly float Radius;

        /// <summary>
        /// The instance will be set to face right (1,0)
        /// </summary>
        public CircleFacer(float radius, Vector2 position, Vector2 velocity, bool isHitBoxActive)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Radius = radius;
            this.IsHitBoxActive = isHitBoxActive;
            Facing = Vector2.UnitX;
        }

        public void Move(float deltaTime)
        {
            Position += Velocity * deltaTime;
        }

        public float GetRotation()
        {
            return (float)Math.Atan2(Facing.Y, Facing.X);
        }

        public bool CollidingWith(CircleFacer target)
        {
            return IsHitBoxActive 
                && Vector2.Distance(Position, target.Position) <= (Radius + target.Radius);
        }

        //Change this to copy to RectangleBoard?
        //public bool SquareCollision(CircleFacer target)
        //{
        //    return (position.X + size > target.position.X 
        //         && position.X < target.position.X + target.size
        //         && position.Y + size > target.position.Y 
        //         && position.Y < target.position.Y + target.size);
        //}

        //TODO Possibly needs some more work: weird jittering
        public static void Bounce(ref CircleFacer cf1, ref CircleFacer cf2)
        {
            if (cf1.IsHitBoxActive && cf2.IsHitBoxActive)
            {
                Vector2 tmpVelo1 = cf1.Velocity;
                Vector2 tmpVelo2 = cf2.Velocity;
                cf1.Velocity = Vector2.Reflect(tmpVelo1, Vector2.Normalize(Vector2.Subtract(cf1.Position, cf2.Position)));
                cf2.Velocity = Vector2.Reflect(tmpVelo2, Vector2.Normalize(Vector2.Subtract(cf2.Position, cf1.Position)));
            }
        }
    }
}
