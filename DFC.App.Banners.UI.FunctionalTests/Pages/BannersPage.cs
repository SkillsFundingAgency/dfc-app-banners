// <copyright file="BannersPage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.Banners.Model;
using DFC.TestAutomation.UI.Extension;
using System;
using TechTalk.SpecFlow;

namespace DFC.App.Banners.UI.FunctionalTests.Pages
{
    internal class BannersPage
    {
        public BannersPage(ScenarioContext context)
        {
            this.Context = context;

            if (this.Context == null)
            {
                throw new NullReferenceException("The scenario context is null. The contact us home page cannot be initialised.");
            }
        }

        private ScenarioContext Context { get; set; }

        public BannersPage NavigateToBannersPage()
        {
            this.Context.GetWebDriver().Url = this.Context.GetSettingsLibrary<AppSettings>().AppSettings.AppBaseUrl.ToString();
            return this;
        }
    }
}
