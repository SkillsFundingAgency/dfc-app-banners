// <copyright file="NavigationSteps.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.Banners.Model;
using DFC.App.Banners.UI.FunctionalTests.Pages;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Globalization;
using TechTalk.SpecFlow;

namespace DFC.App.Banners.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class NavigationSteps
    {
        public NavigationSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [Given(@"I am on the (.*) page")]
        public void GivenIAmOnThePage(string pageName)
        {
            var pageHeadingLocator = By.ClassName("govuk-heading-xl");

            switch (pageName.ToLower(CultureInfo.CurrentCulture))
            {
                case "home":
                    var bannerspage = new BannersPage(this.Context);
                    bannerspage.NavigateToBannersPage();
                    this.Context.GetHelperLibrary<AppSettings>().WebDriverWaitHelper.WaitForElementToContainText(pageHeadingLocator, "National Careers Service");
                    break;

                default:
                    throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The page name provided was not recognised.");
            }
        }
    }
}
