// <copyright file="BasicSteps.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using DFC.TestAutomation.UI.Extension;
using OpenQA.Selenium;
using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace DFC.App.Banners.UI.FunctionalTests.StepDefinitions
{
    [Binding]
    internal class BasicSteps
    {
        public BasicSteps(ScenarioContext context)
        {
            this.Context = context;
        }

        private ScenarioContext Context { get; set; }

        [When(@"I click the (.*) link")]
        public void WhenIClickTheLink(string linkText)
        {
            var link = this.Context.GetWebDriver().FindElement(By.LinkText(linkText));

            if (!link.Displayed)
            {
                throw new OperationCanceledException($"Unable to perform the step: {this.Context.StepContext.StepInfo.Text}. The {linkText} link is not displayed");
            }

            link.Click();
        }
    }
}
