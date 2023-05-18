using System.Collections.Generic;
using Collision;
using Collision.Shapes;
using Utils;


namespace Collision
{
    public class Sat2D
	{
		/** Internal api - test a circle against a polygon */
	    public static ShapeCollision testCircleVsPolygon(Circle circle, Polygon polygon, bool flip = false) {

	        var result = new ShapeCollision();
			var verts = polygon.transformedVertices;

	        var circleX = circle.x;
	        var circleY = circle.y;

	        float testDistance = float.MaxValue;
	        var distance = 0.0f;
	        var closestX = 0.0f;
	        var closestY = 0.0f;

	        for (var i = 0; i < verts.Count; i++) {
	            distance = Util.vec_lengthsq(circleX - verts[i].x, circleY - verts[i].y);

	            if (distance < testDistance) {
	                testDistance = distance;
	                closestX = verts[i].x;
	                closestY = verts[i].y;
	            }
	        }

			var normalAxisX = closestX - circleX;
	        var normalAxisY = closestY - circleY;
	        var normAxisLen = Util.vec_length(normalAxisX, normalAxisY);
            normalAxisX = Util.vec_normalize(normAxisLen, normalAxisX);
            normalAxisY = Util.vec_normalize(normAxisLen, normalAxisY);

            //project all its points, 0 outside the loop
	        var test = 0.0f;
	        var min1 = Util.vec_dot(normalAxisX, normalAxisY, verts[0].x, verts[0].y);
	        var max1 = min1;

	        for (var j = 1; j < verts.Count; j++) {
	            test = Util.vec_dot(normalAxisX, normalAxisY, verts[j].x, verts[j].y);
	            if (test < min1) min1 = test;
	            if (test > max1) max1 = test;
	        }

	        // project the circle
	        var max2 = circle.transformedRadius;
	        var min2 = -circle.transformedRadius;
	        var offset = Util.vec_dot(normalAxisX, normalAxisY, -circleX, -circleY);
	            
	        min1 += offset;
	        max1 += offset;

	        var test1 = min1 - max2;
	        var test2 = min2 - max1;

	        //if either test is greater than 0, there is a gap, we can give up now.
	        if (test1 > 0 || test2 > 0) return null;

	        // circle distance check
	        var distMin = -(max2 - min1);
	        if (flip) distMin *= -1;

	        result.overlap = distMin;
	        result.unitVectorX = normalAxisX;
	        result.unitVectorY = normalAxisY;
	        var closest = System.Math.Abs(distMin);

            // find the normal axis for each point and project
            for (var i = 0; i < verts.Count; i++) {

	            normalAxisX = findNormalAxisX(verts, i);
	            normalAxisY = findNormalAxisY(verts, i);
	            var aLen = Util.vec_length(normalAxisX, normalAxisY);
	            normalAxisX = Util.vec_normalize(aLen, normalAxisX);
	            normalAxisY = Util.vec_normalize(aLen, normalAxisY);

	            // project the polygon(again? yes, circles vs. polygon require more testing...)
	            min1 = Util.vec_dot(normalAxisX, normalAxisY, verts[0].x, verts[0].y);
	            max1 = min1; //set max and min

	            //project all the other points(see, cirlces v. polygons use lots of this...)
				for (var j = 1; j < verts.Count; j++) {
	                test = Util.vec_dot(normalAxisX, normalAxisY, verts[j].x, verts[j].y);
	                if (test < min1) min1 = test;
	                if (test > max1) max1 = test;
	            }

	            // project the circle(again)
	            max2 = circle.transformedRadius; //max is radius
	            min2 = -circle.transformedRadius; //min is negative radius

	            //offset points
	            offset = Util.vec_dot(normalAxisX, normalAxisY, -circleX, -circleY);
	            min1 += offset;
	            max1 += offset;

	            // do the test, again
	            test1 = min1 - max2;
	            test2 = min2 - max1;

	                //failed.. quit now
	            if (test1 > 0 || test2 > 0) {
	                return null;
	            }

	            distMin = -(max2 - min1);
	            if (flip) distMin *= -1;

	            if (System.Math.Abs(distMin) < closest) {
	                result.unitVectorX = normalAxisX;
	                result.unitVectorY = normalAxisY;
	                result.overlap = distMin;
	                closest = System.Math.Abs(distMin);
	            }

	        } //for

	        //if you made it here, there is a collision!!!!!

	        if (flip) {
	        	result.shape1 = polygon;
	        	result.shape2 = circle;
	        } else {
				result.shape1 = circle;
				result.shape2 = polygon;
	        }

	        result.separationX = result.unitVectorX * result.overlap;
	        result.separationY = result.unitVectorY * result.overlap;

	        if (!flip) {
	            result.unitVectorX = -result.unitVectorX;
	            result.unitVectorY = -result.unitVectorY;
	        }

	        return result;
		}

