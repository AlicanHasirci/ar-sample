using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(CombatText))]
public class CombatTextInspector : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
	    if (GUILayout.Button("Test")) CombatText.Show("Test");
    }
}
#endif

public class CombatText : MonoBehaviour {

	public float Duration = 1;
	public Vector2 Offset = Vector2.up;
	public AnimationCurve Ease = AnimationCurve.Linear(0, 0, 1, 1);

	private GameObject _prefab;
	private GameObject _used;
	private GameObject _free;
	private DateTime _lastTextTime;
	private static CombatText _instance;
	
	private void Awake() {
		_prefab = GetComponentInChildren<TextMeshProUGUI>().gameObject;
		_used = CreateContainer("Used");
		_free = CreateContainer("Free");
		_free.SetActive(false);

		_prefab.SetActive(false);

		_instance = this;
	}

	public static void Show(string text) {
		_instance.ShowInternal(text);
	}
	
	private void ShowInternal(string text) {
		var textObject = GetText();
		textObject.text = text;
		textObject.rectTransform.anchoredPosition = Vector2.zero;
		StartCoroutine(Fly(textObject));
	}

	private TextMeshProUGUI GetText() {
		var child = _free.transform.childCount > 0 
			? _free.transform.GetChild(0) 
			: Instantiate(_prefab, _free.transform).transform;
		if (!child.gameObject.activeSelf) child.gameObject.SetActive(true);
		var text = child.GetComponent<TextMeshProUGUI>();
		var color = text.color;
		color.a = 0;
		text.color = color;
		return text;
	}
	
	private GameObject CreateContainer(string n) {
		var container = new GameObject(n);
		var rt = container.AddComponent<RectTransform>();
		rt.parent = transform;
		rt.anchorMin = Vector2.zero;
		rt.anchorMax = Vector2.one;
		rt.sizeDelta = Vector2.zero;
		rt.anchoredPosition = Vector2.zero;
		return container;
	}

	private IEnumerator Fly(Graphic text) {
		var elapsed = 0f;
		var origin = text.rectTransform.anchoredPosition;
		var target = text.rectTransform.anchoredPosition + Offset;
		var iColor = new Color(text.color.r,text.color.g,text.color.b, 1);
		var fColor = new Color(text.color.r,text.color.g,text.color.b, 0);
		text.transform.SetParent(_used.transform);
		var now = DateTime.Now;
		var deltaTime = Duration * .75f - (now - _lastTextTime).TotalSeconds;
		_lastTextTime = now;
		if (deltaTime > 0) {
			_lastTextTime = _lastTextTime.AddSeconds(deltaTime);
			var waitFor = new WaitForSeconds((float) deltaTime);
			yield return waitFor;
		}
		while (elapsed < Duration) {
			elapsed += Time.deltaTime;
			var t = Ease.Evaluate(Mathf.Clamp01(elapsed / Duration));
			text.rectTransform.anchoredPosition = Vector2.Lerp(origin, target, t);
			text.color = Color.Lerp(iColor, fColor, t);
			yield return null;
		}
		text.transform.SetParent(_free.transform);
	}
}
