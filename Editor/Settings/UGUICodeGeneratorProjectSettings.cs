using System;
using System.Collections.Generic;
using System.Linq;
using Codice.CM.WorkspaceServer;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace UGUICodeGenerator
{
    [System.Serializable]
    public class MarkInfo
    {
        public MarkType markType;

        /// <summary>
        /// 标记
        /// </summary>
        public string displayName;

        /// <summary>
        /// 对应的类型
        /// </summary>
        public string type;

        public string fieldNamePrefix;
    }


    public enum MarkType
    {
        /// <summary>
        /// 普通组件
        /// </summary>
        Normal,


        /// <summary>
        /// 自定义的组件
        /// </summary>
        Widget,

        /// <summary>
        /// 自定义的共享组件
        /// </summary>
        SharedWidget,
    }

    [FilePath("ProjectSettings/UGUICodeGeneratorProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class UGUICodeGeneratorProjectSettings : ScriptableSingleton<UGUICodeGeneratorProjectSettings>
    {
        #region 默认值

        private const string DefaultUiPrefabFolder = "Assets/AssetRes/UI/Prefab";
        private const string DefaultUiCoreCodeFolder = "Assets/Scripts/UI/Core";
        private const string DefaultUiBindingCodeFolder = "Assets/Scripts/UI/View/Binding";
        private const string DefaultUiBindingCodeSharedFolder = "Assets/Scripts/UI/View/Binding/Shared";
        private const string DefaultUiLogicCodeFolder = "Assets/Scripts/UI/View/Logic";
        private const string DefaultUiLogicCodeSharedFolder = "Assets/Scripts/UI/View/Logic/Shared";

        private const string DefaultGuiPadTemplatePath = "Packages/com.lz.ugui-code-generator/Templates/";

        private const string DefaultNamespaceName = "Logic";
        private const string DefaultPadBaseClassName = "GUIPadBase";
        private const string DefaultWidgetBaseClassName = "GUIWidgetBase";

        private static List<MarkInfo> CreateDefaultNormalMarkInfos()
        {
            return new List<MarkInfo>()
            {
                new MarkInfo()
                {
                    markType = MarkType.Normal, displayName = "Go",
                    type = typeof(UnityEngine.GameObject).FullName, fieldNamePrefix = "_go"
                },
                new MarkInfo()
                {
                    markType = MarkType.Widget, displayName = "Widget",
                    type = typeof(UnityEngine.GameObject).FullName, fieldNamePrefix = "_widget"
                },
                new MarkInfo()
                {
                    markType = MarkType.Normal, displayName = "Image",
                    type = typeof(UnityEngine.UI.Image).FullName, fieldNamePrefix = "_img"
                },
                new MarkInfo()
                {
                    markType = MarkType.Normal, displayName = "Text", type = "TMPro.TextMeshProUGUI",
                    fieldNamePrefix = "_text"
                },
                new MarkInfo()
                {
                    markType = MarkType.Normal, displayName = "Toggle",
                    type = typeof(UnityEngine.UI.Toggle).FullName, fieldNamePrefix = "_toggle"
                },
                new MarkInfo()
                {
                    markType = MarkType.Normal, displayName = "Button",
                    type = typeof(UnityEngine.UI.Button).FullName, fieldNamePrefix = "_btn"
                },
            };
        }

        #endregion


        [SerializeField] private string uiPrefabFolder = DefaultUiPrefabFolder;
        [SerializeField] private string uiCoreCodeFolder = DefaultUiCoreCodeFolder;
        [SerializeField] private string uiBindingCodeFolder = DefaultUiBindingCodeFolder;
        [SerializeField] private string uiBindingCodeSharedFolder = DefaultUiBindingCodeSharedFolder;
        [SerializeField] private string uiLogicCodeFolder = DefaultUiLogicCodeFolder;
        [SerializeField] private string uiLogicCodeSharedFolder = DefaultUiLogicCodeSharedFolder;

        [SerializeField] private string guiPadTemplatePath = DefaultGuiPadTemplatePath;

        [SerializeField] private string namespaceName = DefaultNamespaceName;
        [SerializeField] private string padBaseClassName = DefaultPadBaseClassName;

        [SerializeField] private string widgetBaseClassName = DefaultWidgetBaseClassName;


        [SerializeField] public List<MarkInfo> normalMarkInfos = CreateDefaultNormalMarkInfos();

        [SerializeField] public List<MarkInfo> sharedMarkInfos;

        public List<string> GetNormalMarkChoices(GameObject[] gameObjects)
        {
            var intersectingComponentNames = gameObjects.GetIntersectingComponentNames();
            List<string> choices = new List<string>();
            choices.Add(CodeConst.GoMark);
            choices.Add(CodeConst.WidgetMark);
            for (var i = 0; i < normalMarkInfos.Count; i++)
            {
                var info = normalMarkInfos[i];
                if (intersectingComponentNames.Contains(info.type))
                    choices.Add(info.displayName);
            }

            return choices;
        }

        public List<string> GetShardMarkChoices(GameObject[] gameObjects)
        {
            List<string> choices = new List<string>();
            choices.Add(CodeConst.NoneMark);
            for (int i = 0; i < sharedMarkInfos.Count; i++)
            {
                var sharedMarkInfo = sharedMarkInfos[i];
                choices.Add(sharedMarkInfo.displayName);
            }

            return choices;
        }


        public string UIPrefabFolder => uiPrefabFolder;

        public string UICoreCodeFolder => uiCoreCodeFolder;

        public string UIBindingCodeFolder => uiBindingCodeFolder;

        public string UILogicCodeFolder => uiLogicCodeFolder;

        public string UIBindingCodeSharedFolder => uiBindingCodeSharedFolder;

        public string UILogicCodeSharedFolder => uiLogicCodeSharedFolder;

        public string NamespaceName => namespaceName;

        public string PadBaseClassName => padBaseClassName;

        public string WidgetBaseClassName => widgetBaseClassName;


        internal void Save()
        {
            Save(true);
        }

        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider()
        {
            return new SettingsProvider("Project/Code Generator/UGUI Code Generator", SettingsScope.Project)
            {
                label = "UGUI Code Generator",
                keywords = new[]
                {
                    "UGUI Code Generator",
                },
                titleBarGuiHandler = () =>
                {
                    if (GUILayout.Button("⋮", EditorStyles.iconButton))
                    {
                    }

                    if (GUILayout.Button("Help!", EditorStyles.miniButton))
                    {
                        Debug.Log("You are on your own.");
                    }
                },
                activateHandler = (_, rootElement) =>
                {
                    var settings = instance;
                    var serializedObject = new SerializedObject(settings);

                    var container = new VisualElement();
                    container.style.paddingLeft = 10;
                    container.style.paddingRight = 10;
                    container.style.paddingTop = 8;

                    container.Add(CreateHeader(settings));

                    var scrollView = new ScrollView();

                    #region 目录

                    var foldout = new Foldout();
                    foldout.style.flexGrow = 1;
                    foldout.text = "Generate Folder";


                    var uiPrefabFolderField = new PropertyField(
                        serializedObject.FindProperty(nameof(uiPrefabFolder)),
                        "UI Prefab Folder");
                    foldout.Add(uiPrefabFolderField);

                    var uiCoreCodeFolderField = new PropertyField(
                        serializedObject.FindProperty(nameof(uiCoreCodeFolder)),
                        "UI Core Code Folder");
                    foldout.Add(uiCoreCodeFolderField);

                    var uiBindingCodeFolderField = new PropertyField(
                        serializedObject.FindProperty(nameof(uiBindingCodeFolder)),
                        "Binding Code Folder");
                    foldout.Add(uiBindingCodeFolderField);

                    var uiLogicCodeFolderField = new PropertyField(
                        serializedObject.FindProperty(nameof(uiLogicCodeFolder)),
                        "Logic Code Folder");
                    foldout.Add(uiLogicCodeFolderField);

                    var uiBindingCodeSharedFolderField = new PropertyField(
                        serializedObject.FindProperty(nameof(uiBindingCodeSharedFolder)),
                        "Shared Binding Code Folder");
                    foldout.Add(uiBindingCodeSharedFolderField);

                    var uiLogicCodeSharedFolderField = new PropertyField(
                        serializedObject.FindProperty(nameof(uiLogicCodeSharedFolder)),
                        "Shared Logic Code Folder");
                    foldout.Add(uiLogicCodeSharedFolderField);

                    #endregion

                    var templateFoldout = new Foldout();
                    templateFoldout.text = "Template Config";

                    var namespaceNameField = new PropertyField(
                        serializedObject.FindProperty(nameof(namespaceName)),
                        "Namespace Name");
                    templateFoldout.Add(namespaceNameField);

                    var padBaseClassNameField = new PropertyField(
                        serializedObject.FindProperty(nameof(padBaseClassName)),
                        "Pad Base Class Name");
                    templateFoldout.Add(padBaseClassNameField);

                    var widgetBaseClassNameField = new PropertyField(
                        serializedObject.FindProperty(nameof(widgetBaseClassName)),
                        "Widget Base Class Name");
                    templateFoldout.Add(widgetBaseClassNameField);


                    var componentMappingsField = new PropertyField(
                        serializedObject.FindProperty(nameof(normalMarkInfos)),
                        "Normal Mark Infos");
                    componentMappingsField.style.paddingLeft = 3f;

                    var sharedComponentMappingsField = new PropertyField(
                        serializedObject.FindProperty(nameof(sharedMarkInfos)),
                        "Shared Mark Infos");
                    sharedComponentMappingsField.style.paddingLeft = 3f;

                    scrollView.Add(foldout);
                    scrollView.Add(templateFoldout);
                    container.Add(scrollView);


                    container.Add(componentMappingsField);
                    container.Add(sharedComponentMappingsField);
                    container.Bind(serializedObject);

                    container.RegisterCallback<SerializedPropertyChangeEvent>(_ => { settings.Save(); });

                    rootElement.Add(container);
                }
            };
        }

        /// <summary>
        /// 创建标题
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private static VisualElement CreateHeader(UGUICodeGeneratorProjectSettings settings)
        {
            var header = new VisualElement();
            header.style.flexDirection = FlexDirection.Row;
            header.style.alignItems = Align.Center;
            header.style.marginBottom = 8;
            header.style.minHeight = 24;
            var title = new Label("UGUI Code Generator");
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 19;
            title.style.flexGrow = 1;
            header.Add(title);
            var menuButton = CreateHeaderButton("_Menu", "More", () => ShowSettingsMenu(settings));
            header.Add(menuButton);
            return header;
        }


        private static Button CreateHeaderButton(string iconName, string tooltip, System.Action clickEvent)
        {
            var button = new Button(clickEvent)
            {
                tooltip = tooltip
            };
            var icon = EditorGUIUtility.IconContent(iconName).image as Texture2D;
            button.style.backgroundImage = new StyleBackground(icon);
            ;
            button.style.width = 18;
            button.style.height = 18;
            button.style.paddingLeft = 0;
            button.style.paddingRight = 0;
            button.style.paddingTop = 0;
            button.style.paddingBottom = 0;
            button.style.marginLeft = 4;
            button.style.marginRight = 0;
            button.style.borderLeftWidth = 0;
            button.style.borderRightWidth = 0;
            button.style.borderTopWidth = 0;
            button.style.borderBottomWidth = 0;
            button.style.backgroundColor = Color.clear;
            return button;
        }

        private static void ShowSettingsMenu(UGUICodeGeneratorProjectSettings settings)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset"), false, settings.ResetToDefaults);
            menu.ShowAsContext();
        }

        /// <summary>
        /// 重置按钮
        /// </summary>
        private void ResetToDefaults()
        {
            uiPrefabFolder = DefaultUiPrefabFolder;
            uiCoreCodeFolder = DefaultUiCoreCodeFolder;
            uiBindingCodeFolder = DefaultUiBindingCodeFolder;
            uiBindingCodeSharedFolder = DefaultUiBindingCodeSharedFolder;
            uiLogicCodeFolder = DefaultUiLogicCodeFolder;
            uiLogicCodeSharedFolder = DefaultUiLogicCodeSharedFolder;
            guiPadTemplatePath = DefaultGuiPadTemplatePath;
            namespaceName = DefaultNamespaceName;
            padBaseClassName = DefaultPadBaseClassName;
            widgetBaseClassName = DefaultWidgetBaseClassName;
            normalMarkInfos = CreateDefaultNormalMarkInfos();
        }

        public MarkInfo GetNormalMarkInfoByMarkName(string mark)
        {
            return normalMarkInfos.Find(i => i.displayName == mark);
        }

        public MarkInfo GetSharedMarkInfoByMarkName(string mark)
        {
            return sharedMarkInfos.Find(i => i.displayName == mark);
        }
    }
}