using System.Reflection;
using I2.Loc;
using TDTN.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kirthos.ArtisanTDModLoader
{
    public class ModTextButton : TextButton
    {
        private TextMeshProUGUI text;
        private string originalText;

        public static void Replace(GameObject go)
        {
            TextButton origin = go.GetComponent<TextButton>();
            string normalFont = (LocalizedString)typeof(TextButton).GetField("normalFont", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(origin);
            string hoverFont = (LocalizedString)typeof(TextButton).GetField("hoverFont", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(origin);
            float normalFontSize = (float)typeof(TextButton).GetField("normalFontSize", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(origin);
            float hoverFontSize = (float)typeof(TextButton).GetField("hoverFontSize", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(origin);
            DestroyImmediate(origin);
            go.AddComponent<ModTextButton>().Init(normalFont, hoverFont, normalFontSize, hoverFontSize);
        }
        
        protected override void Start()
        {
            base.Start();
            text = GetComponent<TextMeshProUGUI>();
            originalText = text.text;
        }
        
        public void Init(string normalFont, string hoverFont, float normalFontSize, float hoverFontSize)
        {
            typeof(TextButton).GetField("normalFont", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, (LocalizedString) normalFont);
            typeof(TextButton).GetField("hoverFont", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, (LocalizedString) hoverFont);
            typeof(TextButton).GetField("normalFontSize", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, normalFontSize);
            typeof(TextButton).GetField("hoverFontSize", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, hoverFontSize);
            typeof(TextButton).GetField("localize", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, GetComponent<Localize>());
            typeof(TextButton).GetField("buttonText", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, GetComponent<TextMeshProUGUI>());
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            text.text = originalText;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            text.text = originalText;
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            text.text = originalText;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            text.text = originalText;
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            text.text = originalText;
        }
    }
}