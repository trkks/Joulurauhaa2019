using Microsoft.Xna.Framework;
using System;

namespace Pipo 
{
    //TODO sprite into own class -> recognition with id -> ECS??
    class CircleFacer
    {
        //public readonly int id; //static readonly?
        //public readonly string Tag; 

        public Vector2 Position;
        public Vector2 Facing;
        public Vector2 Velocity;
        public readonly float Radius;

        private bool isHitBoxActive{ get; set; } //TODO protection level :thinking:

        /// <summary>
        /// The instance will be set to face right (1,0)
        /// </summary>
        public CircleFacer(float radius, Vector2 position, Vector2 velocity, bool isHitBoxActive)
        {
            this.Position = position;
            this.Velocity = velocity;
            this.Radius = radius;
            this.isHitBoxActive = isHitBoxActive;
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
            return Vector2.Distance(Position, target.Position) <= (Radius + target.Radius);
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
            Vector2 tmpVelo1 = cf1.Velocity;
            Vector2 tmpVelo2 = cf2.Velocity;
            cf1.Velocity = Vector2.Reflect(tmpVelo1, Vector2.Normalize(Vector2.Subtract(cf1.Position, cf2.Position)));
            cf2.Velocity = Vector2.Reflect(tmpVelo2, Vector2.Normalize(Vector2.Subtract(cf2.Position, cf1.Position)));
        }
    }
}
