using System;
using SpadesPlus.Bezier;
using UnityEngine;

public class Opponent : MonoBehaviour {

    public GameObject CardPrefab;

    public ThrowAnimation ThrowAnimation;
    private GameObject _card;
    private BezierSpline _throwPath;

    private void Awake() {
        _throwPath = GetComponent<BezierSpline>();
        _card = Instantiate(CardPrefab, transform);
        PositionCard();
    }

    public void Throw() {
        PositionCard();
        StartCoroutine(ThrowAnimation.ThrowCard(_card, _throwPath));
    }

    private void PositionCard () {
        _card.transform.position = transform.position;
        _card.transform.localRotation = Quaternion.Euler(90, 0, 0);
        _card.transform.localScale = Vector3.zero;
    }
}

public struct PropertyBinding<T> {
    [SerializeField] private T _value;
    private event Action<T> Action;

    public T Value {
        get { return _value; }
        set {
            _value = value;
            if (Action != null) Action(_value);
        }
    }

    public PropertyBinding(T value) {
        _value = value;
        Action = null;
    }

    public PropertyBinding<T> Register(Action<T> action) {
        Action += action;
        return this;
    }

    public PropertyBinding<T> Unregister(Action<T> action) {
        if (Action != null) Action -= action;
        return this;
    }

    public static implicit operator T(PropertyBinding<T> pb) {
        return pb._value;
    }

    public static PropertyBinding<T> operator + (PropertyBinding<T> binding, Action<T> action) {
        return binding.Register(action);
    }

    public static PropertyBinding<T> operator - (PropertyBinding<T> binding, Action<T> action) {
        return binding.Unregister(action);
    }
}
