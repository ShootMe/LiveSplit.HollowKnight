#if !Info
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
#endif
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.HollowKnight {
#if !Info
	public class Component : IComponent {
		public TimerModel Model { get; set; }
#else
	public class HollowKnightComponent {
#endif
		public string ComponentName { get { return "Hollow Knight Autosplitter"; } }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		internal static string[] keys = { "CurrentSplit", "State", "GameState", "SceneName", "Charms", "CameraMode", "MenuState", "UIState", "AcceptingInput", "MapZone", "NextSceneName" };
		private Memory mem;
		private int currentSplit = -1, state = 0, lastLogCheck = 0;
		private bool hasLog = false;
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private Settings settings;
		private HashSet<SplitInfo> splitsDone = new HashSet<SplitInfo>();
		private static string LOGFILE = "_HollowKnight.log";
		private PlayerData pdata = new PlayerData();
		public Component() {
			mem = new Memory();
			settings = new Settings();
			foreach (string key in keys) {
				currentValues[key] = "";
			}
		}

		public void GetValues() {
			if (!mem.HookProcess()) { return; }

#if !Info
			if (Model != null) {
				HandleSplits();
			}
#endif

			LogValues();
		}
#if !Info
		private void HandleSplits() {
			bool shouldSplit = false;

			if (currentSplit == -1) {
				shouldSplit = mem.MenuState() == MainMenuState.PLAY_MODE_MENU && mem.GameState() == GameState.MAIN_MENU && !mem.AcceptingInput();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				string nextScene = mem.NextSceneName();
				string sceneName = mem.SceneName();

				if (currentSplit + 1 < Model.CurrentState.Run.Count) {
					foreach (var split in settings.Splits) {
						if (splitsDone.Contains(split)) { continue; }

						switch (split) {
							case var info when (info.Equals(SplitInfo.AbyssShriek)): shouldSplit = mem.PlayerData<int>(Offset.screamLevel) == 2; break;
							case var info when (info.Equals(SplitInfo.AspidHunter)): shouldSplit = mem.PlayerData<int>(Offset.killsSpitter) == 17; break;
							case var info when (info.Equals(SplitInfo.BaldurShell)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_5); break;
							case var info when (info.Equals(SplitInfo.BlackKnight)): shouldSplit = mem.PlayerData<bool>(Offset.killedBlackKnight); break;
							case var info when (info.Equals(SplitInfo.BrokenVessel)): shouldSplit = mem.PlayerData<bool>(Offset.killedInfectedKnight); break;
							case var info when (info.Equals(SplitInfo.BroodingMawlek)): shouldSplit = mem.PlayerData<bool>(Offset.killedMawlek); break;
							case var info when (info.Equals(SplitInfo.CityOfTears)): shouldSplit = mem.PlayerData<bool>(Offset.visitedRuins); break;
							case var info when (info.Equals(SplitInfo.Collector)): shouldSplit = mem.PlayerData<bool>(Offset.collectorDefeated); break;
							case var info when (info.Equals(SplitInfo.ColosseumBronze)): shouldSplit = mem.PlayerData<bool>(Offset.colosseumBronzeCompleted); break;
							case var info when (info.Equals(SplitInfo.ColosseumGold)): shouldSplit = mem.PlayerData<bool>(Offset.colosseumGoldCompleted); break;
							case var info when (info.Equals(SplitInfo.ColosseumSilver)): shouldSplit = mem.PlayerData<bool>(Offset.colosseumSilverCompleted); break;
							case var info when (info.Equals(SplitInfo.CrossroadsStation)): shouldSplit = mem.PlayerData<bool>(Offset.openedCrossroads); break;
							case var info when (info.Equals(SplitInfo.CrystalGuardian1)): shouldSplit = mem.PlayerData<bool>(Offset.defeatedMegaBeamMiner); break;
							case var info when (info.Equals(SplitInfo.CrystalGuardian2)): shouldSplit = mem.PlayerData<int>(Offset.killsMegaBeamMiner) == 0; break;
							case var info when (info.Equals(SplitInfo.CrystalHeart)): shouldSplit = mem.PlayerData<bool>(Offset.hasSuperDash); break;
							case var info when (info.Equals(SplitInfo.CrystalPeak)): shouldSplit = mem.PlayerData<bool>(Offset.visitedMines); break;
							case var info when (info.Equals(SplitInfo.CycloneSlash)): shouldSplit = mem.PlayerData<bool>(Offset.hasCyclone); break;
							case var info when (info.Equals(SplitInfo.Dashmaster)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_31); break;
							case var info when (info.Equals(SplitInfo.DashSlash)): shouldSplit = mem.PlayerData<bool>(Offset.hasDashSlash); break;
							case var info when (info.Equals(SplitInfo.DeepFocus)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_34); break;
							case var info when (info.Equals(SplitInfo.Deepnest)): shouldSplit = mem.PlayerData<bool>(Offset.visitedDeepnest); break;
							case var info when (info.Equals(SplitInfo.DeepnestSpa)): shouldSplit = mem.PlayerData<bool>(Offset.visitedDeepnestSpa); break;
							case var info when (info.Equals(SplitInfo.DeepnestStation)): shouldSplit = mem.PlayerData<bool>(Offset.openedDeepnest); break;
							case var info when (info.Equals(SplitInfo.DefendersCrest)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_10); break;
							case var info when (info.Equals(SplitInfo.DescendingDark)): shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 2; break;
							case var info when (info.Equals(SplitInfo.DesolateDive)): shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 1; break;
							case var info when (info.Equals(SplitInfo.DreamNail)): shouldSplit = mem.PlayerData<bool>(Offset.hasDreamNail); break;
							case var info when (info.Equals(SplitInfo.DreamNail2)): shouldSplit = mem.PlayerData<bool>(Offset.dreamNailUpgraded); break;
							case var info when (info.Equals(SplitInfo.DreamWielder)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_30); break;
							case var info when (info.Equals(SplitInfo.DungDefender)): shouldSplit = mem.PlayerData<bool>(Offset.killedDungDefender); break;
							case var info when (info.Equals(SplitInfo.ElderHu)): shouldSplit = mem.PlayerData<bool>(Offset.killedGhostHu); break;
							case var info when (info.Equals(SplitInfo.FailedKnight)): shouldSplit = mem.PlayerData<bool>(Offset.falseKnightDreamDefeated); break;
							case var info when (info.Equals(SplitInfo.FalseKnight)): shouldSplit = mem.PlayerData<bool>(Offset.killedFlaseKnight); break;
							case var info when (info.Equals(SplitInfo.Flukemarm)): shouldSplit = mem.PlayerData<bool>(Offset.killedFlukeMother); break;
							case var info when (info.Equals(SplitInfo.Flukenest)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_11); break;
							case var info when (info.Equals(SplitInfo.FogCanyon)): shouldSplit = mem.PlayerData<bool>(Offset.visitedFogCanyon); break;
							case var info when (info.Equals(SplitInfo.ForgottenCrossroads)): shouldSplit = mem.PlayerData<bool>(Offset.visitedCrossroads); break;
							case var info when (info.Equals(SplitInfo.FragileGreed)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_24); break;
							case var info when (info.Equals(SplitInfo.FragileHeart)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_23); break;
							case var info when (info.Equals(SplitInfo.FragileStrength)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_25); break;
							case var info when (info.Equals(SplitInfo.FungalWastes)): shouldSplit = mem.PlayerData<bool>(Offset.visitedFungus); break;
							case var info when (info.Equals(SplitInfo.FuryOfTheFallen)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_6); break;
							case var info when (info.Equals(SplitInfo.Galien)): shouldSplit = mem.PlayerData<bool>(Offset.killedGhostGalien); break;
							case var info when (info.Equals(SplitInfo.GatheringSwarm)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_1); break;
							case var info when (info.Equals(SplitInfo.GlowingWomb)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_22); break;
							case var info when (info.Equals(SplitInfo.Gorb)): shouldSplit = mem.PlayerData<bool>(Offset.killedGhostAladar); break;
							case var info when (info.Equals(SplitInfo.GreatSlash)): shouldSplit = mem.PlayerData<bool>(Offset.hasUpwardSlash); break;
							case var info when (info.Equals(SplitInfo.Greenpath)): shouldSplit = mem.PlayerData<bool>(Offset.visitedGreenpath); break;
							case var info when (info.Equals(SplitInfo.GrubberflysElegy)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_35); break;
							case var info when (info.Equals(SplitInfo.Grubsong)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_3); break;
							case var info when (info.Equals(SplitInfo.GruzMother)): shouldSplit = mem.PlayerData<bool>(Offset.killedBigFly); break;
							case var info when (info.Equals(SplitInfo.HeavyBlow)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_15); break;
							case var info when (info.Equals(SplitInfo.Hegemol)): shouldSplit = mem.PlayerData<bool>(Offset.hegemolDefeated); break;
							case var info when (info.Equals(SplitInfo.Hive)): shouldSplit = mem.PlayerData<bool>(Offset.visitedHive); break;
							case var info when (info.Equals(SplitInfo.Hiveblood)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_29); break;
							case var info when (info.Equals(SplitInfo.HollowKnight)): shouldSplit = nextScene.Equals("Dream_Final_Boss", StringComparison.OrdinalIgnoreCase); break;
							case var info when (info.Equals(SplitInfo.Hornet1)): shouldSplit = mem.PlayerData<bool>(Offset.killedHornet); break;
							case var info when (info.Equals(SplitInfo.Hornet2)): shouldSplit = mem.PlayerData<bool>(Offset.hornetOutskirtsDefeated); break;
							case var info when (info.Equals(SplitInfo.HowlingWraiths)): shouldSplit = mem.PlayerData<int>(Offset.screamLevel) == 1; break;
							case var info when (info.Equals(SplitInfo.InfectedCrossroads)): shouldSplit = mem.PlayerData<bool>(Offset.crossroadsInfected) && mem.PlayerData<bool>(Offset.visitedCrossroads); break;
							case var info when (info.Equals(SplitInfo.IsmasTear)): shouldSplit = mem.PlayerData<bool>(Offset.hasAcidArmour); break;
							case var info when (info.Equals(SplitInfo.JonisBlessing)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_27); break;
							case var info when (info.Equals(SplitInfo.KingsBrand)): shouldSplit = mem.PlayerData<bool>(Offset.hasKingsBrand); break;
							case var info when (info.Equals(SplitInfo.Kingsoul)): shouldSplit = mem.PlayerData<int>(Offset.charmCost_36) == 5; break;
							case var info when (info.Equals(SplitInfo.KingsStationStation)): shouldSplit = mem.PlayerData<bool>(Offset.openedRuins2); break;
							case var info when (info.Equals(SplitInfo.LifebloodCore)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_9); break;
							case var info when (info.Equals(SplitInfo.LifebloodHeart)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_8); break;
							case var info when (info.Equals(SplitInfo.Longnail)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_18); break;
							case var info when (info.Equals(SplitInfo.LostKin)): shouldSplit = mem.PlayerData<bool>(Offset.infectedKnightDreamDefeated); break;
							case var info when (info.Equals(SplitInfo.LoveKey)): shouldSplit = mem.PlayerData<bool>(Offset.hasLoveKey); break;
							case var info when (info.Equals(SplitInfo.LumaflyLantern)): shouldSplit = mem.PlayerData<bool>(Offset.hasLantern); break;
							case var info when (info.Equals(SplitInfo.Lurien)): shouldSplit = mem.PlayerData<bool>(Offset.lurienDefeated); break;
							case var info when (info.Equals(SplitInfo.MantisClaw)): shouldSplit = mem.PlayerData<bool>(Offset.hasWallJump); break;
							case var info when (info.Equals(SplitInfo.MantisLords)): shouldSplit = mem.PlayerData<bool>(Offset.defeatedMantisLords); break;
							case var info when (info.Equals(SplitInfo.MarkOfPride)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_13); break;
							case var info when (info.Equals(SplitInfo.Markoth)): shouldSplit = mem.PlayerData<bool>(Offset.killedGhostMarkoth); break;
							case var info when (info.Equals(SplitInfo.Marmu)): shouldSplit = mem.PlayerData<bool>(Offset.killedGhostMarmu); break;
							case var info when (info.Equals(SplitInfo.Mask1)): shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 6; break;
							case var info when (info.Equals(SplitInfo.Mask2)): shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 7; break;
							case var info when (info.Equals(SplitInfo.Mask3)): shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 8; break;
							case var info when (info.Equals(SplitInfo.Mask4)): shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 9; break;
							case var info when (info.Equals(SplitInfo.MegaMossCharger)): shouldSplit = mem.PlayerData<bool>(Offset.megaMossChargerDefeated); break;
							case var info when (info.Equals(SplitInfo.MonarchWings)): shouldSplit = mem.PlayerData<bool>(Offset.hasDoubleJump); break;
							case var info when (info.Equals(SplitInfo.Monomon)): shouldSplit = mem.PlayerData<bool>(Offset.monomonDefeated); break;
							case var info when (info.Equals(SplitInfo.MossKnight)): shouldSplit = mem.PlayerData<bool>(Offset.killedMossKnight); break;
							case var info when (info.Equals(SplitInfo.MothwingCloak)): shouldSplit = mem.PlayerData<bool>(Offset.hasDash); break;
							case var info when (info.Equals(SplitInfo.MushroomBrawler)): shouldSplit = mem.PlayerData<int>(Offset.killsMushroomBrawler) == 6; break;
							case var info when (info.Equals(SplitInfo.NailmastersGlory)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_26); break;
							case var info when (info.Equals(SplitInfo.NailUpgrade1)): shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 1; break;
							case var info when (info.Equals(SplitInfo.NailUpgrade2)): shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 2; break;
							case var info when (info.Equals(SplitInfo.NailUpgrade3)): shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 3; break;
							case var info when (info.Equals(SplitInfo.NailUpgrade4)): shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 4; break;
							case var info when (info.Equals(SplitInfo.NoEyes)): shouldSplit = mem.PlayerData<bool>(Offset.killedGhostNoEyes); break;
							case var info when (info.Equals(SplitInfo.Nosk)): shouldSplit = mem.PlayerData<bool>(Offset.killedMimicSpider); break;
							case var info when (info.Equals(SplitInfo.NotchFogCanyon)): shouldSplit = mem.PlayerData<bool>(Offset.notchFogCanyon); break;
							case var info when (info.Equals(SplitInfo.NotchSalubra1)): shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch1); break;
							case var info when (info.Equals(SplitInfo.NotchSalubra2)): shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch2); break;
							case var info when (info.Equals(SplitInfo.NotchSalubra3)): shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch3); break;
							case var info when (info.Equals(SplitInfo.NotchSalubra4)): shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch4); break;
							case var info when (info.Equals(SplitInfo.NotchShrumalOgres)): shouldSplit = mem.PlayerData<bool>(Offset.notchShroomOgres); break;
							case var info when (info.Equals(SplitInfo.PaleOre)): shouldSplit = mem.PlayerData<int>(Offset.ore) > 0; break;
							case var info when (info.Equals(SplitInfo.QueensGardens)): shouldSplit = mem.PlayerData<bool>(Offset.visitedRoyalGardens); break;
							case var info when (info.Equals(SplitInfo.QueensStationStation)): shouldSplit = mem.PlayerData<bool>(Offset.openedFungalWastes); break;
							case var info when (info.Equals(SplitInfo.QuickSlash)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_32); break;
							case var info when (info.Equals(SplitInfo.QuickFocus)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_7); break;
							case var info when (info.Equals(SplitInfo.RestingGrounds)): shouldSplit = mem.PlayerData<bool>(Offset.visitedRestingGrounds); break;
							case var info when (info.Equals(SplitInfo.RoyalWaterways)): shouldSplit = mem.PlayerData<bool>(Offset.visitedWaterways); break;
							case var info when (info.Equals(SplitInfo.SeerDeparts)): shouldSplit = mem.PlayerData<bool>(Offset.mothDeparted); break;
							case var info when (info.Equals(SplitInfo.ShadeCloak)): shouldSplit = mem.PlayerData<bool>(Offset.hasShadowDash); break;
							case var info when (info.Equals(SplitInfo.ShadeSoul)): shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 2; break;
							case var info when (info.Equals(SplitInfo.ShamanStone)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_19); break;
							case var info when (info.Equals(SplitInfo.ShapeOfUnn)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_28); break;
							case var info when (info.Equals(SplitInfo.SharpShadow)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_16); break;
							case var info when (info.Equals(SplitInfo.SimpleKey)): shouldSplit = mem.PlayerData<int>(Offset.simpleKeys) > 0; break;
							case var info when (info.Equals(SplitInfo.SoulCatcher)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_20); break;
							case var info when (info.Equals(SplitInfo.SoulEater)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_21); break;
							case var info when (info.Equals(SplitInfo.SoulMaster)): shouldSplit = mem.PlayerData<bool>(Offset.killedMageLord); break;
							case var info when (info.Equals(SplitInfo.SoulTyrant)): shouldSplit = mem.PlayerData<bool>(Offset.mageLordDreamDefeated); break;
							case var info when (info.Equals(SplitInfo.SpellTwister)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_33); break;
							case var info when (info.Equals(SplitInfo.SporeShroom)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_17); break;
							case var info when (info.Equals(SplitInfo.StalwartShell)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_4); break;
							case var info when (info.Equals(SplitInfo.SteadyBody)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_14); break;
							case var info when (info.Equals(SplitInfo.TeachersArchive)): shouldSplit = sceneName.Equals("Fungus3_archive", StringComparison.OrdinalIgnoreCase); break;
							case var info when (info.Equals(SplitInfo.ThornsOfAgony)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_12); break;
							case var info when (info.Equals(SplitInfo.TraitorLord)): shouldSplit = mem.PlayerData<bool>(Offset.killedTraitorLord); break;
							case var info when (info.Equals(SplitInfo.TramPass)): shouldSplit = mem.PlayerData<bool>(Offset.hasTramPass); break;
							case var info when (info.Equals(SplitInfo.Uumuu)): shouldSplit = mem.PlayerData<bool>(Offset.killedMegaJellyfish); break;
							case var info when (info.Equals(SplitInfo.VengefulSpirit)): shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 1; break;
							case var info when (info.Equals(SplitInfo.Vessel1)): shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 33; break;
							case var info when (info.Equals(SplitInfo.Vessel2)): shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 66; break;
							case var info when (info.Equals(SplitInfo.Vessel3)): shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 99; break;
							case var info when (info.Equals(SplitInfo.VoidHeart)): shouldSplit = mem.PlayerData<bool>(Offset.gotShadeCharm); break;
							case var info when (info.Equals(SplitInfo.WaywardCompass)): shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_2); break;
							case var info when (info.Equals(SplitInfo.WhitePalace)): shouldSplit = mem.PlayerData<bool>(Offset.visitedWhitePalace); break;
							case var info when (info.Equals(SplitInfo.Xero)): shouldSplit = mem.PlayerData<bool>(Offset.killedGhostXero); break;
							case var info when (info.Equals(SplitInfo.Zote1)): shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedBuzzer); break;
							case var info when (info.Equals(SplitInfo.Zote2)): shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedDeepnest); break;
						}

						if (shouldSplit) {
							splitsDone.Add(split);
							break;
						}
					}
				} else {
					shouldSplit = nextScene.StartsWith("Cinematic_Ending", StringComparison.OrdinalIgnoreCase);
				}

				GameState gameState = mem.GameState();
				if (!settings.OldGameTime) {
					Model.CurrentState.IsGameTimePaused = gameState == GameState.LOADING || (!string.IsNullOrEmpty(nextScene) && nextScene != sceneName);
				} else {
					Model.CurrentState.IsGameTimePaused = gameState == GameState.ENTERING_LEVEL || gameState == GameState.EXITING_LEVEL || gameState == GameState.LOADING || (!string.IsNullOrEmpty(nextScene) && nextScene != sceneName);
				}

			}

			HandleSplit(shouldSplit);
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (shouldReset) {
				if (currentSplit >= 0) {
					Model.Reset();
				}
			} else if (shouldSplit) {
				if (currentSplit < 0) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
		}