		/** Internal api - test a circle against a circle */
	    public static ShapeCollision testCircleVsCircle( Circle circleA, Circle circleB, bool flip = false ) {

			var result = new ShapeCollision();
	        var circle1 = flip ? circleB : circleA;
	        var circle2 = flip ? circleA : circleB;

	        //add both radii together to get the colliding distance
	        var totalRadius = circle1.transformedRadius + circle2.transformedRadius;
	        //find the distance between the two circles using Pythagorean theorem. No square roots for optimization
	        var distancesq = Util.vec_lengthsq(circle1.x - circle2.x, circle1.y - circle2.y);

	        //if your distance is less than the totalRadius square(because distance is squared)
	        if (distancesq < totalRadius * totalRadius) {
	       
	            //find the difference. Square roots are needed here.
	            float difference = (float) (totalRadius - System.Math.Sqrt(distancesq));

                result.shape1 = circle1;
                result.shape2 = circle2;

                var unitVecX = circle1.x - circle2.x;
                var unitVecY = circle1.y - circle2.y;
                var unitVecLen = Util.vec_length(unitVecX, unitVecY);

                unitVecX = Util.vec_normalize(unitVecLen, unitVecX);
                unitVecY = Util.vec_normalize(unitVecLen, unitVecY);

                result.unitVectorX = unitVecX;
                result.unitVectorY = unitVecY;

                //find the movement needed to separate the circles
                result.separationX = result.unitVectorX * difference;
                result.separationY = result.unitVectorY * difference;

                //the magnitude of the overlap
                result.overlap = Util.vec_length(result.separationX, result.separationY);

	            return result;

	        } //if distancesq < r^2

	        return null;

	    }

		/** Internal api - test a polygon against another polygon */
	    static ShapeCollision tmp1 = new ShapeCollision();
		static ShapeCollision tmp2 = new ShapeCollision();

	    public static ShapeCollision testPolygonVsPolygon(Polygon polygon1, Polygon polygon2, bool flip = false ) {

	        var output = new ShapeCollision();
	        
	        if ( (tmp1 = checkPolygons(polygon1, polygon2, flip)) == null) {
	            return null;
	        }

	        if ( (tmp2 = checkPolygons(polygon2, polygon1, !flip)) == null) {
	            return null;
	        }

	        ShapeCollision result = null;
			ShapeCollision other = null;

	        if (System.Math.Abs(tmp1.overlap) < System.Math.Abs(tmp2.overlap)) {
	            result = tmp1;
	            other = tmp2;
	        } else {
	            result = tmp2;
	            other = tmp1;
	        }

	        result.otherOverlap = other.overlap;
	        result.otherSeparationX = other.separationX;
	        result.otherSeparationY = other.separationY;
	        result.otherUnitVectorX = other.unitVectorX;
	        result.otherUnitVectorY = other.unitVectorY;

			output.copy_from(result);
	        result = other = null;

	        return output;

	    } //testPolygonVsPolygon

    

   

        /** Internal api - implementation details for testPolygonVsPolygon */
	    public static ShapeCollision checkPolygons(Polygon polygon1, Polygon polygon2, bool flip = false ) {

	        var result = new ShapeCollision();

	        // TODO: This is unused, check original source
	        var offset = 0.0f;
	        var test1 = 0.0f;
	        var test2 = 0.0f;
	        var testNum = 0.0f;
	        var min1 = 0.0f;
	        var max1 = 0.0f;
	        var min2 = 0.0f;
	        var max2 = 0.0f;
	        var closest = float.MaxValue;

	        var axisX = 0.0f;
	        var axisY = 0.0f;
	        var verts1 = polygon1.transformedVertices;
	        var verts2 = polygon2.transformedVertices;

            // loop to begin projection
            for (var i = 0; i < verts1.Count; i++) {

	            axisX = findNormalAxisX(verts1, i);
	            axisY = findNormalAxisY(verts1, i);
	            //var aLen = Util.vec_length(axisX, axisY);
	            //axisX = Util.vec_normalize(aLen, axisX);
	            //axisY = Util.vec_normalize(aLen, axisY);

                // project polygon1
	            min1 = Util.vec_dot(axisX, axisY, verts1[0].x, verts1[0].y);
	            max1 = min1;

				for (var j = 1; j < verts1.Count; j++) {
	                testNum = Util.vec_dot(axisX, axisY, verts1[j].x, verts1[j].y);
	                if (testNum < min1) min1 = testNum;
	                if (testNum > max1) max1 = testNum;
	            }

	                // project polygon2
	            min2 = Util.vec_dot(axisX, axisY, verts2[0].x, verts2[0].y);
	            max2 = min2;

				for (var j = 1; j < verts2.Count; j++) {
	                testNum = Util.vec_dot(axisX, axisY, verts2[j].x, verts2[j].y);
	                if (testNum < min2) min2 = testNum;
	                if (testNum > max2) max2 = testNum;
	            }

	            if(min1 - max2 > 0 ||  min2 - max1 > 0) return null;

	            var distMin = -(max2 - min1);
	            if (flip) distMin *= -1;

	            if (System.Math.Abs(distMin) < closest) {
	                result.unitVectorX = axisX;
	                result.unitVectorY = axisY;
	                result.overlap = distMin;
	                closest = System.Math.Abs(distMin);
	            }

	        }

	        result.shape1 = flip ? polygon2 : polygon1;
	        result.shape2 = flip ? polygon1 : polygon2;
	        result.separationX = -result.unitVectorX * result.overlap;
	        result.separationY = -result.unitVectorY * result.overlap;

	        if (flip) {
	            result.unitVectorX = -result.unitVectorX;
	            result.unitVectorY = -result.unitVectorY;
	        }

//	        Debug.Log($" ({min1} , {max1}) ({min2}, {max2})");
	        return result;

	    }

		/** Internal helper for ray overlaps */
		public static float rayU(float udelta, float aX, float aY, float bX, float bY, float dX, float dY) {
	        return (dX * (aY - bY) - dY * (aX - bX)) / udelta;
	    } 

	    public static float findNormalAxisX(IList<Vector> verts, int index) {
	        var v2 = (index >= verts.Count - 1) ? verts[0] : verts[index + 1];
	        return -(v2.y - verts[index].y);
	    }

	    public static float findNormalAxisY(IList<Vector> verts, int index) {
	        var v2 = (index >= verts.Count - 1) ? verts[0] : verts[index + 1];
	        return (v2.x - verts[index].x);
	    }
	}
}


        

