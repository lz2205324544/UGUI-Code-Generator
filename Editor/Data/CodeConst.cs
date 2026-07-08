using System.Text.RegularExpressions;

namespace UGUICodeGenerator
{
    public class CodeConst
    {
        /// <summary>
        /// 括号匹配正则(xxx)
        /// </summary>
        public static readonly Regex ParenthesesRegex = new Regex(@"\(([^()]*)\)",RegexOptions.Compiled);

        /// <summary>
        /// 尖括号匹配正则
        /// </summary>
        public static readonly Regex AngleBracketsRegex = new Regex(@"<([^<>]+)>",RegexOptions.Compiled);
        
        public const string NoneMark = "None";
        
        /// <summary>
        /// Go标志
        /// </summary>
        public const string GoMark = "Go";
        
        
        /// <summary>
        /// Widget标志
        /// </summary>
        public const string WidgetMark = "Widget";
        
        /// <summary>
        /// Editor目录
        /// </summary>
        public const string EditorFolder = "Editor";
        
        /// <summary>
        /// Templates目录
        /// </summary>
        public const string TemplatesFolder = "Templates";
        
        /// <summary>
        /// Binding目录
        /// </summary>
        public const string BindingFolder = "Binding";
        
        /// <summary>
        /// Logic目录
        /// </summary>
        public const string LogicFolder = "Logic";
        
        /// <summary>
        /// Core目录
        /// </summary>
        public const string CoreFolder = "Core";
        
        /// <summary>
        /// 通用的Widget前缀
        /// </summary>
        public const string SharedPrefix = "_shared";


        #region 模板中动态替换的占位符
        
        public const string NameSpace = "%Namespace%";
        public const string ClassName = "%ClassName%";
        public const string PadBaseClassName = "%PadBaseClassName%";
        public const string WidgetBaseClassName = "%WidgetBaseClassName%";
        public const string FieldType = "%FieldType%";
        public const string FieldName = "%FieldName%";        
        public const string Fields = "%Fields%";
        
        public const string WidgetFieldType = "%WidgetFieldType%";
        public const string WidgetFieldName = "%WidgetFieldName%";
        public const string WidgetFields = "%WidgetFields%";
        
        public const string FieldIndex = "%FieldIndex%";
        public const string FieldsAssignment = "%FieldsAssignment%";
        public const string WidgetAssignment = "%WidgetAssignment%";
        

        #endregion

        #region 核心文件名

        public const string PadBindingBaseFileName = "PadBindingBase";
        public const string ReferenceControllerFileName = "ReferenceController";
        public const string GUIPadBaseFileName = "GUIPadBase";
        public const string GUIWidgetBaseFileName = "GUIWidgetBase";

        #endregion

        #region 核心模板文件名

        public const string ReferenceControllerTemplateFileName = "ReferenceControllerTemplate.txt";
        public const string PadBindingBaseTemplateFileName = "PadBindingBaseTemplate.txt";
        public const string GUIPadBaseTemplateFileName = "GUIPadBaseTemplate.txt";
        public const string GUIWidgetBaseTemplateFileName = "GUIWidgetBaseTemplate.txt";

        #endregion

        #region 普通模板文件名

        public const string GUIWidgetTemplateBindingsFileName = "GUIWidgetTemplate.Bindings.txt";
        public const string GUIPadTemplateBindingsFileName = "GUIPadTemplate.Bindings.txt";
        public const string FieldDefinitionTemplateFileName = "FieldDefinitionTemplate.txt";
        public const string FieldAssignmentTemplateFileName = "FieldAssignmentTemplate.txt";
        public const string WidgetFieldDefinitionTemplateFileName = "WidgetFieldDefinitionTemplate.txt";
        public const string WidgetFieldAssignmentTemplateFileName = "WidgetFieldAssignmentTemplate.txt";
        
        public const string GUIWidgetTemplateLogicFileName = "GUIWidgetTemplate.Logic.txt";
        public const string GUIPadTemplateLogicFileName = "GUIPadTemplate.Logic.txt";

        #endregion
        
        /// <summary>
        /// ReferenceController类型名
        /// </summary>
        public const string ReferenceController = "ReferenceController";
        
        /// <summary>
        /// ReferenceController中字段名称
        /// </summary>
        public const string ReferenceControllerComponents = "components";



    }
}