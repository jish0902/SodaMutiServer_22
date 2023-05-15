using System;
using System.Collections.Generic;
using System.Numerics;
using Collision;
using Collision.Shapes;
using Server.Game;
using Vector2 = System.Numerics.Vector2;


namespace QuadTree
{
    public class QuadTreeManager 
    {
        //lock을 걸어야 할까? => logic안에서 돌아간다면 상관 x, 분리한다면 o
        
        private QuadTree<GameObject> _quadDynamic = new QuadTree<GameObject>(-100,-100,200,200);
        private readonly List<GameObject> _objects = new List<GameObject>();

       

        public void Insert(GameObject go)
        {
            if (_objects.Contains(go) == false)
            {
                _objects.Add(go);
                _quadDynamic.Insert(go,GetBounds(go));
            }
        }
        
        Circle GetBounds(GameObject obj) 
        {
            return new Circle(obj.PosInfo.PosX,obj.PosInfo.PosY,obj.currentShape.GetapproximateRadius());
        }
        
       
        public bool RemoveObject(GameObject obj)
        {
            return _objects.Remove(obj);
        }

        /// <summary>
        /// Gets the nearest GameObjects to the given GameObject.
        /// </summary>
        /// <param name="obj">GameObject which will be the origin of the check.</param>
        /// <returns>List of GameObjects. Null if GameObject is not present in the QuadTree.</returns>
        public List<GameObject> GetNearestObjects(GameObject obj)
        {
            return GetNearestObjects(GetBounds(obj));
        }
        public List<GameObject> GetNearestObjects(Circle circle)
        {
            var nearest = new List<GameObject>();

            foreach (var obj in _quadDynamic.GetNodesInside(circle))
            {
                nearest.Add(obj);
            }

            return nearest;
        }
        
        public void tUpdate()
        {
            //  we iterate over a copy of _objects because the original gets modified
            var objectsCopy = new List<GameObject>(_objects);
            foreach (var obj in objectsCopy)
            {
                UpdateCollision(obj);
            }

            UpdateQuadTree();
        }

        
        private void UpdateCollision(GameObject gameObject)
        {
            Shape mainCol = gameObject.currentShape;
            List<GameObject> hitList = new List<GameObject>();
            ShapeCollision temp = null;
            
            if (mainCol.Type == ShapeType.CIRCLE)
            {
                foreach (GameObject go in GetNearestObjects(gameObject))
                {
                    var otherCol = go.currentShape;
                    if (otherCol.Type == ShapeType.CIRCLE)
                    {
                        temp = Sat2D.testCircleVsCircle((Circle)mainCol, (Circle)otherCol);

                    }
                    else if (otherCol.Type == ShapeType.POLYGON)
                    {  
                        temp = Sat2D.testCircleVsPolygon((Circle)mainCol, (Polygon)otherCol);
                    }
                    
                    if (temp != null && (temp.overlap > 0 || temp.overlap < 0))
                        hitList.Add(go);
                }
            }
            else if (mainCol.Type == ShapeType.POLYGON)
            {
                foreach (GameObject go in GetNearestObjects(gameObject))
                {
                    var otherCol = go.currentShape;
                    if (otherCol.Type == ShapeType.CIRCLE)
                    {
                        temp = Sat2D.testCircleVsPolygon(polygon:(Polygon)mainCol,circle:(Circle)otherCol);
                    }
                    else if (otherCol.Type == ShapeType.POLYGON)
                    {
                        temp = Sat2D.testPolygonVsPolygon((Polygon)mainCol, (Polygon)otherCol);
                    }

                    if (temp != null && (temp.overlap > 0 || temp.overlap < 0))
                        hitList.Add(go);
                }

            }


            foreach (var go in hitList)
            {
                gameObject.OnCollision(go);
            }
        }
        
        
        private void UpdateQuadTree()
        {
            _quadDynamic.Clear();
            foreach (var obj in _objects)
            {
                //obj.transform.position += new Vector3(Random.(-3, 4), Random.Range(-3, 4), 0);
                _quadDynamic.Insert(obj,GetBounds(obj));
            }
        }
    }
}