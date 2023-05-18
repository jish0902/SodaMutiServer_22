using System.Collections.Generic;
using Collision;
using Collision.Shapes;

namespace Differ
{
    public class Collision
    {
        public Collision()
        {
        }

        /** Test a single shape against another shape.
            When no collision is found between them, this function returns null.
            Returns a `ShapeCollision` if a collision is found. */
        public static ShapeCollision shapeWithShape(Shape shape1, Shape shape2)
        {
            return shape1.test(shape2);
        }

        /** Test a single shape against multiple other shapes.
            When no collision is found, this function returns an empty array, this function will never return null.
            Returns a list of `ShapeCollision` information for each collision found. */
        public static IList<ShapeCollision> shapeWithShapes(Shape shape, IList<Shape> shapes)
        {
            var results = new List<ShapeCollision>();

            foreach (var otherShape in shapes)
            {
                var result = shapeWithShape(shape, otherShape);
                if (result != null)
                {
                    results.Add(result);
                }
            }

            return results;
        }
    }
}