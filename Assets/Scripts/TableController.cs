using GoogleARCore;
using SpadesPlus.Bezier;
using UnityEngine;

public class TableController : MonoBehaviour {

	public GameObject TablePrefab;
	public Camera Camera;
	public BezierSpline _spline;
	private GameObject _table;
	
	public void OnAnchorFound(Anchor anchor, TrackableHit hit) {
		gameObject.SetActive(true);
		transform.SetParent(anchor.transform);
		transform.localRotation = Quaternion.identity;
		transform.localPosition = Vector3.zero;

		var dot = Vector3.Dot(anchor.transform.up, Camera.transform.position - hit.Pose.position);
		if (hit.Distance > 1f && dot > .5f) {
			if (_table == null) _table = Instantiate(TablePrefab, transform);
			_table.transform.localPosition = new Vector3(0, - 0.5f, 0);
			_table.transform.localPosition = Vector3.zero;
			_table.transform.localRotation = Quaternion.Euler(0, -90f, 0);
			_spline.SetControlPoint(_spline.ControlPointCount - 1, anchor.transform.position + new Vector3(0, 0.91f, 0));
		}
		else {
			if (_table != null) _table.SetActive(false);
			_spline.SetControlPoint(_spline.ControlPointCount - 1, anchor.transform.position);
		}
		
	}

	public void OnThrowEnd(GameObject card) {
		card.transform.SetParent(transform, true);
	}
}
