using UnityEngine;

namespace SpadesPlus.Bezier {
	public static class Bezier {
		
		public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
			t = Mathf.Clamp01(t);
			float oneMinusT = 1f - t;
			return
				3f * oneMinusT * oneMinusT * (p1 - p0) +
					6f * oneMinusT * t * (p2 - p1) +
					3f * t * t * (p3 - p2);;
		}
		
		public static Vector3 GetPoint(float t, params Vector3[] points) {
			if (points.Length == 1) return points[0];
			if (points.Length == 2) return Vector3.Lerp(points[0], points[1], t);
		    if (points.Length == 3) return Quadritic(points[0], points[1], points[2], t);
            if (points.Length == 4) return Cubic(points[0], points[1], points[2], points[3], t);
			else {
				Vector3[] npoints = new Vector3[points.Length - 1];
				for(int i = 0; i < npoints.Length ; i++) {
					npoints[i] = Vector3.Lerp(points[i], points[i+1], t);
				}
				return GetPoint(t, npoints);
			}
		}

	    private static Vector3 Quadritic(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
	        var oneMinusT = 1f - t;
	        return (p0 * oneMinusT * oneMinusT) +
	               (p1 * 2 * oneMinusT * t) +
	               (p2 * t * t);
	    }

	    private static Vector3 Cubic(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
	        var oneMinusT = 1f - t;
	        return (p0 * oneMinusT * oneMinusT * oneMinusT) +
	               (p1 * 3 * oneMinusT * oneMinusT * t) +
	               (p2 * 3 * oneMinusT * t * t) +
	               (p3 * t * t * t);
	    }
	}
}