using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


namespace UGUICodeGenerator
{
    public class CodeGeneratorMenu
    {
        
        [MenuItem("GameObject/UGUI Code Generator/Pad Code")]
        [MenuItem("Assets/UGUI Code Generator/Pad Code")]
        public static void CreatePadCodeByPrefab()
        {
            var go = Selection.activeGameObject;
            CodeGenerator generator = new CodeGenerator();
            generator.CreatePadCode(go,null);
        }   

        [MenuItem("GameObject/UGUI Code Generator/Widget Code")]
        [MenuItem("Assets/UGUI Code Generator/Widget Code")]
        public static void CreateWidgetCodeByPrefab()
        {
            var go = Selection.activeGameObject;
            CodeGenerator generator = new CodeGenerator();
            generator.CreateWidgetCode(go,null);
        }


        [MenuItem("Tools/UGUI Code Generator/Init Generate Core Code")]
        public static void CreateBaseCode()
        {
            var referenceController =CodeGenerateHelper.GetCoreTemplate(CodeConst.ReferenceControllerTemplateFileName);
            var padBindingBase = CodeGenerateHelper.GetCoreTemplate(CodeConst.PadBindingBaseTemplateFileName);
            var gUIPadBase = CodeGenerateHelper.GetCoreTemplate(CodeConst.GUIPadBaseTemplateFileName);
            var gUIWidgetBase = CodeGenerateHelper.GetCoreTemplate(CodeConst.GUIWidgetBaseTemplateFileName);

            referenceController = referenceController.Replace(CodeConst.NameSpace,
                UGUICodeGeneratorProjectSettings.instance.NamespaceName);
            CreateCoreCode(referenceController, CodeConst.ReferenceControllerFileName);

            
            padBindingBase = padBindingBase.Replace(CodeConst.NameSpace,
                UGUICodeGeneratorProjectSettings.instance.NamespaceName);
            CreateCoreCode(padBindingBase, CodeConst.PadBindingBaseFileName);
            
            gUIPadBase = gUIPadBase.Replace(CodeConst.NameSpace,
                UGUICodeGeneratorProjectSettings.instance.NamespaceName);
            CreateCoreCode(gUIPadBase, CodeConst.GUIPadBaseFileName);
            
            gUIWidgetBase = gUIWidgetBase.Replace(CodeConst.NameSpace,
                UGUICodeGeneratorProjectSettings.instance.NamespaceName);
            CreateCoreCode(gUIWidgetBase, CodeConst.GUIWidgetBaseFileName);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateCoreCode(string template, string fileName)
        {
            string bindDirectory = $"{UGUICodeGeneratorProjectSettings.instance.UICoreCodeFolder}";
            if (!System.IO.Directory.Exists(bindDirectory))
                System.IO.Directory.CreateDirectory(bindDirectory);
            string path = $"{bindDirectory}/{fileName}.cs";
            File.WriteAllText(path, template, Encoding.UTF8);
        }
    }
}