using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor {

	void OnSceneGUI () {
		FieldOfView fow = target as FieldOfView;
		Vector3 objPos = fow.transform.position;
		Handles.color = Color.white;
		Handles.DrawWireArc (objPos, Vector3.back, Vector3.up, 360, fow.viewDistance);

		Vector2 viewAngleA = fow.DirectionFromAngle (fow.viewAngle / 2, false);
		Vector2 viewAngleB = fow.DirectionFromAngle (-fow.viewAngle / 2, false);

		Handles.DrawLine (objPos, objPos + (Vector3) viewAngleA * fow.viewDistance);
		Handles.DrawLine (objPos, objPos + (Vector3) viewAngleB * fow.viewDistance);

		if (fow.targetIsVisible) {
			Handles.color = Color.red;
			Handles.DrawLine (objPos, fow.targetTransform.position);
		}
	}

}
