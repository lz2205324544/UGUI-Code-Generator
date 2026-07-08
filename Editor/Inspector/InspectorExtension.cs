using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UGUICodeGenerator
{
    [InitializeOnLoad]
    public class InspectorExtension
    {
        private const int CustomAreaIndex = 1;
        private const string AreaName = "inspector-custom-header-area";
        private const string EditorsListClassName = "unity-inspector-editors-list";
        
        private static readonly Type InspectorWindowType =
            typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        
        private static MarkArea markArea;
        

        static InspectorExtension()
        {
            Selection.selectionChanged -= RefreshInspectors;
            EditorApplication.update -= RefreshInspectors;
            EditorApplication.delayCall -= RefreshInspectors;
            Selection.selectionChanged += RefreshInspectors;
            EditorApplication.update += RefreshInspectors;
            EditorApplication.delayCall += RefreshInspectors;
        }

        private static void RefreshInspectors()
        {
            if (InspectorWindowType == null)
                return;

            var inspectorWindows = Resources.FindObjectsOfTypeAll(InspectorWindowType);
            for (int i = 0; i < inspectorWindows.Length; i++)
            {
                if (inspectorWindows[i] is EditorWindow inspectorWindow)
                    RefreshInspector(inspectorWindow.rootVisualElement);
            }
        }

        private static void RefreshInspector(VisualElement rootVisualElement)
        {
            if (rootVisualElement == null)
                return;
            if(!IsShowShowArea()) return;
            var editorListElement = rootVisualElement.Q<VisualElement>(null,EditorsListClassName);
            if (editorListElement == null) return;
            if(editorListElement.childCount<2) return;
            var customArea = editorListElement.Q<VisualElement>(AreaName);
            if (customArea == null)
            {
                if (markArea == null)
                {
                    markArea=new MarkArea();
                }
                customArea = markArea.CreateArea();
                editorListElement.Insert(CustomAreaIndex, customArea);
            }
        }



        private static bool IsShowShowArea()
        {
            var gameObjects = Selection.gameObjects;
            if (gameObjects == null || gameObjects.Length == 0) return false;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if(gameObjects[i].GetComponent<RectTransform>()==null)
                    return false;
            }
            return true;
        }
        
    }
}