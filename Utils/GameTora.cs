using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using UmatoMusume.Models;

namespace UmatoMusume.Utils
{
	public static class GameTora
	{
		private const string DEFAULT_SAVE_PATH = "Assets";
		private const int DELAY_TIME = 1000;
		private const string UMA_DATA_PATH = "Assets/uma_data.json";

		// Progress reporting constants
		private const int PROGRESS_INIT = 0;
		private const int PROGRESS_URL_GATHERING = 10;
		private const int PROGRESS_PROCESSING_WEIGHT = 80;
		private const int PROGRESS_SAVING = 90;
		private const int PROGRESS_COMPLETE = 100;
		private const int PROGRESS_TOTAL = 100;

		private static ChromeDriverService? _service;
		private static readonly ChromeOptions _chromeOptions = new ChromeOptions();

		static GameTora()
		{
			var userDataDir = Path.Combine(Directory.GetCurrentDirectory(), "Extras", "ChromeProfile");
			var extensionDir = Path.Combine(Directory.GetCurrentDirectory(), "Extras", "uBlockOrigin.crx");

			_chromeOptions.BinaryLocation = $"Extras/chrome-{Helper.GetWindowsVersion()}/chrome.exe";
			_chromeOptions.AddArgument("--headless=new");
			_chromeOptions.AddArgument("--window-size=1920,1080");
			_chromeOptions.AddArgument("--disable-gpu");
			_chromeOptions.AddArgument("--no-sandbox");
			_chromeOptions.AddArgument("--enable-features=AllowLegacyMV2Extensions");
			_chromeOptions.AddArgument("--disable-features=ExtensionManifestV2DeprecationWarnings");
			_chromeOptions.AddArgument("--disable-dev-shm-usage");
			_chromeOptions.AddArgument($"user-data-dir={userDataDir}");
			_chromeOptions.AddExtension(extensionDir);
		}

		private static ChromeDriverService CreateDriverService()
		{
			var service = ChromeDriverService.CreateDefaultService($"Extras/chrome-driver-{Helper.GetWindowsVersion()}/");
			service.HideCommandPromptWindow = true;
			service.SuppressInitialDiagnosticInformation = true;
			return service;
		}

		private static void SetupPage(IWebDriver _driver)
		{
			var accept = Helper.FindElementSafe(_driver, By.CssSelector("body > div#__next > div[class*='legal_cookie_banner_wrapper__'] > div > div[class*='legal_cookie_banner_selection__'] > div:last-child > button[class*='legal_cookie_banner_button__']"));
			accept?.Click();

			var option = Helper.FindElementSafe(_driver, By.CssSelector("div[class*='styles_header_settings__']"));
			option?.Click();

			var menuOption = Helper.FindElementSafe(_driver, By.CssSelector("div[class*='tooltips_tooltip__'] > div:last-child > div:last-child  > div:last-child > label"));
			menuOption?.Click();
		}

		public static void DisposeResources()
		{
			try
			{
				_service?.Dispose();
				_service = null;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error disposing ChromeDriverService: {ex.Message}");
			}
		}

