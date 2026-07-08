using System.Collections.Generic;
using UnityEngine;

namespace UGUICodeGenerator
{
    public class NodeInfo
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public NodeInfo ParentNode;
        
        /// <summary>
        /// 父节点的名字（去除所有圆括号和尖括号）
        /// </summary>
        public string ParentNodeName;

        /// <summary>
        /// 父节点的名字（去除所有圆括号和尖括号） （首字母大写）
        /// </summary>
        public string PascalParentNodeName
        {
            get => CodeGenerateHelper.ToPascalCase(ParentNodeName);
        }

        /// <summary>
        /// 当前节点物体
        /// </summary>
        public GameObject Node;
        
        /// <summary>
        /// 经过解析名字后获取到的组件对象
        /// </summary>
        public Object Component;
        
        /// <summary>
        /// 节点物体去除所有圆括号和尖括号后的名字
        /// </summary>
        public string NodeName;
        
        /// <summary>
        /// 节点物体去除所有圆括号和尖括号后的名字(首字母大写)
        /// </summary>
        public string PascalName
        {
            get => CodeGenerateHelper.ToPascalCase(NodeName);
        }
        
        /// <summary>
        /// 字段类型
        /// </summary>
        public string FieldType;
        
        /// <summary>
        /// 字段名称
        /// </summary>
        public string FieldName;


        /// <summary>
        /// 设置父节点
        /// </summary>
        /// <param name="parentNode"></param>
        public void SetParentNode(NodeInfo parentNode)
        {
            ParentNode=parentNode;
            ParentNodeName =  parentNode?.NodeName;
        }

        public NodeInfo(Transform node)
        {
            Node = node.gameObject;
            NodeName = CodeConst.AngleBracketsRegex.Replace(CodeConst.ParenthesesRegex.Replace(node.name, string.Empty),
                string.Empty);
        }
        
        public static NodeInfo Create(Transform node, MarkInfo markInfo,NodeInfo parentNodeInfo)
        {
            var info = new NodeInfo(node);
            info.SetParentNode(parentNodeInfo);
            info.SetMarkComponent(markInfo);
            info.SetField(markInfo);
            return  info;
        }
        
        /// <summary>
        /// 设置解析标志后组件
        /// </summary>
        /// <param name="info"></param>
        protected  void SetMarkComponent(MarkInfo info)
        {
            var component = Node.GetComponent(info?.type??string.Empty);
            if (component == null)
            {
                Component = Node;
            }
            else
            {
                Component = component;
            }
        }

        protected virtual void SetField(MarkInfo info)
        {
            FieldType = Component.GetType().FullName;
            if (info != null)
            {
                string fieldNamePrefix = info?.fieldNamePrefix ?? string.Empty;
                var sameNameCount = CalSameNodeNameCount();
                string id = sameNameCount == 0 ? string.Empty : sameNameCount.ToString();
                if (info.markType == MarkType.Normal)
                {
                    FieldName = $"{fieldNamePrefix}{PascalName}{id}";
                }
                else
                {
                    FieldName = $"{fieldNamePrefix}Go{PascalName}{id}";
                }
            }
        }

        /// <summary>
        /// 计算相同名字出现的次数
        /// </summary>
        /// <returns></returns>
        protected int CalSameNodeNameCount()
        {
            if(ParentNode == null) return 0;
            if (ParentNode is WidgetNodeInfo widgetNodeInfo)
            {
                int count = 0;
                var childNodes = widgetNodeInfo.ChildNodes;
                for (var i = 0; i < childNodes.Count; i++)
                {
                    var childNode = childNodes[i];
                    
                    if (childNode.FieldType==FieldType && childNode.NodeName == NodeName)
                    {
                        count++;
                    }
                }
                return count;
            }
            return 0;
        }
    }

    
}