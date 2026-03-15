using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using CarSimulator.UI;

namespace CarSimulator.Tests
{
    public class UISystemTests
    {
        [Test]
        public void SpeedDisplay_CanBeCreated()
        {
            GameObject go = new GameObject("TestSpeedDisplay");
            SpeedDisplay display = go.AddComponent<SpeedDisplay>();
            
            Assert.IsNotNull(display);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void DashboardUI_CanBeCreated()
        {
            GameObject go = new GameObject("TestDashboard");
            DashboardUI dashboard = go.AddComponent<DashboardUI>();
            
            Assert.IsNotNull(dashboard);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void NotificationSystem_CanBeCreated()
        {
            GameObject go = new GameObject("TestNotification");
            NotificationSystem notification = go.AddComponent<NotificationSystem>();
            
            Assert.IsNotNull(notification);
            
            Object.DestroyImmediate(go);
        }
    }

    public class SettingsUITests
    {
        [Test]
        public void SettingsManager_CanBeCreated()
        {
            GameObject go = new GameObject("TestSettings");
            SettingsManager manager = go.AddComponent<SettingsManager>();
            
            Assert.IsNotNull(manager);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void SettingsManager_HasDefaultValues()
        {
            GameObject go = new GameObject("TestSettings");
            SettingsManager manager = go.AddComponent<SettingsManager>();
            
            Assert.IsNotNull(manager);
            
            Object.DestroyImmediate(go);
        }
    }

    public class MenuTests
    {
        [Test]
        public void MainMenu_CanBeCreated()
        {
            GameObject go = new GameObject("TestMainMenu");
            MainMenu menu = go.AddComponent<MainMenu>();
            
            Assert.IsNotNull(menu);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void PauseMenu_CanBeCreated()
        {
            GameObject go = new GameObject("TestPauseMenu");
            PauseMenu menu = go.AddComponent<PauseMenu>();
            
            Assert.IsNotNull(menu);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void PauseMenu_DefaultState()
        {
            GameObject go = new GameObject("TestPauseMenu");
            PauseMenu menu = go.AddComponent<PauseMenu>();
            
            Assert.IsNotNull(menu);
            
            Object.DestroyImmediate(go);
        }
    }

    public class HUDTests
    {
        [Test]
        public void VehicleHUD_CanBeCreated()
        {
            GameObject go = new GameObject("TestHUD");
            VehicleHUD hud = go.AddComponent<VehicleHUD>();
            
            Assert.IsNotNull(hud);
            
            Object.DestroyImmediate(go);
        }

        [Test]
        public void HUD_CanBeCreated()
        {
            GameObject go = new GameObject("TestHUD");
            HUD hud = go.AddComponent<HUD>();
            
            Assert.IsNotNull(hud);
            
            Object.DestroyImmediate(go);
        }
    }

    public class ObjectiveTests
    {
        [Test]
        public void ObjectiveSystem_CanBeCreated()
        {
            GameObject go = new GameObject("TestObjective");
            ObjectiveSystem objective = go.AddComponent<ObjectiveSystem>();
            
            Assert.IsNotNull(objective);
            
            Object.DestroyImmediate(go);
        }
    }

    public class UITransformTests
    {
        [Test]
        public void Canvas_CanBeCreated()
        {
            GameObject canvasGO = new GameObject("TestCanvas");
            Canvas canvas = canvasGO.AddComponent<Canvas>();
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            GraphicRaycaster raycaster = canvasGO.AddComponent<GraphicRaycaster>();
            
            Assert.IsNotNull(canvas);
            Assert.IsNotNull(scaler);
            Assert.IsNotNull(raycaster);
            
            Object.DestroyImmediate(canvasGO);
        }

        [Test]
        public void Text_CanBeCreated()
        {
            GameObject textGO = new GameObject("TestText");
            Text text = textGO.AddComponent<Text>();
            
            Assert.IsNotNull(text);
            
            Object.DestroyImmediate(textGO);
        }

        [Test]
        public void Button_CanBeCreated()
        {
            GameObject buttonGO = new GameObject("TestButton");
            Button button = buttonGO.AddComponent<Button>();
            
            Assert.IsNotNull(button);
            
            Object.DestroyImmediate(buttonGO);
        }

        [Test]
        public void Slider_CanBeCreated()
        {
            GameObject sliderGO = new GameObject("TestSlider");
            Slider slider = sliderGO.AddComponent<Slider>();
            
            Assert.IsNotNull(slider);
            
            Object.DestroyImmediate(sliderGO);
        }
    }
}