		public static async Task DownloadUmaData(IProgress<ProgressGroup>? _progress = null, string _savePath = DEFAULT_SAVE_PATH + "/uma_data.json")
		{
			Cursor.Current = Cursors.WaitCursor;
			_service ??= CreateDriverService();

			using var driver = new ChromeDriver(_service, _chromeOptions);
			try
			{
				driver.Navigate().GoToUrl("https://gametora.com/umamusume/characters");

				_progress?.Report(new ProgressGroup(PROGRESS_INIT, PROGRESS_TOTAL, "Initializing browser in headless mode..."));

				var currentUmaList = Helper.LoadFromJson<Umamusume>(UMA_DATA_PATH);
				var elements = Helper.FindElementsSafe(driver, By.CssSelector("a[href^='/umamusume/characters']"));

				var urlList = new List<string>();
				var umaDataList = new List<Umamusume>();

				SetupPage(driver);

				await Task.Delay(DELAY_TIME * 2);

				_progress?.Report(new ProgressGroup(PROGRESS_URL_GATHERING, PROGRESS_TOTAL, "Gathering character URLs..."));

				foreach (var element in elements)
				{
					var divEl = Helper.FindElementSafe(element, By.CssSelector("div"));
					if (divEl == null) continue;

					var hiddenEL = divEl.GetAttribute("hidden");
					if (hiddenEL != null) continue;

					var href = element.GetAttribute("href");
					if (href != null && href.Contains("umamusume/characters/"))
					{
						urlList.Add(href);
					}
				}

				int diff = urlList.Count - currentUmaList.Count;

				urlList = urlList.Take(diff).ToList();

				int totalUrls = urlList.Count;
				int currentUrl = 0;

				foreach (var url in urlList)
				{
					currentUrl++;
					string characterName = url.Split('/').Last();

					int _progressPercentage = PROGRESS_URL_GATHERING + (currentUrl * PROGRESS_PROCESSING_WEIGHT / totalUrls);
					_progress?.Report(new ProgressGroup(_progressPercentage, PROGRESS_TOTAL, $"Processing character {currentUrl}/{totalUrls}: {characterName}..."));

					driver.Navigate().GoToUrl(url);
					await Task.Delay(DELAY_TIME);

					var nameElement = Helper.FindElementSafe(driver, By.CssSelector("div[class*='characters_infobox_top'] > div[class*='characters_infobox_character_name'] > a"));
					var name = nameElement?.GetAttribute("innerText")?.Replace("\n", "") ?? "";

					var objectives = Helper.FindElementsSafe(driver, By.CssSelector("div[class*='characters_objective_box'] > div[class*='characters_objective']"));
					var t = new List<UmaObjective>();
					foreach (var objective in objectives)
					{
						var objectiveName = Helper.FindElementSafe(objective, By.CssSelector("div[class*='characters_objective_text'] > div:nth-of-type(1)"));
						var turn = Helper.FindElementSafe(objective, By.CssSelector("div[class*='characters_objective_text'] > div:nth-of-type(2)"));
						var time = Helper.FindElementSafe(objective, By.CssSelector("div[class*='characters_objective_text'] > div:nth-of-type(3)"));
						var objectiveCondition = Helper.FindElementSafe(objective, By.CssSelector("div[class*='characters_objective_text'] > div:nth-of-type(4)"));

						t.Add(
							new UmaObjective(
								objectiveName?.GetAttribute("innerText") ?? "",
								turn?.GetAttribute("innerText") ?? "",
								time?.GetAttribute("innerText") ?? "",
								objectiveCondition?.GetAttribute("innerText") ?? ""
							)
						);
					}

					var eventBoxes = Helper.FindElementsSafe(driver, By.CssSelector("div[class*='eventhelper_elist']"));
					var events = new List<UmaEvent>();
					foreach (var eventBox in eventBoxes)
					{
						var eventElements = Helper.FindElementsSafe(eventBox, By.CssSelector("div[class*='compatibility_viewer_item']"));
						foreach (var eventElement in eventElements)
						{
							var eventName = eventElement.GetAttribute("innerText") ?? "";
							eventElement.Click();

							await Task.Delay(DELAY_TIME);

							var trs = Helper.FindElementsSafe(eventBox, By.CssSelector("table[class*='tooltips_ttable__'] > tbody > tr"));
							foreach (var tr in trs)
							{
								var eventOption = Helper.FindElementSafe(tr, By.CssSelector("td:nth-of-type(1)"));
								var eventValue = Helper.FindElementSafe(tr, By.CssSelector("td:nth-of-type(2)"));
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
								var noChoices = Helper.FindElementsSafe(eventBox, By.CssSelector("div[class*='tooltips_ttable_cell___'] > div"));
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

								if (!noChoices.Any())
								{
									var choice = Helper.FindElementSafe(eventBox, By.CssSelector("div[data-tippy-root] div[class*='tooltips_ttable_cell__']"));
									if (choice != null)
									{
										var eventOption = choice.GetAttribute("innerText") ?? "";
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
					}
					umaDataList.Add(new Umamusume(name, t, events));
				}

				umaDataList.AddRange(currentUmaList);

				_progress?.Report(new ProgressGroup(PROGRESS_SAVING, PROGRESS_TOTAL, "Saving data..."));
				Helper.SaveAsJson(umaDataList, _savePath);

				_progress?.Report(new ProgressGroup(PROGRESS_COMPLETE, PROGRESS_TOTAL, "Complete!"));
				Cursor.Current = Cursors.Default;
				MessageBox.Show($"Uma data saved to {_savePath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				Cursor.Current = Cursors.Default;
				MessageBox.Show($"Error occurred: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw;
			}
		}

		public static async Task DownloadSupportData(IProgress<ProgressGroup>? _progress = null, string _savePath = DEFAULT_SAVE_PATH + "/support_card.json")
		{
			Cursor.Current = Cursors.WaitCursor;
			_service ??= CreateDriverService();

			using var driver = new ChromeDriver(_service, _chromeOptions);
			try
			{
				driver.Navigate().GoToUrl("https://gametora.com/umamusume/supports");
				_progress?.Report(new ProgressGroup(PROGRESS_INIT, PROGRESS_TOTAL, "Initializing browser in headless mode..."));

				await Task.Delay(DELAY_TIME * 2);

				var elements = Helper.FindElementsSafe(driver, By.CssSelector("a[href^='/umamusume/supports']"));
				var urlList = new List<string>();
				var supportCardList = new List<SupportCard>();

				SetupPage(driver);

				await Task.Delay(DELAY_TIME * 2);

				_progress?.Report(new ProgressGroup(PROGRESS_URL_GATHERING, PROGRESS_TOTAL, "Gathering support card URLs..."));

				foreach (var element in elements)
				{
					var divEl = Helper.FindElementSafe(element, By.CssSelector("div"));
					if (divEl == null) continue;

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

					int _progressPercentage = PROGRESS_URL_GATHERING + (currentUrl * PROGRESS_PROCESSING_WEIGHT / totalUrls);
					_progress?.Report(new ProgressGroup(_progressPercentage, PROGRESS_TOTAL, $"Processing support {currentUrl}/{totalUrls}: {supportName}..."));

					driver.Navigate().GoToUrl(url);
					await Task.Delay(DELAY_TIME);

					var eventBoxes = Helper.FindElementsSafe(driver, By.CssSelector("div[class*='eventhelper_elist']"));
					foreach (var eventBox in eventBoxes)
					{
						var eventElements = Helper.FindElementsSafe(eventBox, By.CssSelector("div[class*='compatibility_viewer_item']"));
						foreach (var eventElement in eventElements)
						{
							var eventName = eventElement.GetAttribute("innerText") ?? "";

							eventElement.Click();

							await Task.Delay(DELAY_TIME);

							var trs = Helper.FindElementsSafe(eventBox, By.CssSelector("table[class*='tooltips_ttable__'] > tbody > tr"));
							foreach (var tr in trs)
							{
								var eventOption = Helper.FindElementSafe(tr, By.CssSelector("td:nth-of-type(1)"));
								var eventValue = Helper.FindElementSafe(tr, By.CssSelector("td:nth-of-type(2)"));
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
								var noChoices = Helper.FindElementsSafe(eventBox, By.CssSelector("div[class*='tooltips_ttable_cell___'] > div"));
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

								if (!noChoices.Any())
								{
									var choice = Helper.FindElementSafe(eventBox, By.CssSelector("div[data-tippy-root] div[class*='tooltips_ttable_cell__']"));
									if (choice != null)
									{
										var eventOption = choice.GetAttribute("innerText") ?? "";
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
				}

				_progress?.Report(new ProgressGroup(PROGRESS_SAVING, PROGRESS_TOTAL, "Saving data..."));
				Helper.SaveAsJson(supportCardList, _savePath);

				_progress?.Report(new ProgressGroup(PROGRESS_COMPLETE, PROGRESS_TOTAL, "Complete!"));
				Cursor.Current = Cursors.Default;
				MessageBox.Show($"Support cards saved to {_savePath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (WebDriverException ex)
			{
				Cursor.Current = Cursors.Default;
				MessageBox.Show($"WebDriver error occurred: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw;
			}
			catch (Exception ex)
			{
				Cursor.Current = Cursors.Default;
				MessageBox.Show($"Error occurred: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw;
			}
		}

		public static async Task DownloadAllCareerData(IProgress<ProgressGroup>? _progress = null, string _savePath = DEFAULT_SAVE_PATH + "/career.json")
		{
			Cursor.Current = Cursors.WaitCursor;
			_service ??= CreateDriverService();

			using (var driver = new ChromeDriver(_service, _chromeOptions))
			{
				try
				{
					driver.Navigate().GoToUrl("https://gametora.com/umamusume/training-event-helper");
					var careerList = new List<Career>();
					_progress?.Report(new ProgressGroup(PROGRESS_INIT, PROGRESS_TOTAL, "Initializing browser in headless mode..."));

					SetupPage(driver);
					await Task.Delay(DELAY_TIME * 2);

					IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
					js.ExecuteScript("localStorage.setItem('u-eh-d1','[\"Deck 1\",106101,1,30024,30024,30009,30024,30009,30008]')");
					driver.Navigate().Refresh();


					_progress?.Report(new ProgressGroup(PROGRESS_URL_GATHERING, PROGRESS_TOTAL, "Gathering career data..."));

					Helper.FindElementSafe(driver, By.Id("boxScenario"))?.Click();

					var careerElements = Helper.FindElementsSafe(driver, By.CssSelector("div[class*=tooltips_tooltip_striped] > div")).ToArray();

					int totalElement = careerElements.Length;
					int currentElement = 0;

					for (int i = 0; i < careerElements.Length; i++)
					{
						currentElement++;

						int _progressPercentage = PROGRESS_URL_GATHERING + (currentElement * PROGRESS_PROCESSING_WEIGHT / totalElement);
						_progress?.Report(new ProgressGroup(_progressPercentage, PROGRESS_TOTAL, $"Processing career {currentElement}/{totalElement}"));

						Helper.FindElementSafe(driver, By.Id("boxScenario"))?.Click();
						await Task.Delay(DELAY_TIME);

						var careerElement = Helper.FindElementSafe(driver, By.CssSelector($"div[class*=tooltips_tooltip_striped] > div:nth-of-type({i + 1})"));
						await Task.Delay(DELAY_TIME);
						careerElement?.Click();


						var careerButton = Helper.FindElementSafe(driver, By.CssSelector($"[id=\"{i + 1}\"][class*=\"filters_viewer_image_\"]"));
						await Task.Delay(DELAY_TIME);
						careerButton?.Click();


						var eventElements = Helper.FindElementsSafe(driver, By.CssSelector("div[class*=eventhelper_elist] > div[class*=compatibility_viewer_item]"));

						await Task.Delay(DELAY_TIME);
						foreach (var eventElement in eventElements)
						{
							var eventName = eventElement.GetAttribute("innerText") ?? "";
							eventElement.Click();

							await Task.Delay(DELAY_TIME);

							var trs = Helper.FindElementsSafe(driver, By.CssSelector("table[class*=tooltips_ttable__] > tbody > tr"));
							foreach (var tr in trs)
							{
								var eventOption = Helper.FindElementSafe(tr, By.CssSelector("td:nth-of-type(1)"));
								var eventValue = Helper.FindElementSafe(tr, By.CssSelector("td:nth-of-type(2)"));
								careerList.Add(
									new Career(
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
								var eventOption = Helper.FindElementSafe(driver, By.CssSelector("div[class*=tooltips_ttable_cell__]"));
								if (eventOption != null && eventName != null)
								{
									careerList.Add(
										new Career(
											eventName,
											new Dictionary<string, string>
											{
											{ "", eventOption.GetAttribute("innerText") ?? "" }
											}
										)
									);
								}
							}
						}

					}

					_progress?.Report(new ProgressGroup(PROGRESS_SAVING, PROGRESS_TOTAL, "Saving data..."));
					Helper.SaveAsJson(careerList, _savePath);

					_progress?.Report(new ProgressGroup(PROGRESS_COMPLETE, PROGRESS_TOTAL, "Complete!"));
					Cursor.Current = Cursors.Default;
					MessageBox.Show($"Successfully saved to {_savePath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch (WebDriverException ex)
				{
					Cursor.Current = Cursors.Default;
					MessageBox.Show($"WebDriver error occurred: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					throw;
				}
				catch (Exception ex)
				{
					Cursor.Current = Cursors.Default;
					MessageBox.Show($"Error occurred: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					throw;
				}
			}
		}

		public static async Task DownloadRacesData(IProgress<ProgressGroup>? _progress = null, string _savePath = DEFAULT_SAVE_PATH + "/races.json")
		{
			Cursor.Current = Cursors.WaitCursor;
			_service ??= CreateDriverService();

			using var driver = new ChromeDriver(_service, _chromeOptions);
			try
			{
				driver.Navigate().GoToUrl("https://gametora.com/umamusume/races");
				_progress?.Report(new ProgressGroup(PROGRESS_INIT, PROGRESS_TOTAL, "Initializing browser in headless mode..."));

				SetupPage(driver);

				await Task.Delay(DELAY_TIME * 2);
				_progress?.Report(new ProgressGroup(PROGRESS_URL_GATHERING, PROGRESS_TOTAL, "Gathering races data..."));

				var raceElements = Helper.FindElementsSafe(driver, By.CssSelector("div[class*=\"races_race_list\"] > div[class*=\"races_row\"]"));
				var totalElement = raceElements.Count;
				var currentElement = 0;

				var raceList = new List<Race>();
				foreach (var raceElement in raceElements)
				{
					currentElement++;
					var raceNameElement = Helper.FindElementSafe(raceElement, By.CssSelector("div[class*=\"races_name\"] > div[class*=\"races_item\"]"));
					var raceName = raceNameElement?.GetAttribute("innerText")?.Trim() ?? "";

					if (string.IsNullOrEmpty(raceName)) continue;

					int _progressPercentage = PROGRESS_URL_GATHERING + (currentElement * PROGRESS_PROCESSING_WEIGHT / totalElement);
					_progress?.Report(new ProgressGroup(_progressPercentage, PROGRESS_TOTAL, $"Processing race {currentElement}/{totalElement}: {raceName}..."));

					if (raceName == "Junior Make Debut" || raceName == "Junior Maiden Race")
					{
						raceList.Add(new Race(raceName, "Junior Year Pre-Debut", "Pre Debut", "Varies", "Varies", "Varies", "Varies", "Varies", "Varies"));
						continue;
					}


					var raceDateElement = Helper.FindElementSafe(raceElement, By.CssSelector("div[class*=\"races_date\"]"));

					if (raceDateElement == null) continue;

					var year = Helper.FindElementSafe(raceDateElement, By.CssSelector("div:nth-of-type(1)"))?.GetAttribute("innerText") ?? "";
					var month = Helper.FindElementSafe(raceDateElement, By.CssSelector("div:nth-of-type(2)"))?.GetAttribute("innerText") ?? "";

					if (string.IsNullOrEmpty(year) || string.IsNullOrEmpty(month)) continue;

					var yearText = "";
					var monthText = "";

					switch (year)
					{
						case "First Year":
							yearText = "Junior Year";
							break;
						case "Second Year":
							yearText = "Classic Year";
							break;
						case "Third Year":
							yearText = "Senior Year";
							break;
						default:
							yearText = year;
							break;
					}

					if (DateTime.TryParseExact(month, "MMMM d", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var result))
					{
						monthText = result.ToString("d MMM");
						monthText = monthText.Replace("1", "Early").Replace("2", "Late");
					}

					var dateText = $"{yearText} {monthText}";

					var distanceTypeElement = Helper.FindElementSafe(raceElement, By.CssSelector("div[class*=\"aces_desc_right\"] > div:nth-of-type(1)"));
					var distanceMeterElement = Helper.FindElementSafe(raceElement, By.CssSelector("div[class*=\"aces_desc_right\"] > div:nth-of-type(2)"));

					if (distanceTypeElement == null || distanceMeterElement == null) continue;

					var tabtext1 = Helper.FindElementSafe(distanceTypeElement, By.CssSelector("div[class*=\"races_tabtext\"]"))?.GetAttribute("innerText") ?? "";
					var tabtext2 = Helper.FindElementSafe(distanceMeterElement, By.CssSelector("div[class*=\"races_tabtext\"]"))?.GetAttribute("innerText") ?? "";

					var terrainText = distanceTypeElement.GetAttribute("innerText")?.Replace(tabtext1, "").Trim() ?? "";
					var distanceTypeText = distanceMeterElement.GetAttribute("innerText")?.Replace(tabtext2, "").Trim() ?? "";
					var distanceMeterText = tabtext2;

					var detailsButtonElement = Helper.FindElementSafe(raceElement, By.CssSelector("div[class*=\"races_ribbon\"] > div[class*=\"utils_linkcolor\"]"));
					detailsButtonElement?.Click();

					await Task.Delay(DELAY_TIME);

					var dialogElement = Helper.FindElementSafe(driver, By.CssSelector("div[role=\"dialog\"]"));
					if (dialogElement == null) continue;

					var gradeText = Helper.FindElementSafe(dialogElement, By.CssSelector("div[class*=\"races_det_item\"]:nth-of-type(8)"))?.GetAttribute("innerText") ?? "";

					if (int.TryParse(gradeText, out var check))
					{
						gradeText = Helper.FindElementSafe(dialogElement, By.CssSelector("div[class*=\"races_det_item\"]:nth-of-type(10)"))?.GetAttribute("innerText") ?? "";
					}


					var seasonText = Helper.FindElementSafe(dialogElement, By.CssSelector("div[class*=\"races_det_item\"]:nth-of-type(16)"))?.GetAttribute("innerText") ?? "";

					var racesScheduleItemElement = Helper.FindElementsSafe(dialogElement, By.CssSelector("div[class*=\"races_schedule_item\"]")).ToArray();

					var fansTotalItem = 2;
					var fanReqDivIndex = 0;
					var fanGainedDivIndex = 1;

					if (racesScheduleItemElement.Length < fansTotalItem) continue;

					var fansRequiredElement = racesScheduleItemElement[fanReqDivIndex];
					var fansGainedElement = racesScheduleItemElement[fanGainedDivIndex];

					var fansRequiredText = fansRequiredElement.GetAttribute("innerText")?.Replace("Fans required", "").Trim() ?? "";
					var fansGainedText = fansGainedElement.GetAttribute("innerText")?.Replace("Fans gained", "").Replace("See all", "").Trim() ?? "";

					raceList.Add(new Race
					(
						raceName,
						dateText,
						gradeText,
						terrainText,
						distanceTypeText,
						distanceMeterText,
						seasonText,
						fansRequiredText,
						fansGainedText
					));


					var closeButton = Helper.FindElementSafe(dialogElement, By.CssSelector("img"));
					closeButton?.Click();

					await Task.Delay(DELAY_TIME);
				}

				_progress?.Report(new ProgressGroup(PROGRESS_SAVING, PROGRESS_TOTAL, "Saving data..."));
				Helper.SaveAsJson(raceList, _savePath);

				_progress?.Report(new ProgressGroup(PROGRESS_COMPLETE, PROGRESS_TOTAL, "Complete!"));
				Cursor.Current = Cursors.Default;
				MessageBox.Show($"Successfully saved to {_savePath}", "Download Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (WebDriverException ex)
			{
				Cursor.Current = Cursors.Default;
				MessageBox.Show($"WebDriver error occurred: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw;
			}
			catch (Exception ex)
			{
				Cursor.Current = Cursors.Default;
				MessageBox.Show($"Error occurred: {ex.Message}", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				throw;
			}
		}
	}
}
