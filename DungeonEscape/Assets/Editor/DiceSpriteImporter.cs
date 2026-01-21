using UnityEditor;
using UnityEngine;

public class DiceSpriteImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // Auto-configure dice face textures as sprites
        if (assetPath.Contains("Resources/DiceFaces/"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Bilinear;
            importer.mipmapEnabled = false;
        }
    }
}

// Menu item to manually reimport dice faces
public static class DiceFaceReimporter
{
    [MenuItem("Tools/Reimport Dice Faces as Sprites")]
    public static void ReimportDiceFaces()
    {
        string[] diceFiles = new string[]
        {
            "Assets/Resources/DiceFaces/diceOne.png",
            "Assets/Resources/DiceFaces/diceTwo.png",
            "Assets/Resources/DiceFaces/diceThree.png",
            "Assets/Resources/DiceFaces/diceFour.png",
            "Assets/Resources/DiceFaces/diceFive.png",
            "Assets/Resources/DiceFaces/diceSix.png"
        };

        foreach (string path in diceFiles)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.filterMode = FilterMode.Bilinear;
                importer.mipmapEnabled = false;
                importer.SaveAndReimport();
                Debug.Log($"Reimported {path} as Sprite");
            }
            else
            {
                Debug.LogWarning($"Could not find importer for {path}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Dice face reimport complete!");
    }
}
