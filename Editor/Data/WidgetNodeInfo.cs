using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UGUICodeGenerator
{
    public class WidgetNodeInfo : NodeInfo
    {
        public bool IsShared;
        public string WidgetFieldType;
        public string WidgetFieldName;
        public string FileName;
        public List<NodeInfo> ChildNodes = new List<NodeInfo>();

        public WidgetNodeInfo(Transform node) : base(node)
        {
            var type = CodeGenerateHelper.FindType(CodeGenerateHelper.GetReferenceControllerFullName());
            var com = Node.GetComponent(type);
            if (com == null)
                com = Node.gameObject.AddComponent(type);
            
            Component = com;
            GetChildNodes(node, ref ChildNodes);
        }
        

        /// <summary>
        /// 获取子节点数据
        /// </summary>
        /// <param name="node"></param>
        /// <param name="childNodes"></param>
        private void GetChildNodes(Transform node, ref List<NodeInfo> childNodes)
        {
            for (int i = 0; i < node.childCount; i++)
            {
                var childNode = node.GetChild(i);
                var childNodeName = childNode.gameObject.name;
                bool isFindChild = true;
                var marks = RegexHelper.GetMarksFromMatchParentheses(childNodeName);
                if (marks.Count > 0)
                {
                    for (int j = 0; j < marks.Count; j++)
                    {
                        var mark = marks[j];
                        var markInfo = UGUICodeGeneratorProjectSettings.instance.GetNormalMarkInfoByMarkName(mark);
                        if (markInfo != null)
                        {
                            if (mark == CodeConst.WidgetMark)
                            {
                                var widgetNodeInfo = Create(childNode, markInfo,false,this);
                                childNodes.Add(widgetNodeInfo);
                                isFindChild = false;
                            }
                            else
                            {
                                var nodeInfo = NodeInfo.Create(childNode,markInfo,this);
                                childNodes.Add(nodeInfo);
                            }
                        }
                    }
                }

                var shareMark = RegexHelper.GetMarksFromMatchAngleBracketsRegex(childNodeName);
                if (!string.IsNullOrEmpty(shareMark))
                {
                    var markInfo = UGUICodeGeneratorProjectSettings.instance.GetSharedMarkInfoByMarkName(shareMark);
                    if (markInfo == null)
                    {
                        markInfo = new MarkInfo();
                        markInfo.displayName = shareMark;
                        markInfo.markType = MarkType.SharedWidget;
                        markInfo.fieldNamePrefix = CodeConst.SharedPrefix;
                    }
                    var widgetNodeInfo = Create(childNode, markInfo,true,this);
                    childNodes.Add(widgetNodeInfo);
                    isFindChild = false;
                }

                if (isFindChild)
                {
                    GetChildNodes(childNode, ref childNodes);
                }
            }
        }

        public static WidgetNodeInfo Create(Transform childNode, MarkInfo markInfo, bool isShared,NodeInfo parentNodeInfo)
        {
            var widgetNodeInfo = new WidgetNodeInfo(childNode);
            widgetNodeInfo.IsShared = isShared;
            widgetNodeInfo.SetParentNode(parentNodeInfo);
            widgetNodeInfo.SetMarkComponent(markInfo);
            widgetNodeInfo.SetField(markInfo);
            widgetNodeInfo.UpdateReferenceController();
            return widgetNodeInfo;
        }

        protected override void SetField(MarkInfo info)
        {
            base.SetField(info);
            if (info != null)
            {
                var sameNameCount = CalSameNodeNameCount();
                string id = sameNameCount == 0 ? string.Empty : sameNameCount.ToString();
                if (IsShared)
                {
                    WidgetFieldType = info.type??CodeGenerateHelper.ToPascalCase(info.displayName);
                    WidgetFieldName = $"{info.fieldNamePrefix}{PascalName}Widget{id}";
                    FileName = info.type??CodeGenerateHelper.ToPascalCase(info.displayName);
                }
                else
                {
                    WidgetFieldType = PascalName;
                    if (!string.IsNullOrEmpty(PascalParentNodeName))
                    {
                        WidgetFieldType =$"{PascalParentNodeName}_{WidgetFieldType}";
                    }
                    WidgetFieldName = $"{info.fieldNamePrefix}{PascalName}{id}";
                    FileName = WidgetFieldType.Replace("_",".");
                }
                
            }
        }

        private void UpdateReferenceController()
        {
            var type = CodeGenerateHelper.FindType(CodeGenerateHelper.GetReferenceControllerFullName());
            var referenceController = Node.GetComponent(type);
            if (referenceController == null)
                referenceController = Node.gameObject.AddComponent(type);

            FieldInfo field = referenceController.GetType().GetField(CodeConst.ReferenceControllerComponents);
            var components = (List<UnityEngine.Object>)field.GetValue(referenceController);
            
            components.Clear();

            foreach (var childNode in ChildNodes)
            {
                components.Add(childNode.Component);
            }

            UnityEditor.EditorUtility.SetDirty(Node);
        }



    }
}