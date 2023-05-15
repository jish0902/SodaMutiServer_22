using System.Collections.Generic;
using Utils;

namespace Collision.Shapes;

public class Rectangle : Polygon
{
    public float Top;
    public float Left;
    public float Width;
    public float Height;

    //Todo base 고치기!!!!!!
    public Rectangle(float left, float top, float width, float height) : base(left+width/2,top+height/2,null)
    {
        Left = left;
        Top = top;
        Width = width;
        Height = height;

        Type = ShapeType.RECTANGLE;
    }
    
    /** Helper generate a rectangle at x,y with a given width/height and centered state.
            Centered by default. Returns a ready made `Polygon` collision `Shape` */
    public static Polygon rectangle(float x, float y, float width, float height, bool centered = true) {

        var vertices = new List<Vector>();

        if (centered) {

            vertices.Add( new Vector( -width / 2, -height / 2) );
            vertices.Add( new Vector(  width / 2, -height / 2) );
            vertices.Add( new Vector(  width / 2,  height / 2) );
            vertices.Add( new Vector( -width / 2,  height / 2) );

        } else {

            vertices.Add( new Vector( 0, 0 ) );
            vertices.Add( new Vector( width, 0 ) );
            vertices.Add( new Vector( width, height) );
            vertices.Add( new Vector( 0, height) );

        }

        return new Polygon(x,y,vertices);
    }
}