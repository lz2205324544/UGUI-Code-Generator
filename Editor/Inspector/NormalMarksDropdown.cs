using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UGUICodeGenerator
{
    public class NormalMarksDropdown
    {
        public MaskField maskField;
        public Action RemoveSharedMarkAction;
        

        public MaskField Create()
        {
            maskField = new MaskField();
            maskField.label = "Normal Mark";
            maskField.tooltip = "Code Generate Mark";
            maskField.style.width = 250;
            maskField.choices = UGUICodeGeneratorProjectSettings.instance.GetNormalMarkChoices(Selection.gameObjects);
            maskField.value = StringListToMask(maskField.choices, GetIntersectingMarksFormGameObjects());
            maskField.RegisterCallback<ChangeEvent<int>>((evt) =>
            {
                var value = evt.newValue;
                UpdateGameObjectsName(Selection.gameObjects, value, maskField.choices);
            });
            return maskField;
        }




        /// <summary>
        /// 更新传入GameObject数组的名字
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="selectedValue"></param>
        /// <param name="choices"></param>
        private  void UpdateGameObjectsName(GameObject[] objs, int selectedValue, List<string> choices)
        {
            if (objs == null || objs.Length == 0) return;
            for (int i = 0; i < objs.Length; i++)
            {
                UpdateGameObjectName(objs[i], selectedValue, choices);
            }
        }

        /// <summary>
        /// 更新GameObject名字
        /// </summary>
        /// <param name="go"></param>
        /// <param name="selectedValue"></param>
        /// <param name="choices"></param>
        private  void UpdateGameObjectName(GameObject go, int selectedValue, List<string> choices)
        {
            string name = go.name;
            var curMarks = CodeConst.ParenthesesRegex.Matches(name).Select(m => m.Groups[1].Value).ToHashSet();
            for (int i = 0; i < choices.Count; i++)
            {
                var mark = choices[i];
                if ((selectedValue & (1 << i)) != 0)
                {
                    if (!curMarks.Contains(mark))
                    {
                        name = $"({mark}){name}";
                    }
                    
                    if (mark == CodeConst.WidgetMark) //如果设置的是Widget，那么就不会是共享的
                    {
                        RemoveSharedMarkAction?.Invoke();
                    }
                }
                else
                {
                    name = name.Replace($"({mark})", "");
                }
            }

            go.name = name;
        }

        /// <summary>
        /// 将传入的标记转为int值
        /// </summary>
        /// <param name="choices"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        private  int StringListToMask(List<string> choices, HashSet<string> selected)
        {
            int mask = 0;
            foreach (var item in selected)
            {
                int index = choices.IndexOf(item);
                if (index >= 0)
                    mask |= 1 << index;
            }

            return mask;
        }


        /// <summary>
        /// 获取所有选中的GameObject的标记
        /// </summary>
        /// <returns></returns>
        private  HashSet<string> GetIntersectingMarksFormGameObjects()
        {
            var objs = Selection.gameObjects;
            if (objs == null || objs.Length == 0) return null;
            var intersectNames = RegexHelper.GetMarksFromMatchParentheses(objs[0].name).ToHashSet();
            foreach (var obj in objs)
            {
                intersectNames.IntersectWith(RegexHelper.GetMarksFromMatchParentheses(obj.name));
            }

            return intersectNames;
        }

        /// <summary>
        /// 去除Widget标记
        /// </summary>
        public void RemoveWidgetMark()
        {
            var choices = maskField.choices;
            for (int i = 0; i < choices.Count; i++)
            {
                var choice = choices[i];
                if (choice == CodeConst.WidgetMark)
                {
                    maskField.value &= ~(1 << i);
                }
            }
        }
    }
}