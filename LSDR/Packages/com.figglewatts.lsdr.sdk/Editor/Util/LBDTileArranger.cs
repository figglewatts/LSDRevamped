using LSDR.SDK.Entities;
using UnityEditor;
using UnityEngine;

namespace LSDR.SDK.Editor.Util
{
    public class LBDTileArranger
    {
        [MenuItem("LSDR SDK/Arrange tiles in selected prefabs")]
        public static void ArrangeSelectedPrefabs()
        {
            foreach (GameObject selected in Selection.GetFiltered<GameObject>(SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(selected);
                GameObject root = PrefabUtility.LoadPrefabContents(path);

                var contributor = root.GetComponentInChildren<LBDGraphContributor>();
                if (contributor != null && contributor.LayoutType == LBDGraphContributor.LBDLayoutType.Horizontal)
                {
                    contributor.ArrangeTiles();
                    PrefabUtility.SaveAsPrefabAsset(root, path);
                    Debug.Log($"Arranged tiles of '{path}'");
                }

                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }
}