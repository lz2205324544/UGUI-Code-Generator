using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UGUICodeGenerator
{
    public static class GameObjectExtension
    {
        public static HashSet<string> GetIntersectingComponentNames(this GameObject[] objs)
        {
            if (objs == null || objs.Length == 0) return null;
            var intersectComponents = objs[0].GetComponents<Component>().Select(c=>c.GetType().FullName).ToHashSet();
            foreach (var obj in objs)
            {
                intersectComponents.IntersectWith(obj.GetComponents<Component>().Select(c=>c.GetType().FullName));
            }
            return intersectComponents;
        }
        
    }
}