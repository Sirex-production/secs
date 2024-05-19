#if UNITY_EDITOR

using UnityEditor;

namespace Secs.Debug
{
    [CustomEditor(typeof(EcsSingletonEntitiesObserver))]
    public sealed class EcsSingletonEntitiesObserverInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Singleton Entities : ");
        }
    }
}
#endif