using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace UmatoMusume.Utils
{
	public class Game8Resolver_Config
	{
		// ============ CONFIG ============
		static readonly bool DEBUG = true;

		static readonly string CACHE_DIR = Path.Combine(AppContext.BaseDirectory, "cache");

		const string GAME8_EVENT_CHECKER_URL = "https://game8.co/games/Umamusume-Pretty-Derby/archives/539000";
		const string GAME8_ALL_EVENTS_URL = "https://game8.co/games/Umamusume-Pretty-Derby/archives/539612";

		static readonly string CACHE_EVENTS_DIR = Path.Combine(CACHE_DIR, "events");
		static readonly string CACHE_INDEX_PATH = Path.Combine(CACHE_DIR, "events_index.json");

		const int INDEX_TTL_HOURS = 24;
		const int EVENT_TTL_DAYS = 7;
		const double NETWORK_THROTTLE_SEC = 1.5;

		static readonly string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
											 "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";

		static DateTimeOffset _lastHttpTime = DateTimeOffset.MinValue;
		static readonly HttpClient httpClient = new HttpClient();


		public static void EnsureDirs()
		{
			Directory.CreateDirectory(CACHE_DIR);
			Directory.CreateDirectory(CACHE_EVENTS_DIR);
		}

		static string NowIso()
		{
			return DateTimeOffset.Now.ToString("O");
		}

		static string Sha1Hex(string s)
		{
			using var sha1 = SHA1.Create();
			var bytes = Encoding.UTF8.GetBytes(s);
			var hash = sha1.ComputeHash(bytes);
			var sb = new StringBuilder();
			foreach (var b in hash) sb.Append(b.ToString("x2"));
			return sb.ToString();
		}

		static string NormalizeQuery(string text)
		{
			if (string.IsNullOrEmpty(text)) return "";
			var t = text.Trim().ToLowerInvariant();
			t = t.Replace("’", "'").Replace("“", "\"").Replace("”", "\"");
			t = Regex.Replace(t, @"\s+", " ");
			return t;
		}

		static string Slugify(string s)
		{
			if (s == null) return "";
			var t = s.ToLowerInvariant().Replace("’", "'");
			t = Regex.Replace(t, @"[^a-z0-9]+", "-").Trim('-');
			return t;
		}

		static async Task<string> HttpGetAsync(string url, int timeoutSeconds = 20)
		{
			// throttle
			var now = DateTimeOffset.UtcNow;
			var elapsed = (now - _lastHttpTime).TotalSeconds;
			if (elapsed < NETWORK_THROTTLE_SEC)
			{
				var wait = TimeSpan.FromSeconds(NETWORK_THROTTLE_SEC - elapsed);
				await Task.Delay(wait);
			}

			try
			{
				using var req = new HttpRequestMessage(HttpMethod.Get, url);
				req.Headers.Add("User-Agent", USER_AGENT);
				req.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
				req.Headers.Add("Accept-Language", "en-US,en;q=0.9");
				req.Headers.Add("Cache-Control", "no-cache");
				req.Headers.Add("Pragma", "no-cache");
				req.Headers.Add("DNT", "1");
				req.Headers.Add("Upgrade-Insecure-Requests", "1");
				req.Headers.Add("Referer", GAME8_EVENT_CHECKER_URL);

				//httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
				var resp = await httpClient.SendAsync(req);
				_lastHttpTime = DateTimeOffset.UtcNow;
				if (resp.IsSuccessStatusCode)
				{
					var txt = await resp.Content.ReadAsStringAsync();
					if (!string.IsNullOrEmpty(txt)) return txt;
				}
				if (DEBUG) Console.WriteLine($"[DEBUG] HTTP {(int)resp.StatusCode} for {url}");
				return null;
			}
			catch (Exception e)
			{
				if (DEBUG) Console.WriteLine("[DEBUG] HTTP error for " + url + ": " + e.StackTrace);
				return null;
			}
		}

		// ============ Resolver ============
		public class Game8Resolver
		{
			public List<(string title, string url)> EventsIndex { get; private set; } = new();
			private bool _indexLoaded = false;

			private string IndexCachePath => CACHE_INDEX_PATH;

			private (DateTimeOffset fetched, JArray links)? LoadIndexCache()
			{
				if (!File.Exists(IndexCachePath)) return null;
				try
				{
					var text = File.ReadAllText(IndexCachePath, Encoding.UTF8);
					var jo = JObject.Parse(text);
					var fetched = DateTimeOffset.Parse(jo.Value<string>("fetched_at"), CultureInfo.InvariantCulture);
					if (DateTimeOffset.UtcNow - fetched <= TimeSpan.FromHours(INDEX_TTL_HOURS))
					{
						var links = (JArray)jo["links"];
						return (fetched, links);
					}
				}
				catch (Exception)
				{
					// ignore
				}
				return null;
			}

			private void SaveIndexCache(List<(string title, string url)> links)
			{
				try
				{
					var doc = new JObject
					{
						["fetched_at"] = NowIso(),
						["count"] = links.Count,
						["links"] = new JArray(links.Select(l => new JObject { ["title"] = l.title, ["url"] = l.url }))
					};
					File.WriteAllText(IndexCachePath, doc.ToString(Formatting.Indented), Encoding.UTF8);
				}
				catch (Exception e)
				{
					if (DEBUG) Console.WriteLine("[DEBUG] save index cache error: " + e.Message);
				}
			}

			private string EventCachePath(string url)
			{
				return Path.Combine(CACHE_EVENTS_DIR, Sha1Hex(url) + ".json");
			}

			private JObject LoadEventCache(string url)
			{
				var path = EventCachePath(url);
				if (!File.Exists(path)) return null;
				try
				{
					var text = File.ReadAllText(path, Encoding.UTF8);
					var jo = JObject.Parse(text);
					var fetched = DateTimeOffset.Parse(jo["source"].Value<string>("retrieved_at"), CultureInfo.InvariantCulture);
					var now = DateTimeOffset.Now;
					if (now - fetched <= TimeSpan.FromDays(EVENT_TTL_DAYS))
					{
						return jo;
					}
				}
				catch (Exception)
				{
					// ignore
				}
				return null;
			}

			private void SaveEventCache(string url, JObject evt)
			{
				try
				{
					File.WriteAllText(EventCachePath(url), evt.ToString(Formatting.Indented), Encoding.UTF8);
				}
				catch (Exception e)
				{
					if (DEBUG) Console.WriteLine("[DEBUG] save event cache error: " + e.Message);
				}
			}

			public async Task<bool> LoadIndexAsync()
			{
				// 1) cache
				var cached = LoadIndexCache();
				if (cached != null)
				{
					var cacheLinks = (JArray)cached.Value.links;
					EventsIndex = cacheLinks.Select(x => (x.Value<string>("title"), x.Value<string>("url"))).ToList();
					_indexLoaded = true;
					if (DEBUG) Console.WriteLine($"[DEBUG] loaded index cache: {EventsIndex.Count} links");
					return true;
				}

				// 2) fetch
				var html = await HttpGetAsync(GAME8_ALL_EVENTS_URL);
				if (string.IsNullOrEmpty(html)) return false;

				var doc = new HtmlDocument();
				doc.LoadHtml(html);

				var links = new List<(string title, string url)>();
				foreach (var a in doc.DocumentNode.SelectNodes("//a[@href]") ?? Enumerable.Empty<HtmlNode>())
				{
					var href = a.GetAttributeValue("href", "");
					var title = a.InnerText?.Trim() ?? "";
					if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(href) && href.StartsWith("https://game8.co/games/Umamusume-Pretty-Derby/archives/"))
					{
						links.Add((title, href));
					}
				}
				// deduplicate keep order
				var seen = new HashSet<string>();
				var clean = new List<(string title, string url)>();
				foreach (var (t, u) in links)
				{
					if (!seen.Contains(u))
					{
						seen.Add(u);
						clean.Add((t, u));
					}
				}
				EventsIndex = clean;
				_indexLoaded = true;
				SaveIndexCache(clean);
				if (DEBUG) Console.WriteLine($"[DEBUG] fetched index: {EventsIndex.Count} links");
				return true;
			}

			public async Task<JObject> FetchEventAsync(string url)
			{
				// cache
				var cached = LoadEventCache(url);
				if (cached != null)
				{
					if (DEBUG) Console.WriteLine("[DEBUG] event from cache: " + url);
					return cached;
				}

				var html = await HttpGetAsync(url);
				if (string.IsNullOrEmpty(html)) return null;
				if (DEBUG) Console.WriteLine($"[DEBUG] fetch_event len={html.Length} url={url} head={(html.Length > 200 ? html.Substring(0, 200) : html).Replace("\n", " ")}");

				var doc = new HtmlDocument();
				try
				{
					doc.LoadHtml(html);
				}
				catch (Exception)
				{
					// fallback is same: HtmlAgilityPack is robust
					doc.LoadHtml(html);
				}

				// title
				var pageTitleNode = doc.DocumentNode.SelectSingleNode("//h1|//h2");
				var eventName = pageTitleNode != null ? HtmlEntity.DeEntitize(pageTitleNode.InnerText).Trim() : "Unknown Event";

				string sourceType = "trainee";
				string sourceName = null;
				var m = Regex.Match(eventName, @"\(([^)]+)\)");
				if (m.Success) sourceName = m.Groups[1].Value.Trim();

				// locate section
				HtmlNode section = null;
				var wanted = new[] {
					"choices and outcomes", "choice and outcome", "choices & outcomes",
					"event choices and outcomes", "choices / outcomes"
				};
				foreach (var hx in doc.DocumentNode.SelectNodes("//h1|//h2|//h3|//h4") ?? Enumerable.Empty<HtmlNode>())
				{
					var txt = HtmlEntity.DeEntitize(hx.InnerText).Trim().ToLowerInvariant();
					if (wanted.Any(w => txt.Contains(w)))
					{
						section = hx;
						break;
					}
				}

				var choicesData = new List<JObject>();

				List<JObject> NormalizeEffects(string outcomeText)
				{
					var splits = Regex.Split(outcomeText, @"[•·・]\s*");
					if (splits.Length == 1)
						splits = Regex.Split(outcomeText, @"\s{2,}| / ");
					var effects = new List<JObject>();
					foreach (var raw0 in splits)
					{
						var raw = raw0.Trim();
						if (string.IsNullOrEmpty(raw)) continue;
						var disp = raw;

						var mAllStats = Regex.Match(raw, @"^(All Stats)\s*\+(\d+)", RegexOptions.IgnoreCase);
						if (mAllStats.Success)
						{
							effects.Add(new JObject { ["type"] = "all_stats", ["value"] = int.Parse(mAllStats.Groups[2].Value), ["unit"] = "pts", ["notes"] = null, ["display_text"] = disp });
							continue;
						}
						var mStat = Regex.Match(raw, @"^(Speed|Power|Stamina|Guts|Wisdom)\s*\+(\d+)", RegexOptions.IgnoreCase);
						if (mStat.Success)
						{
							effects.Add(new JObject { ["type"] = mStat.Groups[1].Value.ToLowerInvariant(), ["value"] = int.Parse(mStat.Groups[2].Value), ["unit"] = "pts", ["notes"] = null, ["display_text"] = disp });
							continue;
						}
						var mSkillPts = Regex.Match(raw, @"^(Skill\s*Pts?|Skill\s*Points?)\s*\+(\d+)", RegexOptions.IgnoreCase);
						if (mSkillPts.Success)
						{
							effects.Add(new JObject { ["type"] = "skill_points", ["value"] = int.Parse(mSkillPts.Groups[2].Value), ["unit"] = "pts", ["notes"] = null, ["display_text"] = disp });
							continue;
						}
						var mEnergy = Regex.Match(raw, @"^(Energy)\s*\+(\d+)", RegexOptions.IgnoreCase);
						if (mEnergy.Success)
						{
							effects.Add(new JObject { ["type"] = "energy", ["value"] = int.Parse(mEnergy.Groups[2].Value), ["unit"] = "pts", ["notes"] = null, ["display_text"] = disp });
							continue;
						}

						var mMot = Regex.Match(raw, @"^Motivation\s+(Up|Down)", RegexOptions.IgnoreCase);
						if (mMot.Success)
						{
							effects.Add(new JObject { ["type"] = "motivation", ["value"] = mMot.Groups[1].Value.ToLowerInvariant(), ["unit"] = null, ["notes"] = null, ["display_text"] = disp });
							continue;
						}

						if (raw.ToLowerInvariant().Contains("hint"))
						{
							int? level = null;
							var m3 = Regex.Match(raw, @"Lv\.?\s*(\d+)", RegexOptions.IgnoreCase);
							if (m3.Success) level = int.Parse(m3.Groups[1].Value);
							var skillName = Regex.Replace(raw, @"(?i)skill\s*hint.*", "").Trim(" -:·•.".ToCharArray());
							var sn = (!string.IsNullOrEmpty(skillName) && !skillName.ToLowerInvariant().StartsWith("skill")) ? skillName : null;
							var val = new JObject { ["skill"] = sn, ["level"] = (level.HasValue ? (JToken)level.Value : JValue.CreateNull()) };
							effects.Add(new JObject { ["type"] = "skill_hint", ["value"] = val, ["unit"] = (level.HasValue ? "lv" : null), ["notes"] = null, ["display_text"] = disp });
							continue;
						}

						if (Regex.IsMatch(raw, "fatigue", RegexOptions.IgnoreCase))
						{
							string val = null;
							var lower = raw.ToLowerInvariant();
							if (lower.Contains("increase") || lower.Contains("up") || lower.Contains("+")) val = "up";
							else if (lower.Contains("decrease") || lower.Contains("down") || lower.Contains("-")) val = "down";
							effects.Add(new JObject { ["type"] = "fatigue", ["value"] = val, ["unit"] = null, ["notes"] = null, ["display_text"] = disp });
							continue;
						}

						effects.Add(new JObject { ["type"] = "flag", ["value"] = raw, ["unit"] = null, ["notes"] = "unparsed", ["display_text"] = disp });
					}
					if (effects.Count == 0)
					{
						effects.Add(new JObject { ["type"] = "flag", ["value"] = outcomeText, ["unit"] = null, ["notes"] = "display-only", ["display_text"] = outcomeText });
					}
					return effects;
				}

				bool DetectRandom(string text)
				{
					var t = (text ?? "").ToLowerInvariant();
					return new[] { "random", "chance", "~", "may", "might" }.Any(k => t.Contains(k));
				}

				bool ParseTable(HtmlNode table)
				{
					var rows = table.SelectNodes(".//tr");
					if (rows == null || rows.Count == 0) return false;
					var header = rows[0].SelectNodes(".//th|.//td")?.Select(n => HtmlEntity.DeEntitize(n.InnerText).Trim().ToLowerInvariant()).ToList() ?? new List<string>();
					bool looksHeader = header.Count > 0 && (header.Any(c => c.Contains("choice")) || header.Any(c => c.Contains("outcome")));
					int start = looksHeader ? 1 : 0;
					bool found = false;
					for (int i = start; i < rows.Count; i++)
					{
						var r = rows[i];
						var cols = r.SelectNodes(".//th|.//td")?.Select(n => HtmlEntity.DeEntitize(n.InnerText).Trim()).ToList() ?? new List<string>();
						if (cols.Count < 2) continue;
						var left = cols[0];
						var right = cols[1];
						if (string.IsNullOrEmpty(left) || string.IsNullOrEmpty(right)) continue;
						if (left.ToLowerInvariant().StartsWith("choice") && right.ToLowerInvariant().StartsWith("outcome")) continue;
						var effects = NormalizeEffects(right);
						choicesData.Add(new JObject
						{
							["label"] = left,
							["index"] = choicesData.Count + 1,
							["effects"] = new JArray(effects),
							["risk"] = null,
							["random"] = DetectRandom(right)
						});
						found = true;
					}
					return found;
				}

				bool anyParsed = false;
				foreach (var table in doc.DocumentNode.SelectNodes("//table") ?? Enumerable.Empty<HtmlNode>())
				{
					if (ParseTable(table))
					{
						anyParsed = true;
						break;
					}
				}

				if (!anyParsed && section != null)
				{
					// find next ul/ol after section
					HtmlNode ul = null;
					var next = section.NextSibling;
					while (next != null && ul == null)
					{
						if (next.Name == "ul" || next.Name == "ol")
						{
							ul = next;
							break;
						}
						// sometimes wrapped
						var inside = next.SelectSingleNode(".//ul|.//ol");
						if (inside != null)
						{
							ul = inside;
							break;
						}
						next = next.NextSibling;
					}
					if (ul != null)
					{
						var items = ul.SelectNodes("./li") ?? ul.SelectNodes(".//li");
						if (items != null)
						{
							int idx = 1;
							foreach (var li in items)
							{
								var line = HtmlEntity.DeEntitize(li.InnerText).Trim();
								if (string.IsNullOrEmpty(line)) continue;
								var effects = NormalizeEffects(line);
								choicesData.Add(new JObject
								{
									["label"] = $"Choice {idx}",
									["index"] = idx,
									["effects"] = new JArray(effects),
									["risk"] = null,
									["random"] = DetectRandom(line)
								});
								idx++;
							}
						}
						anyParsed = choicesData.Count > 0;
					}
				}

				if (!anyParsed)
				{
					// fallback parse: look for tr nodes where first column contains "choice"
					foreach (var r in doc.DocumentNode.SelectNodes("//tr") ?? Enumerable.Empty<HtmlNode>())
					{
						var cols = r.SelectNodes(".//th|.//td")?.Select(n => HtmlEntity.DeEntitize(n.InnerText).Trim()).ToList() ?? new List<string>();
						if (cols.Count >= 2 && cols[0].ToLowerInvariant().Contains("choice"))
						{
							var left = cols[0];
							var right = cols[1];
							var effects = NormalizeEffects(right);
							choicesData.Add(new JObject
							{
								["label"] = left,
								["index"] = choicesData.Count + 1,
								["effects"] = new JArray(effects),
								["risk"] = null,
								["random"] = DetectRandom(right)
							});
						}
					}
					anyParsed = choicesData.Count > 0;
				}

				var evt = new JObject
				{
					["schema_version"] = "1.0",
					["event_id"] = BuildEventId(eventName),
					["name"] = StripEventName(eventName),
					["source_type"] = sourceType,
					["source_name"] = sourceName,
					["language"] = "en",
					["server"] = "Global",
					["choices"] = new JArray(choicesData),
					["notes"] = new JArray(),
					["tags"] = new JArray(),
					["variants"] = new JArray(),
					["matching"] = new JObject
					{
						["normalized_query"] = "",
						["match_score"] = 1.0,
						["ambiguity"] = "none",
						["candidates"] = new JArray()
					},
					["source"] = new JObject
					{
						["site"] = "game8",
						["url"] = url,
						["retrieved_at"] = NowIso(),
						["hash"] = $"sha1:{Sha1Hex(JsonConvert.SerializeObject(choicesData, Formatting.None))}"
					},
					["errors"] = choicesData.Count > 0 ? new JArray() : new JArray("choices_not_found")
				};

				SaveEventCache(url, evt);
				return evt;
			}

			public static string StripEventName(string eventName) => eventName ?? "";

			public static string BuildEventId(string eventName)
			{
				if (string.IsNullOrEmpty(eventName)) return "";
				var m = Regex.Match(eventName, @"^(.*?)\s*\(([^)]+)\)\s*$");
				if (m.Success)
				{
					var basePart = Slugify(m.Groups[1].Value);
					var who = Slugify(m.Groups[2].Value);
					return $"{who}_{basePart}";
				}
				return Slugify(eventName);
			}

			// PUBLIC: resolve query
			public async Task<JObject> ResolveAsync(string queryName, string sourceHint = null)
			{
				if (!_indexLoaded)
				{
					if (!await LoadIndexAsync())
					{
						return new JObject { ["status"] = "not_found", ["events"] = new JArray(), ["message"] = "Failed to load Game8 index." };
					}
				}

				var cands = SearchCandidates(queryName, sourceHint);
				if (!cands.Any())
				{
					return new JObject { ["status"] = "not_found", ["events"] = new JArray(), ["message"] = "No matching events on Game8." };
				}

				var topScore = cands[0].score;
				var amb = cands.Where(c => (topScore - c.score) <= 0.08 && c.score >= 0.55).ToList();
				if (amb.Count > 1)
				{
					var arr = new JArray(amb.Take(5).Select(a => new JObject
					{
						["name"] = a.title,
						["source"] = new JObject { ["url"] = a.url },
						["matching"] = new JObject { ["match_score"] = Math.Round(a.score, 3) }
					}));
					return new JObject { ["status"] = "ambiguous", ["events"] = arr, ["message"] = "Multiple close matches. Select one." };
				}

				var best = cands[0];
				JObject evt = await FetchEventAsync(best.url);
				if (evt == null || evt["choices"] == null || !evt["choices"].HasValues)
				{
					// try next candidates quickly
					foreach (var candidate in cands.Skip(1).Take(2))
					{
						var tryEvt = await FetchEventAsync(candidate.url);
						if (tryEvt != null && tryEvt["choices"] != null && tryEvt["choices"].HasValues)
						{
							best = candidate;
							evt = tryEvt;
							break;
						}
					}
				}

				if (evt == null || evt["choices"] == null || !evt["choices"].HasValues)
				{
					return new JObject { ["status"] = "partial", ["events"] = new JArray(), ["message"] = "Event page found but choices could not be parsed." };
				}

				evt["matching"]["normalized_query"] = NormalizeQuery(queryName);
				evt["matching"]["match_score"] = Math.Round(best.score, 3);
				return new JObject { ["status"] = "ok", ["events"] = new JArray(evt), ["message"] = "" };
			}

			public List<(string title, string url, double score)> SearchCandidates(string queryName, string sourceHint = null)
			{
				var q = NormalizeQuery(queryName);
				var sh = string.IsNullOrEmpty(sourceHint) ? null : NormalizeQuery(sourceHint);

				var cands = new List<(string title, string url, double score)>();
				foreach (var (title, url) in EventsIndex)
				{
					var tnorm = NormalizeQuery(title);
					double score = 0.0;
					if (q == tnorm) score = 1.0;
					else if (!string.IsNullOrEmpty(q) && tnorm.Contains(q)) score = 0.9;
					else
					{
						var qs = new HashSet<string>(q.Split(' ', StringSplitOptions.RemoveEmptyEntries));
						var ts = new HashSet<string>(tnorm.Split(' ', StringSplitOptions.RemoveEmptyEntries));
						if (qs.Count > 0 && ts.Count > 0)
						{
							var inter = qs.Intersect(ts).Count();
							var uni = qs.Union(ts).Count();
							score = (double)inter / Math.Max(1, uni);
						}
					}
					if (!string.IsNullOrEmpty(sh) && tnorm.Contains(sh)) score += 0.2;
					if (score >= 0.35) cands.Add((title, url, Math.Min(score, 1.0)));
				}
				cands.Sort((a, b) => b.score.CompareTo(a.score));
				return cands.Take(12).ToList();
			}
			// --------------------------
			// UMA cache helpers & scraping
			// --------------------------
			static readonly string UMA_LIST_CACHE = Path.Combine(CACHE_DIR, "uma_list.json");
			private JObject LoadUmaCache()
			{
				try
				{
					if (!File.Exists(UMA_LIST_CACHE)) return null;
					var text = File.ReadAllText(UMA_LIST_CACHE, Encoding.UTF8);
					var jo = JObject.Parse(text);
					return jo;
				}
				catch (Exception ex)
				{
					if (DEBUG) Console.WriteLine("[DEBUG] LoadUmaCache error: " + ex.Message);
					return null;
				}
			}

			private void SaveUmaCache(JObject jo)
			{
				try
				{
					File.WriteAllText(UMA_LIST_CACHE, jo.ToString(Formatting.Indented), Encoding.UTF8);
				}
				catch (Exception ex)
				{
					if (DEBUG) Console.WriteLine("[DEBUG] SaveUmaCache error: " + ex.Message);
				}
			}

			/// <summary>
			/// Fetches UMA names for the fixed button indices 6..8.
			/// Uses uma_list.json cache (per-index entries) with TTL UMA_TTL_DAYS.
			/// Returns a map: index -> list of td texts (empty list if not found).
			/// </summary>
			public async Task<List<string>> FetchAllUmaNameAsync()
			{
				const string baseUrl = "https://game8.co/games/Umamusume-Pretty-Derby/archives/535926";
				var result = new List<string>();
				var indices = new[] { 5, 6, 7 };

				// Ensure cache directory exists (LoadUmaCache/SaveUmaCache handle file IO, but ensure root exists)
				try { Directory.CreateDirectory(CACHE_DIR); } catch { /* ignore */ }

				// Load cache (may be null)
				var cache = LoadUmaCache() ?? new JObject();
				bool cacheChanged = false;

				// Try to reuse base page HTML (fetch once)
				string baseHtml = null;
				HtmlDocument baseDoc = null;

				// If any index has a fresh cache entry, use it. For others we'll fetch.
				foreach (var idx in indices)
				{
					var key = idx.ToString();
					var texts = new List<string>();

					// Check cache entry
					if (cache.TryGetValue(key, out var token))
					{
						try
						{
							var entry = (JObject)token;
							var fetchedStr = entry.Value<string>("retrieved_at");
							if (!string.IsNullOrEmpty(fetchedStr))
							{
								var fetched = DateTimeOffset.Parse(fetchedStr, CultureInfo.InvariantCulture);
								if (DateTimeOffset.Now - fetched <= TimeSpan.FromDays(EVENT_TTL_DAYS))
								{
									var arr = entry["texts"] as JArray;
									if (arr != null)
									{
										texts = arr.Select(x => x.ToString()).Where(s => !string.IsNullOrEmpty(s)).ToList();
										result.AddRange(texts);
										if (DEBUG) Console.WriteLine($"[DEBUG] UMA cache hit for index {idx}");
										continue; // next index
									}
								}
							}
						}
						catch (Exception ex)
						{
							if (DEBUG) Console.WriteLine("[DEBUG] parse cache entry error: " + ex.Message);
							// fall through to re-fetch
						}
					}

					// Need to fetch details for this index
					// Fetch base page (once)
					if (baseDoc == null)
					{
						baseHtml = await HttpGetAsync(baseUrl);
						if (string.IsNullOrEmpty(baseHtml))
						{
							if (DEBUG) Console.WriteLine("[DEBUG] failed to fetch UMA base page");
							continue;
						}
						baseDoc = new HtmlDocument();
						baseDoc.LoadHtml(baseHtml);
					}

					// Attempt primary XPath for anchor
					string xpathAnchor = $"/html/body/div[1]/div[1]/div[2]/div[1]/div[2]/p[{idx}]/a";
					var aNode = baseDoc.DocumentNode.SelectSingleNode(xpathAnchor);

					// Fallback: pick nth <p> under container and find <a>
					if (aNode == null)
					{
						if (DEBUG) Console.WriteLine($"[DEBUG] anchor not found by exact XPath {xpathAnchor}, trying fallback for index {idx}.");
						var container = baseDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[2]/div[1]/div[2]");
						if (container != null)
						{
							var pNodes = container.SelectNodes("./p") ?? container.SelectNodes(".//p");
							if (pNodes != null && pNodes.Count >= idx)
							{
								var pNode = pNodes[idx - 1];
								aNode = pNode.SelectSingleNode(".//a");
							}
						}
					}

					if (aNode == null)
					{
						if (DEBUG) Console.WriteLine($"[DEBUG] could not locate anchor for button index {idx}");
						continue;
					}

					var href = aNode.GetAttributeValue("href", null);
					if (string.IsNullOrEmpty(href))
					{
						if (DEBUG) Console.WriteLine("[DEBUG] anchor has no href for index " + idx);
						continue;
					}

					var hrefAbs = MakeAbsoluteUrl(baseUrl, href);
					if (DEBUG) Console.WriteLine($"[DEBUG] following href for index {idx}: {hrefAbs}");

					var detailHtml = await HttpGetAsync(hrefAbs);
					if (string.IsNullOrEmpty(detailHtml))
					{
						if (DEBUG) Console.WriteLine("[DEBUG] failed to fetch details page: " + hrefAbs);
						continue;
					}

					var detailDoc = new HtmlDocument();
					detailDoc.LoadHtml(detailHtml);

					// Primary XPath for td nodes
					var tdNodes = detailDoc.DocumentNode.SelectNodes("/html/body/div[1]/div[1]/div[2]/div[1]/div[2]/table[1]/tbody/tr/td");

					// Fallback: first table under container
					if (tdNodes == null || tdNodes.Count == 0)
					{
						if (DEBUG) Console.WriteLine("[DEBUG] primary td XPath not found for index " + idx + ", trying fallback.");
						var container2 = detailDoc.DocumentNode.SelectSingleNode("/html/body/div[1]/div[1]/div[2]/div[1]/div[2]");
						if (container2 != null)
						{
							var firstTable = container2.SelectSingleNode(".//table");
							if (firstTable != null)
							{
								tdNodes = firstTable.SelectNodes(".//td");
							}
						}
					}
					List<string> textsList = new();
					if (tdNodes != null && tdNodes.Count > 0)
					{
						// Build texts list
						textsList = tdNodes
							.Select(td => HtmlEntity.DeEntitize(td.InnerText ?? "").Trim())
							.Where(s => !string.IsNullOrEmpty(s))
							.ToList();

						result.AddRange(textsList);
					}
					else
					{
						if (DEBUG) Console.WriteLine("[DEBUG] no td nodes found on detail page for index " + idx);
					}

					// Update cache for this index
					var newEntry = new JObject
					{
						["retrieved_at"] = NowIso(),
						["href"] = hrefAbs,
						["texts"] = new JArray(textsList)
					};
					cache[idx.ToString()] = newEntry;
					cacheChanged = true;
				} // end foreach index

				// Save cache if changed
				if (cacheChanged)
				{
					try
					{
						SaveUmaCache(cache);
						if (DEBUG) Console.WriteLine("[DEBUG] UMA cache updated.");
					}
					catch (Exception ex)
					{
						if (DEBUG) Console.WriteLine("[DEBUG] failed saving UMA cache: " + ex.Message);
					}
				}

				return result;
			}


			private static string MakeAbsoluteUrl(string baseUrl, string href)
			{
				try
				{
					if (Uri.IsWellFormedUriString(href, UriKind.Absolute)) return href;
					var baseUri = new Uri(baseUrl);
					var abs = new Uri(baseUri, href).ToString();
					return abs;
				}
				catch
				{
					return href;
				}
			}
		} // end Game8Resolver class
	}
}
