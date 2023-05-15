using System;
using System.Collections.Generic;
using Utils;

namespace Collision.Shapes
{
    public enum ShapeType
    {
        SHAPE = 0,
        CIRCLE = 1,
        RECTANGLE = 2,
        POLYGON = 3,
    }
    public class Shape
    {
        /** The state of this shape, if inactive can be ignored in results */
        public bool active = true;

        /** The name of this shape, to help in debugging */
        public string name;
        
        /** A list of tags to use for marking shapes with data for later use, by key/value */
        public IDictionary<String, String> tags {get; private set;}
        
        //어떤 종류인지
        public ShapeType Type;

        public Vector position {
            get => _position;
            set {
                _position = value;
                refresh_transform();
            }
        }
        /** The x position of this shape */
        public float x {
            get => _position.x;
            set {
                _position.x = value;
                refresh_transform();
            }
        }
        
        public float y {
            get => _position.y;
            set {
                _position.y = value;
                refresh_transform();
            }
        }

        /** The rotation of this shape, in degrees */
        public float rotation {
            get => _rotation;
            set {
                _rotation = value;
                refresh_transform();
            }
        }

        /** The scale in the x direction of this shape */
        public float scaleX {
            get => _scaleX;

            set {
                _scaleX = value;
                refresh_transform();
            }
        }
        /** The scale in the y direction of this shape */
        public float scaleY {
            get => _scaleY;

            set {
                _scaleY = value;
                refresh_transform();
            }
        }
        
        private Vector _position;
        private float _rotation = 0;

        private float _scaleX = 1;
        private float _scaleY = 1;

        protected bool _transformed = false;
        protected Matrix _transformMatrix;
    
        
        public Shape (float x, float y)
        {
            tags = new Dictionary<string, string>();

            _position = new Vector(x, y);
            _rotation = 0;

            _scaleX = 1;
            _scaleY = 1;

            _transformMatrix = new Matrix();
            _transformMatrix.makeTranslation(x, y);
        }

        
        /** Test this shape against another shape. */
        public virtual ShapeCollision test(Shape shape) {
            return null;
        }
    
        /** Test this shape against a circle. */
        public virtual ShapeCollision testCircle( Circle circle, bool flip = false ) {
            return null;
        }

        /** Test this shape against a polygon. */
        public virtual ShapeCollision testPolygon( Polygon polygon, bool flip = false ) {
            return null;
        }

        private void refresh_transform() {

            _transformMatrix.compose(position, rotation, new Vector(scaleX, scaleY));
            _transformed = false;

        }

        public float GetapproximateRadius()
        {
            if (Type == ShapeType.CIRCLE)
                return ((Circle)this).radius;
            else if (Type == ShapeType.POLYGON) //할일 : 대각으로 바꿔야함
            {
                float max = 0;
                float value = 0;
                var t = ((Polygon)this).transformedVertices;
                
                for (int i = 0; i < t.Count; i++)
                {
                    
                    if (i == t.Count - 1)
                        value = t[i].Distance(t[0]);
                    else
                        value = t[i].Distance(t[i + 1]);
                    

                    if (max < value)
                        max = value;
                }

                return max;
            }
            else
            {
                return -1;
            }
        }
        /** clean up and destroy this shape */
        public void destroy() {

            _position = null;
            _transformMatrix = null;

        }
    
    }
}

