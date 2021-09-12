using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateCustomSpriteWindow : EditorWindow {

    private static Color Transparent = new Color(0, 0, 0, 0);

    private enum TextureSize {
        m_32 = 32,
        m_64 = 64,
        m_128 = 128,
        m_256 = 256,
        m_512 = 512,
        m_1024 = 1024,
        m_2048 = 2048,
        m_4096 = 4096,
        m_8192 = 8192
    }

    private TextureSize textureSize = TextureSize.m_512;
    private Color overrideColor = Color.white;
    private int outlineWidth = 2;
    private int cornerRadius = 32;

    [MenuItem("Custom/Create Custom Sprites")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(CreateCustomSpriteWindow), false, "Create Custom Sprites");
    }

    private void OnGUI() {
        GUILayout.Label("General Settings", EditorStyles.boldLabel);
        textureSize = (TextureSize)EditorGUILayout.EnumPopup("Texture Size", textureSize);
        overrideColor = EditorGUILayout.ColorField("Sprite Color", overrideColor);

        GUILayout.Space(10);

        GUILayout.Label("Outlined Circle", EditorStyles.boldLabel);
        outlineWidth = EditorGUILayout.IntSlider("Outline Width", outlineWidth, 0, (int)textureSize / 2);
        if (GUILayout.Button("Generate")) {
            GenerateOutlinedCircle((int)textureSize, outlineWidth, overrideColor);
        }

        GUILayout.Space(10);

        GUILayout.Label("Rounded Rectangle", EditorStyles.boldLabel);
        cornerRadius = EditorGUILayout.IntSlider("Corner Radius", cornerRadius, 0, (int)textureSize / 2);
        if (GUILayout.Button("Generate")) {
            GenerateOutlinedRectangle((int)textureSize, (int)textureSize, cornerRadius, outlineWidth, overrideColor);
        }

    }

    private static void GenerateOutlinedCircle(int textureSize, int stroke, Color color) {
        Texture2D texture = CreateOutlinedCircleTexture(textureSize, stroke, color);
        string createdFile = WriteDataToCurrentFolderWithName(texture.EncodeToPNG(), "OutlinedCircle_Width" + stroke, ".png");
        ProcessSpriteImportData(createdFile, Vector4.zero);
        AssetDatabase.Refresh();
    }

    private static Texture2D CreateOutlinedCircleTexture(int textureSize, int stroke, Color color) {
        int radius = textureSize / 2;
        Texture2D texture = CreateTextureWithFillColor(textureSize, textureSize, Transparent);
        DrawSolidCircleWithColor(radius, color, ref texture);
        DrawSolidCircleWithColor(radius - stroke, Transparent, ref texture);
        return texture;
    }

    private static Texture2D CreateTextureWithFillColor(int width, int height, Color color) {
        Texture2D texture = new Texture2D(width, height);
        for (int i = 0; i < texture.width; i++) {
            for (int j = 0; j < texture.height; j++) {
                texture.SetPixel(i, j, color);
            }
        }
        return texture;
    }

    private static void DrawSolidCircleWithColor(int radius, Color color, ref Texture2D texture) {
        int halfWidth = texture.width / 2;
        int halfHeight = texture.height / 2;
        int squareRadius = radius * radius;
        int distanceX, distanceY;
        for (int i = halfWidth - radius; i < halfWidth + radius; i++) {
            for (int j = halfHeight - radius; j < halfHeight + radius; j++) {
                distanceX = halfWidth - i;
                distanceY = halfHeight - j;
                if (distanceX * distanceX + distanceY * distanceY < squareRadius) {
                    texture.SetPixel(i, j, color);
                }
            }
        }
    }

    private static void GenerateOutlinedRectangle(int width, int height, int corner, int stroke, Color color) {
        Texture2D texture = CreateOutlinedRectangle(width, height, corner, stroke, color);
        string createdFile = WriteDataToCurrentFolderWithName(texture.EncodeToPNG(), "OutlinedRectangle_Corner" + corner, ".png");
        ProcessSpriteImportData(createdFile, Vector4.one * corner);
        AssetDatabase.Refresh();
    }

    private static Texture2D CreateOutlinedRectangle(int width, int height, int corner, int stroke, Color color) {
        Texture2D texture = CreateTextureWithFillColor(width, height, color);
        CutoutCornerFromRectangle(ref texture, corner, stroke);
        return texture;
    }

    private static void CutoutCornerFromRectangle(ref Texture2D texture, int corner, int stroke) {

        int squareRadius = corner * corner;
        int pivotX, pivotY;
        int distanceX, distanceY;

        // Left
        for (int i = 0; i < corner; i++) {

            // Bottom left
            pivotX = corner - 1;
            pivotY = corner - 1;
            for (int j = 0; j < corner; j++) {
                distanceX = pivotX - i;
                distanceY = pivotY - j;
                if (distanceX * distanceX + distanceY * distanceY > squareRadius) {
                    texture.SetPixel(i, j, Transparent);
                }
            }

            // Top left
            pivotX = corner - 1;
            pivotY = texture.height - corner - 1;
            for (int j = texture.height - corner - 1; j < texture.height; j++) {
                distanceX = pivotX - i;
                distanceY = pivotY - j;
                if (distanceX * distanceX + distanceY * distanceY > squareRadius) {
                    texture.SetPixel(i, j, Transparent);
                }
            }
        }

        // Right
        for (int i = texture.width - corner - 1; i < texture.width; i++) {

            // Bottom right
            pivotX = texture.width - corner - 1;
            pivotY = corner - 1;
            for (int j = 0; j < corner; j++) {
                distanceX = pivotX - i;
                distanceY = pivotY - j;
                if (distanceX * distanceX + distanceY * distanceY > squareRadius) {
                    texture.SetPixel(i, j, Transparent);
                }
            }

            // Top right
            pivotX = texture.width - corner - 1;
            pivotY = texture.height - corner - 1;
            for (int j = texture.height - corner - 1; j < texture.height; j++) {
                distanceX = pivotX - i;
                distanceY = pivotY - j;
                if (distanceX * distanceX + distanceY * distanceY > squareRadius) {
                    texture.SetPixel(i, j, Transparent);
                }
            }
        }

    }

    private static void GenerateSpriteBorder(Sprite sprite, int border) {
        Debug.Log(border);
        sprite.border.Set(border, border, border, border);
        Debug.Log(sprite.border);
        AssetDatabase.Refresh();
    }

    private static string WriteDataToCurrentFolderWithName(byte[] data, string name, string suffix) {
        var path = Path.Combine(GetSelectedProjectFolderPath(), name);
        if (File.Exists(path + suffix)) {
            int repeatCount = 1;
            while (File.Exists(path + " " + repeatCount + suffix)) {
                repeatCount++;
            }
            return WriteBytesAndProcessImport(path + " " + repeatCount + suffix, data);
        } else {
            return WriteBytesAndProcessImport(path + suffix, data);
        }
    }

    private static string WriteBytesAndProcessImport(string path, byte[] data) {
        File.WriteAllBytes(path, data);
        return path;
    }

    private static void ProcessSpriteImportData(string path, Vector4 border) {
        AssetDatabase.ImportAsset(path);
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteBorder = border;
        AssetDatabase.WriteImportSettingsIfDirty(path);
        Debug.Log("Sprite created at: " + path);
    }

    // https://forum.unity.com/threads/how-to-get-currently-selected-folder-for-putting-new-asset-into.81359/
    private static string GetSelectedProjectFolderPath() {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets)) {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path)) {
                path = Path.GetDirectoryName(path);
            }
            break;
        }
        return path;
    }

}
