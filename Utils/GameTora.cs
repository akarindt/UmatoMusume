﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
    public static class GameTora
    {
        private const string DEFAULT_SAVE_PATH = "Assets";
        private const int DELAY_TIME = 1000;

        // Progress reporting constants
        private const int PROGRESS_INIT = 0;
        private const int PROGRESS_URL_GATHERING = 10;
        private const int PROGRESS_PROCESSING_WEIGHT = 80;
        private const int PROGRESS_SAVING = 90;
        private const int PROGRESS_COMPLETE = 100;
        private const int PROGRESS_TOTAL = 100;

        public static async Task DownloadUmaData(IProgress<(int Current, int Total, string Message)>? progress = null, string savePath = DEFAULT_SAVE_PATH + "/uma_data.json")
        {
            Cursor.Current = Cursors.WaitCursor;
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--window-size=1920,1080");
            chromeOptions.AddArgument("--no-sandbox");

            IWebDriver driver = new ChromeDriver(chromeOptions);
            try
            {
                driver.Navigate().GoToUrl("https://gametora.com/umamusume/characters");

                progress?.Report((PROGRESS_INIT, PROGRESS_TOTAL, "Initializing browser in headless mode..."));

                var elements = driver.FindElements(By.CssSelector("body > div#__next > div > div > main > main > div:last-child > a"));
                var urlList = new List<string>();
                var umaDataList = new List<Umamusume>();

                var accept = Utils.FindElementSafe(driver, By.CssSelector("body > div#__next > div[class*=legal_cookie_banner_wrapper__] > div > div[class*=legal_cookie_banner_selection__] > div:last-child > button[class*=legal_cookie_banner_button__]"));
                accept?.Click();

                var option = Utils.FindElementSafe(driver, By.CssSelector("body > div#__next > div > div[class*=styles_page__] > header[id*=styles_page-header__] > div[class*=styles_header_settings__]"));
                option?.Click();

                var menuOption = Utils.FindElementSafe(driver, By.CssSelector("body > div[data-tippy-root] > div.tippy-box > div.tippy-content > div > div[class*=tooltips_tooltip__] > div:last-child > div:last-child  > div:last-child > label"));
                menuOption?.Click();

                await Task.Delay(DELAY_TIME * 2);

                progress?.Report((PROGRESS_URL_GATHERING, PROGRESS_TOTAL, "Gathering character URLs..."));

                foreach (var element in elements)
                {
                    var divEl = element.FindElement(By.CssSelector("div"));
                    var hiddenEL = divEl.GetAttribute("hidden");
                    if (hiddenEL != null) continue;

                    var href = element.GetAttribute("href");
                    if (href != null && href.Contains("umamusume/characters/"))
                    {
                        urlList.Add(href);
                    }
                }

                int totalUrls = urlList.Count;
                int currentUrl = 0;

                foreach (var url in urlList)
                {
                    currentUrl++;
                    string characterName = url.Split('/').Last();

                    int progressPercentage = PROGRESS_URL_GATHERING + (currentUrl * PROGRESS_PROCESSING_WEIGHT / totalUrls);
                    progress?.Report((progressPercentage, PROGRESS_TOTAL, $"Processing character {currentUrl}/{totalUrls}: {characterName}..."));

                    driver.Navigate().GoToUrl(url);
                    await Task.Delay(DELAY_TIME);
                    var nameElement = Utils.FindElementSafe(driver, By.CssSelector("body > div#__next > div > div > main > main > div:last-child > div > div[class*=characters_infobox_top] > div[class*=characters_infobox_character_name] > a"));
                    var name = nameElement?.GetAttribute("innerText")?.Replace("\n", "") ?? "";

                    var objectives = Utils.FindElementsSafe(driver, By.CssSelector("body > div#__next > div > div > main > main > div:last-child > div > div[class*=characters_objective_box] > div[class*=characters_objective]"));
                    var t = new List<UmaObjective>();
                    foreach (var objective in objectives)
                    {
                        var objectiveName = Utils.FindElementSafe(objective, By.CssSelector("div[class*=characters_objective_text] > div:nth-of-type(1)"));
                        var turn = Utils.FindElementSafe(objective, By.CssSelector("div[class*=characters_objective_text] > div:nth-of-type(2)"));
                        var time = Utils.FindElementSafe(objective, By.CssSelector("div[class*=characters_objective_text] > div:nth-of-type(3)"));
                        var objectiveCondition = Utils.FindElementSafe(objective, By.CssSelector("div[class*=characters_objective_text] > div:nth-of-type(4)"));

                        t.Add(
                            new UmaObjective(
                                objectiveName?.GetAttribute("innerText") ?? "",
                                turn?.GetAttribute("innerText") ?? "",
                                time?.GetAttribute("innerText") ?? "",
                                objectiveCondition?.GetAttribute("innerText") ?? ""
                            )
                        );
                    }

                    var eventBoxes = Utils.FindElementsSafe(driver, By.CssSelector("body > div#__next > div > div > main > main > div:last-child > div > div:last-child > div > div[class*=eventhelper_elist]"));
                    var events = new List<UmaEvent>();
                    foreach (var eventBox in eventBoxes)
                    {
                        var eventElements = Utils.FindElementsSafe(eventBox, By.CssSelector("div[class*=compatibility_viewer_item]"));
                        foreach (var eventElement in eventElements)
                        {
                            var eventName = eventElement.GetAttribute("innerText") ?? "";

                            eventElement.Click();

                            await Task.Delay(DELAY_TIME);

                            var trs = Utils.FindElementsSafe(eventBox, By.CssSelector("div[data-tippy-root] > div > div > div > div > div > table[class*=tooltips_ttable__] > tbody > tr"));
                            foreach (var tr in trs)
                            {
                                var eventOption = Utils.FindElementSafe(tr, By.CssSelector("td:nth-of-type(1)"));
                                var eventValue = Utils.FindElementSafe(tr, By.CssSelector("td:nth-of-type(2)"));
                                events.Add(
                                    new UmaEvent(
                                        eventName,
                                        new Dictionary<string, string>
                                        {
                                        { eventOption?.GetAttribute("innerText") ?? "", eventValue?.GetAttribute("innerText") ?? "" }
                                        }
                                    )
                                );
                            }

                            if (!trs.Any())
                            {
                                var noChoices = Utils.FindElementsSafe(eventBox, By.CssSelector("div[data-tippy-root] > div > div > div > div > div > div[class*=tooltips_ttable_cell___] > div"));
                                foreach (var noChoice in noChoices)
                                {
                                    var eventOption = noChoice.GetAttribute("innerText") ?? "";
                                    events.Add(
                                        new UmaEvent(
                                            eventName,
                                            new Dictionary<string, string>
                                            {
                                            { "", eventOption }
                                            }
                                        )
                                    );
                                }
                            }
                        }
                    }
                    umaDataList.Add(new Umamusume(name, t, events));
                }

                progress?.Report((PROGRESS_SAVING, PROGRESS_TOTAL, "Saving data..."));
                Utils.SaveAsJson(umaDataList, savePath);

                progress?.Report((PROGRESS_COMPLETE, PROGRESS_TOTAL, "Complete!"));
                Cursor.Current = Cursors.Default;
                MessageBox.Show($"Uma data saved to {savePath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                driver.Quit();
            }
        }

        public static async Task DownloadSupportData(IProgress<(int Current, int Total, string Message)>? progress = null, string savePath = DEFAULT_SAVE_PATH + "/support_card.json")
        {
            Cursor.Current = Cursors.WaitCursor;
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--window-size=1920,1080");
            chromeOptions.AddArgument("--no-sandbox");

            IWebDriver driver = new ChromeDriver(chromeOptions);

            try
            {
                driver.Navigate().GoToUrl("https://gametora.com/umamusume/supports");
                progress?.Report((PROGRESS_INIT, PROGRESS_TOTAL, "Initializing browser in headless mode..."));

                var elements = driver.FindElements(By.CssSelector("body > div#__next > div > div > main > main > div:last-child > a"));
                var urlList = new List<string>();
                var supportCardList = new List<SupportCard>();

                var accept = Utils.FindElementSafe(driver, By.CssSelector("body > div#__next > div[class*=legal_cookie_banner_wrapper__] > div > div[class*=legal_cookie_banner_selection__] > div:last-child > button[class*=legal_cookie_banner_button__]"));
                accept?.Click();

                var option = Utils.FindElementSafe(driver, By.CssSelector("body > div#__next > div > div[class*=styles_page__] > header[id*=styles_page-header__] > div[class*=styles_header_settings__]"));
                option?.Click();

                var menuOption = Utils.FindElementSafe(driver, By.CssSelector("body > div[data-tippy-root] > div.tippy-box > div.tippy-content > div > div[class*=tooltips_tooltip__] > div:last-child > div:last-child  > div:last-child > label"));
                menuOption?.Click();

                await Task.Delay(DELAY_TIME * 2);

                progress?.Report((PROGRESS_URL_GATHERING, PROGRESS_TOTAL, "Gathering support card URLs..."));

                foreach (var element in elements)
                {
                    var divEl = element.FindElement(By.CssSelector("div"));
                    var hiddenEL = divEl.GetAttribute("hidden");
                    if (hiddenEL != null) continue;

                    var href = element.GetAttribute("href");
                    if (href != null && href.Contains("umamusume/supports/"))
                    {
                        urlList.Add(href);
                    }
                }

                int totalUrls = urlList.Count;
                int currentUrl = 0;

                foreach (var url in urlList)
                {
                    currentUrl++;
                    string supportName = url.Split('/').Last();

                    int progressPercentage = PROGRESS_URL_GATHERING + (currentUrl * PROGRESS_PROCESSING_WEIGHT / totalUrls);
                    progress?.Report((progressPercentage, PROGRESS_TOTAL, $"Processing support {currentUrl}/{totalUrls}: {supportName}..."));

                    driver.Navigate().GoToUrl(url);
                    await Task.Delay(DELAY_TIME);

                    var eventBoxes = Utils.FindElementsSafe(driver, By.CssSelector("body > div#__next > div > div > main > main > div:last-child > div > div:last-child > div > div[class*=eventhelper_elist]"));
                    foreach (var eventBox in eventBoxes)
                    {
                        var eventElements = Utils.FindElementsSafe(eventBox, By.CssSelector("div[class*=compatibility_viewer_item]"));
                        foreach (var eventElement in eventElements)
                        {
                            var eventName = eventElement.GetAttribute("innerText") ?? "";

                            eventElement.Click();

                            await Task.Delay(DELAY_TIME);

                            var trs = Utils.FindElementsSafe(eventBox, By.CssSelector("div[data-tippy-root] > div > div > div > div > div > table[class*=tooltips_ttable__] > tbody > tr"));
                            foreach (var tr in trs)
                            {
                                var eventOption = Utils.FindElementSafe(tr, By.CssSelector("td:nth-of-type(1)"));
                                var eventValue = Utils.FindElementSafe(tr, By.CssSelector("td:nth-of-type(2)"));
                                supportCardList.Add(
                                    new SupportCard(
                                        eventName,
                                        new Dictionary<string, string>
                                        {
                                            { eventOption?.GetAttribute("innerText") ?? "", eventValue?.GetAttribute("innerText") ?? "" }
                                        }
                                    )
                                );
                            }

                            if (!trs.Any())
                            {
                                var noChoices = Utils.FindElementsSafe(eventBox, By.CssSelector("div[data-tippy-root] > div > div > div > div > div > div[class*=tooltips_ttable_cell___] > div"));
                                foreach (var noChoice in noChoices)
                                {
                                    var eventOption = noChoice.GetAttribute("innerText") ?? "";
                                    supportCardList.Add(
                                        new SupportCard(
                                            eventName,
                                            new Dictionary<string, string>
                                            {
                                                { "", eventOption }
                                            }
                                        )
                                    );
                                }
                            }
                        }
                    }
                }

                progress?.Report((PROGRESS_SAVING, PROGRESS_TOTAL, "Saving data..."));
                Utils.SaveAsJson(supportCardList, savePath);

                progress?.Report((PROGRESS_COMPLETE, PROGRESS_TOTAL, "Complete!"));
                Cursor.Current = Cursors.Default;
                MessageBox.Show($"Support cards saved to {savePath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}
