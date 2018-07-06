using System;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;
using UnityEngine.Events;
public class AnchorController : MonoBehaviour {
	private const TrackableHitFlags RaycastFilter = TrackableHitFlags.PlaneWithinPolygon |
	                                                TrackableHitFlags.FeaturePointWithSurfaceNormal;
	
	public Camera FirstPersonCamera;
	public AnchorFoundEvent AnchorFound;
	
	public Anchor Anchor { get; private set; }
	
	private readonly List<DetectedPlane> _planes = new List<DetectedPlane>();
	private int _surfaceCount;
	private bool _searchingAnchor;

	public void FindNewAnchor() {
		_searchingAnchor = true;
		CombatText.Show("Looking For Anchor...");
	}

	public void DeleteAnchor() {
		Destroy(Anchor.gameObject);
	}
	
	private void Update () {
		if (!_searchingAnchor) return;
		
		Session.GetTrackables(_planes);
		TrackableHit hit;
		if (Frame.Raycast(Screen.width * .5f, Screen.height * .5f, RaycastFilter, out hit)) {
			var delta = FirstPersonCamera.transform.position - hit.Pose.position;
			var dot = Vector3.Dot(delta, hit.Pose.rotation * Vector3.up);
			if (hit.Trackable is DetectedPlane && dot > 0) {
				var newAnchor = hit.Trackable.CreateAnchor(hit.Pose);

				if (Anchor != null) {
					for (var i = 0; i < Anchor.transform.childCount; i++) {
						var child = Anchor.transform.GetChild(i);
						child.SetParent(newAnchor.transform, false);
					}
					Destroy(Anchor.gameObject);
				}

				Anchor = newAnchor;
				
				if (AnchorFound != null) AnchorFound.Invoke(Anchor, hit);
				_searchingAnchor = false;
				CombatText.Show("Anchor Found.");
			}
		}
	}

	private void OnDrawGizmos() {
		if (Anchor != null) {
			Gizmos.DrawSphere(Anchor.transform.position, .1f);
		}
	}
}

[Serializable] public class AnchorFoundEvent : UnityEvent<Anchor, TrackableHit> { }
