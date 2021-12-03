// <copyright file="ValidationSteps.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.App.Banners.Model;
using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System.Globalization;
using TechTalk.SpecFlow;

namespace DFC.App.Banners.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class ValidationSteps
    {
        public ValidationSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [Then(@"I am on the (.*) page")]
        public void ThenIAmOnThePage(string pageName)
        {
            this.Context.GetWebDriver().SwitchTo().Window(this.Context.GetWebDriver().WindowHandles[1]);

            switch (pageName.ToLower(CultureInfo.CurrentCulture))
            {
                case "Ipsos Mori sruvey page":
                   if (this.Context.GetWebDriver().Title.ToString() != "Ipsos MORI Surveys")
                    {
                        throw new NotFoundException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The expected page is not displayed");
                    }

                   break;

                default:
                   break;
            }
        }
    }
}