using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UGUICodeGenerator
{
    public class SharedMarksDropdown
    {
        public Action RemoveWidgetMarkAction;
        public DropdownField dropdownField;



        public DropdownField Create()
        {
            dropdownField = new DropdownField();
            dropdownField.label = "SharedMark";
            dropdownField.labelElement.style.unityTextAlign = TextAnchor.MiddleRight;
            dropdownField.tooltip = "SharedMark";
            dropdownField.style.width = 250;
            dropdownField.choices = GetChoices();
            dropdownField.value = GetDefaultValue();
            dropdownField.RegisterCallback<ChangeEvent<string>>((evt) =>
            {
                var value = evt.newValue;
                if (value == "Add Shared Marks")
                {
                    SettingsService.OpenProjectSettings("Project/Code Generator/UGUI Code Generator");
                    dropdownField.SetValueWithoutNotify(evt.previousValue);
                }
                else
                {
                    UpdateGameObjectsName(Selection.gameObjects,value);
                    if (value != CodeConst.NoneMark)
                    {
                        RemoveWidgetMarkAction?.Invoke();
                    }

                }
            });
            return dropdownField;
        }


        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <returns></returns>
        private  string GetDefaultValue()
        {
            var intersectingMarksFormGameObjects = GetIntersectingMarksFormGameObjects();
            if (intersectingMarksFormGameObjects == null||intersectingMarksFormGameObjects.Count==0) return CodeConst.NoneMark;
            var first = intersectingMarksFormGameObjects.First();
            if(string.IsNullOrEmpty(first)) return CodeConst.NoneMark;
            return first;
        }

        /// <summary>
        /// 更新传入GameObject数组的名字
        /// </summary>
        /// <param name="objs"></param>
        /// <param name="sharedMark"></param>
        private  void UpdateGameObjectsName(GameObject[] objs, string sharedMark)
        {
            if(objs == null || objs.Length == 0) return;
            for (int i = 0; i < objs.Length; i++)
            {
                UpdateGameObjectName(objs[i],sharedMark);
            }
        }

        /// <summary>
        /// 更新GameObject名字
        /// </summary>
        /// <param name="go"></param>
        /// <param name="sharedMark"></param>
        private  void UpdateGameObjectName(GameObject go, string sharedMark)
        {
            string name = go.name;
            var match = CodeConst.AngleBracketsRegex.Match(name);
            if (match.Success)
            {
                var curMark = match.Groups[1].Value;
                if (sharedMark != CodeConst.NoneMark)
                {
                    name = name.Replace($"<{curMark}>", $"<{sharedMark}>");
                }
                else
                {
                    name = name.Replace($"<{curMark}>", string.Empty);
                }
                
            }
            else
            {
                if (sharedMark != CodeConst.NoneMark)
                {
                    name = $"<{sharedMark}>{name}";
                }
            }
            go.name = name;
        }

        private  List<string> GetChoices()
        {
            var shardMarkChoices = UGUICodeGeneratorProjectSettings.instance.GetShardMarkChoices(Selection.gameObjects);
            shardMarkChoices.Add("Add Shared Marks");
            return shardMarkChoices;
        }

        /// <summary>
        /// 获取所有选中的GameObject的标记
        /// </summary>
        /// <returns></returns>
        private  HashSet<string> GetIntersectingMarksFormGameObjects()
        {
            var objs = Selection.gameObjects;
            if (objs == null || objs.Length == 0) return null;
            HashSet<string> intersectNames=new HashSet<string>();
            intersectNames.Add(RegexHelper.GetMarksFromMatchAngleBracketsRegex(objs[0].name));
            foreach (var obj in objs)
            {
                intersectNames.IntersectWith(new List<string>(){RegexHelper.GetMarksFromMatchAngleBracketsRegex(obj.name)});
            }
            return intersectNames;
        }

        /// <summary>
        /// 去除SharedMark标记
        /// </summary>
        public void RemoveSharedMark()
        {
            dropdownField.value=CodeConst.NoneMark;
        }
    }
}