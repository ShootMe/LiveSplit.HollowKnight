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
    public class HollowKnightComponent : IComponent {
        public TimerModel Model { get; set; }
#else
	public class HollowKnightComponent {
#endif
        public string ComponentName { get { return "Hollow Knight Autosplitter"; } }
        public IDictionary<string, Action> ContextMenuControls { get { return null; } }
        internal static string[] keys = {
            "CurrentSplit", 
            "State", 
            "GameState", 
            "SceneName", 
            "Charms", 
            "CameraMode", 
            "MenuState", 
            "UIState", 
            "AcceptingInput", 
            "MapZone", 
            "NextSceneName",
            "ActorState", 
            "Loading", 
            "Transition", 
            "Teleporting", 
            "LookFor"
        };
        private HollowKnightMemory mem;
        private int currentSplit = -1, state = 0, lastLogCheck = 0;
        private bool hasLog = false;
        private Dictionary<string, string> currentValues = new Dictionary<string, string>();
        private HollowKnightSettings settings;
        private HashSet<SplitName> splitsDone = new HashSet<SplitName>();
        private SplitName lastSplitDone;
        private static string LOGFILE = "_HollowKnight.log";
        private PlayerData pdata = new PlayerData();
        private GameState lastGameState;
        private bool menuSplitHelper;
        private bool lookForTeleporting;
#if !Info
        public HollowKnightComponent(LiveSplitState state) {
            mem = new HollowKnightMemory();
            settings = new HollowKnightSettings();
            foreach (string key in keys) {
                currentValues[key] = "";
            }

            if (state != null) {
                Model = new TimerModel() { CurrentState = state };
                Model.InitializeGameTime();
                Model.CurrentState.IsGameTimePaused = true;
                state.OnReset += OnReset;
                state.OnPause += OnPause;
                state.OnResume += OnResume;
                state.OnStart += OnStart;
                state.OnSplit += OnSplit;
                state.OnUndoSplit += OnUndoSplit;
                state.OnSkipSplit += OnSkipSplit;
            }
        }
#else
		public HollowKnightComponent() {
			mem = new HollowKnightMemory();
			settings = new HollowKnightSettings();
			foreach (string key in keys) {
				currentValues[key] = "";
			}
		}
#endif

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
            string nextScene = mem.NextSceneName();
            string sceneName = mem.SceneName();

            if (currentSplit == -1) {
                shouldSplit = (nextScene.Equals("Tutorial_01", StringComparison.OrdinalIgnoreCase) &&
                               mem.GameState() == GameState.ENTERING_LEVEL) ||
                              nextScene is "GG_Vengefly_V" or "GG_Boss_Door_Entrance" or "GG_Entrance_Cutscene";

            } else if (Model.CurrentState.CurrentPhase == TimerPhase.Running && settings.Splits.Count > 0) {
                GameState gameState = mem.GameState();
                UIState uIState = mem.UIState();
                //SplitName finalSplit = settings.Splits[settings.Splits.Count - 1];

                if (!settings.AutosplitEndRuns) {
                    if (currentSplit + 1 < Model.CurrentState.Run.Count) {
                        if (!settings.Ordered) {
                            shouldSplit = NotOrderedSplits(gameState, uIState, nextScene, sceneName);
                        
                        } else if (currentSplit < settings.Splits.Count) {
                            shouldSplit = OrderedSplits(gameState, uIState, nextScene, sceneName);
                        }
                    } else {
                        shouldSplit = nextScene.StartsWith("Cinematic_Ending", StringComparison.OrdinalIgnoreCase) || nextScene == "GG_End_Sequence";
                    }
                } else {
                    if (currentSplit < Model.CurrentState.Run.Count) {
                        if (currentSplit + 1 == Model.CurrentState.Run.Count) {
                            shouldSplit = nextScene.StartsWith("Cinematic_Ending", StringComparison.OrdinalIgnoreCase) || nextScene == "GG_End_Sequence";
                        }
                        if (!shouldSplit) {
                            if (!settings.Ordered) {
                                shouldSplit = NotOrderedSplits(gameState, uIState, nextScene, sceneName);
                            } else if (currentSplit < settings.Splits.Count) {
                                shouldSplit = OrderedSplits(gameState, uIState, nextScene, sceneName);
                            }        
                        }
                    }
                }
                LoadRemoval(gameState, uIState, nextScene, sceneName);
            }
 
            HandleSplit(shouldSplit);
        }

        private void LoadRemoval(GameState gameState, UIState uIState, string nextScene, string sceneName) {
            uIState = mem.UIState();
            bool loadingMenu = (sceneName != "Menu_Title" && string.IsNullOrEmpty(nextScene)) 
                || (sceneName != "Menu_Title" && nextScene == "Menu_Title" 
                || (sceneName == "Quit_To_Menu"));
            if (gameState == GameState.PLAYING && lastGameState == GameState.MAIN_MENU) {
                lookForTeleporting = true;
            }
            bool teleporting = mem.CameraTeleporting();
            if (lookForTeleporting && (teleporting || (gameState != GameState.PLAYING && gameState != GameState.ENTERING_LEVEL))) {
                lookForTeleporting = false;
            }

            // TODO: look into Current Patch quitout issues.
            Model.CurrentState.IsGameTimePaused =
                (gameState == GameState.PLAYING && teleporting && !mem.HazardRespawning())
                || lookForTeleporting
                || ((gameState is GameState.PLAYING or GameState.ENTERING_LEVEL) && uIState != UIState.PLAYING)
                || (gameState != GameState.PLAYING && !mem.AcceptingInput())
                || gameState is GameState.EXITING_LEVEL or GameState.LOADING
                || mem.HeroTransitionState() == HeroTransitionState.WAITING_TO_ENTER_LEVEL
                || (uIState != UIState.PLAYING
                    && (loadingMenu || (uIState != UIState.PAUSED && (!string.IsNullOrEmpty(nextScene) || sceneName == "_test_charms")))
                    && nextScene != sceneName)
                || (mem.TileMapDirty() && !mem.UsesSceneTransitionRoutine())
                /* comment below parts out for release 
                || (mem.MenuState() == MainMenuState.EXIT_PROMPT && mem.GameState() == GameState.PAUSED && uIState == UIState.PAUSED
                    && (mem.CameraMode() is CameraMode.FROZEN or CameraMode.FOLLOWING) && (mem.GetCameraTargetMode() is TargetMode.FREE or TargetMode.FOLLOW_HERO))
                || (mem.MenuState() == MainMenuState.LOGO && mem.GameState() == GameState.INACTIVE && uIState == UIState.INACTIVE)
                */
                ;
            lastGameState = gameState;
        }

        private bool NotOrderedSplits(GameState gameState, UIState uIState, string nextScene, string sceneName) {

            foreach (SplitName split in settings.Splits) {
                if (splitsDone.Contains(split)) { 
                    continue; 
                } 
                else if (split.ToString().StartsWith("Menu")) {
                    if (!menuSplitHelper) 
                        menuSplitHelper = split == SplitName.Menu || 
                            CheckSplit(split, nextScene, sceneName) && !((gameState == GameState.INACTIVE && uIState == UIState.INACTIVE) || (gameState == GameState.MAIN_MENU));
                    if (menuSplitHelper) {
                        if (CheckSplit(SplitName.Menu, nextScene, sceneName)) {
                            splitsDone.Add(split);
                            lastSplitDone = split;
                            menuSplitHelper = false;
                            if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Split: " + split);
                            return true;
                        }
                    }
                }
                else if (!((gameState == GameState.INACTIVE && uIState == UIState.INACTIVE) || (gameState == GameState.MAIN_MENU))) {
                    if (CheckSplit(split, nextScene, sceneName)) {
                        splitsDone.Add(split);
                        lastSplitDone = split;
                        menuSplitHelper = false;
                        if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Split: " + split);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool OrderedSplits(GameState gameState, UIState uIState, string nextScene, string sceneName) {
            SplitName split = settings.Splits[currentSplit];

            if (split.ToString().StartsWith("Menu")) {
                if (!menuSplitHelper) menuSplitHelper = split == SplitName.Menu || CheckSplit(split, nextScene, sceneName);
                if (menuSplitHelper) {
                    if (CheckSplit(SplitName.Menu, nextScene, sceneName)) {
                        menuSplitHelper = false;
                        if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Split: " + split);
                        return true;
                    }
                }
            } else {
                if (CheckSplit(split, nextScene, sceneName)
                        && !((gameState == GameState.INACTIVE && uIState == UIState.INACTIVE) || (gameState == GameState.MAIN_MENU))) {
                    if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Split: " + split);
                    return true;
                }
            }
            return false;
        }
        

        private bool CheckSplit(SplitName split, string nextScene, string sceneName) {
            bool shouldSplit = false;

            switch (split) {
                case SplitName.Abyss: shouldSplit = mem.PlayerData<bool>(Offset.visitedAbyss); break;
                case SplitName.AbyssShriek: shouldSplit = mem.PlayerData<int>(Offset.screamLevel) == 2; break;
                case SplitName.Aluba: shouldSplit = mem.PlayerData<bool>(Offset.killedLazyFlyer); break;
                case SplitName.AspidHunter: shouldSplit = mem.PlayerData<int>(Offset.killsSpitter) == 17; break;
                case SplitName.BaldurShell: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_5); break;
                case SplitName.BeastsDenTrapBench: shouldSplit = mem.PlayerData<bool>(Offset.spiderCapture); break;
                case SplitName.BlackKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedBlackKnight); break;
                case SplitName.BrettaRescued: shouldSplit = mem.PlayerData<bool>(Offset.brettaRescued); break;
                case SplitName.BrummFlame: shouldSplit = mem.PlayerData<bool>(Offset.gotBrummsFlame); break;
                case SplitName.BrokenVessel: shouldSplit = mem.PlayerData<bool>(Offset.killedInfectedKnight); break;
                case SplitName.BroodingMawlek: shouldSplit = mem.PlayerData<bool>(Offset.killedMawlek); break;
                case SplitName.CityOfTears: shouldSplit = mem.PlayerData<bool>(Offset.visitedRuins); break;
                case SplitName.Collector: shouldSplit = mem.PlayerData<bool>(Offset.collectorDefeated); break;
                case SplitName.Colosseum: shouldSplit = mem.PlayerData<bool>(Offset.seenColosseumTitle); break;
                case SplitName.ColosseumBronze: shouldSplit = mem.PlayerData<bool>(Offset.colosseumBronzeCompleted); break;
                case SplitName.ColosseumGold: shouldSplit = mem.PlayerData<bool>(Offset.colosseumGoldCompleted); break;
                case SplitName.ColosseumSilver: shouldSplit = mem.PlayerData<bool>(Offset.colosseumSilverCompleted); break;
                case SplitName.CrossroadsStation: shouldSplit = mem.PlayerData<bool>(Offset.openedCrossroads); break;
                case SplitName.CrystalGuardian1: shouldSplit = mem.PlayerData<bool>(Offset.defeatedMegaBeamMiner); break;
                case SplitName.CrystalGuardian2: shouldSplit = mem.PlayerData<int>(Offset.killsMegaBeamMiner) == 0; break;
                case SplitName.CrystalHeart: shouldSplit = mem.PlayerData<bool>(Offset.hasSuperDash); break;
                case SplitName.CrystalPeak: shouldSplit = mem.PlayerData<bool>(Offset.visitedMines); break;
                case SplitName.CycloneSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasCyclone); break;
                case SplitName.Dashmaster: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_31); break;
                case SplitName.DashSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasUpwardSlash); break;
                case SplitName.DeepFocus: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_34); break;
                case SplitName.Deepnest: shouldSplit = mem.PlayerData<bool>(Offset.visitedDeepnest); break;
                case SplitName.DeepnestSpa: shouldSplit = mem.PlayerData<bool>(Offset.visitedDeepnestSpa); break;
                case SplitName.DeepnestStation: shouldSplit = mem.PlayerData<bool>(Offset.openedDeepnest); break;
                case SplitName.DefendersCrest: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_10); break;
                case SplitName.DescendingDark: shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 2; break;
                case SplitName.DesolateDive: shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 1; break;
                case SplitName.Dirtmouth: shouldSplit = mem.PlayerData<bool>(Offset.visitedDirtmouth); break;
                case SplitName.Dreamer1: shouldSplit = mem.PlayerData<int>(Offset.guardiansDefeated) == 1; break;
                case SplitName.Dreamer2: shouldSplit = mem.PlayerData<int>(Offset.guardiansDefeated) == 2; break;
                case SplitName.Dreamer3: shouldSplit = mem.PlayerData<int>(Offset.guardiansDefeated) == 3; break;
                case SplitName.DreamNail: shouldSplit = mem.PlayerData<bool>(Offset.hasDreamNail); break;
                case SplitName.DreamNail2: shouldSplit = mem.PlayerData<bool>(Offset.dreamNailUpgraded); break;
                case SplitName.DreamGate: shouldSplit = mem.PlayerData<bool>(Offset.hasDreamGate); break;
                case SplitName.Dreamshield: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_38); break;
                case SplitName.DreamWielder: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_30); break;
                case SplitName.DungDefender: shouldSplit = mem.PlayerData<bool>(Offset.killedDungDefender); break;
                case SplitName.ElderbugFlower: shouldSplit = mem.PlayerData<bool>(Offset.elderbugGaveFlower); break;
                case SplitName.ElderHu: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostHu); break;
                case SplitName.ElegantKey: shouldSplit = mem.PlayerData<bool>(Offset.hasWhiteKey); break;
                case SplitName.FailedKnight: shouldSplit = mem.PlayerData<bool>(Offset.falseKnightDreamDefeated); break;
                case SplitName.FalseKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedFalseKnight); break;
                case SplitName.Flukemarm: shouldSplit = mem.PlayerData<bool>(Offset.killedFlukeMother); break;
                case SplitName.Flukenest: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_11); break;
                case SplitName.FogCanyon: shouldSplit = mem.PlayerData<bool>(Offset.visitedFogCanyon); break;
                case SplitName.ForgottenCrossroads: shouldSplit = mem.PlayerData<bool>(Offset.visitedCrossroads); break;
                case SplitName.FragileGreed: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_24); break;
                case SplitName.FragileHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_23); break;
                case SplitName.FragileStrength: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_25); break;
                case SplitName.FungalWastes: shouldSplit = mem.PlayerData<bool>(Offset.visitedFungus); break;
                case SplitName.FuryOfTheFallen: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_6); break;
                case SplitName.Galien: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostGalien); break;
                case SplitName.GatheringSwarm: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_1); break;
                case SplitName.GlowingWomb: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_22); break;
                case SplitName.Godhome: shouldSplit = mem.PlayerData<bool>(Offset.visitedGodhome); break;
                case SplitName.GodTamer: shouldSplit = mem.PlayerData<bool>(Offset.killedLobsterLancer); break;
                case SplitName.GodTuner: shouldSplit = mem.PlayerData<bool>(Offset.hasGodfinder); break;
                case SplitName.Gorb: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostAladar); break;
                case SplitName.GorgeousHusk: shouldSplit = mem.PlayerData<bool>(Offset.killedGorgeousHusk); break;
                case SplitName.GreatSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasDashSlash); break;
                case SplitName.Greenpath: shouldSplit = mem.PlayerData<bool>(Offset.visitedGreenpath); break;
                case SplitName.GreenpathStation: shouldSplit = mem.PlayerData<bool>(Offset.openedGreenpath); break;
                case SplitName.Grimmchild: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_40); break;
                case SplitName.Grimmchild2: shouldSplit = mem.PlayerData<int>(Offset.grimmChildLevel) == 2; break;
                case SplitName.Grimmchild3: shouldSplit = mem.PlayerData<int>(Offset.grimmChildLevel) == 3; break;
                case SplitName.Grimmchild4: shouldSplit = mem.PlayerData<int>(Offset.grimmChildLevel) == 4; break;
                case SplitName.GrubberflysElegy: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_35); break;
                case SplitName.Grubsong: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_3); break;
                case SplitName.GreatHopper: shouldSplit = mem.PlayerData<bool>(Offset.killedGiantHopper); break;
                case SplitName.GreyPrince: shouldSplit = mem.PlayerData<bool>(Offset.killedGreyPrince); break;
                case SplitName.GruzMother: shouldSplit = mem.PlayerData<bool>(Offset.killedBigFly); break;
                case SplitName.HeavyBlow: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_15); break;
                case SplitName.Hegemol: shouldSplit = mem.PlayerData<bool>(Offset.maskBrokenHegemol); break;
                case SplitName.HiddenStationStation: shouldSplit = mem.PlayerData<bool>(Offset.openedHiddenStation); break;
                case SplitName.Hive: shouldSplit = mem.PlayerData<bool>(Offset.visitedHive); break;
                case SplitName.Hiveblood: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_29); break;
                case SplitName.HollowKnightDreamnail: shouldSplit = nextScene.Equals("Dream_Final_Boss", StringComparison.OrdinalIgnoreCase); break;
                case SplitName.HollowKnightBoss: shouldSplit = mem.PlayerData<bool>(Offset.killedHollowKnight); break;
                case SplitName.RadianceBoss: shouldSplit = mem.PlayerData<bool>(Offset.killedFinalBoss); break;
                case SplitName.Hornet1: shouldSplit = mem.PlayerData<bool>(Offset.killedHornet); break;
                case SplitName.Hornet2: shouldSplit = mem.PlayerData<bool>(Offset.hornetOutskirtsDefeated); break;
                case SplitName.HowlingWraiths: shouldSplit = mem.PlayerData<int>(Offset.screamLevel) == 1; break;
                case SplitName.HuntersMark: shouldSplit = mem.PlayerData<bool>(Offset.killedHunterMark); break;
                case SplitName.HuskMiner: shouldSplit = mem.PlayerData<bool>(Offset.killedZombieMiner); break;
                case SplitName.InfectedCrossroads: shouldSplit = mem.PlayerData<bool>(Offset.crossroadsInfected) && mem.PlayerData<bool>(Offset.visitedCrossroads); break;
                case SplitName.IsmasTear: shouldSplit = mem.PlayerData<bool>(Offset.hasAcidArmour); break;
                case SplitName.JonisBlessing: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_27); break;
                case SplitName.KingdomsEdge: shouldSplit = mem.PlayerData<bool>(Offset.visitedOutskirts); break;
                case SplitName.KingsBrand: shouldSplit = mem.PlayerData<bool>(Offset.hasKingsBrand); break;
                case SplitName.Kingsoul: shouldSplit = mem.PlayerData<int>(Offset.charmCost_36) == 5 && mem.PlayerData<int>(Offset.royalCharmState) == 3; break;
                case SplitName.KingsStationStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRuins2); break;
                //case SplitName.Lemm1: shouldSplit = mem.PlayerData<bool>(Offset.metRelicDealer); break;
                case SplitName.Lemm2: shouldSplit = mem.PlayerData<bool>(Offset.metRelicDealerShop); break;
                case SplitName.LifebloodCore: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_9); break;
                case SplitName.LifebloodHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_8); break;
                case SplitName.LittleFool: shouldSplit = mem.PlayerData<bool>(Offset.littleFoolMet); break;
                case SplitName.Longnail: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_18); break;
                case SplitName.LostKin: shouldSplit = mem.PlayerData<bool>(Offset.infectedKnightDreamDefeated); break;
                case SplitName.LoveKey: shouldSplit = mem.PlayerData<bool>(Offset.hasLoveKey); break;
                case SplitName.LumaflyLantern: shouldSplit = mem.PlayerData<bool>(Offset.hasLantern); break;
                case SplitName.Lurien: shouldSplit = mem.PlayerData<bool>(Offset.maskBrokenLurien); break;
                case SplitName.MantisClaw: shouldSplit = mem.PlayerData<bool>(Offset.hasWallJump); break;
                case SplitName.MantisLords: shouldSplit = mem.PlayerData<bool>(Offset.defeatedMantisLords); break;
                case SplitName.MarkOfPride: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_13); break;
                case SplitName.Markoth: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostMarkoth); break;
                case SplitName.Marmu: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostMarmu); break;
                case SplitName.MaskFragment1: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 5 && mem.PlayerData<int>(Offset.heartPieces) == 1; break;
                case SplitName.MaskFragment2: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 5 && mem.PlayerData<int>(Offset.heartPieces) == 2; break;
                case SplitName.MaskFragment3: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 5 && mem.PlayerData<int>(Offset.heartPieces) == 3; break;
                case SplitName.Mask1: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 6; break;
                case SplitName.MaskFragment5: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 5 || (mem.PlayerData<int>(Offset.maxHealthBase) == 6 && mem.PlayerData<int>(Offset.heartPieces) == 1); break;
                case SplitName.MaskFragment6: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 6 || (mem.PlayerData<int>(Offset.maxHealthBase) == 6 && mem.PlayerData<int>(Offset.heartPieces) == 2); break;
                case SplitName.MaskFragment7: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 7 || (mem.PlayerData<int>(Offset.maxHealthBase) == 6 && mem.PlayerData<int>(Offset.heartPieces) == 3); break;
                case SplitName.Mask2: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 7; break;
                case SplitName.MaskFragment9: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 9 || (mem.PlayerData<int>(Offset.maxHealthBase) == 7 && mem.PlayerData<int>(Offset.heartPieces) == 1); break;
                case SplitName.MaskFragment10: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 10 || (mem.PlayerData<int>(Offset.maxHealthBase) == 7 && mem.PlayerData<int>(Offset.heartPieces) == 2); break;
                case SplitName.MaskFragment11: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 11 || (mem.PlayerData<int>(Offset.maxHealthBase) == 7 && mem.PlayerData<int>(Offset.heartPieces) == 3); break;
                case SplitName.Mask3: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 8; break;
                case SplitName.MaskFragment13: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 13 || (mem.PlayerData<int>(Offset.maxHealthBase) == 8 && mem.PlayerData<int>(Offset.heartPieces) == 1); break;
                case SplitName.MaskFragment14: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 14 || (mem.PlayerData<int>(Offset.maxHealthBase) == 8 && mem.PlayerData<int>(Offset.heartPieces) == 2); break;
                case SplitName.MaskFragment15: shouldSplit = mem.PlayerData<int>(Offset.heartPieces) == 15 || (mem.PlayerData<int>(Offset.maxHealthBase) == 8 && mem.PlayerData<int>(Offset.heartPieces) == 3); break;
                case SplitName.Mask4: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 9; break;
                case SplitName.MatoOroNailBros: shouldSplit = mem.PlayerData<bool>(Offset.killedNailBros); break;
                case SplitName.MegaMossCharger: shouldSplit = mem.PlayerData<bool>(Offset.megaMossChargerDefeated); break;
                case SplitName.MenderBug: shouldSplit = mem.PlayerData<bool>(Offset.killedMenderBug); break;
                case SplitName.MonarchWings: shouldSplit = mem.PlayerData<bool>(Offset.hasDoubleJump); break;
                case SplitName.Monomon: shouldSplit = mem.PlayerData<bool>(Offset.maskBrokenMonomon); break;
                case SplitName.MossKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedMossKnight); break;
                case SplitName.MothwingCloak: shouldSplit = mem.PlayerData<bool>(Offset.hasDash); break;
                case SplitName.MrMushroom1: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 2; break;
                case SplitName.MrMushroom2: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 3; break;
                case SplitName.MrMushroom3: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 4; break;
                case SplitName.MrMushroom4: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 5; break;
                case SplitName.MrMushroom5: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 6; break;
                case SplitName.MrMushroom6: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 7; break;
                case SplitName.MrMushroom7: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 8; break;
                case SplitName.MushroomBrawler: shouldSplit = mem.PlayerData<int>(Offset.killsMushroomBrawler) == 6; break;
                case SplitName.NailmastersGlory: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_26); break;
                case SplitName.NailUpgrade1: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 1; break;
                case SplitName.NailUpgrade2: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 2; break;
                case SplitName.NailUpgrade3: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 3; break;
                case SplitName.NailUpgrade4: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 4; break;
                case SplitName.NightmareKingGrimm: shouldSplit = mem.PlayerData<bool>(Offset.killedNightmareGrimm); break;
                case SplitName.NightmareLantern: shouldSplit = mem.PlayerData<bool>(Offset.nightmareLanternLit); break;
                case SplitName.NightmareLanternDestroyed: shouldSplit = mem.PlayerData<bool>(Offset.destroyedNightmareLantern); break;
                case SplitName.NoEyes: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostNoEyes); break;
                case SplitName.Nosk: shouldSplit = mem.PlayerData<bool>(Offset.killedMimicSpider); break;
                case SplitName.NotchFogCanyon: shouldSplit = mem.PlayerData<bool>(Offset.notchFogCanyon); break;
                case SplitName.NotchSalubra1: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch1); break;
                case SplitName.NotchSalubra2: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch2); break;
                case SplitName.NotchSalubra3: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch3); break;
                case SplitName.NotchSalubra4: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch4); break;
                case SplitName.NotchShrumalOgres: shouldSplit = mem.PlayerData<bool>(Offset.notchShroomOgres); break;
                case SplitName.PaleLurkerKey: shouldSplit = mem.PlayerData<bool>(Offset.gotLurkerKey); break;
                case SplitName.PaleOre: shouldSplit = mem.PlayerData<int>(Offset.ore) > 0; break;
                case SplitName.Pantheon1: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier1); break;
                case SplitName.Pantheon2: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier2); break;
                case SplitName.Pantheon3: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier3); break;
                case SplitName.Pantheon4: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier4); break;
                case SplitName.Pantheon5: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier5); break;
                case SplitName.PathOfPain: shouldSplit = mem.PlayerData<bool>(Offset.newDataBindingSeal); break;
                case SplitName.PureVessel: shouldSplit = mem.PlayerData<bool>(Offset.killedHollowKnightPrime); break;
                case SplitName.QueensGardens: shouldSplit = mem.PlayerData<bool>(Offset.visitedRoyalGardens); break;
                case SplitName.QueensGardensStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRoyalGardens); break;
                case SplitName.QueensStationStation: shouldSplit = mem.PlayerData<bool>(Offset.openedFungalWastes); break;
                case SplitName.QuickSlash: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_32); break;
                case SplitName.QuickFocus: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_7); break;
                case SplitName.RestingGrounds: shouldSplit = mem.PlayerData<bool>(Offset.visitedRestingGrounds); break;
                case SplitName.RestingGroundsStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRestingGrounds); break;
                case SplitName.RoyalWaterways: shouldSplit = mem.PlayerData<bool>(Offset.visitedWaterways); break;
                case SplitName.SalubrasBlessing: shouldSplit = mem.PlayerData<bool>(Offset.salubraBlessing); break;
                case SplitName.SeerDeparts: shouldSplit = mem.PlayerData<bool>(Offset.mothDeparted); break;
                case SplitName.ShadeCloak: shouldSplit = mem.PlayerData<bool>(Offset.hasShadowDash); break;
                case SplitName.ShadeSoul: shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 2; break;
                case SplitName.ShamanStone: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_19); break;
                case SplitName.ShapeOfUnn: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_28); break;
                case SplitName.SharpShadow: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_16); break;
                case SplitName.SheoPaintmaster: shouldSplit = mem.PlayerData<bool>(Offset.killedPaintmaster); break;
                case SplitName.SimpleKey: shouldSplit = mem.PlayerData<int>(Offset.simpleKeys) > 0; break;
                case SplitName.SlyKey: shouldSplit = mem.PlayerData<bool>(Offset.hasSlykey); break;
                case SplitName.SlyNailsage: shouldSplit = mem.PlayerData<bool>(Offset.killedNailsage); break;
                case SplitName.SoulCatcher: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_20); break;
                case SplitName.SoulEater: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_21); break;
                case SplitName.SoulMaster: shouldSplit = mem.PlayerData<bool>(Offset.killedMageLord); break;
                case SplitName.SoulTyrant: shouldSplit = mem.PlayerData<bool>(Offset.mageLordDreamDefeated); break;
                case SplitName.SpellTwister: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_33); break;
                case SplitName.SporeShroom: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_17); break;
                case SplitName.SpiritGladeOpen: shouldSplit = mem.PlayerData<bool>(Offset.gladeDoorOpened); break;
                case SplitName.Sprintmaster: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_37); break;
                case SplitName.StalwartShell: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_4); break;
                case SplitName.StagnestStation: shouldSplit = nextScene.Equals("Cliffs_03", StringComparison.OrdinalIgnoreCase) 
                                                              && mem.PlayerData<bool>(Offset.travelling) 
                                                              && mem.PlayerData<bool>(Offset.openedStagNest); break;
                case SplitName.SteadyBody: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_14); break;
                case SplitName.StoreroomsStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRuins1); break;
                case SplitName.TeachersArchive: shouldSplit = sceneName.Equals("Fungus3_archive", StringComparison.OrdinalIgnoreCase); break;
                case SplitName.ThornsOfAgony: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_12); break;
                case SplitName.TraitorLord: shouldSplit = mem.PlayerData<bool>(Offset.killedTraitorLord); break;
                case SplitName.TramPass: shouldSplit = mem.PlayerData<bool>(Offset.hasTramPass); break;
                case SplitName.TroupeMasterGrimm: shouldSplit = mem.PlayerData<bool>(Offset.killedGrimm); break;
                case SplitName.UnchainedHollowKnight: shouldSplit = mem.PlayerData<bool>(Offset.unchainedHollowKnight); break;
                case SplitName.Uumuu: shouldSplit = mem.PlayerData<bool>(Offset.killedMegaJellyfish); break;
                case SplitName.VengefulSpirit: shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 1; break;
                case SplitName.VesselFragment1: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 0 && mem.PlayerData<int>(Offset.vesselFragments) == 1; break;
                case SplitName.VesselFragment2: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 0 && mem.PlayerData<int>(Offset.vesselFragments) == 2; break;
                case SplitName.Vessel1: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 33; break;
                case SplitName.VesselFragment4: shouldSplit = mem.PlayerData<int>(Offset.vesselFragments) == 4 || (mem.PlayerData<int>(Offset.MPReserveMax) == 33 && mem.PlayerData<int>(Offset.vesselFragments) == 1); break;
                case SplitName.VesselFragment5: shouldSplit = mem.PlayerData<int>(Offset.vesselFragments) == 5 || (mem.PlayerData<int>(Offset.MPReserveMax) == 33 && mem.PlayerData<int>(Offset.vesselFragments) == 2); break;
                case SplitName.Vessel2: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 66; break;
                case SplitName.VesselFragment7: shouldSplit = mem.PlayerData<int>(Offset.vesselFragments) == 7 || (mem.PlayerData<int>(Offset.MPReserveMax) == 66 && mem.PlayerData<int>(Offset.vesselFragments) == 1); break;
                case SplitName.VesselFragment8: shouldSplit = mem.PlayerData<int>(Offset.vesselFragments) == 8 || (mem.PlayerData<int>(Offset.MPReserveMax) == 66 && mem.PlayerData<int>(Offset.vesselFragments) == 2); break;
                case SplitName.Vessel3: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 99; break;
                case SplitName.VoidHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotShadeCharm); break;
                case SplitName.WatcherChandelier: shouldSplit = mem.PlayerData<bool>(Offset.watcherChandelier); break;
                case SplitName.WaywardCompass: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_2); break;
                case SplitName.Weaversong: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_39); break;
                case SplitName.WhiteDefender: shouldSplit = mem.PlayerData<bool>(Offset.killedWhiteDefender); break;
                case SplitName.WhitePalace: shouldSplit = mem.PlayerData<bool>(Offset.visitedWhitePalace); break;
                case SplitName.Xero: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostXero); break;
                case SplitName.Zote1: shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedBuzzer); break;
                case SplitName.Zote2: shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedDeepnest); break;
                case SplitName.ZoteKilled: shouldSplit = mem.PlayerData<bool>(Offset.killedZote); break;

                case SplitName.Flame1: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 1; break;
                case SplitName.Flame2: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 2; break;
                case SplitName.Flame3: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 3; break;

                case SplitName.HiveKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedHiveKnight); break;

                case SplitName.Ore1:
                case SplitName.Ore2:
                case SplitName.Ore3:
                case SplitName.Ore4:
                case SplitName.Ore5:
                case SplitName.Ore6:
                    int upgrades = mem.PlayerData<int>(Offset.nailSmithUpgrades);
                    int oreFromUpgrades = (upgrades * (upgrades - 1)) / 2;
                    int ore = oreFromUpgrades + mem.PlayerData<int>(Offset.ore);

                    switch (split) {
                        case SplitName.Ore1: shouldSplit = ore == 1; break;
                        case SplitName.Ore2: shouldSplit = ore == 2; break;
                        case SplitName.Ore3: shouldSplit = ore == 3; break;
                        case SplitName.Ore4: shouldSplit = ore == 4; break;
                        case SplitName.Ore5: shouldSplit = ore == 5; break;
                        case SplitName.Ore6: shouldSplit = ore == 6; break;
                    }

                    break;

                case SplitName.Grub1: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 1; break;
                case SplitName.Grub2: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 2; break;
                case SplitName.Grub3: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 3; break;
                case SplitName.Grub4: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 4; break;
                case SplitName.Grub5: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 5; break;
                case SplitName.Grub6: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 6; break;
                case SplitName.Grub7: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 7; break;
                case SplitName.Grub8: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 8; break;
                case SplitName.Grub9: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 9; break;
                case SplitName.Grub10: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 10; break;
                case SplitName.Grub11: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 11; break;
                case SplitName.Grub12: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 12; break;
                case SplitName.Grub13: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 13; break;
                case SplitName.Grub14: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 14; break;
                case SplitName.Grub15: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 15; break;
                case SplitName.Grub16: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 16; break;
                case SplitName.Grub17: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 17; break;
                case SplitName.Grub18: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 18; break;
                case SplitName.Grub19: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 19; break;
                case SplitName.Grub20: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 20; break;
                case SplitName.Grub21: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 21; break;
                case SplitName.Grub22: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 22; break;
                case SplitName.Grub23: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 23; break;
                case SplitName.Grub24: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 24; break;
                case SplitName.Grub25: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 25; break;
                case SplitName.Grub26: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 26; break;
                case SplitName.Grub27: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 27; break;
                case SplitName.Grub28: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 28; break;
                case SplitName.Grub29: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 29; break;
                case SplitName.Grub30: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 30; break;
                case SplitName.Grub31: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 31; break;
                case SplitName.Grub32: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 32; break;
                case SplitName.Grub33: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 33; break;
                case SplitName.Grub34: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 34; break;
                case SplitName.Grub35: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 35; break;
                case SplitName.Grub36: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 36; break;
                case SplitName.Grub37: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 37; break;
                case SplitName.Grub38: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 38; break;
                case SplitName.Grub39: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 39; break;
                case SplitName.Grub40: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 40; break;
                case SplitName.Grub41: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 41; break;
                case SplitName.Grub42: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 42; break;
                case SplitName.Grub43: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 43; break;
                case SplitName.Grub44: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 44; break;
                case SplitName.Grub45: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 45; break;
                case SplitName.Grub46: shouldSplit = mem.PlayerData<int>(Offset.grubsCollected) == 46; break;

                case SplitName.TreeCity: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Ruins1_17"); break;
                case SplitName.TreeCliffs: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Cliffs_01"); break;
                case SplitName.TreeCrossroads: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Crossroads_07"); break;
                case SplitName.TreeDeepnest: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Deepnest_39"); break;
                case SplitName.TreeGlade: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("RestingGrounds_08"); break;
                case SplitName.TreeGreenpath: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Fungus1_13"); break;
                case SplitName.TreeHive: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Hive_02"); break;
                case SplitName.TreeKingdomsEdge: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Deepnest_East_07"); break;
                case SplitName.TreeLegEater: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Fungus2_33"); break;
                case SplitName.TreeMantisVillage: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Fungus2_17"); break;
                case SplitName.TreeMound: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Crossroads_ShamanTemple"); break;
                case SplitName.TreePeak: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Mines_23"); break;
                case SplitName.TreeQueensGardens: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Fungus3_11"); break;
                case SplitName.TreeRestingGrounds: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("RestingGrounds_05"); break;
                case SplitName.TreeWaterways: shouldSplit = mem.PlayerDataStringList(Offset.scenesEncounteredDreamPlantC).Contains("Abyss_01"); break;

                case SplitName.KingsPass: shouldSplit = sceneName.StartsWith("Tutorial_01") && nextScene.StartsWith("Town"); break;
                case SplitName.KingsPassEnterFromTown: shouldSplit = sceneName.StartsWith("Town") && nextScene.StartsWith("Tutorial_01"); break;
                case SplitName.BlueLake: shouldSplit = !sceneName.StartsWith("Crossroads_50") && nextScene.StartsWith("Crossroads_50"); break;

                case SplitName.VengeflyKingP: shouldSplit = sceneName.StartsWith("GG_Vengefly") && nextScene.StartsWith("GG_Gruz_Mother"); break;
                case SplitName.GruzMotherP: shouldSplit = sceneName.StartsWith("GG_Gruz_Mother") && nextScene.StartsWith("GG_False_Knight"); break;
                case SplitName.FalseKnightP: shouldSplit = sceneName.StartsWith("GG_False_Knight") && nextScene.StartsWith("GG_Mega_Moss_Charger"); break;
                case SplitName.MassiveMossChargerP: shouldSplit = sceneName.StartsWith("GG_Mega_Moss_Charger") && nextScene.StartsWith("GG_Hornet_1"); break;
                case SplitName.Hornet1P: shouldSplit = sceneName.StartsWith("GG_Hornet_1") && (nextScene == "GG_Spa" || nextScene == "GG_Engine"); break;
                case SplitName.GorbP: shouldSplit = sceneName.StartsWith("GG_Ghost_Gorb") && nextScene.StartsWith("GG_Dung_Defender"); break;
                case SplitName.DungDefenderP: shouldSplit = sceneName.StartsWith("GG_Dung_Defender") && nextScene.StartsWith("GG_Mage_Knight"); break;
                case SplitName.SoulWarriorP: shouldSplit = sceneName.StartsWith("GG_Mage_Knight") && nextScene.StartsWith("GG_Brooding_Mawlek"); break;
                case SplitName.BroodingMawlekP: shouldSplit = sceneName.StartsWith("GG_Brooding_Mawlek") && (nextScene == "GG_Engine" || nextScene.StartsWith("GG_Nailmasters")); break;
                case SplitName.OroMatoNailBrosP: shouldSplit = sceneName.StartsWith("GG_Nailmasters") && (nextScene == "GG_End_Sequence" || nextScene == "GG_Spa"); break;

                case SplitName.XeroP: shouldSplit = sceneName.StartsWith("GG_Ghost_Xero") && nextScene.StartsWith("GG_Crystal_Guardian"); break;
                case SplitName.CrystalGuardianP: shouldSplit = sceneName.StartsWith("GG_Crystal_Guardian") && nextScene.StartsWith("GG_Soul_Master"); break;
                case SplitName.SoulMasterP: shouldSplit = sceneName.StartsWith("GG_Soul_Master") && nextScene.StartsWith("GG_Oblobbles"); break;
                case SplitName.OblobblesP: shouldSplit = sceneName.StartsWith("GG_Oblobbles") && nextScene.StartsWith("GG_Mantis_Lords"); break;
                case SplitName.MantisLordsP: shouldSplit = sceneName.StartsWith("GG_Mantis_Lords") && nextScene == "GG_Spa"; break;
                case SplitName.MarmuP: shouldSplit = sceneName.StartsWith("GG_Ghost_Marmu") && (nextScene.StartsWith("GG_Nosk") || nextScene.StartsWith("GG_Flukemarm")); break;
                case SplitName.NoskP: shouldSplit = sceneName.StartsWith("GG_Nosk") && nextScene.StartsWith("GG_Flukemarm"); break;
                case SplitName.FlukemarmP: shouldSplit = sceneName.StartsWith("GG_Flukemarm") && nextScene.StartsWith("GG_Broken_Vessel"); break;
                case SplitName.BrokenVesselP: shouldSplit = sceneName.StartsWith("GG_Broken_Vessel") && (nextScene == "GG_Engine" || nextScene.StartsWith("GG_Ghost_Galien")); break;
                case SplitName.SheoPaintmasterP: shouldSplit = sceneName.StartsWith("GG_Painter") && (nextScene == "GG_End_Sequence" || nextScene == "GG_Spa"); break;

                case SplitName.HiveKnightP: shouldSplit = sceneName.StartsWith("GG_Hive_Knight") && nextScene.StartsWith("GG_Ghost_Hu"); break;
                case SplitName.ElderHuP: shouldSplit = sceneName.StartsWith("GG_Ghost_Hu") && nextScene.StartsWith("GG_Collector"); break;
                case SplitName.CollectorP: shouldSplit = sceneName.StartsWith("GG_Collector") && nextScene.StartsWith("GG_God_Tamer"); break;
                case SplitName.GodTamerP: shouldSplit = sceneName.StartsWith("GG_God_Tamer") && nextScene.StartsWith("GG_Grimm"); break;
                case SplitName.TroupeMasterGrimmP: shouldSplit = sceneName.StartsWith("GG_Grimm") && nextScene == "GG_Spa"; break;
                case SplitName.GalienP: shouldSplit = sceneName.StartsWith("GG_Ghost_Galien") && (nextScene.StartsWith("GG_Grey_Prince_Zote") || nextScene.StartsWith("GG_Painter")); break;
                case SplitName.GreyPrinceZoteP: shouldSplit = sceneName.StartsWith("GG_Grey_Prince_Zote") && (nextScene.StartsWith("GG_Uumuu") || nextScene.StartsWith("GG_Failed_Champion")); break;
                case SplitName.UumuuP: shouldSplit = sceneName.StartsWith("GG_Uumuu") && (nextScene.StartsWith("GG_Hornet_2") || nextScene.StartsWith("GG_Nosk_Hornet")); break;
                case SplitName.Hornet2P: shouldSplit = sceneName.StartsWith("GG_Hornet_2") && (nextScene == "GG_Engine" || nextScene == "GG_Spa"); break;
                case SplitName.SlyP: shouldSplit = sceneName.StartsWith("GG_Sly") && (nextScene == "GG_End_Sequence" || nextScene.StartsWith("GG_Hornet_2")); break;

                case SplitName.EnragedGuardianP: shouldSplit = sceneName.StartsWith("GG_Crystal_Guardian_2") && nextScene.StartsWith("GG_Lost_Kin"); break;
                case SplitName.LostKinP: shouldSplit = sceneName.StartsWith("GG_Lost_Kin") && nextScene.StartsWith("GG_Ghost_No_Eyes"); break;
                case SplitName.NoEyesP: shouldSplit = sceneName.StartsWith("GG_Ghost_No_Eyes") && nextScene.StartsWith("GG_Traitor_Lord"); break;
                case SplitName.TraitorLordP: shouldSplit = sceneName.StartsWith("GG_Traitor_Lord") && nextScene.StartsWith("GG_White_Defender"); break;
                case SplitName.WhiteDefenderP: shouldSplit = sceneName.StartsWith("GG_White_Defender") && nextScene == "GG_Spa"; break;
                case SplitName.FailedChampionP: shouldSplit = sceneName.StartsWith("GG_Failed_Champion") && (nextScene.StartsWith("GG_Ghost_Markoth") || nextScene.StartsWith("GG_Grimm_Nightmare")); break;
                case SplitName.MarkothP: shouldSplit = sceneName.StartsWith("GG_Ghost_Markoth") && (nextScene.StartsWith("GG_Watcher_Knights") || nextScene.StartsWith("GG_Grey_Prince_Zote")); break;
                case SplitName.WatcherKnightsP: shouldSplit = sceneName.StartsWith("GG_Watcher_Knights") && (nextScene.StartsWith("GG_Soul_Tyrant") || nextScene.StartsWith("GG_Uumuu")); break;
                case SplitName.SoulTyrantP: shouldSplit = sceneName.StartsWith("GG_Soul_Tyrant") && (nextScene == "GG_Engine_Prime" || nextScene.StartsWith("GG_Ghost_Markoth")); break;
                case SplitName.PureVesselP: shouldSplit = sceneName.StartsWith("GG_Hollow_Knight") && (nextScene == "GG_End_Sequence" || nextScene.StartsWith("GG_Radiance") || nextScene == "GG_Door_5_Finale"); break;

                case SplitName.NoskHornetP: shouldSplit = sceneName.StartsWith("GG_Nosk_Hornet") && nextScene.StartsWith("GG_Sly"); break;
                case SplitName.NightmareKingGrimmP: shouldSplit = sceneName.StartsWith("GG_Grimm_Nightmare") && nextScene == "GG_Spa"; break;
                
                case SplitName.WhitePalaceOrb1: shouldSplit = mem.PlayerData<bool>(Offset.whitePalaceOrb_1); break;
                case SplitName.WhitePalaceOrb2: shouldSplit = mem.PlayerData<bool>(Offset.whitePalaceOrb_2); break;
                case SplitName.WhitePalaceOrb3: shouldSplit = mem.PlayerData<bool>(Offset.whitePalaceOrb_3); break;
                case SplitName.WhitePalaceSecretRoom: shouldSplit = mem.PlayerData<bool>(Offset.whitePalaceSecretRoomVisited); break;

                case SplitName.WhitePalaceLeftEntry: shouldSplit = nextScene.StartsWith("White_Palace_04") && nextScene != sceneName; break;
                case SplitName.WhitePalaceLeftWingMid: shouldSplit = sceneName.StartsWith("White_Palace_04") && nextScene.StartsWith("White_Palace_14"); break;
                case SplitName.WhitePalaceRightEntry: shouldSplit = nextScene.StartsWith("White_Palace_15") && nextScene != sceneName; break;
                case SplitName.WhitePalaceRightClimb: shouldSplit = sceneName.StartsWith("White_Palace_05") && nextScene.StartsWith("White_Palace_16"); break;
                case SplitName.WhitePalaceRightSqueeze: shouldSplit = sceneName.StartsWith("White_Palace_16") && nextScene.StartsWith("White_Palace_05"); break;
                case SplitName.WhitePalaceRightDone: shouldSplit = sceneName.StartsWith("White_Palace_05") && nextScene.StartsWith("White_Palace_15"); break;
                case SplitName.WhitePalaceTopEntry: shouldSplit = sceneName.StartsWith("White_Palace_03_hub") && nextScene.StartsWith("White_Palace_06"); break;
                case SplitName.WhitePalaceTopClimb: shouldSplit = sceneName.StartsWith("White_Palace_06") && nextScene.StartsWith("White_Palace_07"); break;
                case SplitName.WhitePalaceTopLeverRoom: shouldSplit = sceneName.StartsWith("White_Palace_07") && nextScene.StartsWith("White_Palace_12"); break;
                case SplitName.WhitePalaceTopLastPlats: shouldSplit = sceneName.StartsWith("White_Palace_12") && nextScene.StartsWith("White_Palace_13"); break;
                case SplitName.WhitePalaceThroneRoom: shouldSplit = sceneName.StartsWith("White_Palace_13") && nextScene.StartsWith("White_Palace_09"); break;
                case SplitName.WhitePalaceAtrium: shouldSplit = nextScene.StartsWith("White_Palace_03_hub") && nextScene != sceneName; break;

                case SplitName.PathOfPainEntry: shouldSplit = nextScene.StartsWith("White_Palace_18") && nextScene != sceneName; break;
                case SplitName.PathOfPainTransition1: shouldSplit = nextScene.StartsWith("White_Palace_17") && nextScene != sceneName; break;
                case SplitName.PathOfPainTransition2: shouldSplit = nextScene.StartsWith("White_Palace_19") && nextScene != sceneName; break;
                case SplitName.PathOfPainTransition3: shouldSplit = nextScene.StartsWith("White_Palace_20") && nextScene != sceneName; break;

                case SplitName.WhiteFragmentLeft: shouldSplit = mem.PlayerData<bool>(Offset.gotQueenFragment); break;
                case SplitName.WhiteFragmentRight: shouldSplit = mem.PlayerData<bool>(Offset.gotKingFragment); break;
                
                case SplitName.TollBenchQG: shouldSplit = mem.PlayerData<bool>(Offset.tollBenchQueensGardens); break;
                case SplitName.TollBenchCity: shouldSplit = mem.PlayerData<bool>(Offset.tollBenchCity); break;
                case SplitName.TollBenchBasin: shouldSplit = mem.PlayerData<bool>(Offset.tollBenchAbyss); break;

                case SplitName.CityGateOpen: shouldSplit = mem.PlayerData<bool>(Offset.openedCityGate); break;
                case SplitName.NailsmithKilled: shouldSplit = mem.PlayerData<bool>(Offset.nailsmithKilled); break;

                /*
                 case SplitName.NailsmithSpared: shouldSplit = mem.PlayerData<bool>(Offset.nailsmithSpared); break;
            case SplitName.MageDoor: shouldSplit = mem.PlayerData<bool>(Offset.openedMageDoor); break;
            case SplitName.MageWindow: shouldSplit = mem.PlayerData<bool>(Offset.brokenMageWindow); break;
            case SplitName.MageLordEncountered: shouldSplit = mem.PlayerData<bool>(Offset.mageLordEncountered); break;
            case SplitName.MageDoor2: shouldSplit = mem.PlayerData<bool>(Offset.openedMageDoor_v2); break;
            case SplitName.MageWindowGlass: shouldSplit = mem.PlayerData<bool>(Offset.brokenMageWindowGlass); break;
            case SplitName.MageLordEncountered2: shouldSplit = mem.PlayerData<bool>(Offset.mageLordEncountered_2); break;
                */
                case SplitName.TramDeepnest: shouldSplit = mem.PlayerData<bool>(Offset.openedTramLower); break;
                case SplitName.WaterwaysManhole: shouldSplit = mem.PlayerData<bool>(Offset.openedWaterwaysManhole); break;
                case SplitName.NotchGrimm: shouldSplit = mem.PlayerData<bool>(Offset.gotGrimmNotch); break;
                //case SplitName.NotchSly1: shouldSplit = mem.PlayerData<bool>(Offset.slyNotch1); break;
                //case SplitName.NotchSly2: shouldSplit = mem.PlayerData<bool>(Offset.slyNotch2); break;
                case SplitName.SlyRescued: shouldSplit = mem.PlayerData<bool>(Offset.slyRescued); break;
                
                case SplitName.FlowerQuest: shouldSplit = mem.PlayerData<bool>(Offset.xunFlowerGiven); break;
                case SplitName.CityKey: shouldSplit = mem.PlayerData<bool>(Offset.hasCityKey); break;
                //case SplitName.Al2ba: shouldSplit = mem.PlayerData<int>(Offset.killsLazyFlyer) == 2; break;
                //case SplitName.Revek: shouldSplit = mem.PlayerData<int>(Offset.gladeGhostsKilled) == 19; break;
                //case SplitName.EquippedFragileHealth: shouldSplit = mem.PlayerData<bool>(Offset.equippedCharm_23); break;
                case SplitName.CanOvercharm: shouldSplit = mem.PlayerData<bool>(Offset.canOvercharm); break;
                case SplitName.MetGreyMourner: shouldSplit = mem.PlayerData<bool>(Offset.metXun); break;
                case SplitName.GreyMournerSeerAscended:
                    shouldSplit = mem.PlayerData<bool>(Offset.metXun) && mem.PlayerData<bool>(Offset.mothDeparted);
                    break;
                case SplitName.HasDelicateFlower: shouldSplit = mem.PlayerData<bool>(Offset.hasXunFlower); break;

                //case SplitName.AreaTestingSanctum: shouldSplit = mem.PlayerData<int>(Offset.currentArea) == (int)MapZone.SOUL_SOCIETY; break;
                //case SplitName.AreaTestingSanctumUpper: shouldSplit = mem.PlayerData<int>(Offset.currentArea) == (int)MapZone.MAGE_TOWER; break;

                case SplitName.killedSanctumWarrior: shouldSplit = mem.PlayerData<bool>(Offset.killedMageKnight); break;
                case SplitName.killedSoulTwister: shouldSplit = mem.PlayerData<bool>(Offset.killedMage); break;

                case SplitName.EnterNKG: shouldSplit = sceneName.StartsWith("Grimm_Main_Tent") && nextScene.StartsWith("Grimm_Nightmare"); break;
                case SplitName.EnterGreenpath: shouldSplit = !sceneName.StartsWith("Fungus1_01") && nextScene.StartsWith("Fungus1_01"); break;
                case SplitName.EnterGreenpathWithOvercharm: 
                    shouldSplit = !sceneName.StartsWith("Fungus1_01") 
                        && nextScene.StartsWith("Fungus1_01") 
                        && mem.PlayerData<bool>(Offset.canOvercharm); 
                    break;
                case SplitName.EnterSanctum: shouldSplit = !sceneName.StartsWith("Ruins1_23") && nextScene.StartsWith("Ruins1_23"); break;
                case SplitName.EnterSanctumWithShadeSoul: 
                    shouldSplit = !sceneName.StartsWith("Ruins1_23") 
                        && nextScene.StartsWith("Ruins1_23") 
                        && mem.PlayerData<int>(Offset.fireballLevel) == 2; 
                    break;
                case SplitName.EnterAnyDream: shouldSplit = nextScene.StartsWith("Dream_") && nextScene != sceneName; break;
                case SplitName.DgateKingdomsEdgeAcid:
                    shouldSplit =
                        mem.PlayerDataString<string>(Offset.dreamGateScene).StartsWith("Deepnest_East_04") &&
                        (mem.PlayerData<float>(Offset.dreamGateX) > 27.0f && mem.PlayerData<float>(Offset.dreamGateX) < 29f) &&
                        (mem.PlayerData<float>(Offset.dreamGateY) > 7.0f && mem.PlayerData<float>(Offset.dreamGateY) < 9f);
                    break;

                case SplitName.FailedChampionEssence: shouldSplit = mem.PlayerData<bool>(Offset.falseKnightOrbsCollected); break;
                case SplitName.SoulTyrantEssence: shouldSplit = mem.PlayerData<bool>(Offset.mageLordOrbsCollected); break;
                case SplitName.LostKinEssence: shouldSplit = mem.PlayerData<bool>(Offset.infectedKnightOrbsCollected); break;
                case SplitName.WhiteDefenderEssence: shouldSplit = mem.PlayerData<bool>(Offset.whiteDefenderOrbsCollected); break;
                case SplitName.GreyPrinceEssence: shouldSplit = mem.PlayerData<bool>(Offset.greyPrinceOrbsCollected); break;

                case SplitName.PreGrimmShop:
                    shouldSplit = mem.PlayerData<bool>(Offset.hasLantern) &&
                        mem.PlayerData<int>(Offset.maxHealthBase) == 6 &&
                        (mem.PlayerData<int>(Offset.vesselFragments) == 4 ||
                            (mem.PlayerData<int>(Offset.MPReserveMax) == 33 && mem.PlayerData<int>(Offset.vesselFragments) == 2));
                    break;
                case SplitName.ElderHuEssence: shouldSplit = mem.PlayerData<int>(Offset.elderHuDefeated) == 2; break;
                case SplitName.GalienEssence: shouldSplit = mem.PlayerData<int>(Offset.galienDefeated) == 2; break;
                case SplitName.GorbEssence: shouldSplit = mem.PlayerData<int>(Offset.aladarSlugDefeated) == 2; break;
                case SplitName.MarmuEssence: shouldSplit = mem.PlayerData<int>(Offset.mumCaterpillarDefeated) == 2; break;
                case SplitName.NoEyesEssence: shouldSplit = mem.PlayerData<int>(Offset.noEyesDefeated) == 2; break;
                case SplitName.XeroEssence: shouldSplit = mem.PlayerData<int>(Offset.xeroDefeated) == 2; break;
                case SplitName.MarkothEssence: shouldSplit = mem.PlayerData<int>(Offset.markothDefeated) == 2; break;

                case SplitName.DungDefenderIdol:
                    shouldSplit = mem.PlayerData<bool>(Offset.foundTrinket3) && sceneName.StartsWith("Waterways_15"); 
                    break;
                case SplitName.WaterwaysEntry: shouldSplit = nextScene.StartsWith("Waterways_01") && nextScene != sceneName; break;
                case SplitName.FogCanyonEntry: shouldSplit = nextScene.StartsWith("Fungus3_26") && nextScene != sceneName; break;
                case SplitName.SoulMasterEncountered: shouldSplit = mem.PlayerData<bool>(Offset.mageLordEncountered); break;

                //case SplitName.CrystalMoundExit: shouldSplit = sceneName.StartsWith("Mines_35") && nextScene != sceneName; break;
                case SplitName.CrystalPeakEntry: shouldSplit = nextScene.StartsWith("Mines_02") && nextScene != sceneName; break;
                case SplitName.QueensGardensEntry: shouldSplit = nextScene.StartsWith("Fungus3_34") && nextScene != sceneName; break;
                case SplitName.BasinEntry: shouldSplit = nextScene.StartsWith("Abyss_04") && nextScene != sceneName; break;
                case SplitName.HiveEntry: shouldSplit = nextScene.StartsWith("Hive_01") && nextScene != sceneName; break;
                case SplitName.KingdomsEdgeEntry: shouldSplit = nextScene.StartsWith("Deepnest_East_03") && nextScene != sceneName; break;
                case SplitName.KingdomsEdgeOvercharmedEntry: 
                    shouldSplit = 
                        nextScene.StartsWith("Deepnest_East_03") && 
                        nextScene != sceneName && 
                        mem.PlayerData<bool>(Offset.overcharmed); 
                    break;
                case SplitName.AllCharmNotchesLemm2CP: 
                    shouldSplit = 
                        mem.PlayerData<int>(Offset.soldTrinket1) == 1 &&
                        mem.PlayerData<int>(Offset.soldTrinket2) == 6 &&
                        mem.PlayerData<int>(Offset.soldTrinket3) == 4;
                    break;
                case SplitName.HappyCouplePlayerDataEvent: shouldSplit = mem.PlayerData<bool>(Offset.nailsmithConvoArt); break;
                case SplitName.GodhomeBench: shouldSplit = sceneName.StartsWith("GG_Spa") && sceneName != nextScene; break;
                
                case SplitName.Menu: shouldSplit = sceneName == "Menu_Title"; break;
                case SplitName.MenuClaw: shouldSplit = mem.PlayerData<bool>(Offset.hasWallJump); break;
                case SplitName.MenuGorgeousHusk: shouldSplit = mem.PlayerData<bool>(Offset.killedGorgeousHusk); break;
                case SplitName.TransClaw: shouldSplit = mem.PlayerData<bool>(Offset.hasWallJump) && nextScene != sceneName; break;
                case SplitName.TransGorgeousHusk: shouldSplit = mem.PlayerData<bool>(Offset.killedGorgeousHusk) && nextScene != sceneName; break;
                case SplitName.TransDescendingDark: shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 2 && nextScene != sceneName; break;
                case SplitName.PlayerDeath: shouldSplit = mem.PlayerData<int>(Offset.health) == 0; break;
                case SplitName.SlyShopFinished:
                    shouldSplit =
                        mem.PlayerData<int>(Offset.vesselFragments) == 8 || (mem.PlayerData<int>(Offset.MPReserveMax) == 66
                        && mem.PlayerData<int>(Offset.vesselFragments) == 2)
                        && !sceneName.StartsWith("Room_shop")
                        && mem.PlayerData<bool>(Offset.gotCharm_37);
                    break;
                case SplitName.ElegantKeyShoptimised:
                    shouldSplit =
                        shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 5 && mem.PlayerData<int>(Offset.heartPieces) == 1
                        && mem.PlayerData<bool>(Offset.hasWhiteKey);
                    break;
            }
            return shouldSplit;
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
                    pdata.UpdateData(mem, WriteLogWithTime);
                }

                string prev = "", curr = "";
                foreach (string key in keys) {
                    prev = currentValues[key];

                    switch (key) {
                        case "CurrentSplit":
                            curr = currentSplit.ToString();
                            break;
                        case "State": curr = state.ToString(); break;
                        case "GameState": curr = mem.GameState().ToString(); break;
                        case "SceneName": curr = mem.SceneName(); break;
                        case "NextSceneName": curr = mem.NextSceneName(); break;
                        case "MapZone": curr = ((MapZone)mem.PlayerData<int>(Offset.mapZone)).ToString(); break;
                        case "CameraMode": curr = mem.CameraMode().ToString(); break;
                        case "MenuState": curr = mem.MenuState().ToString(); break;
                        case "UIState": curr = mem.UIState().ToString(); break;
                        case "AcceptingInput": curr = mem.AcceptingInput().ToString(); break;
                        case "ActorState": curr = mem.HeroActorState().ToString(); break;
                        case "Loading":
                            GameState gameState = mem.GameState();
                            UIState uiState = mem.UIState();
                            string nextScene = mem.NextSceneName();
                            string sceneName = mem.SceneName();
                            bool loadingMenu = (string.IsNullOrEmpty(nextScene) && sceneName != "Menu_Title") || (nextScene == "Menu_Title" && sceneName != "Menu_Title");
                            curr = ((gameState == GameState.PLAYING && mem.CameraTeleporting()) || 
                                    lookForTeleporting || 
                                    ((gameState == GameState.PLAYING || gameState == GameState.ENTERING_LEVEL) && 
                                        uiState != UIState.PLAYING) || 
                                    (gameState != GameState.PLAYING && !mem.AcceptingInput()) || 
                                    gameState == GameState.EXITING_LEVEL || 
                                    gameState == GameState.LOADING || 
                                    mem.HeroTransitionState() == HeroTransitionState.WAITING_TO_ENTER_LEVEL || 
                                    (uiState != UIState.PLAYING && (uiState != UIState.PAUSED || loadingMenu) && 
                                        (!string.IsNullOrEmpty(nextScene) || sceneName == "_test_charms" || loadingMenu) && 
                                        nextScene != sceneName)
                                    ).ToString();
                            break;
                        case "Transition": curr = mem.HeroTransitionState().ToString(); break;
                        case "Teleporting": curr = mem.CameraTeleporting().ToString(); break;
                        case "LookFor": curr = lookForTeleporting.ToString(); break;
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
                    //Console.WriteLine(currentValues);
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
            GetValues();
        }

        public void OnReset(object sender, TimerPhase e) {
            currentSplit = -1;
            state = 0;
            lookForTeleporting = false;
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
            Model.CurrentState.IsGameTimePaused = true;
            Model.CurrentState.SetGameTime(Model.CurrentState.CurrentTime.RealTime);
            splitsDone.Clear();
            WriteLog("---------New Game-------------------------------");
        }
        public void OnUndoSplit(object sender, EventArgs e) {
            currentSplit--;
            //if (!settings.Ordered) splitsDone.Remove(lastSplitDone); Reminder of THIS BREAKS THINGS
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
        public void Dispose() {
#if !Info
            if (Model != null) {
                Model.CurrentState.OnReset -= OnReset;
                Model.CurrentState.OnPause -= OnPause;
                Model.CurrentState.OnResume -= OnResume;
                Model.CurrentState.OnStart -= OnStart;
                Model.CurrentState.OnSplit -= OnSplit;
                Model.CurrentState.OnUndoSplit -= OnUndoSplit;
                Model.CurrentState.OnSkipSplit -= OnSkipSplit;
                Model = null;
            }
#endif
        }
    }
}