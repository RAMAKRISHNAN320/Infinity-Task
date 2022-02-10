using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PathCreation;

namespace PathCreationEditor
{

#if UNITY_EDITOR
    [CustomEditor(typeof(PathSpawnerTest))]
    public class PathSpawnerEditor : Editor
    {
        const float screenPolylineMaxAngleError = .3f;
        const float screenPolylineMinVertexDst = .01f;

        PathCreationEditor.ScreenSpacePolyLine.MouseInfo pathMouseInfo;
        PathCreationEditor.ScreenSpacePolyLine screenSpaceLine;
        bool hasUpdatedScreenSpaceLine;

        int heightCubeAmount = 1;
        int obstacleID = 0;

        PathSpawnerTest spawner;
        VertexPath Path
        {
            get
            {
                return spawner.pathPrefab.path;
            }
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            switch (spawner.selectedType)
            {
                case EEntityType.OBSTACLE:
                    DrawObstacleInspector();
                    break;
                case EEntityType.CUBE:
                    DrawCubeInspector();
                    break;
                case EEntityType.COIN:
                    DrawCoinInspector();
                    break;
                default:
                    break;
            }
        }

        void DrawObstacleInspector()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                obstacleID = EditorGUILayout.IntSlider(obstacleID, 0, PrefabsHolder.instance.obstaclePrefabs.Count - 1);

                if (check.changed)
                {
                    SceneView.RepaintAll();
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }
        }
        void DrawCubeInspector()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                heightCubeAmount = EditorGUILayout.IntSlider(heightCubeAmount, 1, 10);

                if (check.changed)
                {
                    SceneView.RepaintAll();
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }
        }
        void DrawCoinInspector()
        {

        }

        private void OnSceneGUI()
        {
            if (spawner.pathPrefab)
            {
                Input();
            }
        }

        void Input()
        {
            Event guiEvent = Event.current;
            hasUpdatedScreenSpaceLine = false;
            UpdatePathMouseInfo();
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
            {
                Vector3 newPathPoint = pathMouseInfo.closestWorldPointToMouse;

                switch (spawner.selectedType)
                {
                    case EEntityType.OBSTACLE:
                        Undo.RecordObject(spawner, "Added Entity");
                        spawner.AddSpawnEntity(spawner.selectedType, newPathPoint, obstacleID, pathMouseInfo.distanceAlongPathWorld);
                        spawner.UpdateEntities();
                        break;
                    case EEntityType.CUBE:
                        Undo.RecordObject(spawner, "Added Entity");
                        spawner.AddSpawnEntity(spawner.selectedType, newPathPoint, 0, pathMouseInfo.distanceAlongPathWorld, heightCubeAmount);
                        spawner.UpdateEntities();
                        break;
                    case EEntityType.COIN:
                        break;
                    default:
                        break;
                }

            }
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.control)
            {
                Undo.RecordObject(spawner, "Removed Entity");
                Vector3 newPathPoint = pathMouseInfo.closestWorldPointToMouse;
                spawner.RemoveEntity(spawner.GetClosestEntityToPoint(newPathPoint));
                spawner.UpdateEntities();
            }
            HandleUtility.AddDefaultControl(0);

        }

        private void OnEnable()
        {
            spawner = (PathSpawnerTest)target;
        }

        void UpdatePathMouseInfo()
        {
            if (!hasUpdatedScreenSpaceLine || (screenSpaceLine != null && screenSpaceLine.TransformIsOutOfDate()))
            {
                screenSpaceLine = new PathCreationEditor.ScreenSpacePolyLine(bezierPath, spawner.pathPrefab.transform, screenPolylineMaxAngleError, screenPolylineMinVertexDst);
                hasUpdatedScreenSpaceLine = true;
            }
            pathMouseInfo = screenSpaceLine.CalculateMouseInfo();
        }

        BezierPath bezierPath
        {
            get
            {
                return data.bezierPath;
            }
        }

        PathCreatorData data
        {
            get
            {
                return spawner.pathPrefab.EditorData;
            }
        }
    }
#endif


}
