using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Labyrinth.Collectibles;
using Labyrinth.Core;
using Labyrinth.Player;

namespace Labyrinth.UI
{
    public class UIFactory : IUIFactory
    {
        public void CreateHud()
        {
            var diamondCollector = ServiceLocator.Get<DiamondCollector>();
            var session = ServiceLocator.Get<GameSession>();
            var stamina = ServiceLocator.Get<PlayerStamina>();

            EnsureEventSystem();

            var canvasGO = new GameObject("HUD Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);

            Text diamondText = CreateAnchoredText(canvasGO.transform, "DiamondCounter", "Diamonds: 0 / 0",
                new Vector2(0f, 1f), new Vector2(20f, -20f), 32, TextAnchor.UpperLeft);

            Image staminaFill = CreateStaminaBar(canvasGO.transform);

            GameObject winPanel = CreatePanel(canvasGO.transform, "WinPanel", new Color(0f, 0f, 0f, 0.75f));
            CreateAnchoredText(winPanel.transform, "Title", "Победа!", new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), 48, TextAnchor.MiddleCenter);
            Text winCounterText = CreateAnchoredText(winPanel.transform, "Counter", "Diamonds collected: 0 / 0", new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), 28, TextAnchor.MiddleCenter);
            Button winRestartButton = CreateButton(winPanel.transform, "RestartButton", "Restart", new Vector2(0f, -80f));

            GameObject losePanel = CreatePanel(canvasGO.transform, "LosePanel", new Color(0.3f, 0f, 0f, 0.75f));
            CreateAnchoredText(losePanel.transform, "Title", "Поражение", new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), 48, TextAnchor.MiddleCenter);
            Button loseRestartButton = CreateButton(losePanel.transform, "RestartButton", "Restart", new Vector2(0f, -40f));

            var uiManager = canvasGO.AddComponent<UIManager>();
            uiManager.Initialize(diamondText, staminaFill, winPanel, winCounterText, losePanel, winRestartButton, loseRestartButton, diamondCollector, session, stamina);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        }

        private static Text CreateAnchoredText(Transform parent, string name, string content, Vector2 anchor, Vector2 anchoredPosition, int fontSize, TextAnchor alignment)
        {
            var go = new GameObject(name, typeof(Text));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = anchor;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(600f, 100f);

            var text = go.GetComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            return text;
        }

        private static Image CreateStaminaBar(Transform parent)
        {
            var backgroundGO = new GameObject("StaminaBarBackground", typeof(Image));
            backgroundGO.transform.SetParent(parent, false);

            var backgroundRect = backgroundGO.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0f, 1f);
            backgroundRect.anchorMax = new Vector2(0f, 1f);
            backgroundRect.pivot = new Vector2(0f, 1f);
            backgroundRect.anchoredPosition = new Vector2(20f, -70f);
            backgroundRect.sizeDelta = new Vector2(250f, 24f);
            backgroundGO.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.5f);

            var fillGO = new GameObject("StaminaBarFill", typeof(Image));
            fillGO.transform.SetParent(backgroundGO.transform, false);

            var fillRect = fillGO.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(2f, 2f);
            fillRect.offsetMax = new Vector2(-2f, -2f);

            var fillImage = fillGO.GetComponent<Image>();
            fillImage.sprite = CreateWhiteSprite();
            fillImage.color = new Color(1f, 0.8f, 0.2f);
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
            fillImage.fillAmount = 1f;

            return fillImage;
        }

        private static Sprite CreateWhiteSprite()
        {
            Texture2D texture = Texture2D.whiteTexture;
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        private static GameObject CreatePanel(Transform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(Image));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            go.GetComponent<Image>().color = color;
            go.SetActive(false);
            return go;
        }

        private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            var go = new GameObject(name, typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(200f, 60f);
            go.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.9f);

            var labelGO = new GameObject("Label", typeof(Text));
            labelGO.transform.SetParent(go.transform, false);
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var labelText = labelGO.GetComponent<Text>();
            labelText.text = label;
            labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            labelText.fontSize = 24;
            labelText.alignment = TextAnchor.MiddleCenter;
            labelText.color = Color.black;

            return go.GetComponent<Button>();
        }
    }
}
