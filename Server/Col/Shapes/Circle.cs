using System;
using System.Numerics;

namespace Collision.Shapes
{
    public class Circle : Shape
    {
        
        public Vector2 Position;
        private float _radius;
        public float radius { get { return _radius; } set { _radius = value; name = "circle " + radius; } }
        
        
        public bool isEmpty =>
            //bounds.Width == 0 || bounds.Height == 0
            radius <= 0;
        
        public float transformedRadius => radius * scaleX;

        
        public Circle (float x, float y, float radius) : base(x, y)
        {
            this.radius = radius;
            Type = ShapeType.CIRCLE;
            name = "Circle";
        }

        public override ShapeCollision test (Shape shape)
        {
            return shape.testCircle(this, true);
        }

        public override ShapeCollision testCircle (Circle circle, bool flip = false)
        {
            return Sat2D.testCircleVsCircle(this, circle, flip);
        }

        public override ShapeCollision testPolygon (Polygon polygon, bool flip = false)
        {
            return Sat2D.testCircleVsPolygon( this, polygon, flip );
        }

        // For insertion
        public bool ContainedBy(Rectangle rect)
        {
            return rect.Left <= Position.X - radius &&
                   rect.Top <= Position.Y - radius &&
                   rect.Left + rect.Width >= Position.X + radius &&
                   rect.Top + rect.Height >= Position.Y + radius;
        }

        // The rest is for query 
        public bool Contains(Rectangle rect)
        {
            // The distance to the furthest corner is less than the radius 
            return new Vector2(
                Math.Max(Math.Abs(Position.X - rect.Left), Math.Abs(Position.X - (rect.Left + rect.Width))),
                Math.Max(Math.Abs(Position.Y - rect.Top), Math.Abs(Position.Y - (rect.Top + rect.Height)))
            ).LengthSquared() < radius * radius;
        }

        // https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection/1879223#1879223
        public bool IntersectsWith(Rectangle rect)
        {
            return Vector2.DistanceSquared(Position, new Vector2(
                Math.Clamp(Position.X, rect.Left, rect.Left + rect.Width),
                Math.Clamp(Position.Y, rect.Top, rect.Top + rect.Height)
            )) < radius * radius;
        }

        public bool IntersectsWith(Circle circle)
        {
            return Vector2.DistanceSquared(Position, circle.Position) <
                   (radius + circle.radius) * (radius + circle.radius);
        }
    }
}