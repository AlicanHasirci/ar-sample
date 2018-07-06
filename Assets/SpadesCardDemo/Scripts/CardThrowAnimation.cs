using System;
using System.Collections;
using SpadesPlus.Bezier;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class ThrowCallback : UnityEvent<GameObject> { }

[Serializable]
public struct ThrowAnimation {
    public float Duration;
    public float RandomRotation;
    public float InitialScale;
    public float FinalScale;
    public Vector3 TableRotation;
    public Vector3AnimationCurve Position;
    public AnimationCurve Rotation;
    public ThrowCallback ThrowCallback;
    
    public IEnumerator ThrowCard(GameObject card, BezierSpline path) {
        var elapsedTime = 0f;
        var zr = UnityEngine.Random.Range(-RandomRotation, RandomRotation);
        var originRotation = card.transform.rotation;
        var targetRotation =
            Quaternion.Euler(TableRotation) *
            Quaternion.Euler(0, 0, zr);
        while (elapsedTime < Duration) {
            elapsedTime += Time.deltaTime;
            var tp = Position.Evaluate(elapsedTime / Duration);
            var tr = Rotation.Evaluate(elapsedTime / Duration);
            var np = new Vector3(
                    path.GetPoint(tp.x).x,
                    path.GetPoint(tp.y).y,
                    path.GetPoint(tp.z).z
                );
            card.transform.position = np;
            card.transform.rotation = Quaternion.Lerp(originRotation, targetRotation, tr);
            card.transform.localScale = Vector3.Lerp(Vector3.one * InitialScale, Vector3.one * FinalScale, tp.y);
            yield return null;
        }
        if (ThrowCallback != null) ThrowCallback.Invoke(card);
    }
}

[Serializable]
public struct Vector3AnimationCurve {
    public AnimationCurve X;
    public AnimationCurve Y;
    public AnimationCurve Z;

    public Vector3 Evaluate(float t) {
        return new Vector3(
                X.Evaluate(t),
                Y.Evaluate(t),
                Z.Evaluate(t)
            );
    }
}