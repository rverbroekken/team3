using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;

// CardDataEditor.cs
[CustomEditor(typeof(LevelData))]
[CanEditMultipleObjects]
public class LevelDataEditor : Editor
{
    private ReorderableList abilityList;

    private SerializedProperty onPlayedProp;

    private struct AbilityCreationParams
    {
        public string Path;
    }

    public void OnEnable()
    {
        onPlayedProp = serializedObject.FindProperty("onPlayed");

        abilityList = new ReorderableList(
                serializedObject,
                onPlayedProp,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true);

        abilityList.drawHeaderCallback = (Rect rect) => {
            EditorGUI.LabelField(rect, "OnPlayed Abilities");
        };

        abilityList.onRemoveCallback = (ReorderableList l) => {
            var element = l.serializedProperty.GetArrayElementAtIndex(l.index);
            var obj = element.objectReferenceValue;

            AssetDatabase.RemoveObjectFromAsset(obj);

            DestroyImmediate(obj, true);
            l.serializedProperty.DeleteArrayElementAtIndex(l.index);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        };

        abilityList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
            SerializedProperty element = onPlayedProp.GetArrayElementAtIndex(index);

            rect.y += 2;
            rect.width -= 10;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (element.objectReferenceValue == null)
            {
                return;
            }
            string label = element.objectReferenceValue.name;
            EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

            // Convert this element's data to a SerializedObject so we can iterate
            // through each SerializedProperty and render a PropertyField.
            SerializedObject nestedObject = new SerializedObject(element.objectReferenceValue);

            // Loop over all properties and render them
            SerializedProperty prop = nestedObject.GetIterator();
            float y = rect.y;
            while (prop.NextVisible(true))
            {
                if (prop.name == "m_Script")
                {
                    continue;
                }

                rect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, prop);
            }

            nestedObject.ApplyModifiedProperties();

            // Mark edits for saving
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

        };

        abilityList.elementHeightCallback = (int index) => {
            float baseProp = EditorGUI.GetPropertyHeight(
                abilityList.serializedProperty.GetArrayElementAtIndex(index), true);

            float additionalProps = 0;
            SerializedProperty element = onPlayedProp.GetArrayElementAtIndex(index);
            if (element.objectReferenceValue != null)
            {
                SerializedObject ability = new SerializedObject(element.objectReferenceValue);
                SerializedProperty prop = ability.GetIterator();
                while (prop.NextVisible(true))
                {
                    // XXX: This logic stays in sync with loop in drawElementCallback.
                    if (prop.name == "m_Script")
                    {
                        continue;
                    }
                    additionalProps += EditorGUIUtility.singleLineHeight;
                }
            }

            float spacingBetweenElements = EditorGUIUtility.singleLineHeight / 2;

            return baseProp + spacingBetweenElements + additionalProps;
        };

        abilityList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) => {
            var menu = new GenericMenu();
            var guids = AssetDatabase.FindAssets("", new[] { "Assets/ItemsData" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var type = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                if (type.name == "LevelAbility")
                {
                    continue;
                }

                menu.AddItem(
                    new GUIContent(Path.GetFileNameWithoutExtension(path)),
                    false,
                    addClickHandler,
                    new AbilityCreationParams() { Path = path });
            }
            menu.ShowAsContext();
        };
    }

    private void addClickHandler(object dataObj)
    {
        // Make room in list
        var data = (AbilityCreationParams)dataObj;
        var index = abilityList.serializedProperty.arraySize;
        abilityList.serializedProperty.arraySize++;
        abilityList.index = index;
        var element = abilityList.serializedProperty.GetArrayElementAtIndex(index);

        // Create the new Ability
        var type = AssetDatabase.LoadAssetAtPath(data.Path, typeof(UnityEngine.Object));
        var newAbility = ScriptableObject.CreateInstance(type.name);
        newAbility.name = type.name;

        // Add it to CardData
        var cardData = (LevelData)target;
        AssetDatabase.AddObjectToAsset(newAbility, cardData);
        AssetDatabase.SaveAssets();
        element.objectReferenceValue = newAbility;
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        abilityList.DoLayoutList();

        if (GUILayout.Button("Delete All Abilities"))
        {
            var path = AssetDatabase.GetAssetPath(target);
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is LevelAbility)
                {
                    Object.DestroyImmediate(assets[i], true);
                }
            }

            // You needed to add this line here otherwise it keeps destroyed objects in the array.
            abilityList.serializedProperty.ClearArray();
            AssetDatabase.SaveAssets();
        }

        serializedObject.ApplyModifiedProperties();
    }
}