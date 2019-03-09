//  
// Copyright (c) 2018 Shane Harper
// Licensed under the MIT. See LICENSE file full license information.  
//  

using UnityEngine;
using UnityEngine.UI;

namespace Polyglot.Components
{
    /// <summary>
    ///     Localize a unity UI text component
    /// </summary>
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("Polyglot/Loc Text")]
    public class LocText : LocComponentKeyed
    {
        /// <summary>
        ///     The Text component affected by this component
        /// </summary>
        public Text Target
        {
            get { return _target; }
        }
        
        public override void RefreshLocalization()
        {
            string value;
            if (_target == null || string.IsNullOrEmpty(Key) || !LocManager.TryGetString(Key, out value)) return;

            // Apply case
            switch (_changeCase)
            {
                case TextCase.ToUpper:
                    value = value.ToUpper();
                    break;
                case TextCase.ToLower:
                    value = value.ToLower();
                    break;
            }

            // Set new text value
            _target.text = value;

            // Update to preferred size setting
            ResizeToPreferredSize(_usePreferredSize);
        }

        private void ResizeToPreferredSize(Axis axis)
        {
            // If resizing is disabled, return
            if (axis == Axis.None) return;
            _target.Rebuild(CanvasUpdate.Layout);

            // Get current size
            var rectTransform = (RectTransform) transform;
            var size = rectTransform.sizeDelta;

            // Apply change
            switch (axis)
            {
                case Axis.Horizontal:
                    size.x = _target.preferredWidth;
                    break;
                case Axis.Vertical:
                    size.y = _target.preferredHeight;
                    break;
                case Axis.Both:
                    size = new Vector2(_target.preferredWidth, _target.preferredHeight);
                    break;
            }

            if (rectTransform.sizeDelta != size)
                rectTransform.sizeDelta = size;
        }

        protected override void OnValidate()
        {
            // Get text component from attached game object if not set
            if (_target == null) _target = GetComponent<Text>();
            base.OnValidate();
        }

        #region Inspector

        [Tooltip("Text component to localize")]
        [SerializeField] private Text _target;
        [Space] 
        [Tooltip("Force upper or lower case in localized text?")]
        [SerializeField] private TextCase _changeCase;
        [Tooltip("Resize the text box to 'preferred size' when component is localized?")]
        [SerializeField] private Axis _usePreferredSize;

        #endregion
    }
}