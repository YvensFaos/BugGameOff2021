    using UnityEngine;

    [UnityEditor.CustomEditor(typeof(MoveEvent))]
    public class MoveEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var moveEvent = (MoveEvent) target;

            if (GUILayout.Button("Move"))
            {
                moveEvent.Move();
            }
        }
    }
