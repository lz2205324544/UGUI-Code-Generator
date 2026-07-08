using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace UGUICodeGenerator
{
    public class CodeGenerator
    {
        private string _widgetTemplateBindings;
        private string _padTemplateBindings;
        private string _fieldDefinitionTemplate;
        private string _fieldAssignmentTemplate;
        private string _widgetFieldDefinitionTemplate;
        private string _widgetFieldAssignmentTemplate;

        private string _padTemplateLogic;
        private string _widgetTemplateLogic;


        public CodeGenerator()
        {
            _widgetTemplateBindings = CodeGenerateHelper.GetBindingTemplate(CodeConst.GUIWidgetTemplateBindingsFileName);
            _padTemplateBindings = CodeGenerateHelper.GetBindingTemplate(CodeConst.GUIPadTemplateBindingsFileName);
            _fieldDefinitionTemplate = CodeGenerateHelper.GetBindingTemplate(CodeConst.FieldDefinitionTemplateFileName);
            _fieldAssignmentTemplate = CodeGenerateHelper.GetBindingTemplate(CodeConst.FieldAssignmentTemplateFileName);
            _widgetFieldDefinitionTemplate = CodeGenerateHelper.GetBindingTemplate(CodeConst.WidgetFieldDefinitionTemplateFileName);
            _widgetFieldAssignmentTemplate = CodeGenerateHelper.GetBindingTemplate(CodeConst.WidgetFieldAssignmentTemplateFileName);
            
            
            _padTemplateLogic = CodeGenerateHelper.GetLogicTemplate(CodeConst.GUIPadTemplateLogicFileName);
            _widgetTemplateLogic = CodeGenerateHelper.GetLogicTemplate(CodeConst.GUIWidgetTemplateLogicFileName);
        }

        public string CreatePadCode(GameObject go, MarkInfo markInfo)
        {
            CodeGenerateHelper.RemoveMissingScriptGameObject(go);
            
            var widgetNodeInfo = WidgetNodeInfo.Create(go.transform, markInfo, false, null);
            var className = CodeGenerateHelper.ToPascalCase(go.name);
            widgetNodeInfo.WidgetFieldType = className;
            widgetNodeInfo.FileName = className;
            CreateBindingCode(widgetNodeInfo,
                className, false);
            
            CreateLogicCode(widgetNodeInfo,
                className, false);

            EditorUtility.SetDirty(go);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return className;
        }
        
        public string CreateWidgetCode(GameObject go, MarkInfo markInfo)
        {
            CodeGenerateHelper.RemoveMissingScriptGameObject(go);
            
            var widgetNodeInfo = WidgetNodeInfo.Create(go.transform, markInfo, true, null);
            var className = CodeGenerateHelper.ToPascalCase(go.name);
            widgetNodeInfo.WidgetFieldType = className;
            widgetNodeInfo.FileName = className;
            CreateBindingCode(widgetNodeInfo,
                className, true);
            
            CreateLogicCode(widgetNodeInfo,
                className, true);

            EditorUtility.SetDirty(go);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return className;
        }


        /// <summary>
        /// 生成逻辑代码
        /// </summary>
        /// <param name="widgetNodeInfo"></param>
        /// <param name="folderPath"></param>
        /// <param name="isWidget"></param>
        public void CreateLogicCode(WidgetNodeInfo widgetNodeInfo, string folderPath, bool isWidget = true)
        {
            var className = CodeGenerateHelper.ToPascalCase(widgetNodeInfo.WidgetFieldType);
            string widgetClass = null;
            if (isWidget)
            {
                widgetClass = _widgetTemplateLogic;
                widgetClass = widgetClass.Replace(CodeConst.WidgetBaseClassName,
                    UGUICodeGeneratorProjectSettings.instance.WidgetBaseClassName);
            }
            else
            {
                widgetClass = _padTemplateLogic;
                widgetClass = widgetClass.Replace(CodeConst.PadBaseClassName,
                    UGUICodeGeneratorProjectSettings.instance.PadBaseClassName);
            }
            
            widgetClass = widgetClass.Replace(CodeConst.NameSpace,
                UGUICodeGeneratorProjectSettings.instance.NamespaceName);
            widgetClass = widgetClass.Replace(CodeConst.ClassName, className);
            
            
            string bindDirectory = $"{UGUICodeGeneratorProjectSettings.instance.UILogicCodeFolder}/{folderPath}";

            if (widgetNodeInfo.IsShared)
            {
                bindDirectory = $"{UGUICodeGeneratorProjectSettings.instance.UILogicCodeSharedFolder}";
            }

            if (!System.IO.Directory.Exists(bindDirectory))
                System.IO.Directory.CreateDirectory(bindDirectory);

            string path = $"{bindDirectory}/{className}.cs";
            
            if (!System.IO.File.Exists(path))
                File.WriteAllText(path, widgetClass, Encoding.UTF8);
            
            
            foreach (var childNodeInfo in widgetNodeInfo.ChildNodes)
            {
                if (childNodeInfo is WidgetNodeInfo info)
                {
                    CreateLogicCode(info, folderPath);
                }
            }
            
        }

        /// <summary>
        /// 创建绑定代码
        /// </summary>
        /// <param name="widgetNodeInfo"></param>
        /// <param name="folderPath"></param>
        /// <param name="isWidget"></param>
        public void CreateBindingCode(WidgetNodeInfo widgetNodeInfo, string folderPath, bool isWidget = true)
        {
            var className = CodeGenerateHelper.ToPascalCase(widgetNodeInfo.WidgetFieldType);
            string widgetClass = null;
            if (isWidget)
            {
                widgetClass = _widgetTemplateBindings;
                widgetClass = widgetClass.Replace(CodeConst.WidgetBaseClassName,
                    UGUICodeGeneratorProjectSettings.instance.WidgetBaseClassName);
            }
            else
            {
                widgetClass = _padTemplateBindings;
                widgetClass = widgetClass.Replace(CodeConst.PadBaseClassName,
                    UGUICodeGeneratorProjectSettings.instance.PadBaseClassName);
            }

            widgetClass = widgetClass.Replace(CodeConst.NameSpace,
                UGUICodeGeneratorProjectSettings.instance.NamespaceName);
            widgetClass = widgetClass.Replace(CodeConst.ClassName, className);
            
            StringBuilder sb = new StringBuilder();
            foreach (var childNodeInfo in widgetNodeInfo.ChildNodes)
            {
                sb.AppendLine(_fieldDefinitionTemplate.Replace(CodeConst.FieldType, childNodeInfo.FieldType)
                    .Replace(CodeConst.FieldName, childNodeInfo.FieldName));
            }

            widgetClass = widgetClass.Replace(CodeConst.Fields, sb.ToString());

            sb.Clear();
            foreach (var childNodeInfo in widgetNodeInfo.ChildNodes)
            {
                if (childNodeInfo is WidgetNodeInfo info)
                {
                    sb.AppendLine(_widgetFieldDefinitionTemplate
                        .Replace(CodeConst.WidgetFieldType, info.WidgetFieldType)
                        .Replace(CodeConst.WidgetFieldName, info.WidgetFieldName));
                }
            }

            widgetClass = widgetClass.Replace(CodeConst.WidgetFields, sb.ToString());
            sb.Clear();
            var childNodes = widgetNodeInfo.ChildNodes;
            for (int i = 0; i < childNodes.Count; i++)
            {
                var childNodeInfo = childNodes[i];
                sb.AppendLine(_fieldAssignmentTemplate.Replace(CodeConst.FieldName, childNodeInfo.FieldName)
                    .Replace(CodeConst.FieldIndex, i.ToString()).Replace(CodeConst.FieldType, childNodeInfo.FieldType));
            }

            widgetClass = widgetClass.Replace(CodeConst.FieldsAssignment, sb.ToString());

            sb.Clear();
            foreach (var childNodeInfo in widgetNodeInfo.ChildNodes)
            {
                if (childNodeInfo is WidgetNodeInfo info)
                {
                    sb.AppendLine(
                        _widgetFieldAssignmentTemplate.Replace(CodeConst.WidgetFieldName, info.WidgetFieldName)
                            .Replace(CodeConst.FieldName, childNodeInfo.FieldName));
                }
            }

            widgetClass = widgetClass.Replace(CodeConst.WidgetAssignment, sb.ToString());

            string bindDirectory = $"{UGUICodeGeneratorProjectSettings.instance.UIBindingCodeFolder}/{folderPath}";

            if (widgetNodeInfo.IsShared)
            {
                bindDirectory = $"{UGUICodeGeneratorProjectSettings.instance.UIBindingCodeSharedFolder}";
            }

            if (!System.IO.Directory.Exists(bindDirectory))
                System.IO.Directory.CreateDirectory(bindDirectory);

            string path = $"{bindDirectory}/{widgetNodeInfo.FileName}.Binding.cs";

            File.WriteAllText(path, widgetClass, Encoding.UTF8);

            foreach (var childNodeInfo in widgetNodeInfo.ChildNodes)
            {
                if (childNodeInfo is WidgetNodeInfo info)
                {
                    CreateBindingCode(info, folderPath);
                }
            }
        }

       
    }
}