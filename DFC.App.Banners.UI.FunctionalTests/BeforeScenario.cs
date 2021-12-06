// <copyright file="BeforeScenario.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.Banners.Model;
using DFC.TestAutomation.UI;
using DFC.TestAutomation.UI.Extension;
using DFC.TestAutomation.UI.Helper;
using DFC.TestAutomation.UI.Settings;
using DFC.TestAutomation.UI.Support;
using System;
using TechTalk.SpecFlow;

namespace DFC.App.Banners
{
    [Binding]
    public class BeforeScenario
    {
        public BeforeScenario(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException($"The scenario context is null. The {this.GetType().Name} class cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        [BeforeScenario(Order = 0)]
        public void SetObjectContext(ObjectContext objectContext)
        {
            this.Context.SetObjectContext(objectContext);
        }

        [BeforeScenario(Order = 1)]
        public void SetSettingsLibrary()
        {
            this.Context.SetSettingsLibrary(new SettingsLibrary<AppSettings>());
        }

        [BeforeScenario(Order = 2)]
        public void SetApplicationUrl()
        {
            string appBaseUrl = this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString();
        }

        [BeforeScenario(Order = 3)]
        public void ConfigureBrowserStack()
        {
            this.Context.GetSettingsLibrary<AppSettings>().BrowserStackSettings.Name = this.Context.ScenarioInfo.Title;
            this.Context.GetSettingsLibrary<AppSettings>().BrowserStackSettings.Build = "Banners";
        }

        [BeforeScenario(Order = 4)]
        public void SetupWebDriver()
        {
            var settingsLibrary = this.Context.GetSettingsLibrary<AppSettings>();
            var webDriver = new WebDriverSupport<AppSettings>(settingsLibrary).Create();
            webDriver.Manage().Window.Maximize();
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(settingsLibrary.TestExecutionSettings.TimeoutSettings.PageNavigation);
            this.Context.SetWebDriver(webDriver);
        }

        [BeforeScenario(Order = 5)]
        public void SetUpHelpers()
        {
            var helperLibrary = new HelperLibrary<AppSettings>(this.Context.GetWebDriver(), this.Context.GetSettingsLibrary<AppSettings>());
            this.Context.SetHelperLibrary(helperLibrary);
        }
    }
}
