using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.PackageManager;

namespace UGUICodeGenerator
{
    public class CodeGenerateHelper
    {
        public static string ToPascalCase(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }

        /// <summary>
        /// 获取普通面板和Widget绑定模板
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetBindingTemplate(string fileName)
        {
            PackageInfo package =
                PackageInfo.FindForAssembly(typeof(CodeGeneratorMenu).Assembly);
            var path = Path.Combine(package.resolvedPath, CodeConst.EditorFolder, CodeConst.TemplatesFolder,
                CodeConst.BindingFolder, fileName);
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 获取核心模板（ReferenceController和PadBindingBase等等）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetCoreTemplate(string fileName)
        {
            PackageInfo package =
                PackageInfo.FindForAssembly(typeof(CodeGeneratorMenu).Assembly);
            var path = Path.Combine(package.resolvedPath, CodeConst.EditorFolder, CodeConst.TemplatesFolder,
                CodeConst.CoreFolder, fileName);
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 获取逻辑模板
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetLogicTemplate(string fileName)
        {
            PackageInfo package =
                PackageInfo.FindForAssembly(typeof(CodeGeneratorMenu).Assembly);
            var path = Path.Combine(package.resolvedPath, CodeConst.EditorFolder, CodeConst.TemplatesFolder,
                CodeConst.LogicFolder, fileName);
            return File.ReadAllText(path);
        }

        public static Type FindType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null)
                return type;

            return AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType(typeName))
                .FirstOrDefault(t => t != null);
        }

        /// <summary>
        /// ReferenceController全名
        /// </summary>
        /// <returns></returns>
        public static string GetReferenceControllerFullName()
        {
            return $"{UGUICodeGeneratorProjectSettings.instance.NamespaceName}.{CodeConst.ReferenceController}";
        }

        /// <summary>
        /// 移除丢失组件
        /// </summary>
        /// <param name="gameObject"></param>
        public static void RemoveMissingScriptGameObject(UnityEngine.GameObject gameObject)
        {
            UnityEditor.GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
            foreach (UnityEngine.Transform childT in gameObject.transform)
                RemoveMissingScriptGameObject(childT.gameObject);
        }
    }
}