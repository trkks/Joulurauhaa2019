using Microsoft.Xna.Framework;

namespace Pipo 
{
    //TODO Add sprite data
    //EDIT sprite into own class -> recognition with id -> ECS??
    class SquareCollFacer
    {
        //readonly int id; //static readonly?
        public readonly string tag; 

        public Vector2 position;
        public Vector2 velocity;
        public Vector2 facing; //TODO int rotation?
        public int size;
        public bool isHitBoxActive;

        public SquareCollFacer(string tag0, 
                        Vector2 pos, Vector2 velo, int size0, bool active)
        {
            tag = tag0;

            position = pos;
            velocity = velo;
            size = size0;
            isHitBoxActive = active;

            facing = new Vector2(1,0);
        }

        public bool Colliding(SquareCollFacer target)
        {
            return (position.X + size > target.position.X 
                 && position.X < target.position.X + target.size
                 && position.Y + size > target.position.Y 
                 && position.Y < target.position.Y + target.size);
        }
    }
}
