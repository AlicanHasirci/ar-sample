using System;
using System.Collections.Generic;
using DG.Tweening;
using SpadesPlus.Bezier;
using UnityEngine;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(Hand))]
public class HandInspector : Editor {
    public override void OnInspectorGUI () {
        var hand = (Hand)target;
        EditorGUI.BeginChangeCheck ();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck()) {
            hand.PositionCards();
        }
    }

    private void OnSceneGUI () {
        var hand = (Hand)target;
        var fanCenter = hand.transform.TransformPoint(hand.FanCenter);
        Handles.color = Color.red;
        var difference = (hand.transform.position - fanCenter).normalized;
        var normal = Vector3.Cross(difference, Vector3.right);
        var angle = hand.Arc * .5f;
        var from = Quaternion.AngleAxis(angle, -normal) * (difference) * hand.Radius;
        var to = Quaternion.AngleAxis(angle, normal) * (difference) * hand.Radius;
        Handles.DrawDottedLine(from + fanCenter, fanCenter, 3f);
        Handles.DrawDottedLine(to + fanCenter, fanCenter, 3f);
        Handles.DrawWireArc(fanCenter, normal, from, hand.Arc, hand.Radius);
    }
}
#endif


public class Hand : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {
    public struct CardTransform {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    public GameObject CardPrefab;
    public int CardCount = 13;
    public float Radius = 5f;
    public float Arc = 45f;
    public JumpAnimation JumpAnimation;
    public ThrowAnimation ThrowAnimation;

    private List<GameObject> _cards;
    private List<CardTransform> _transforms;
    private BezierSpline _throwPath;
    private GameObject _selectedCard;

    public Vector3 FanCenter {
        get { return new Vector3(0, -Radius, 0); }
    }

    private void Awake() {
        _cards = new List<GameObject>(CardCount);
        _transforms = new List<CardTransform>(CardCount);
        _throwPath = GetComponent<BezierSpline>();
        for (var i = 0; i < CardCount; i++) {
            var go = Instantiate(CardPrefab, transform);
            _cards.Add(go);
        }
        CalculateTransforms();
        PositionCards();
    }

    public void DealCards() {
        foreach (var go in _cards) {
            go.transform.SetParent(transform);
        }
        PositionCards();
    }

    public void PositionCards () {
        if (_cards == null) return;
        for (var i = 0; i < _cards.Count; i++) {
            var card = _cards[i];
            card.transform.localPosition = _transforms[i].Position;
            card.transform.localRotation = _transforms[i].Rotation;
        }
    }

    private void CalculateTransforms() {
        for (var i = 0; i < _cards.Count; i++) {
            var slice = Arc / (CardCount - 1);
            var angle = (90 + Arc * .5f) + (slice * -i);
            var x = FanCenter.x
                    + Radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            var y = FanCenter.y
                    + Radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            var ct = new CardTransform() {
                Position = new Vector3(x,y,0),
                Rotation = Quaternion.Euler(0, -3f, angle - 91)
            };
            _transforms.Add(ct);
        }
    }

    private void SelectCard(GameObject card) {
        if (_selectedCard == card) return;
        DeselectCard();
        _selectedCard = card;

        _selectedCard.transform.DOKill();
        _selectedCard.transform.DOLocalJump(
            _selectedCard.transform.localPosition + JumpAnimation.delta,
            JumpAnimation.power,
            JumpAnimation.number,
            JumpAnimation.duration);
    }

    private void DeselectCard() {
        if (_selectedCard == null) return;

        MoveToOriginalPosition();
        _selectedCard = null;
    }

    private void MoveToOriginalPosition () {
        if (_selectedCard == null) {
            return;
        }
        _selectedCard.transform.SetParent(transform);
        var index = _cards.IndexOf(_selectedCard);
        _selectedCard.transform.DOKill();
        _selectedCard.transform.DOLocalMove (_transforms[index].Position, .35f);
        _selectedCard.transform.DOLocalRotateQuaternion(_transforms[index].Rotation, .35f);
        _selectedCard.transform.DOScale(Vector3.one, .35f);
    }

    public void OnPointerDown (PointerEventData eventData) {
        GameObject card;
        if (TryGetCard(eventData, out card)) {
            if (_selectedCard != null && _selectedCard == card) {
                SetControlPoints(_selectedCard.transform);
                StartCoroutine(ThrowAnimation.ThrowCard(_selectedCard, _throwPath));
            }
            else {
                SelectCard(card);
            }
        }
        else {
            DeselectCard();
        }
    }

    public void OnPointerUp (PointerEventData eventData) {
        GameObject card;
        if (TryGetCard(eventData, out card)) {
        }
    }

    public void OnDrag (PointerEventData eventData) {
        GameObject card;
        if (TryGetCard(eventData, out card)) {
            if (eventData.position.y > Screen.height * 0.3f && _selectedCard != null) {
                StartCoroutine(ThrowAnimation.ThrowCard(_selectedCard, _throwPath));
            }
            else {
                if (card != _selectedCard) {
                    SelectCard(card);
                }
            }
        }
    }

    private static bool TryGetCard (PointerEventData eventData, out GameObject card) {
        card = eventData.pointerCurrentRaycast.gameObject;
        return card != null;
    }

    private void SetControlPoints(Transform card) {
        var origin = card.position;
        var target = _throwPath.GetControlPoint(_throwPath.ControlPointCount - 1);
        var delta = target - origin;
        _throwPath.SetControlPoint(0, origin);
        var len = _throwPath.ControlPointCount - 1;
        for (var i = 1; i < len; i++) {
            var pos = origin + ((i / (float) len) * delta) + card.localPosition;
            _throwPath.SetControlPoint(i, pos);
        }
    }
}

[Serializable]
public struct JumpAnimation {
    [SerializeField] public Vector3 delta;
    [SerializeField] public float power;
    [SerializeField] public int number;
    [SerializeField] public float duration;

    public void Jump(Transform t) {
        t.DOLocalJump(t.localPosition + delta, power, number, duration).Play();
    }

    public void JumpBack(Transform t) {
        t.DOLocalMove(t.localPosition - delta, duration).Play();
    }
}