#endif
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				if (mem.UIState() == UIState.PLAYING) {
					byte[] playerData = mem.GetPlayerData();
					pdata.UpdateData(mem.Program, playerData, WriteLogWithTime);
				}

				string prev = "", curr = "";
				foreach (string key in keys) {
					prev = currentValues[key];

					switch (key) {
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "State": curr = state.ToString(); break;
						case "GameState": curr = mem.GameState().ToString(); break;
						case "SceneName": curr = mem.SceneName(); break;
						case "NextSceneName": curr = mem.NextSceneName(); break;
						case "Charms": curr = mem.CharmCount().ToString(); break;
						case "MapZone": curr = ((MapZone)mem.PlayerData<int>(Offset.mapZone)).ToString(); break;
						case "CameraMode": curr = mem.CameraMode().ToString(); break;
						case "MenuState": curr = mem.MenuState().ToString(); break;
						case "UIState": curr = mem.UIState().ToString(); break;
						case "AcceptingInput": curr = mem.AcceptingInput().ToString(); break;
						default: curr = ""; break;
					}

					if (!prev.Equals(curr)) {
						WriteLogWithTime(key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}
		private void WriteLog(string data) {
			if (hasLog || !Console.IsOutputRedirected) {
				if (!Console.IsOutputRedirected) {
					Console.WriteLine(data);
				}
				if (hasLog) {
					using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
						wr.WriteLine(data);
					}
				}
			}
		}
		private void WriteLogWithTime(string data) {
#if !Info
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null && Model.CurrentState.CurrentTime.RealTime.HasValue ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + data);
#else
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + ": " + data);
#endif
		}

#if !Info
		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			if (Model == null) {
				Model = new TimerModel() { CurrentState = lvstate };
				Model.InitializeGameTime();
				Model.CurrentState.IsGameTimePaused = true;
				lvstate.OnReset += OnReset;
				lvstate.OnPause += OnPause;
				lvstate.OnResume += OnResume;
				lvstate.OnStart += OnStart;
				lvstate.OnSplit += OnSplit;
				lvstate.OnUndoSplit += OnUndoSplit;
				lvstate.OnSkipSplit += OnSkipSplit;
			}

			GetValues();
		}

		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			state = 0;
			Model.CurrentState.IsGameTimePaused = true;
			splitsDone.Clear();
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			state = 0;
			Model.CurrentState.IsGameTimePaused = false;
			splitsDone.Clear();
			WriteLog("---------New Game-------------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			state = 0;
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
		}
		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
#endif
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() { }
	}
}