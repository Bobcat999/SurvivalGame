using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObject/Item")]
public class Item : ScriptableObject
{
    [Header("Loading Data")]
    public string id;

    [Header("Only Gameplay")]
    public ItemType type;
    public int fuelStrength = 0;//the amount of fire it adds to a smelter

    [Header("Only UI")]
    [Multiline()]
    public string description;
    public bool stackable = true;

    [Header("Both")]
    public Sprite image;


    //hide in inspector
    [HideInInspector] public TileBase tile;
    [HideInInspector] public Vector3Int range = new Vector3Int(5, 5, 0);//default value
    [HideInInspector] public BreakType breakingType = BreakType.None;
    [HideInInspector] public ToolMaterial toolMaterial = ToolMaterial.None;
    [HideInInspector] public float breakingSpeed = 1f;

    #region Editor
#if UNITY_EDITOR

    [CustomEditor(typeof(Item))]
    public class ItemEditor:Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Item item = (Item)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Item Specific Options");

            switch(item.type)
            {
                case ItemType.Block:
                    EditorGUILayout.LabelField("Block Tile");
                    item.tile = EditorGUILayout.ObjectField(item.tile, typeof(TileBase), false) as TileBase;
                    break;
                case ItemType.Tool:
                    EditorGUILayout.LabelField("Breaking Type");
                    item.breakingType = (BreakType)EditorGUILayout.EnumPopup(item.breakingType);
                    EditorGUILayout.LabelField("Tool Material");
                    item.toolMaterial = (ToolMaterial)EditorGUILayout.EnumPopup(item.toolMaterial);
                    EditorGUILayout.LabelField("Breaking Speed");
                    item.breakingSpeed = EditorGUILayout.FloatField(item.breakingSpeed);
                    break;
                case ItemType.Weapon:
                    break;
                case ItemType.Item: 
                    break; 
                default:
                    break;
            }
        }

    }

#endif

    #endregion

}


public enum ItemType
{
    Block,
    Tool,
    Weapon,
    Item
}

public enum ToolMaterial//goes from least rare to most rare
{
    None,
    Wood,
    Stone,
    Iron,
    Copper,
    Gold
}