using UnityEngine;
using UnityEngine.UIElements;

namespace UGUICodeGenerator
{
    public class MarkArea
    {
        private const string AreaName = "inspector-custom-header-area";
        private NormalMarksDropdown _normalMarksDropdown=new NormalMarksDropdown();
        private SharedMarksDropdown _sharedMarksDropdown=new SharedMarksDropdown();

        public MarkArea()
        {
            _sharedMarksDropdown.RemoveWidgetMarkAction = _normalMarksDropdown.RemoveWidgetMark;
            _normalMarksDropdown.RemoveSharedMarkAction = _sharedMarksDropdown.RemoveSharedMark;
        }

        public VisualElement CreateArea()
        {
            var root = new VisualElement
            {
                name = AreaName
            };

            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems = Align.FlexStart;
            root.style.paddingLeft = 10;
            root.style.paddingRight = 4;
            root.style.paddingTop = 4;
            root.style.paddingBottom = 4;
            root.style.borderTopWidth = 1;
            root.style.borderBottomWidth = 1;
            root.style.borderTopColor = new Color(0.18f, 0.18f, 0.18f);
            root.style.borderBottomColor = new Color(0.18f, 0.18f, 0.18f);
            root.style.backgroundColor = new Color(0.22f, 0.22f, 0.22f);
            
            
            root.Add(_normalMarksDropdown.Create());
            root.Add(_sharedMarksDropdown.Create());

            
            return root;
        }
    }
}