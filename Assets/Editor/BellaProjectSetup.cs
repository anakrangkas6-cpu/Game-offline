#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BellaVirtualLife.Profile;
using BellaVirtualLife.Life;
using BellaVirtualLife.CameraSystem;
using BellaVirtualLife.Face;
using BellaVirtualLife.UI;

public static class BellaProjectSetup
{
    [MenuItem("Bella/Create Starter Scene")]
    public static void CreateStarterScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var root = new GameObject("BellaWorld");

        var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "TemporaryGround";
        ground.transform.localScale = new Vector3(8, 1, 8);
        ground.transform.SetParent(root.transform);

        var character = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        character.name = "BellaCharacter_PLACEHOLDER";
        character.transform.position = new Vector3(0, 1, 0);
        character.transform.SetParent(root.transform);
        Object.DestroyImmediate(character.GetComponent<CapsuleCollider>());
        var characterController = character.AddComponent<CharacterController>();
        characterController.height = 2f;
        characterController.radius = 0.45f;
        characterController.center = new Vector3(0f, 1f, 0f);
        var motor = character.AddComponent<BellaVirtualLife.CameraSystem.BellaCharacterMotor>();

        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0, 3, -5);
        var cam = cameraObject.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Skybox;
        cameraObject.AddComponent<AudioListener>();
        var follow = cameraObject.AddComponent<ThirdPersonCamera>();
        follow.SetTarget(character.transform);

        var lightObject = new GameObject("Directional Light");
        lightObject.transform.rotation = Quaternion.Euler(50, -30, 0);
        lightObject.AddComponent<Light>().type = LightType.Directional;

        var profileObject = new GameObject("BellaSystems");
        var profile = profileObject.AddComponent<BellaProfile>();
        profileObject.AddComponent<LifeSimulation>();

        var eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();

        var canvasObject = new GameObject("BellaCanvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        // Left-side movement joystick.
        var joystickObject = new GameObject("MovementJoystick", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        joystickObject.transform.SetParent(canvasObject.transform, false);
        var joystickRect = joystickObject.GetComponent<RectTransform>();
        joystickRect.anchorMin = new Vector2(0f, 0f);
        joystickRect.anchorMax = new Vector2(0f, 0f);
        joystickRect.pivot = new Vector2(0.5f, 0.5f);
        joystickRect.anchoredPosition = new Vector2(125f, 125f);
        joystickRect.sizeDelta = new Vector2(150f, 150f);
        var joystickImage = joystickObject.GetComponent<Image>();
        joystickImage.color = new Color(1f, 1f, 1f, 0.22f);

        var handleObject = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        handleObject.transform.SetParent(joystickObject.transform, false);
        var handleRect = handleObject.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.5f, 0.5f);
        handleRect.anchorMax = new Vector2(0.5f, 0.5f);
        handleRect.sizeDelta = new Vector2(58f, 58f);
        handleObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.65f);
        var joystick = joystickObject.AddComponent<VirtualJoystick>();
        var joystickSerialized = new SerializedObject(joystick);
        joystickSerialized.FindProperty("handle").objectReferenceValue = handleRect;
        joystickSerialized.ApplyModifiedPropertiesWithoutUndo();
        motor.SetJoystick(joystick);

        var panel = new GameObject("HUDPanel");
        panel.transform.SetParent(canvasObject.transform, false);
        var textObject = new GameObject("NeedsText");
        textObject.transform.SetParent(panel.transform, false);
        var status = textObject.AddComponent<Text>();
        status.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        status.fontSize = 24;
        status.color = Color.white;
        status.text = "Bella Virtual Life";
        status.rectTransform.anchorMin = new Vector2(0, 1);
        status.rectTransform.anchorMax = new Vector2(1, 1);
        status.rectTransform.pivot = new Vector2(.5f, 1);
        status.rectTransform.anchoredPosition = new Vector2(0, -20);
        status.rectTransform.sizeDelta = new Vector2(-40, 120);

        var faceObject = new GameObject("FaceCustomization");
        faceObject.transform.SetParent(profileObject.transform);
        var face = faceObject.AddComponent<FaceCustomizationController>();

        var hud = canvasObject.AddComponent<BellaHUD>();
        var so = new SerializedObject(hud);
        so.FindProperty("profile").objectReferenceValue = profile;
        so.FindProperty("life").objectReferenceValue = profileObject.GetComponent<LifeSimulation>();
        so.FindProperty("face").objectReferenceValue = face;
        so.FindProperty("statusText").objectReferenceValue = status;
        so.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/BellaHome.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/BellaHome.unity", true) };
        Debug.Log("Bella starter scene created. Replace capsule and plane with licensed production assets.");
    }

    [InitializeOnLoadMethod]
    private static void EnsureSceneExists()
    {
        if (!System.IO.Directory.Exists("Assets/Scenes"))
            System.IO.Directory.CreateDirectory("Assets/Scenes");
    }
}
#endif
