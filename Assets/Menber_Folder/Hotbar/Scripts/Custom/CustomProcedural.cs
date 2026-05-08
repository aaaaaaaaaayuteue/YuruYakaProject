using UnityEngine;
using UnityEditor;
using Hotbar.Procedural;
using Unity.VisualScripting;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting.FullSerializer.Internal;

namespace Hotbar.Custom
{
    public class CustomProcedural
    {
        private const float defaultRoomSize = 20.0f;


        [MenuItem("Hotbar/Procedural/[Wall]Up ")]
        public static void CreateWallUp()
        {
            CustomProceduralEditor.SetRoomSize(new Vector3(defaultRoomSize, defaultRoomSize, defaultRoomSize));
            CustomProceduralEditor.CreateWall(ProceduralManager.WallType.Up);
        }

        [MenuItem("Hotbar/Procedural/[Wall]Down ")]
        public static void CreateWallDown()
        {
            CustomProceduralEditor.SetRoomSize(new Vector3(defaultRoomSize, defaultRoomSize, defaultRoomSize));
            CustomProceduralEditor.CreateWall(ProceduralManager.WallType.Down);
        }

        [MenuItem("Hotbar/Procedural/[Wall]Left ")]
        public static void CreateWallLeft()
        {
            CustomProceduralEditor.SetRoomSize(new Vector3(defaultRoomSize, defaultRoomSize, defaultRoomSize));
            CustomProceduralEditor.CreateWall(ProceduralManager.WallType.Left);
        }

        [MenuItem("Hotbar/Procedural/[Wall]Right ")]
        public static void CreateWallRight()
        {
            CustomProceduralEditor.SetRoomSize(new Vector3(defaultRoomSize, defaultRoomSize, defaultRoomSize));
            CustomProceduralEditor.CreateWall(ProceduralManager.WallType.Right);
        }

        [MenuItem("Hotbar/Procedural/[Wall]Front ")]
        public static void CreateWallFront()
        {
            CustomProceduralEditor.SetRoomSize(new Vector3(defaultRoomSize, defaultRoomSize, defaultRoomSize));
            CustomProceduralEditor.CreateWall(ProceduralManager.WallType.Front);
        }

        [MenuItem("Hotbar/Procedural/[Wall]Back ")]
        public static void CreateWallBack()
        {
            CustomProceduralEditor.SetRoomSize(new Vector3(defaultRoomSize, defaultRoomSize, defaultRoomSize));
            CustomProceduralEditor.CreateWall(ProceduralManager.WallType.Back);
        }

        [MenuItem("Hotbar/Procedural/[Room] ")]
        public static void CreateProceduralRoom()
        {
            CustomProceduralEditor.SetRoomSize(new Vector3(defaultRoomSize, defaultRoomSize, defaultRoomSize));
            CustomProceduralEditor.CreateProceduralRoom();
        }
    }

    public class CustomProceduralEditor : EditorWindow
    {
        [Header("Setting")]
        public Vector2 scroll;

        [Header("[Room Info]")]
        public static Vector3 roomSize;
        public static float thickness = 1.0f;
        public static ProceduralManager.TextureType textureType;
        public static bool enableLight = true;

        [Header("[Additional]")]
        public static int roomCount = 1;
        public static float offset = 1f;

        [Header("[Light")]
        public static ProceduralManager.LightType lightType;
        public static Vector2 lightCount;
        public static float lightOffset;
        public static Vector2 lightRandomize;

        [MenuItem("Hotbar/Procedural/[Generator] Custom Room")]
        public static void CreateCustomRoomWindow()
        {
             GetWindow<CustomProceduralEditor>("Custom Room");
        }

        public void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);

            #region Generate
            GUILayout.Label("[Hotbar Procedural Room Generator]");
            GUILayout.Space(10.0f);

            textureType = (ProceduralManager.TextureType)EditorGUILayout.EnumPopup("Material", textureType);
            GUILayout.Space(10.0f);

            roomSize = EditorGUILayout.Vector3Field("Size", roomSize);
            GUILayout.Space(10.0f);

            thickness = EditorGUILayout.FloatField("Thickness", thickness);
            GUILayout.Space(10.0f);

            #region Light
            enableLight = EditorGUILayout.BeginToggleGroup("Enable Light", enableLight);
            GUILayout.Space(10.0f);
            GUILayout.Label("[Light]");
            GUILayout.Space(10.0f);

            lightType = (ProceduralManager.LightType)EditorGUILayout.EnumPopup("Light Type", lightType);
            GUILayout.Space(10.0f);

            lightCount = EditorGUILayout.Vector2Field("Light Count", lightCount);
            GUILayout.Space(10.0f);

            lightOffset = EditorGUILayout.FloatField("Light Offset", lightOffset);
            GUILayout.Space(10.0f);
            EditorGUILayout.EndToggleGroup();
            #endregion

            if (GUILayout.Button("Generate Room"))
            {
                CreateProceduralRoom();
            }
            #endregion

            #region Additional
            GUILayout.Space(20.0f);

            GUILayout.Label("[Additional]");
            GUILayout.Space(10.0f);

            roomCount = EditorGUILayout.IntField("Room Count", roomCount);
            GUILayout.Space(10.0f);

            offset = EditorGUILayout.FloatField("Offset", offset);
            GUILayout.Space(10.0f);

            if (GUILayout.Button("[Left] Add Room"))
            {
                CreateProceduralRoomToLeft();
            }

            if (GUILayout.Button("[Right] Add Room"))
            {
                CreateProceduralRoomToRight();
            }

            if (GUILayout.Button("[Up] Add Room"))
            {
                CreateProceduralRoomToUp();
            }

            if (GUILayout.Button("[Down] Add Room"))
            {
                CreateProceduralRoomToDown();
            }

            if (GUILayout.Button("[Front] Add Room"))
            {
                CreateProceduralRoomToFront();
            }

            if (GUILayout.Button("[Back] Add Room"))
            {
                CreateProceduralRoomToBack();
            }
            #endregion

            EditorGUILayout.EndScrollView();
        }

        #region Wall

        #region Public
        public static void SetRoomSize(Vector3 boundSize)
        {
            roomSize = boundSize;
        }
        public static Room CreateProceduralRoom()
        {
            Debug.LogError("[Room Size] => " + roomSize);
            Debug.LogError("[Room Thickness] => " + thickness);

            var go = new GameObject();
            var room = go.AddComponent<Room>();

            var wall1 = CreateWall(ProceduralManager.WallType.Up);
            var wall2 = CreateWall(ProceduralManager.WallType.Down);
            var wall3 = CreateWall(ProceduralManager.WallType.Right);
            var wall4 = CreateWall(ProceduralManager.WallType.Left);
            var wall5 = CreateWall(ProceduralManager.WallType.Front);
            var wall6 = CreateWall(ProceduralManager.WallType.Back);

            go.name = "[Procedural] Room";
            wall1.transform.SetParent(go.transform);
            wall2.transform.SetParent(go.transform);
            wall3.transform.SetParent(go.transform);
            wall4.transform.SetParent(go.transform);
            wall5.transform.SetParent(go.transform);
            wall6.transform.SetParent(go.transform);

            wall1.ChangeMaterial(textureType);
            wall2.ChangeMaterial(textureType);
            wall3.ChangeMaterial(textureType);
            wall4.ChangeMaterial(textureType);
            wall5.ChangeMaterial(textureType);
            wall6.ChangeMaterial(textureType);

            room.AddWallInfo(wall1);
            room.AddWallInfo(wall2);
            room.AddWallInfo(wall3);
            room.AddWallInfo(wall4);
            room.AddWallInfo(wall5);
            room.AddWallInfo(wall6);

            if(enableLight)
            {
                CreateProceduralLight(room, wall1);
            }

            return room;
        }
        public static Wall CreateWall(ProceduralManager.WallType wallType)
        {

            System.Action<Wall> Initialize = (target) =>
            {
                target.Initialize(ProceduralManager.ProceduralType.Wall)
                    .Build();

                switch (wallType)
                {
                    case ProceduralManager.WallType.Up:
                        target.SetBoundSize(new Vector3(roomSize.x, thickness, roomSize.z));
                        target.transform.localPosition = Vector3.up * roomSize.y / 2;
                        break;
                    case ProceduralManager.WallType.Down:
                        target.SetBoundSize(new Vector3(roomSize.x, thickness, roomSize.z));
                        target.transform.localPosition = Vector3.down * roomSize.y / 2; 
                        break;
                    case ProceduralManager.WallType.Front:
                        target.SetBoundSize(new Vector3(roomSize.x, roomSize.y, thickness));
                        target.transform.localPosition = Vector3.forward * roomSize.z / 2; 
                        break;
                    case ProceduralManager.WallType.Back:
                        target.SetBoundSize(new Vector3(roomSize.x, roomSize.y, thickness));
                        target.transform.localPosition = Vector3.back * roomSize.z / 2;
                        break;
                    case ProceduralManager.WallType.Left:
                        target.SetBoundSize(new Vector3(thickness, roomSize.y, roomSize.z));
                        target.transform.localPosition = Vector3.left * roomSize.x / 2;
                        break;
                    case ProceduralManager.WallType.Right:
                        target.SetBoundSize(new Vector3(thickness, roomSize.y, roomSize.z));
                        target.transform.localPosition = Vector3.right * roomSize.x / 2;
                        break;
                }

                Debug.LogError($"Set rotation: {target.transform.localEulerAngles}");
                Debug.LogError($"Set scale: {target.transform.localScale}");
                Debug.LogError($"[Procedural] Prefab Load Success : {target.name}");
            };

            var path = "Assets/Menber_Folder/Hotbar/Prefabs/Wall.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                Debug.LogError($"[Procedural] Prefab is not exist in path : {path}");

            var go = PrefabUtility.InstantiatePrefab(prefab);
            if (go == null)
                Debug.LogError($"[Procedural] Target Instantiate Failed : {prefab.name}");

            var target = go.GetComponent<Wall>();

            Initialize(target);

            PrefabUtility.RecordPrefabInstancePropertyModifications(target.transform);
            Undo.RegisterCreatedObjectUndo(target, "Spawn Prefab");
            EditorUtility.SetDirty(target);
            Selection.activeGameObject = target.gameObject;

            return target;
        }
        #endregion

        #region Private
        private static List<Room> CreateProceduralRoomToLeft()
        {
            var defaultPos = Vector3.zero;
            List<Room> roomList = new List<Room>();

            for (int i = 1; i <= roomCount; i++)
            {
                var room = CreateProceduralRoom();
                room.transform.position += defaultPos + new Vector3((-roomSize.x * i) + offset, 0, 0);
                roomList.Add(room);
            }

            return roomList;
        }
        private static List<Room> CreateProceduralRoomToRight()
        {
            var defaultPos = Vector3.zero;
            List<Room> roomList = new List<Room>();

            for (int i = 1; i <= roomCount; i++)
            {
                var room = CreateProceduralRoom();
                room.transform.position += defaultPos + new Vector3((roomSize.x * i) + offset, 0, 0);
                roomList.Add(room);
            }

            return roomList;
        }
        private static List<Room> CreateProceduralRoomToUp()
        {
            var defaultPos = Vector3.zero;
            List<Room> roomList = new List<Room>();

            for (int i = 1; i <= roomCount; i++)
            {
                var room = CreateProceduralRoom();
                room.transform.position += defaultPos + new Vector3(0, (roomSize.y * i) + offset, 0);
                roomList.Add(room);
            }

            return roomList;
        }
        private static List<Room> CreateProceduralRoomToDown()
        {
            var defaultPos = Vector3.zero;
            List<Room> roomList = new List<Room>();

            for (int i = 1; i <= roomCount; i++)
            {
                var room = CreateProceduralRoom();
                room.transform.position += defaultPos + new Vector3(0, (-roomSize.y * i) + offset, 0);
                roomList.Add(room);
            }

            return roomList;
        }
        private static List<Room> CreateProceduralRoomToFront()
        {
            var defaultPos = Vector3.zero;
            List<Room> roomList = new List<Room>();

            for (int i = 1; i <= roomCount; i++)
            {
                var room = CreateProceduralRoom();
                room.transform.position += defaultPos + new Vector3(0, 0, (roomSize.z * i) + offset);
                roomList.Add(room);
            }

            return roomList;
        }
        private static List<Room> CreateProceduralRoomToBack()
        {
            var defaultPos = Vector3.zero;
            List<Room> roomList = new List<Room>();

            for (int i = 1; i <= roomCount; i++)
            {
                var room = CreateProceduralRoom();
                room.transform.position += defaultPos + new Vector3(0, 0, (-roomSize.z * i) + offset);
                roomList.Add(room);
            }

            return roomList;
        }

        #endregion
        #endregion

        #region Light

        public static List<Procedural.Light> CreateProceduralLight(Room room, Wall wall)
        {
            var go = new GameObject();
            var lightList = new List<Procedural.Light>();
            var startPoint = wall.transform.position - new Vector3(wall.GetBoundSize().x / 2, lightOffset, wall.GetBoundSize().z / 2);
            var offset = new Vector2(wall.GetBoundSize().x, wall.GetBoundSize().z) / lightCount;

            go.name = "[Light]";
            go.transform.SetParent(room.transform);

            for (int z = 0; z < lightCount.y; z++)
            {
                for (int x = 0; x < lightCount.x; x++)
                {
                    var light = CreateLight();
                    light.transform.position = startPoint + new Vector3(offset.x * x, 0, offset.y * z);
                    lightList.Add(light);
                }
            }

            foreach (var light in lightList)
            {
                light.transform.SetParent(go.transform);
            }


            return lightList;
        }

        public static Procedural.Light CreateLight()
        {

            System.Action<Procedural.Light> Initialize = (target) =>
            {
                target.Initialize(ProceduralManager.ProceduralType.Light)
                    .Build();

                Debug.LogError($"[Procedural] Prefab Load Success : {target.name}");
            };

            var path = "Assets/Menber_Folder/Hotbar/Prefabs/Light.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                Debug.LogError($"[Procedural] Prefab is not exist in path : {path}");

            var go = PrefabUtility.InstantiatePrefab(prefab);
            if (go == null)
                Debug.LogError($"[Procedural] Target Instantiate Failed : {prefab.name}");

            var target = go.GetComponent<Procedural.Light>();

            Initialize(target);

            PrefabUtility.RecordPrefabInstancePropertyModifications(target.transform);
            Undo.RegisterCreatedObjectUndo(target, "Spawn Prefab");
            EditorUtility.SetDirty(target);
            Selection.activeGameObject = target.gameObject;

            target.ChangeLight(lightType);
            return target;
        }

        #endregion
    }
}

