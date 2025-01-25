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
        private List<SplitName> failedValues = new List<SplitName>();
        private SplitName lastSplitDone;
        private static string LOGFILE = "_HollowKnight.log";
        private PlayerData pdata = new PlayerData();
        private GameState lastGameState;
        private bool menuSplitHelper;
        private bool lookForTeleporting;
        private List<string> menuingSceneNames = new List<string> { "Menu_Title", "Quit_To_Menu", "PermaDeath" };
        private List<string> debugSaveStateSceneNames = new List<string> { "Room_Mender_House", "Room_Sly_Storeroom" };

        enum SplitterAction {
            Pass,
            Split,
            Skip,
            Reset
        }


#if !Info
        // Remembered data for ghost splits
        private HollowKnightStoredData store;
#endif

#if !Info
        public HollowKnightComponent(LiveSplitState state) {
            mem = new HollowKnightMemory();
            store = new HollowKnightStoredData(mem);
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

                if (state.CurrentTimingMethod == TimingMethod.RealTime) {
                    var timingMessage = MessageBox.Show(
                        "Hollow Knight uses Time without Loads (Game Time) as the main timing method.\n" +
                        "LiveSplit is currently set to show Real Time (RTA).\n" +
                        "Would you like to set the timing method to Game Time?",
                        "LiveSplit | Hollow Knight",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    );

                    if (timingMessage == DialogResult.Yes) {
                        state.CurrentTimingMethod = TimingMethod.GameTime;
                    }
                }
            }

            store.ResetKills();

        }
#else
		public HollowKnightComponent() {
        mem = new HollowKnightMemory();
        settings = new HollowKnightSettings();
        foreach (string key in keys) {
            currentValues[key] = "";

        store.ResetKills();

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
            SplitterAction action = SplitterAction.Pass;
            string nextScene = mem.NextSceneName();
            string sceneName = mem.SceneName();

            if (currentSplit == -1) {
                if (settings.AutosplitStartRuns != null) {
                    action = CheckSplit(settings.AutosplitStartRuns.Value, nextScene, sceneName);
                } else {
                    action = ((nextScene.Equals("Tutorial_01", StringComparison.OrdinalIgnoreCase) &&
                        mem.GameState() == GameState.ENTERING_LEVEL) ||
                        nextScene is "GG_Vengefly_V" or "GG_Boss_Door_Entrance" or "GG_Entrance_Cutscene") ? SplitterAction.Split : SplitterAction.Pass;
                }
            } else if (Model.CurrentState.CurrentPhase == TimerPhase.Running && settings.Splits.Count > 0) {
                GameState gameState = mem.GameState();
                UIState uIState = mem.UIState();
                //SplitName finalSplit = settings.Splits[settings.Splits.Count - 1];

                if (!settings.AutosplitEndRuns) {
                    if (currentSplit + 1 < Model.CurrentState.Run.Count) {
                        if (!settings.Ordered) {
                            action = NotOrderedSplits(gameState, uIState, nextScene, sceneName);

                        } else if (currentSplit < settings.Splits.Count) {
                            action = OrderedSplits(gameState, uIState, nextScene, sceneName);
                        }
                    } else {
                        action = (nextScene.StartsWith("Cinematic_Ending", StringComparison.OrdinalIgnoreCase) || nextScene == "GG_End_Sequence") ? SplitterAction.Split : SplitterAction.Pass;
                    }
                } else {
                    if (currentSplit < Model.CurrentState.Run.Count) {
                        if (currentSplit + 1 == Model.CurrentState.Run.Count) {
                            action = (nextScene.StartsWith("Cinematic_Ending", StringComparison.OrdinalIgnoreCase) || nextScene == "GG_End_Sequence") ? SplitterAction.Split : SplitterAction.Pass;
                        }
                        if (action == SplitterAction.Pass) {
                            if (!settings.Ordered) {
                                action = NotOrderedSplits(gameState, uIState, nextScene, sceneName); // unordered splits not compatible with Skip splits
                            } else if (currentSplit < settings.Splits.Count) {
                                action = OrderedSplits(gameState, uIState, nextScene, sceneName);
                            }
                        }
                    }
                }
                LoadRemoval(gameState, uIState, nextScene, sceneName);
            }

            store.Update();
            HandleSplit(action);
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

        private SplitterAction NotOrderedSplits(GameState gameState, UIState uIState, string nextScene, string sceneName) {

            foreach (SplitName Split in settings.Splits) {
                if (splitsDone.Contains(Split)) {
                    continue;
                } else if (Split.ToString().StartsWith("Menu")) {
                    if (!menuSplitHelper)
                        menuSplitHelper = Split == SplitName.Menu ||
                            CheckSplit(Split, nextScene, sceneName) == SplitterAction.Split && !((gameState == GameState.INACTIVE && uIState == UIState.INACTIVE) || (gameState == GameState.MAIN_MENU));
                    if (menuSplitHelper) {
                        if (CheckSplit(SplitName.Menu, nextScene, sceneName) == SplitterAction.Split) {
                            splitsDone.Add(Split);
                            lastSplitDone = Split;
                            menuSplitHelper = false;
                            if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Split: " + Split);
                            return SplitterAction.Split;
                        }
                    }
                } else if (!((gameState == GameState.INACTIVE && uIState == UIState.INACTIVE) || (gameState == GameState.MAIN_MENU))) {
                    if (CheckSplit(Split, nextScene, sceneName) == SplitterAction.Split) {
                        splitsDone.Add(Split);
                        lastSplitDone = Split;
                        menuSplitHelper = false;
                        if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Split: " + Split);
                        return SplitterAction.Split;
                    }
                }
            }
            return SplitterAction.Pass;
        }

        private SplitterAction OrderedSplits(GameState gameState, UIState uIState, string nextScene, string sceneName) {
            SplitName Split = settings.Splits[currentSplit];

            if (Split.ToString().StartsWith("Menu")) {
                if (!menuSplitHelper) menuSplitHelper = Split == SplitName.Menu || CheckSplit(Split, nextScene, sceneName) != SplitterAction.Pass;
                if (menuSplitHelper) {
                    if (CheckSplit(SplitName.Menu, nextScene, sceneName) == SplitterAction.Split) {
                        menuSplitHelper = false;
                        if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Split: " + Split);
                        return SplitterAction.Split;
                    } else if (CheckSplit(SplitName.Menu, nextScene, sceneName) == SplitterAction.Skip) {
                        menuSplitHelper = false;
                        if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Skip: " + Split);
                        return SplitterAction.Skip;
                    }
                }
            } else {
                if (CheckSplit(Split, nextScene, sceneName) == SplitterAction.Split
                        && !((gameState == GameState.INACTIVE && uIState == UIState.INACTIVE) || (gameState == GameState.MAIN_MENU))) {
                    if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Split: " + Split);
                    return SplitterAction.Split;
                } else if (CheckSplit(Split, nextScene, sceneName) == SplitterAction.Skip
                        && !((gameState == GameState.INACTIVE && uIState == UIState.INACTIVE) || (gameState == GameState.MAIN_MENU))) {
                    if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Skip: " + Split);
                    return SplitterAction.Skip;
                } else if (CheckSplit(Split, nextScene, sceneName) == SplitterAction.Reset
                        && !((gameState == GameState.INACTIVE && uIState == UIState.INACTIVE) || (gameState == GameState.MAIN_MENU))) {
                    if (hasLog || !Console.IsOutputRedirected) WriteLogWithTime("Reset: " + Split);
                    return SplitterAction.Reset;
                }
            }
            return SplitterAction.Pass;
        }

        private bool shouldSplitTransition(string nextScene, string sceneName) {
            if (nextScene != sceneName && !store.SplitThisTransition) {
                return !(
                    string.IsNullOrEmpty(sceneName) ||
                    string.IsNullOrEmpty(nextScene) ||
                    menuingSceneNames.Contains(sceneName) ||
                    menuingSceneNames.Contains(nextScene)
                );
            }
            return false;
        }


        private SplitterAction CheckSplit(SplitName split, string nextScene, string sceneName) {
            string currScene = sceneName;
            bool shouldSplit = false;
            bool shouldSkip = false;
            bool shouldReset = false;
            SplitterAction action;

            switch (split) {

                #region Start and End

                case SplitName.StartNewGame:
                    shouldSplit =
                        (nextScene.Equals("Tutorial_01", StringComparison.OrdinalIgnoreCase)
                        && mem.GameState() == GameState.ENTERING_LEVEL)
                        || nextScene is "GG_Entrance_Cutscene";
                    break;
                case SplitName.StartPantheon:
                    shouldSplit =
                        nextScene is "GG_Vengefly_V" or "GG_Boss_Door_Entrance";
                    break;

                case SplitName.RandoWake:
                    shouldSplit =
                        !mem.PlayerData<bool>(Offset.disablePause)
                        && mem.GameState() == GameState.PLAYING
                        && !menuingSceneNames.Contains(currScene);
                    break;

                case SplitName.EndingSplit: shouldSplit = nextScene.StartsWith("Cinematic_Ending", StringComparison.OrdinalIgnoreCase); break;
                case SplitName.EndingA: shouldSplit = nextScene.Equals("Cinematic_Ending_A", StringComparison.OrdinalIgnoreCase); break;
                case SplitName.EndingB: shouldSplit = nextScene.Equals("Cinematic_Ending_B", StringComparison.OrdinalIgnoreCase); break;
                case SplitName.EndingC: shouldSplit = nextScene.Equals("Cinematic_Ending_C", StringComparison.OrdinalIgnoreCase); break;
                case SplitName.EndingD: shouldSplit = nextScene.Equals("Cinematic_Ending_D", StringComparison.OrdinalIgnoreCase); break;
                case SplitName.EndingE: shouldSplit = nextScene.Equals("Cinematic_Ending_E", StringComparison.OrdinalIgnoreCase); break;

                #endregion Start and End

                #region Skills

                case SplitName.VengefulSpirit: shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 1; break;
                case SplitName.ShadeSoul: shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 2; break;
                case SplitName.DesolateDive: shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 1; break;
                case SplitName.DescendingDark: shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 2; break;
                case SplitName.HowlingWraiths: shouldSplit = mem.PlayerData<int>(Offset.screamLevel) == 1; break;
                case SplitName.AbyssShriek: shouldSplit = mem.PlayerData<int>(Offset.screamLevel) == 2; break;

                case SplitName.DreamNail: shouldSplit = mem.PlayerData<bool>(Offset.hasDreamNail); break;
                case SplitName.DreamGate: shouldSplit = mem.PlayerData<bool>(Offset.hasDreamGate); break;
                case SplitName.DreamNail2: shouldSplit = mem.PlayerData<bool>(Offset.dreamNailUpgraded); break;

                case SplitName.CycloneSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasCyclone); break;
                case SplitName.GreatSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasDashSlash); break;
                case SplitName.DashSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasUpwardSlash); break;

                case SplitName.MothwingCloak: shouldSplit = mem.PlayerData<bool>(Offset.hasDash); break;
                case SplitName.ShadeCloak: shouldSplit = mem.PlayerData<bool>(Offset.hasShadowDash); break;
                case SplitName.MantisClaw: shouldSplit = mem.PlayerData<bool>(Offset.hasWallJump); break;
                case SplitName.MonarchWings: shouldSplit = mem.PlayerData<bool>(Offset.hasDoubleJump); break;
                case SplitName.CrystalHeart: shouldSplit = mem.PlayerData<bool>(Offset.hasSuperDash); break;
                case SplitName.IsmasTear: shouldSplit = mem.PlayerData<bool>(Offset.hasAcidArmour); break;

                #endregion Skills

                #region Dreamers

                case SplitName.Lurien: shouldSplit = mem.PlayerData<bool>(Offset.maskBrokenLurien); break;
                case SplitName.Monomon: shouldSplit = mem.PlayerData<bool>(Offset.maskBrokenMonomon); break;
                case SplitName.Hegemol: shouldSplit = mem.PlayerData<bool>(Offset.maskBrokenHegemol); break;

                case SplitName.Dreamer1: shouldSplit = mem.PlayerData<int>(Offset.guardiansDefeated) == 1; break;
                case SplitName.Dreamer2: shouldSplit = mem.PlayerData<int>(Offset.guardiansDefeated) == 2; break;
                case SplitName.Dreamer3: shouldSplit = mem.PlayerData<int>(Offset.guardiansDefeated) == 3; break;

                // Old Timings, mark deprecated or whatever
                case SplitName.LurienDreamer: shouldSplit = mem.PlayerData<bool>(Offset.lurienDefeated); break;
                case SplitName.MonomonDreamer: shouldSplit = mem.PlayerData<bool>(Offset.monomonDefeated); break;
                case SplitName.HegemolDreamer: shouldSplit = mem.PlayerData<bool>(Offset.hegemolDefeated); break;

                #endregion Dreamers

                #region Charms

                case SplitName.GatheringSwarm: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_1); break;
                case SplitName.WaywardCompass: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_2); break;
                case SplitName.Grubsong: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_3); break;
                case SplitName.StalwartShell: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_4); break;
                case SplitName.BaldurShell: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_5); break;
                case SplitName.FuryOfTheFallen: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_6); break;
                case SplitName.QuickFocus: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_7); break;
                case SplitName.LifebloodHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_8); break;
                case SplitName.LifebloodCore: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_9); break;
                case SplitName.DefendersCrest: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_10); break;
                case SplitName.Flukenest: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_11); break;
                case SplitName.ThornsOfAgony: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_12); break;
                case SplitName.MarkOfPride: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_13); break;
                case SplitName.SteadyBody: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_14); break;
                case SplitName.HeavyBlow: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_15); break;
                case SplitName.SharpShadow: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_16); break;
                case SplitName.SporeShroom: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_17); break;
                case SplitName.Longnail: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_18); break;
                case SplitName.ShamanStone: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_19); break;
                case SplitName.SoulCatcher: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_20); break;
                case SplitName.SoulEater: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_21); break;
                case SplitName.GlowingWomb: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_22); break;
                // fragile charms are below
                case SplitName.NailmastersGlory: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_26); break;
                case SplitName.JonisBlessing: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_27); break;
                case SplitName.ShapeOfUnn: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_28); break;
                case SplitName.Hiveblood: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_29); break;
                case SplitName.DreamWielder: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_30); break;
                case SplitName.Dashmaster: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_31); break;
                case SplitName.QuickSlash: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_32); break;
                case SplitName.SpellTwister: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_33); break;
                case SplitName.DeepFocus: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_34); break;
                case SplitName.GrubberflysElegy: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_35); break;
                // kingsoul/void heart are below
                case SplitName.Sprintmaster: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_37); break;
                case SplitName.Dreamshield: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_38); break;
                case SplitName.Weaversong: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_39); break;

                // Grimmchild
                case SplitName.Grimmchild: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_40); break;
                case SplitName.Grimmchild2: shouldSplit = mem.PlayerData<int>(Offset.grimmChildLevel) == 2; break;
                case SplitName.Grimmchild3: shouldSplit = mem.PlayerData<int>(Offset.grimmChildLevel) == 3; break;
                case SplitName.Grimmchild4: shouldSplit = mem.PlayerData<int>(Offset.grimmChildLevel) == 4; break;
                case SplitName.CarefreeMelody: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_40) && mem.PlayerData<int>(Offset.grimmChildLevel) == 5; break;

                // Fragile / Unbreakable charms
                case SplitName.FragileHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_23); break;
                case SplitName.UnbreakableHeart: shouldSplit = mem.PlayerData<bool>(Offset.fragileHealth_unbreakable); break;
                case SplitName.FragileGreed: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_24); break;
                case SplitName.UnbreakableGreed: shouldSplit = mem.PlayerData<bool>(Offset.fragileGreed_unbreakable); break;
                case SplitName.FragileStrength: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_25); break;
                case SplitName.UnbreakableStrength: shouldSplit = mem.PlayerData<bool>(Offset.fragileStrength_unbreakable); break;

                case SplitName.AllBreakables:
                    shouldSplit = mem.PlayerData<bool>(Offset.brokenCharm_23)
                        && mem.PlayerData<bool>(Offset.brokenCharm_24)
                        && mem.PlayerData<bool>(Offset.brokenCharm_25);
                    break;
                case SplitName.AllUnbreakables:
                    shouldSplit = mem.PlayerData<bool>(Offset.fragileGreed_unbreakable)
                        && mem.PlayerData<bool>(Offset.fragileHealth_unbreakable)
                        && mem.PlayerData<bool>(Offset.fragileStrength_unbreakable);
                    break;

                // Kingsoul / Void Heart
                case SplitName.Kingsoul: shouldSplit = mem.PlayerData<int>(Offset.charmCost_36) == 5 && mem.PlayerData<int>(Offset.royalCharmState) == 3; break;
                case SplitName.VoidHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotShadeCharm); break;
                case SplitName.WhiteFragmentLeft: shouldSplit = mem.PlayerData<bool>(Offset.gotQueenFragment); break;
                case SplitName.WhiteFragmentRight: shouldSplit = mem.PlayerData<bool>(Offset.gotKingFragment); break;
                case SplitName.OnObtainWhiteFragment: shouldSplit = store.CheckIncreased(Offset.royalCharmState); break;

                #endregion Charms

                #region Bosses

                // Progression Bosses
                case SplitName.FalseKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedFalseKnight); break;
                case SplitName.BlackKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedBlackKnight); break; // watcher knights
                case SplitName.BrokenVessel: shouldSplit = mem.PlayerData<bool>(Offset.killedInfectedKnight); break;
                case SplitName.HollowKnightBoss: shouldSplit = mem.PlayerData<bool>(Offset.killedHollowKnight); break;
                case SplitName.HollowKnightDreamnail:
                    shouldSplit = nextScene.Equals("Dream_Final_Boss", StringComparison.OrdinalIgnoreCase);
                    shouldSkip = mem.PlayerData<bool>(Offset.killedHollowKnight);
                    break;
                case SplitName.Hornet1: shouldSplit = mem.PlayerData<bool>(Offset.killedHornet); break;
                case SplitName.Hornet2: shouldSplit = mem.PlayerData<bool>(Offset.hornetOutskirtsDefeated); break;
                case SplitName.RadianceBoss: shouldSplit = mem.PlayerData<bool>(Offset.killedFinalBoss); break;
                case SplitName.SoulMaster: shouldSplit = mem.PlayerData<bool>(Offset.killedMageLord); break;
                case SplitName.SoulMasterEncountered: shouldSplit = mem.PlayerData<bool>(Offset.mageLordEncountered); break;
                case SplitName.SoulMasterPhase1: shouldSplit = mem.PlayerData<bool>(Offset.mageLordEncountered_2); break;
                case SplitName.TraitorLord: shouldSplit = mem.PlayerData<bool>(Offset.killedTraitorLord); break;
                case SplitName.Uumuu: shouldSplit = mem.PlayerData<bool>(Offset.killedMegaJellyfish); break;
                case SplitName.UumuuEncountered: shouldSplit = mem.PlayerData<bool>(Offset.encounteredMegaJelly); break;

                // Overworld Bosses
                case SplitName.BroodingMawlek: shouldSplit = mem.PlayerData<bool>(Offset.killedMawlek); break;
                case SplitName.Collector: shouldSplit = mem.PlayerData<bool>(Offset.collectorDefeated); break;
                case SplitName.CrystalGuardian1: shouldSplit = mem.PlayerData<bool>(Offset.defeatedMegaBeamMiner); break;
                case SplitName.CrystalGuardian2: shouldSplit = mem.PlayerData<int>(Offset.killsMegaBeamMiner) == 0; break;
                case SplitName.DungDefender: shouldSplit = mem.PlayerData<bool>(Offset.killedDungDefender); break;
                case SplitName.Flukemarm: shouldSplit = mem.PlayerData<bool>(Offset.killedFlukeMother); break;
                case SplitName.GodTamer: shouldSplit = mem.PlayerData<bool>(Offset.killedLobsterLancer); break;
                case SplitName.GruzMother: shouldSplit = mem.PlayerData<bool>(Offset.killedBigFly); break;
                case SplitName.HiveKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedHiveKnight); break;
                case SplitName.MantisLords: shouldSplit = mem.PlayerData<bool>(Offset.defeatedMantisLords); break;
                case SplitName.MegaMossCharger: shouldSplit = mem.PlayerData<bool>(Offset.megaMossChargerDefeated); break;
                case SplitName.Nosk: shouldSplit = mem.PlayerData<bool>(Offset.killedMimicSpider); break;
                case SplitName.KilledOblobbles: shouldSplit = mem.PlayerData<int>(Offset.killsOblobble) == 1; break;
                case SplitName.killedSanctumWarrior: shouldSplit = mem.PlayerData<bool>(Offset.killedMageKnight); break;

                // Pantheon Bosses
                case SplitName.MatoOroNailBros: shouldSplit = mem.PlayerData<bool>(Offset.killedNailBros); break;
                case SplitName.SheoPaintmaster: shouldSplit = mem.PlayerData<bool>(Offset.killedPaintmaster); break;
                case SplitName.SlyNailsage: shouldSplit = mem.PlayerData<bool>(Offset.killedNailsage); break;
                case SplitName.PureVessel: shouldSplit = mem.PlayerData<bool>(Offset.killedHollowKnightPrime); break;

                // Dream Warriors
                case SplitName.ElderHu: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostHu); break;
                case SplitName.Galien: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostGalien); break;
                case SplitName.Gorb: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostAladar); break;
                case SplitName.Markoth: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostMarkoth); break;
                case SplitName.Marmu: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostMarmu); break;
                case SplitName.NoEyes: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostNoEyes); break;
                case SplitName.Xero: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostXero); break;

                // Dream Bosses
                case SplitName.FailedKnight: shouldSplit = mem.PlayerData<bool>(Offset.falseKnightDreamDefeated); break;
                case SplitName.GreyPrince: shouldSplit = mem.PlayerData<bool>(Offset.killedGreyPrince); break;
                case SplitName.SoulTyrant: shouldSplit = mem.PlayerData<bool>(Offset.mageLordDreamDefeated); break;
                case SplitName.LostKin: shouldSplit = mem.PlayerData<bool>(Offset.infectedKnightDreamDefeated); break;
                case SplitName.WhiteDefender: shouldSplit = mem.PlayerData<bool>(Offset.killedWhiteDefender); break;

                // Grimm twice
                case SplitName.TroupeMasterGrimm: shouldSplit = mem.PlayerData<bool>(Offset.killedGrimm); break;
                case SplitName.NightmareKingGrimm: shouldSplit = mem.PlayerData<bool>(Offset.killedNightmareGrimm); break;

                #endregion Bosses

                #region Items

                // Keys (Purist)
                case SplitName.ElegantKey: shouldSplit = mem.PlayerData<bool>(Offset.hasWhiteKey); break;
                case SplitName.LoveKey: shouldSplit = mem.PlayerData<bool>(Offset.hasLoveKey); break;
                case SplitName.PaleLurkerKey: shouldSplit = mem.PlayerData<bool>(Offset.gotLurkerKey); break;
                case SplitName.SimpleKey: shouldSplit = mem.PlayerData<int>(Offset.simpleKeys) > 0; break;
                case SplitName.OnObtainSimpleKey: shouldSplit = store.CheckIncremented(Offset.simpleKeys); break;
                case SplitName.SlyKey: shouldSplit = mem.PlayerData<bool>(Offset.hasSlykey); break;
                case SplitName.SlySimpleKey: shouldSplit = mem.PlayerData<bool>(Offset.slySimpleKey); break;

                // Keys (Radical)
                case SplitName.KingsBrand: shouldSplit = mem.PlayerData<bool>(Offset.hasKingsBrand); break;
                case SplitName.LumaflyLantern: shouldSplit = mem.PlayerData<bool>(Offset.hasLantern); break;
                case SplitName.TramPass: shouldSplit = mem.PlayerData<bool>(Offset.hasTramPass); break;
                case SplitName.CityKey: shouldSplit = mem.PlayerData<bool>(Offset.hasCityKey); break;

                // Charm Notches
                case SplitName.NotchFogCanyon: shouldSplit = mem.PlayerData<bool>(Offset.notchFogCanyon); break;
                case SplitName.NotchSalubra1: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch1); break;
                case SplitName.NotchSalubra2: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch2); break;
                case SplitName.NotchSalubra3: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch3); break;
                case SplitName.NotchSalubra4: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch4); break;
                case SplitName.NotchShrumalOgres: shouldSplit = mem.PlayerData<bool>(Offset.notchShroomOgres); break;
                case SplitName.NotchGrimm: shouldSplit = mem.PlayerData<bool>(Offset.gotGrimmNotch); break;
                case SplitName.OnObtainCharmNotch: shouldSplit = store.CheckIncreased(Offset.charmSlots); break;

                // Relics
                case SplitName.OnObtainWanderersJournal: shouldSplit = store.CheckIncremented(Offset.trinket1); break;
                case SplitName.AllSeals: shouldSplit = mem.PlayerData<int>(Offset.trinket2) + mem.PlayerData<int>(Offset.soldTrinket2) == 17; break;
                case SplitName.OnObtainHallownestSeal: shouldSplit = store.CheckIncremented(Offset.trinket2); break;
                case SplitName.SoulSanctumSeal: shouldSplit = store.CheckIncremented(Offset.trinket2) && currScene == "Ruins1_32"; break;
                case SplitName.OnObtainKingsIdol: shouldSplit = store.CheckIncremented(Offset.trinket3); break;
                case SplitName.GladeIdol: shouldSplit = store.CheckIncreased(Offset.trinket3) && currScene.StartsWith("RestingGrounds_08"); break;
                case SplitName.DungDefenderIdol:
                    shouldSplit = store.CheckIncreased(Offset.trinket3) && currScene.StartsWith("Waterways_15");
                    break;
                case SplitName.ArcaneEgg8: shouldSplit = mem.PlayerData<int>(Offset.trinket4) == 8; break;
                case SplitName.OnObtainArcaneEgg: shouldSplit = store.CheckIncremented(Offset.trinket4); break;
                case SplitName.OnObtainRancidEgg: shouldSplit = store.CheckIncremented(Offset.rancidEggs); break;

                // Other
                case SplitName.GodTuner: shouldSplit = mem.PlayerData<bool>(Offset.hasGodfinder); break;
                case SplitName.SalubrasBlessing: shouldSplit = mem.PlayerData<bool>(Offset.salubraBlessing); break;
                case SplitName.AllEggs: shouldSplit = mem.PlayerData<int>(Offset.rancidEggs) + mem.PlayerData<int>(Offset.jinnEggsSold) == 21; break;

                #endregion Items

                #region Transitions

                // these aren't really sorted, there's so many and my brain can only handle so much split sorting, maybe next update

                case SplitName.AncestralMound: shouldSplit = nextScene.Equals("Crossroads_ShamanTemple") && nextScene != currScene; break;
                case SplitName.TransCollector: shouldSplit = mem.PlayerData<bool>(Offset.collectorDefeated) && currScene.StartsWith("Ruins2_11") && nextScene != currScene; break;
                case SplitName.TeachersArchive: shouldSplit = currScene.Equals("Fungus3_archive", StringComparison.OrdinalIgnoreCase); break;
                case SplitName.TransVS: shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 1 && nextScene != currScene; break;
                case SplitName.KingsPass: shouldSplit = currScene.StartsWith("Tutorial_01") && nextScene.StartsWith("Town"); break;
                case SplitName.KingsPassEnterFromTown: shouldSplit = currScene.StartsWith("Town") && nextScene.StartsWith("Tutorial_01"); break;
                case SplitName.BlueLake: shouldSplit = !currScene.StartsWith("Crossroads_50") && nextScene.StartsWith("Crossroads_50"); break; // blue lake is Crossroads_50
                case SplitName.CatacombsEntry: shouldSplit = !currScene.StartsWith("RestingGrounds_10") && nextScene.StartsWith("RestingGrounds_10"); break;
                case SplitName.EnterNKG: shouldSplit = currScene.StartsWith("Grimm_Main_Tent") && nextScene.StartsWith("Grimm_Nightmare"); break;
                case SplitName.EnterGreenpath: shouldSplit = !currScene.StartsWith("Fungus1_01") && nextScene.StartsWith("Fungus1_01"); break;
                case SplitName.EnterGreenpathWithOvercharm:
                    shouldSplit = !currScene.StartsWith("Fungus1_01")
                        && nextScene.StartsWith("Fungus1_01")
                        && mem.PlayerData<bool>(Offset.canOvercharm);
                    break;
                case SplitName.EnterSanctum: shouldSplit = !currScene.StartsWith("Ruins1_23") && nextScene.StartsWith("Ruins1_23"); break;
                case SplitName.EnterSanctumWithShadeSoul:
                    shouldSplit = !currScene.StartsWith("Ruins1_23")
                        && nextScene.StartsWith("Ruins1_23")
                        && mem.PlayerData<int>(Offset.fireballLevel) == 2;
                    break;
                case SplitName.EnterAnyDream: shouldSplit = nextScene.StartsWith("Dream_") && nextScene != currScene; break;
                case SplitName.EnterGodhome: shouldSplit = nextScene.StartsWith("GG_Atrium") && nextScene != currScene; break;
                case SplitName.EnterJunkPit: shouldSplit = nextScene.Equals("GG_Waterways") && nextScene != currScene; break;
                case SplitName.EnterDeepnest:
                    shouldSplit =
                        (nextScene.Equals("Fungus2_25")
                        || nextScene.Equals("Deepnest_42")
                        || nextScene.Equals("Abyss_03b")
                        || nextScene.Equals("Deepnest_01b"))
                        && nextScene != currScene;
                    break;
                case SplitName.EnterBeastDen: shouldSplit = nextScene.Equals("Deepnest_Spider_Town") && nextScene != currScene; break;
                case SplitName.EnterCrown: shouldSplit = nextScene.Equals("Mines_23") && nextScene != currScene; break;
                case SplitName.EnterDirtmouth: shouldSplit = nextScene.Equals("Town") && nextScene != currScene; break;
                case SplitName.EnterRafters: shouldSplit = nextScene.Equals("Ruins1_03") && nextScene != currScene; break;
                case SplitName.SalubraExit: shouldSplit = currScene.Equals("Room_Charm_Shop") && nextScene != currScene; break;
                // since Ruins1_18 has both bench and bridge, don't include Ruins1_18 bridge to Ruins2_03b
                case SplitName.SpireBenchExit: shouldSplit = currScene.StartsWith("Ruins1_18") && nextScene.StartsWith("Ruins2_01"); break;
                case SplitName.PreGrimmShopTrans:
                    shouldSplit = mem.PlayerData<bool>(Offset.hasLantern)
                        && mem.PlayerData<int>(Offset.maxHealthBase) == 6
                        && (mem.PlayerData<int>(Offset.vesselFragments) == 4 || (mem.PlayerData<int>(Offset.MPReserveMax) == 33 && mem.PlayerData<int>(Offset.vesselFragments) == 2))
                        && !currScene.StartsWith("Room_shop");
                    break;
                case SplitName.WaterwaysEntry:
                    shouldSplit =
                        (nextScene.StartsWith("Waterways_01") // Simple Key manhole entrance
                        || nextScene.StartsWith("Waterways_07")) // Right of Spike-tunnel, also where Tram entrance meets the rest
                        && nextScene != currScene;
                    break;
                case SplitName.FogCanyonEntry:
                    shouldSplit =
                        (nextScene.StartsWith("Fungus3_01") // West Fog Canyon entrance from Greenpath
                        || nextScene.StartsWith("Fungus3_02") // West Fog Canyon entrance from Queen's Station or QGA
                        || nextScene.StartsWith("Fungus3_24") // West Fog Canyon entrance from Queen's Gardens via Overgrown Mound
                        || nextScene.StartsWith("Fungus3_26")) // East Fog Canyon, where the Crossroads acid and Leg Eater acid entrances meet
                        && nextScene != currScene;
                    break;
                case SplitName.FungalWastesEntry:
                    shouldSplit =
                        (nextScene.StartsWith("Fungus2_06") // Room outside Leg Eater
                        || nextScene.StartsWith("Fungus2_03") // From Queens' Station
                        || nextScene.StartsWith("Fungus2_23") // Bretta from Waterways
                        || nextScene.StartsWith("Fungus2_20")) // Spore Shroom room, from QG (this one's unlikely to come up)
                        && nextScene != currScene;
                    break;
                case SplitName.CrystalMoundExit: shouldSplit = currScene.StartsWith("Mines_35") && nextScene != currScene; break;
                case SplitName.CrystalPeakEntry: shouldSplit = (nextScene.StartsWith("Mines_02") || nextScene.StartsWith("Mines_10")) && nextScene != currScene; break;
                case SplitName.QueensGardensEntry: shouldSplit = (nextScene.StartsWith("Fungus3_34") || nextScene.StartsWith("Deepnest_43")) && nextScene != currScene; break;
                case SplitName.BasinEntry: shouldSplit = nextScene.StartsWith("Abyss_04") && nextScene != currScene; break;
                case SplitName.HiveEntry: shouldSplit = nextScene.StartsWith("Hive_01") && nextScene != currScene; break;
                case SplitName.KingdomsEdgeEntry: shouldSplit = nextScene.StartsWith("Deepnest_East_03") && nextScene != currScene; break;
                case SplitName.KingdomsEdgeOvercharmedEntry:
                    shouldSplit =
                        nextScene.StartsWith("Deepnest_East_03")
                        && nextScene != currScene
                        && mem.PlayerData<bool>(Offset.overcharmed);
                    break;
                case SplitName.GodhomeBench: shouldSplit = currScene.StartsWith("GG_Spa") && currScene != nextScene && !store.SplitThisTransition; break;
                case SplitName.GodhomeLoreRoom:
                    shouldSplit =
                        (currScene.StartsWith("GG_Engine") || currScene.StartsWith("GG_Unn") || currScene.StartsWith("GG_Wyrm"))
                        && currScene != nextScene
                        && !store.SplitThisTransition;
                    break;
                case SplitName.TransClaw: shouldSplit = mem.PlayerData<bool>(Offset.hasWallJump) && nextScene != currScene; break;
                case SplitName.TransGorgeousHusk: shouldSplit = mem.PlayerData<bool>(Offset.killedGorgeousHusk) && nextScene != currScene; break;
                case SplitName.TransDescendingDark: shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 2 && nextScene != currScene; break;
                case SplitName.TransTear: shouldSplit = mem.PlayerData<bool>(Offset.hasAcidArmour) && nextScene != currScene; break;
                case SplitName.TransTearWithGrub:
                    shouldSplit =
                        mem.PlayerData<bool>(Offset.hasAcidArmour)
                        && mem.PlayerDataStringList(Offset.scenesGrubRescued).Contains("Waterways_13")
                        && nextScene != currScene; break;
                case SplitName.TransShadeSoul: // Should work with both normal movement and room dupes
                    shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 2
                        && mem.HeroTransitionState() == HeroTransitionState.WAITING_TO_ENTER_LEVEL;
                    break;
                case SplitName.AnyTransition:
                    shouldSplit = nextScene != currScene && !store.SplitThisTransition
                                    && !(string.IsNullOrEmpty(currScene) || string.IsNullOrEmpty(nextScene)
                                        || menuingSceneNames.Contains(currScene) || menuingSceneNames.Contains(nextScene));
                    break;
                case SplitName.TransitionAfterSaveState:
                    if (!(debugSaveStateSceneNames.Contains(nextScene)
                        || debugSaveStateSceneNames.Contains(currScene))) {
                        goto case SplitName.AnyTransition;
                    }
                    break;
                case SplitName.TransitionExcludingDiscontinuities:
                    if (!(debugSaveStateSceneNames.Contains(nextScene)
                        || debugSaveStateSceneNames.Contains(currScene)
                        || mem.PlayerData<int>(Offset.health) == 0
                        || mem.EntryGateName() is "dreamGate" or "door_dreamReturn")) {
                        goto case SplitName.AnyTransition;
                    }
                    break;
                case SplitName.QueensGardensPostArenaTransition: shouldSplit = nextScene.StartsWith("Fungus3_13") && nextScene != currScene; break;
                case SplitName.QueensGardensFrogsTrans: shouldSplit = nextScene.StartsWith("Fungus1_23") && nextScene != currScene; break;
                case SplitName.Pantheon1to4Entry: shouldSplit = nextScene.StartsWith("GG_Boss_Door_Entrance") && nextScene != currScene; break;
                case SplitName.Pantheon5Entry: shouldSplit = nextScene.StartsWith("GG_Vengefly_V") && nextScene != currScene; break;
                case SplitName.EnterHornet1: shouldSplit = nextScene.StartsWith("Fungus1_04") && nextScene != currScene; break;
                case SplitName.EnterSoulMaster: shouldSplit = nextScene.StartsWith("Ruins1_24") && nextScene != currScene; break;
                case SplitName.EnterHiveKnight: shouldSplit = nextScene.StartsWith("Hive_05") && nextScene != currScene; break;
                case SplitName.EnterHornet2: shouldSplit = nextScene.StartsWith("Deepnest_East_Hornet") && nextScene != currScene; break;
                case SplitName.EnterBroodingMawlek: shouldSplit = nextScene.StartsWith("Crossroads_09") && nextScene != currScene; break;
                case SplitName.EnterNosk: shouldSplit = nextScene.StartsWith("Deepnest_32") && nextScene != currScene; break;
                case SplitName.EnterTMG:
                    shouldSplit = nextScene.StartsWith("Grimm_Main_Tent") && nextScene != currScene
                        && mem.PlayerData<bool>(Offset.equippedCharm_40) // Equipped Grimmchild
                        && mem.PlayerData<int>(Offset.grimmChildLevel) == 2
                        && mem.PlayerData<int>(Offset.flamesCollected) == 3; break;
                case SplitName.EnterLoveTower: shouldSplit = nextScene.StartsWith("Ruins2_11") && nextScene != currScene; break;

                case SplitName.EnterCityTollBenchRoom: shouldSplit = nextScene.StartsWith("Ruins1_31") && nextScene != currScene; break;

                case SplitName.VengeflyKingTrans: shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedBuzzer) && nextScene != currScene; break;
                case SplitName.MegaMossChargerTrans: shouldSplit = mem.PlayerData<bool>(Offset.megaMossChargerDefeated) && nextScene != currScene; break;
                case SplitName.ElderHuTrans: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostHu) && nextScene != currScene; break;
                case SplitName.BlackKnightTrans: shouldSplit = mem.PlayerData<bool>(Offset.killedBlackKnight) && nextScene != currScene; break;
                case SplitName.BrokenVesselTrans:
                    shouldSplit = nextScene != currScene
                        && mem.PlayerData<bool>(Offset.killedInfectedKnight)
                        && mem.PlayerData<int>(Offset.health) > 0;
                    break;
                case SplitName.LumaflyLanternTransition: shouldSplit = mem.PlayerData<bool>(Offset.hasLantern) && !currScene.StartsWith("Room_shop"); break;

                // White palace and path of pain transition splits are in the White Palace & Path of Pain #region

                #endregion Transitions

                #region White Palace & Path of Pain

                #region Orbs

                case SplitName.WhitePalaceOrb1: shouldSplit = mem.PlayerData<bool>(Offset.whitePalaceOrb_1); break;
                case SplitName.WhitePalaceOrb2: shouldSplit = mem.PlayerData<bool>(Offset.whitePalaceOrb_2); break;
                case SplitName.WhitePalaceOrb3: shouldSplit = mem.PlayerData<bool>(Offset.whitePalaceOrb_3); break;

                #endregion Orbs

                #region Rooms

                case SplitName.WhitePalaceLowerEntry: shouldSplit = nextScene.StartsWith("White_Palace_01") && nextScene != currScene; break;
                case SplitName.WhitePalaceLowerOrb: shouldSplit = nextScene.StartsWith("White_Palace_02") && nextScene != currScene; break;
                case SplitName.WhitePalaceSecretRoom: shouldSplit = mem.PlayerData<bool>(Offset.whitePalaceSecretRoomVisited); break;
                case SplitName.WhitePalaceEntry: shouldSplit = nextScene.StartsWith("White_Palace_11") && nextScene != currScene; break;
                case SplitName.WhitePalaceLeftEntry: shouldSplit = nextScene.StartsWith("White_Palace_04") && nextScene != currScene; break;
                case SplitName.WhitePalaceLeftWingMid: shouldSplit = currScene.StartsWith("White_Palace_04") && nextScene.StartsWith("White_Palace_14"); break;
                case SplitName.WhitePalaceRightEntry: shouldSplit = nextScene.StartsWith("White_Palace_15") && nextScene != currScene; break;
                case SplitName.WhitePalaceRightClimb: shouldSplit = currScene.StartsWith("White_Palace_05") && nextScene.StartsWith("White_Palace_16"); break;
                case SplitName.WhitePalaceRightSqueeze: shouldSplit = currScene.StartsWith("White_Palace_16") && nextScene.StartsWith("White_Palace_05"); break;
                case SplitName.WhitePalaceRightDone: shouldSplit = currScene.StartsWith("White_Palace_05") && nextScene.StartsWith("White_Palace_15"); break;
                case SplitName.WhitePalaceTopEntry: shouldSplit = currScene.StartsWith("White_Palace_03_hub") && nextScene.StartsWith("White_Palace_06"); break;
                case SplitName.WhitePalaceTopClimb: shouldSplit = currScene.StartsWith("White_Palace_06") && nextScene.StartsWith("White_Palace_07"); break;
                case SplitName.WhitePalaceTopLeverRoom: shouldSplit = currScene.StartsWith("White_Palace_07") && nextScene.StartsWith("White_Palace_12"); break;
                case SplitName.WhitePalaceTopLastPlats: shouldSplit = currScene.StartsWith("White_Palace_12") && nextScene.StartsWith("White_Palace_13"); break;
                case SplitName.WhitePalaceThroneRoom: shouldSplit = currScene.StartsWith("White_Palace_13") && nextScene.StartsWith("White_Palace_09"); break;
                case SplitName.WhitePalaceAtrium: shouldSplit = nextScene.StartsWith("White_Palace_03_hub") && nextScene != currScene; break;

                #endregion Rooms

                #region Path of Pain

                case SplitName.PathOfPainEntry: shouldSplit = nextScene.StartsWith("White_Palace_18") && currScene.StartsWith("White_Palace_06"); break;
                case SplitName.PathOfPainTransition1: shouldSplit = nextScene.StartsWith("White_Palace_17") && currScene.StartsWith("White_Palace_18"); break;
                case SplitName.PathOfPainTransition2: shouldSplit = nextScene.StartsWith("White_Palace_19") && currScene.StartsWith("White_Palace_17"); break;
                case SplitName.PathOfPainTransition3: shouldSplit = nextScene.StartsWith("White_Palace_20") && currScene.StartsWith("White_Palace_19"); break;
                case SplitName.PathOfPainRoom4DDark: shouldSplit = currScene.StartsWith("White_Palace_20") && mem.OnGround() && mem.Spellquake(); break;
                case SplitName.PathOfPain: shouldSplit = mem.PlayerData<bool>(Offset.newDataBindingSeal); break;

                #endregion Path of Pain

                #endregion White Palace & Path of Pain

                #region Stags

                case SplitName.RidingStag: shouldSplit = store.CheckToggledTrue(Offset.travelling); break;
                case SplitName.StagMoved: shouldSplit = store.CheckChanged(Offset.stagPosition); break;

                case SplitName.CrossroadsStation: shouldSplit = mem.PlayerData<bool>(Offset.openedCrossroads); break;
                case SplitName.DeepnestStation: shouldSplit = mem.PlayerData<bool>(Offset.openedDeepnest); break;
                case SplitName.GreenpathStation: shouldSplit = mem.PlayerData<bool>(Offset.openedGreenpath); break;
                case SplitName.HiddenStationStation: shouldSplit = mem.PlayerData<bool>(Offset.openedHiddenStation); break;
                case SplitName.KingsStationStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRuins2); break;
                case SplitName.QueensGardensStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRoyalGardens); break;
                case SplitName.QueensStationStation: shouldSplit = mem.PlayerData<bool>(Offset.openedFungalWastes); break;
                case SplitName.RestingGroundsStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRestingGrounds); break;
                case SplitName.StagnestStation:
                    shouldSplit = nextScene.Equals("Cliffs_03", StringComparison.OrdinalIgnoreCase)
                        && mem.PlayerData<bool>(Offset.travelling)
                        && mem.PlayerData<bool>(Offset.openedStagNest); break;
                case SplitName.StoreroomsStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRuins1); break;

                #endregion Stags

                #region Areas

                case SplitName.Abyss: shouldSplit = mem.PlayerData<bool>(Offset.visitedAbyss); break;
                case SplitName.CityOfTears: shouldSplit = mem.PlayerData<bool>(Offset.visitedRuins); break;
                case SplitName.Colosseum: shouldSplit = mem.PlayerData<bool>(Offset.seenColosseumTitle); break;
                case SplitName.CrystalPeak: shouldSplit = mem.PlayerData<bool>(Offset.visitedMines); break;
                case SplitName.Deepnest: shouldSplit = mem.PlayerData<bool>(Offset.visitedDeepnest); break;
                case SplitName.DeepnestSpa: shouldSplit = mem.PlayerData<bool>(Offset.visitedDeepnestSpa); break;
                case SplitName.Dirtmouth: shouldSplit = mem.PlayerData<bool>(Offset.visitedDirtmouth); break;
                case SplitName.FogCanyon: shouldSplit = mem.PlayerData<bool>(Offset.visitedFogCanyon); break;
                case SplitName.ForgottenCrossroads:
                    shouldSplit = mem.PlayerData<bool>(Offset.visitedCrossroads);
                    shouldSkip = !currScene.StartsWith("Crossroads_"); // in most cases it will cause the Split to Skip if this Split is triggered by the splits file getting Reset from incompatible splits
                    break;
                case SplitName.FungalWastes: shouldSplit = mem.PlayerData<bool>(Offset.visitedFungus); break;
                case SplitName.Godhome: shouldSplit = mem.PlayerData<bool>(Offset.visitedGodhome); break;
                case SplitName.Greenpath: shouldSplit = mem.PlayerData<bool>(Offset.visitedGreenpath); break;
                case SplitName.Hive: shouldSplit = mem.PlayerData<bool>(Offset.visitedHive); break;
                case SplitName.InfectedCrossroads: shouldSplit = mem.PlayerData<bool>(Offset.crossroadsInfected) && mem.PlayerData<bool>(Offset.visitedCrossroads); break;
                case SplitName.KingdomsEdge: shouldSplit = mem.PlayerData<bool>(Offset.visitedOutskirts); break;
                case SplitName.QueensGardens: shouldSplit = mem.PlayerData<bool>(Offset.visitedRoyalGardens); break;
                case SplitName.RestingGrounds: shouldSplit = mem.PlayerData<bool>(Offset.visitedRestingGrounds); break;
                case SplitName.RoyalWaterways: shouldSplit = mem.PlayerData<bool>(Offset.visitedWaterways); break;
                case SplitName.WhitePalace: shouldSplit = mem.PlayerData<bool>(Offset.visitedWhitePalace); break;

                #endregion Areas

                #region Miscellaneous

                case SplitName.ManualSplit: shouldSplit = false; break;

                case SplitName.DgateKingdomsEdgeAcid:
                    shouldSplit =
                        mem.PlayerDataString<string>(Offset.dreamGateScene).StartsWith("Deepnest_East_04")
                        && (mem.PlayerData<float>(Offset.dreamGateX) > 27.0f && mem.PlayerData<float>(Offset.dreamGateX) < 29f)
                        && (mem.PlayerData<float>(Offset.dreamGateY) > 7.0f && mem.PlayerData<float>(Offset.dreamGateY) < 9f);
                    break;

                case SplitName.PureSnail: // the award for the most miscellaneous split goes to this one probably
                    int health = mem.PlayerData<int>(Offset.health);
                    shouldSplit = mem.Focusing() // Healing
                        && 0 < store.HealthBeforeFocus
                        && (store.HealthBeforeFocus < health
                            || (health == mem.PlayerData<int>(Offset.maxHealth)
                                && mem.PlayerData<int>(Offset.MPCharge) + 33 <= store.MPChargeBeforeFocus))
                        && mem.PlayerData<bool>(Offset.equippedCharm_5) // Baldur Shell
                        && mem.PlayerData<bool>(Offset.equippedCharm_7) // Quick Focus
                        && mem.PlayerData<bool>(Offset.equippedCharm_17) // Spore Shroom
                        && mem.PlayerData<bool>(Offset.equippedCharm_28); // Shape of Unn
                    break;

                #endregion Miscellaneous

                #region Enemies

                case SplitName.Aluba: shouldSplit = mem.PlayerData<bool>(Offset.killedLazyFlyer); break;
                case SplitName.AspidHunter: shouldSplit = mem.PlayerData<int>(Offset.killsSpitter) == 17; break;
                case SplitName.GorgeousHusk: shouldSplit = mem.PlayerData<bool>(Offset.killedGorgeousHusk); break;
                case SplitName.GreatHopper: shouldSplit = mem.PlayerData<bool>(Offset.killedGiantHopper); break;
                case SplitName.GreatHuskSentry: shouldSplit = mem.PlayerData<bool>(Offset.killedGreatShieldZombie); break;
                case SplitName.HuskMiner: shouldSplit = store.CheckIncreasedBy(Offset.killsZombieMiner, -1); break;
                case SplitName.killedSoulTwister: shouldSplit = mem.PlayerData<bool>(Offset.killedMage); break;
                case SplitName.Maggots: shouldSplit = mem.PlayerData<int>(Offset.killsPrayerSlug) == 0; break;
                case SplitName.MenderBug: shouldSplit = mem.PlayerData<bool>(Offset.killedMenderBug); break;
                case SplitName.MossKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedMossKnight); break;
                case SplitName.MushroomBrawler: shouldSplit = mem.PlayerData<int>(Offset.killsMushroomBrawler) == 6; break;
                case SplitName.RollerHuntersNotes: shouldSplit = mem.PlayerData<int>(Offset.killsRoller) == 0; break;

                #endregion Enemies

                #region NPC Interactions

                case SplitName.NailsmithKilled: shouldSplit = mem.PlayerData<bool>(Offset.nailsmithKilled); break;
                case SplitName.NailsmithChoice:
                    shouldSplit = mem.PlayerData<bool>(Offset.nailsmithKilled);
                    shouldSkip = mem.PlayerData<bool>(Offset.nailsmithSpared);
                    break;
                case SplitName.HappyCouplePlayerDataEvent: shouldSplit = mem.PlayerData<bool>(Offset.nailsmithConvoArt); break;

                case SplitName.WhiteDefenderStatueUnlocked:
                    shouldSplit =
                        currScene == "Waterways_15"
                        && mem.PlayerData<bool>(Offset.dungDefenderAwoken)
                        && mem.PlayerData<bool>(Offset.dungDefenderLeft)
                        && store.DungDefenderAwakeConvoOnEntry
                        && (mem.GetCameraTarget().X < 29.5);
                    break;

                case SplitName.Lemm2: shouldSplit = mem.PlayerData<bool>(Offset.metRelicDealerShop); break; // finally deleted Lemm1, I don't think I ever saw it uncommented
                case SplitName.AllCharmNotchesLemm2CP:
                    shouldSplit =
                        mem.PlayerData<int>(Offset.soldTrinket1) == 1
                        && mem.PlayerData<int>(Offset.soldTrinket2) == 6
                        && mem.PlayerData<int>(Offset.soldTrinket3) == 4;
                    break;

                case SplitName.SlyRescued: shouldSplit = mem.PlayerData<bool>(Offset.slyRescued); break;
                case SplitName.SlyShopFinished:
                    shouldSplit =
                        mem.PlayerData<int>(Offset.vesselFragments) == 8 || (mem.PlayerData<int>(Offset.MPReserveMax) == 66
                        && mem.PlayerData<int>(Offset.vesselFragments) == 2)
                        && !currScene.StartsWith("Room_shop")
                        && mem.PlayerData<bool>(Offset.gotCharm_37);
                    break;
                case SplitName.ElegantKeyShoptimised:
                    shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 5 && mem.PlayerData<int>(Offset.heartPieces) == 1
                        && mem.PlayerData<bool>(Offset.hasWhiteKey);
                    break;
                case SplitName.PreGrimmShop:
                    shouldSplit = mem.PlayerData<bool>(Offset.hasLantern)
                        && mem.PlayerData<int>(Offset.maxHealthBase) == 6
                        && (mem.PlayerData<int>(Offset.vesselFragments) == 4 || (mem.PlayerData<int>(Offset.MPReserveMax) == 33 && mem.PlayerData<int>(Offset.vesselFragments) == 2));
                    break;

                case SplitName.BrettaRescued: shouldSplit = mem.PlayerData<bool>(Offset.brettaRescued); break;
                case SplitName.BrummFlame: shouldSplit = mem.PlayerData<bool>(Offset.gotBrummsFlame); break;
                case SplitName.CorniferAtHome:
                    shouldSplit = mem.PlayerData<bool>(Offset.corniferAtHome) && currScene.StartsWith("Town") && nextScene.StartsWith("Room_mapper");
                    break;
                case SplitName.MetEmilitia: shouldSplit = mem.PlayerData<bool>(Offset.metEmilitia); break;
                case SplitName.SavedCloth: shouldSplit = mem.PlayerData<bool>(Offset.savedCloth); break;
                case SplitName.HuntersMark: shouldSplit = mem.PlayerData<bool>(Offset.killedHunterMark); break;
                case SplitName.LittleFool: shouldSplit = mem.PlayerData<bool>(Offset.littleFoolMet); break;


                // Flower Quest               
                case SplitName.MetGreyMourner: shouldSplit = mem.PlayerData<bool>(Offset.metXun); break;
                case SplitName.FlowerQuest: shouldSplit = mem.PlayerData<bool>(Offset.xunFlowerGiven); break;
                case SplitName.HasDelicateFlower: shouldSplit = mem.PlayerData<bool>(Offset.hasXunFlower); break;
                case SplitName.FlowerRewardGiven: shouldSplit = mem.PlayerData<bool>(Offset.xunRewardGiven); break;
                case SplitName.ElderbugFlower: shouldSplit = mem.PlayerData<bool>(Offset.elderbugGaveFlower); break;
                case SplitName.givenGodseekerFlower: shouldSplit = mem.PlayerData<bool>(Offset.givenGodseekerFlower); break;
                case SplitName.givenOroFlower: shouldSplit = mem.PlayerData<bool>(Offset.givenOroFlower); break;
                case SplitName.givenWhiteLadyFlower: shouldSplit = mem.PlayerData<bool>(Offset.givenWhiteLadyFlower); break;
                case SplitName.givenEmilitiaFlower: shouldSplit = mem.PlayerData<bool>(Offset.givenEmilitiaFlower); break;
                case SplitName.GreyMournerSeerAscended:
                    shouldSplit = mem.PlayerData<bool>(Offset.metXun) && mem.PlayerData<bool>(Offset.mothDeparted);
                    break;

                case SplitName.SeerDeparts: shouldSplit = mem.PlayerData<bool>(Offset.mothDeparted); break;
                case SplitName.SpiritGladeOpen: shouldSplit = mem.PlayerData<bool>(Offset.gladeDoorOpened); break;

                case SplitName.Zote1: shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedBuzzer); break;
                case SplitName.Zote2: shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedDeepnest); break;
                case SplitName.ZoteKilled: shouldSplit = mem.PlayerData<bool>(Offset.killedZote); break;


                #endregion NPC Interactions

                #region Events

                case SplitName.AbyssDoor: shouldSplit = mem.PlayerData<bool>(Offset.abyssGateOpened); break;
                case SplitName.AbyssLighthouse: shouldSplit = mem.PlayerData<bool>(Offset.abyssLighthouse); break;
                case SplitName.BeastsDenTrapBench: shouldSplit = mem.PlayerData<bool>(Offset.spiderCapture); break;
                case SplitName.CanOvercharm: shouldSplit = mem.PlayerData<bool>(Offset.canOvercharm); break;
                case SplitName.CityGateOpen: shouldSplit = mem.PlayerData<bool>(Offset.openedCityGate); break;
                case SplitName.CityGateAndMantisLords: shouldSplit = mem.PlayerData<bool>(Offset.openedCityGate) && mem.PlayerData<bool>(Offset.defeatedMantisLords); break;
                case SplitName.EternalOrdealAchieved: shouldSplit = mem.PlayerData<bool>(Offset.ordealAchieved); break;
                case SplitName.EternalOrdealUnlocked: shouldSplit = mem.PlayerData<bool>(Offset.zoteStatueWallBroken); break;
                case SplitName.MineLiftOpened: shouldSplit = mem.PlayerData<bool>(Offset.mineLiftOpened); break;
                case SplitName.PlayerDeath: shouldSplit = mem.PlayerData<int>(Offset.health) == 0; break;
                case SplitName.SoulTyrantEssenceWithSanctumGrub:
                    shouldSplit =
                        mem.PlayerData<bool>(Offset.mageLordOrbsCollected)
                        && mem.PlayerDataStringList(Offset.scenesGrubRescued).Contains("Ruins1_32"); break;
                case SplitName.ShadeKilled: shouldSplit = store.CheckToggledFalse(Offset.soulLimited); break;
                case SplitName.TramDeepnest: shouldSplit = mem.PlayerData<bool>(Offset.openedTramLower); break;
                case SplitName.OnDefeatGPZ: shouldSplit = store.CheckIncremented(Offset.greyPrinceDefeats); break;
                case SplitName.OnDefeatWhiteDefender: shouldSplit = store.CheckIncremented(Offset.whiteDefenderDefeats); break;
                case SplitName.OnGhostCoinsIncremented: shouldSplit = store.CheckIncremented(Offset.ghostCoins); break;
                case SplitName.OnObtainGrub: shouldSplit = store.CheckIncremented(Offset.grubsCollected); break;
                case SplitName.OnUseSimpleKey: shouldSplit = store.CheckIncreasedBy(Offset.simpleKeys, -1); break;
                case SplitName.UnchainedHollowKnight: shouldSplit = mem.PlayerData<bool>(Offset.unchainedHollowKnight); break;
                case SplitName.WatcherChandelier: shouldSplit = mem.PlayerData<bool>(Offset.watcherChandelier); break;
                case SplitName.WaterwaysManhole: shouldSplit = mem.PlayerData<bool>(Offset.openedWaterwaysManhole); break;

                #endregion Events

                #region Grimm Quest

                case SplitName.Flame1: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 1; break;
                case SplitName.Flame2: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 2; break;
                case SplitName.Flame3: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 3; break;
                case SplitName.TransFlame1: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 1 && nextScene != currScene; break;
                case SplitName.TransFlame2: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 2 && nextScene != currScene; break;
                case SplitName.TransFlame3: shouldSplit = mem.PlayerData<int>(Offset.flamesCollected) == 3 && nextScene != currScene; break;
                // Brumm's flame is in NPC Interactions #region

                case SplitName.NightmareLantern: shouldSplit = mem.PlayerData<bool>(Offset.nightmareLanternLit); break;
                case SplitName.NightmareLanternDestroyed: shouldSplit = mem.PlayerData<bool>(Offset.destroyedNightmareLantern); break;
                // NKG boss fight sorted under Bosses #region

                #endregion Grimm Quest

                #region Mister Mushroom

                case SplitName.MrMushroom1: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 2; break;
                case SplitName.MrMushroom2: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 3; break;
                case SplitName.MrMushroom3: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 4; break;
                case SplitName.MrMushroom4: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 5; break;
                case SplitName.MrMushroom5: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 6; break;
                case SplitName.MrMushroom6: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 7; break;
                case SplitName.MrMushroom7: shouldSplit = mem.PlayerData<int>(Offset.mrMushroomState) == 8; break;

                #endregion Mister Mushroom

                #region Pale Ore & Nail Upgrades

                case SplitName.PaleOre: shouldSplit = mem.PlayerData<int>(Offset.ore) > 0; break;
                case SplitName.OnObtainPaleOre: shouldSplit = store.CheckIncremented(Offset.ore); break;

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

                case SplitName.NailUpgrade1: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 1; break;
                case SplitName.NailUpgrade2: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 2; break;
                case SplitName.NailUpgrade3: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 3; break;
                case SplitName.NailUpgrade4: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 4; break;

                #endregion Pale Ore & Nail Upgrades

                #region Grubs & Mimics

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

                case SplitName.GrubBasinDive: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Abyss_17"; break;
                case SplitName.GrubBasinWings: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Abyss_19"; break;
                case SplitName.GrubCityBelowLoveTower: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Ruins2_07"; break;
                case SplitName.GrubCityBelowSanctum: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Ruins1_05"; break;
                case SplitName.GrubCityCollector: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Ruins2_11"; break;
                case SplitName.GrubCityCollectorAll: shouldSplit = mem.PlayerDataStringList(Offset.scenesGrubRescued).Contains("Ruins2_11"); break;
                case SplitName.GrubCityGuardHouse: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Ruins_House_01"; break;
                case SplitName.GrubCitySanctum: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Ruins1_32"; break;
                case SplitName.GrubCitySpire: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Ruins2_03"; break;
                case SplitName.GrubCliffsBaldurShell: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus1_28"; break;
                case SplitName.GrubCrossroadsAcid: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Crossroads_35"; break;
                case SplitName.GrubCrossroadsGuarded: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Crossroads_48"; break;
                case SplitName.GrubCrossroadsSpikes: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Crossroads_31"; break;
                case SplitName.GrubCrossroadsVengefly: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Crossroads_05"; break;
                case SplitName.GrubCrossroadsWall: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Crossroads_03"; break;
                case SplitName.GrubCrystalPeaksBottomLever: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Mines_04"; break;
                case SplitName.GrubCrystalPeaksCrown: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Mines_24"; break;
                case SplitName.GrubCrystalPeaksCrushers: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Mines_19"; break;
                case SplitName.GrubCrystalPeaksCrystalHeart: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Mines_31"; break;
                case SplitName.GrubCrystalPeaksMimics: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Mines_16"; break;
                case SplitName.GrubCrystalPeaksMound: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Mines_35"; break;
                case SplitName.GrubCrystalPeaksSpikes: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Mines_03"; break;
                case SplitName.GrubDeepnestBeastsDen: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Deepnest_Spider_Town"; break;
                case SplitName.GrubDeepnestDark: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Deepnest_39"; break;
                case SplitName.GrubDeepnestMimics: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Deepnest_36"; break;
                case SplitName.GrubDeepnestNosk: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Deepnest_31"; break;
                case SplitName.GrubDeepnestSpikes: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Deepnest_03"; break;
                case SplitName.GrubFogCanyonArchives: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus3_47"; break;
                case SplitName.GrubFungalBouncy: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus2_18"; break;
                case SplitName.GrubFungalSporeShroom: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus2_20"; break;
                case SplitName.GrubGreenpathCornifer: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus1_06"; break;
                case SplitName.GrubGreenpathHunter: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus1_07"; break;
                case SplitName.GrubGreenpathMossKnight: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus1_21"; break;
                case SplitName.GrubGreenpathVesselFragment: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus1_13"; break;
                case SplitName.GrubHiveExternal: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Hive_03"; break;
                case SplitName.GrubHiveInternal: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Hive_04"; break;
                case SplitName.GrubKingdomsEdgeCenter: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Deepnest_East_11"; break;
                case SplitName.GrubKingdomsEdgeOro: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Deepnest_East_14"; break;
                case SplitName.GrubQueensGardensBelowStag: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus3_10"; break;
                case SplitName.GrubQueensGardensUpper: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus3_22"; break;
                case SplitName.GrubQueensGardensWhiteLady: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Fungus3_48"; break;
                case SplitName.GrubRestingGroundsCrypts: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "RestingGrounds_10"; break;
                case SplitName.GrubWaterwaysCenter: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Waterways_04"; break;
                case SplitName.GrubWaterwaysHwurmps: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Waterways_14"; break;
                case SplitName.GrubWaterwaysIsma: shouldSplit = store.CheckIncremented(Offset.grubsCollected) && currScene == "Waterways_13"; break;

                case SplitName.Mimic1: shouldSplit = mem.PlayerData<int>(Offset.killsGrubMimic) == 4; break;
                case SplitName.Mimic2: shouldSplit = mem.PlayerData<int>(Offset.killsGrubMimic) == 3; break;
                case SplitName.Mimic3: shouldSplit = mem.PlayerData<int>(Offset.killsGrubMimic) == 2; break;
                case SplitName.Mimic4: shouldSplit = mem.PlayerData<int>(Offset.killsGrubMimic) == 1; break;
                case SplitName.Mimic5: shouldSplit = mem.PlayerData<int>(Offset.killsGrubMimic) == 0; break;

                #endregion Grubs & Mimics

                #region Dream Trees

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

                #endregion Dream Trees

                case SplitName.ColosseumBronze: shouldSplit = mem.PlayerData<bool>(Offset.colosseumBronzeCompleted); break;
                case SplitName.ColosseumGold: shouldSplit = mem.PlayerData<bool>(Offset.colosseumGoldCompleted); break;
                case SplitName.ColosseumSilver: shouldSplit = mem.PlayerData<bool>(Offset.colosseumSilverCompleted); break;
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
                case SplitName.Pantheon1: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier1); break;
                case SplitName.Pantheon2: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier2); break;
                case SplitName.Pantheon3: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier3); break;
                case SplitName.Pantheon4: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier4); break;
                case SplitName.Pantheon5: shouldSplit = mem.PlayerData<bool>(Offset.bossDoorStateTier5); break;
                case SplitName.VesselFragment1: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 0 && mem.PlayerData<int>(Offset.vesselFragments) == 1; break;
                case SplitName.VesselFragment2: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 0 && mem.PlayerData<int>(Offset.vesselFragments) == 2; break;
                case SplitName.Vessel1: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 33; break;
                case SplitName.VesselFragment4: shouldSplit = mem.PlayerData<int>(Offset.vesselFragments) == 4 || (mem.PlayerData<int>(Offset.MPReserveMax) == 33 && mem.PlayerData<int>(Offset.vesselFragments) == 1); break;
                case SplitName.VesselFragment5: shouldSplit = mem.PlayerData<int>(Offset.vesselFragments) == 5 || (mem.PlayerData<int>(Offset.MPReserveMax) == 33 && mem.PlayerData<int>(Offset.vesselFragments) == 2); break;
                case SplitName.Vessel2: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 66; break;
                case SplitName.VesselFragment7: shouldSplit = mem.PlayerData<int>(Offset.vesselFragments) == 7 || (mem.PlayerData<int>(Offset.MPReserveMax) == 66 && mem.PlayerData<int>(Offset.vesselFragments) == 1); break;
                case SplitName.VesselFragment8: shouldSplit = mem.PlayerData<int>(Offset.vesselFragments) == 8 || (mem.PlayerData<int>(Offset.MPReserveMax) == 66 && mem.PlayerData<int>(Offset.vesselFragments) == 2); break;
                case SplitName.Vessel3: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 99; break;

                case SplitName.Essence100: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 100; break;
                case SplitName.Essence200: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 200; break;
                case SplitName.Essence300: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 300; break;
                case SplitName.Essence400: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 400; break;
                case SplitName.Essence500: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 500; break;
                case SplitName.Essence600: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 600; break;
                case SplitName.Essence700: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 700; break;
                case SplitName.Essence800: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 800; break;
                case SplitName.Essence900: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 900; break;
                case SplitName.Essence1000: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1000; break;
                case SplitName.Essence1100: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1100; break;
                case SplitName.Essence1200: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1200; break;
                case SplitName.Essence1300: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1300; break;
                case SplitName.Essence1400: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1400; break;
                case SplitName.Essence1500: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1500; break;
                case SplitName.Essence1600: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1600; break;
                case SplitName.Essence1700: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1700; break;
                case SplitName.Essence1800: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1800; break;
                case SplitName.Essence1900: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 1900; break;
                case SplitName.Essence2000: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 2000; break;
                case SplitName.Essence2100: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 2100; break;
                case SplitName.Essence2200: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 2200; break;
                case SplitName.Essence2300: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 2300; break;
                case SplitName.Essence2400: shouldSplit = mem.PlayerData<int>(Offset.dreamOrbs) >= 2400; break;

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
                case SplitName.GalienP: shouldSplit = sceneName.StartsWith("GG_Ghost_Galien") && (nextScene.StartsWith("GG_Grey_Prince_Zote") || nextScene.StartsWith("GG_Painter") || nextScene.StartsWith("GG_Uumuu")); break;
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
                case SplitName.MarkothP: shouldSplit = sceneName.StartsWith("GG_Ghost_Markoth") && (nextScene.StartsWith("GG_Watcher_Knights") || nextScene.StartsWith("GG_Grey_Prince_Zote") || nextScene.StartsWith("GG_Failed_Champion")); break;
                case SplitName.WatcherKnightsP: shouldSplit = sceneName.StartsWith("GG_Watcher_Knights") && (nextScene.StartsWith("GG_Soul_Tyrant") || nextScene.StartsWith("GG_Uumuu")); break;
                case SplitName.SoulTyrantP: shouldSplit = sceneName.StartsWith("GG_Soul_Tyrant") && (nextScene == "GG_Engine_Prime" || nextScene.StartsWith("GG_Ghost_Markoth")); break;
                case SplitName.PureVesselP: shouldSplit = sceneName.StartsWith("GG_Hollow_Knight") && (nextScene == "GG_End_Sequence" || nextScene.StartsWith("GG_Radiance") || nextScene == "GG_Door_5_Finale"); break;

                case SplitName.NoskHornetP: shouldSplit = sceneName.StartsWith("GG_Nosk_Hornet") && nextScene.StartsWith("GG_Sly"); break;
                case SplitName.NightmareKingGrimmP: shouldSplit = sceneName.StartsWith("GG_Grimm_Nightmare") && nextScene == "GG_Spa"; break;

                // sit at benches
                case SplitName.BenchAny: shouldSplit = mem.PlayerData<bool>(Offset.atBench); break;
                /*
                case SplitName.BenchDirtmouth : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Town"; break;
                case SplitName.BenchMato : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Room_Naimlaster"; break;
                case SplitName.BenchCrossroadsHotsprings : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Crossroads_30"; break;*/
                case SplitName.BenchCrossroadsStag: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Crossroads_47"; break;/*
                case SplitName.BenchSalubra : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Crossroads_04"; break;
                case SplitName.BenchAncestralMound : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Crossroads_ShamanTemple"; break;
                case SplitName.BenchBlackEgg : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Room_Final_Boss_Atrium"; break;
                case SplitName.BenchWaterfall : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus1_01b"; break;
                case SplitName.BenchStoneSanctuary : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus1_37"; break;
                case SplitName.BenchGreenpathToll : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus1_31"; break;*/
                case SplitName.BenchGreenpathStag: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus1_16_alt"; break;/*
                case SplitName.BenchLakeOfUnn : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Room_Slug_Shrine"; break;
                case SplitName.BenchSheo : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus1_16"; break;
                case SplitName.BenchArchives : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus3_Archive"; break;*/
                case SplitName.BenchQueensStation: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus2_02"; break;/*
                case SplitName.BenchLegEater : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus2_26"; break;
                case SplitName.BenchBretta : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus2_13"; break;
                case SplitName.BenchMantisVillage : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus2_31"; break;
                case SplitName.BenchQuirrel : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Ruins1_02"; break;
                case SplitName.BenchCityToll : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Ruins1_31"; break;*/
                case SplitName.BenchStorerooms: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Ruins1_29"; break;
                case SplitName.BenchSpire: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Ruins1_18"; break;
                case SplitName.BenchSpireGHS: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Ruins1_18" && mem.PlayerData<int>(Offset.killsGreatShieldZombie) < 10; break;
                case SplitName.BenchKingsStation: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Ruins2_08"; break;/*
                case SplitName.BenchPleasureHouse : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Ruins_Bathhouse"; break;
                case SplitName.BenchWaterways : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Waterways"; break;
                case SplitName.BenchDeepnestHotsprings : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Deepnest_30"; break;
                case SplitName.BenchFailedTramway : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Deepnest_14"; break;
                case SplitName.BenchDeepnestSpiderTown : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Deepnest_Spider_Town"; break;
                case SplitName.BenchBasinToll : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Abyss_18"; break;*/
                case SplitName.BenchHiddenStation: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Abyss_22"; break;/*
                case SplitName.BenchOro : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Deepnest_East_06"; break;
                case SplitName.BenchCamp : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Deepnest_East_13"; break;
                case SplitName.BenchColosseum : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName.StartsWith("Room_Colosseum"); break;
                case SplitName.BenchHive : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName.StartsWith("Hive"); break;
                case SplitName.BenchDarkRoom : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Mines_29"; break;
                case SplitName.BenchCG1 : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Mines_18"; break;*/
                case SplitName.BenchRGStag: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "RestingGrounds_09"; break;/*
                case SplitName.BenchFlowerQuest : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "RestingGrounds_12"; break;
                case SplitName.BenchQGCornifer : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus1_24"; break;
                case SplitName.BenchQGToll : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus3_50"; break;*/
                case SplitName.BenchQGStag: shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "Fungus3_40"; break;/*
                case SplitName.BenchTram : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && (sceneName == "Room_Tram" || sceneName == "Room_Tram_RG"); break;
                case SplitName.BenchWhitePalaceEntrance : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "White_Palace_01"; break;
                case SplitName.BenchWhitePalaceAtrium : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "White_Palace_03"; break;
                case SplitName.BenchWhitePalaceBalcony : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "White_Palace_06"; break;
                case SplitName.BenchGodhomeAtrium : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "GG_Atrium"; break;
                case SplitName.BenchHallOfGods : shouldSplit = mem.PlayerData<bool>(Offset.atBench) && sceneName == "GG_Workshop"; break;                
                */

                // unlock toll benches
                case SplitName.TollBenchQG: shouldSplit = mem.PlayerData<bool>(Offset.tollBenchQueensGardens); break;
                case SplitName.TollBenchCity: shouldSplit = mem.PlayerData<bool>(Offset.tollBenchCity); break;
                case SplitName.TollBenchBasin: shouldSplit = mem.PlayerData<bool>(Offset.tollBenchAbyss); break;

                /*
                 case SplitName.NailsmithSpared: shouldSplit = mem.PlayerData<bool>(Offset.nailsmithSpared); break;
            case SplitName.MageDoor: shouldSplit = mem.PlayerData<bool>(Offset.openedMageDoor); break;
            case SplitName.MageWindow: shouldSplit = mem.PlayerData<bool>(Offset.brokenMageWindow); break;
            case SplitName.MageLordEncountered: shouldSplit = mem.PlayerData<bool>(Offset.mageLordEncountered); break;
            case SplitName.MageDoor2: shouldSplit = mem.PlayerData<bool>(Offset.openedMageDoor_v2); break;
            case SplitName.MageWindowGlass: shouldSplit = mem.PlayerData<bool>(Offset.brokenMageWindowGlass); break;
            case SplitName.MageLordEncountered2: shouldSplit = mem.PlayerData<bool>(Offset.mageLordEncountered_2); break;
                */
                //case SplitName.NotchSly1: shouldSplit = mem.PlayerData<bool>(Offset.slyNotch1); break;
                //case SplitName.NotchSly2: shouldSplit = mem.PlayerData<bool>(Offset.slyNotch2); break;

                //case SplitName.Al2ba: shouldSplit = mem.PlayerData<int>(Offset.killsLazyFlyer) == 2; break;
                //case SplitName.Revek: shouldSplit = mem.PlayerData<int>(Offset.gladeGhostsKilled) == 19; break;
                //case SplitName.EquippedFragileHealth: shouldSplit = mem.PlayerData<bool>(Offset.equippedCharm_23); break;

                //case SplitName.AreaTestingSanctum: shouldSplit = mem.PlayerData<int>(Offset.currentArea) == (int)MapZone.SOUL_SOCIETY; break;
                //case SplitName.AreaTestingSanctumUpper: shouldSplit = mem.PlayerData<int>(Offset.currentArea) == (int)MapZone.MAGE_TOWER; break;

                case SplitName.FailedChampionEssence: shouldSplit = mem.PlayerData<bool>(Offset.falseKnightOrbsCollected); break;
                case SplitName.SoulTyrantEssence: shouldSplit = mem.PlayerData<bool>(Offset.mageLordOrbsCollected); break;
                case SplitName.LostKinEssence: shouldSplit = mem.PlayerData<bool>(Offset.infectedKnightOrbsCollected); break;
                case SplitName.WhiteDefenderEssence: shouldSplit = mem.PlayerData<bool>(Offset.whiteDefenderOrbsCollected); break;
                case SplitName.GreyPrinceEssence: shouldSplit = mem.PlayerData<bool>(Offset.greyPrinceOrbsCollected); break;

                case SplitName.ElderHuEssence: shouldSplit = mem.PlayerData<int>(Offset.elderHuDefeated) == 2; break;
                case SplitName.GalienEssence: shouldSplit = mem.PlayerData<int>(Offset.galienDefeated) == 2; break;
                case SplitName.GorbEssence: shouldSplit = mem.PlayerData<int>(Offset.aladarSlugDefeated) == 2; break;
                case SplitName.MarmuEssence: shouldSplit = mem.PlayerData<int>(Offset.mumCaterpillarDefeated) == 2; break;
                case SplitName.NoEyesEssence: shouldSplit = mem.PlayerData<int>(Offset.noEyesDefeated) == 2; break;
                case SplitName.XeroEssence: shouldSplit = mem.PlayerData<int>(Offset.xeroDefeated) == 2; break;
                case SplitName.MarkothEssence: shouldSplit = mem.PlayerData<int>(Offset.markothDefeated) == 2; break;

                case SplitName.Menu: shouldSplit = sceneName == "Menu_Title"; break;
                case SplitName.MenuClaw: shouldSplit = mem.PlayerData<bool>(Offset.hasWallJump); break;
                case SplitName.MenuGorgeousHusk: shouldSplit = mem.PlayerData<bool>(Offset.killedGorgeousHusk); break;
                case SplitName.MenuIsmasTear: shouldSplit = mem.PlayerData<bool>(Offset.hasAcidArmour); break;
                case SplitName.MenuShadeSoul: shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 2; break;

                case SplitName.mapDirtmouth: shouldSplit = mem.PlayerData<bool>(Offset.mapDirtmouth); break;
                case SplitName.mapCrossroads: shouldSplit = mem.PlayerData<bool>(Offset.mapCrossroads); break;
                case SplitName.mapGreenpath: shouldSplit = mem.PlayerData<bool>(Offset.mapGreenpath); break;
                case SplitName.mapFogCanyon: shouldSplit = mem.PlayerData<bool>(Offset.mapFogCanyon); break;
                case SplitName.mapRoyalGardens: shouldSplit = mem.PlayerData<bool>(Offset.mapRoyalGardens); break;
                case SplitName.mapFungalWastes: shouldSplit = mem.PlayerData<bool>(Offset.mapFungalWastes); break;
                case SplitName.mapCity: shouldSplit = mem.PlayerData<bool>(Offset.mapCity); break;
                case SplitName.mapWaterways: shouldSplit = mem.PlayerData<bool>(Offset.mapWaterways); break;
                case SplitName.mapMines: shouldSplit = mem.PlayerData<bool>(Offset.mapMines); break;
                case SplitName.mapDeepnest: shouldSplit = mem.PlayerData<bool>(Offset.mapDeepnest); break;
                case SplitName.mapCliffs: shouldSplit = mem.PlayerData<bool>(Offset.mapCliffs); break;
                case SplitName.mapOutskirts: shouldSplit = mem.PlayerData<bool>(Offset.mapOutskirts); break;
                case SplitName.mapRestingGrounds: shouldSplit = mem.PlayerData<bool>(Offset.mapRestingGrounds); break;
                case SplitName.mapAbyss: shouldSplit = mem.PlayerData<bool>(Offset.mapAbyss); break;

                case SplitName.OnObtainGhostMarissa:
                    shouldSplit = store.CheckIncremented(Offset.dreamOrbs) && sceneName == "Ruins_Bathhouse";
                    break;
                case SplitName.OnObtainGhostCaelifFera:
                    shouldSplit = store.CheckIncremented(Offset.dreamOrbs) && sceneName == "Fungus1_24";
                    break;
                case SplitName.OnObtainGhostPoggy:
                    shouldSplit = store.CheckIncremented(Offset.dreamOrbs) && sceneName == "Ruins_Elevator";
                    break;
                case SplitName.OnObtainGhostGravedigger:
                    shouldSplit = store.CheckIncremented(Offset.dreamOrbs) && sceneName == "Town";
                    break;
                case SplitName.OnObtainGhostJoni:
                    shouldSplit = store.CheckIncremented(Offset.dreamOrbs) && sceneName == "Cliffs_05";
                    break;
                case SplitName.OnObtainGhostCloth:
                    shouldSplit = store.CheckIncremented(Offset.dreamOrbs) && sceneName == "Fungus3_23" && store.TraitorLordDeadOnEntry;
                    break;
                case SplitName.OnObtainGhostVespa:
                    // UsesSceneTransitionRoutine is true on patches where Hive Knight exists
                    bool hiveKnightBeenDead = !mem.UsesSceneTransitionRoutine() || store.CheckBeenTrue(Offset.killedHiveKnight);
                    shouldSplit = store.CheckIncremented(Offset.dreamOrbs) && sceneName == "Hive_05" && hiveKnightBeenDead;
                    break;
                case SplitName.OnObtainGhostRevek:
                    if (sceneName == "RestingGrounds_08") {
                        shouldSplit = store.GladeEssence == 19 || store.GladeEssence == 18 && store.CheckIncremented(Offset.dreamOrbs);
                    }
                    break;

                case SplitName.MaskShardMawlek: if (sceneName == "Crossroads_09") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardGrubfather: if (sceneName == "Crossroads_38") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardBretta: if (sceneName == "Room_Bretta") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardQueensStation: if (sceneName == "Fungus2_01") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardEnragedGuardian: if (sceneName == "Mines_32") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardSeer: if (sceneName == "RestingGrounds_07") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardGoam: if (sceneName == "Crossroads_13") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardStoneSanctuary: if (sceneName == "Fungus1_36") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardWaterways: if (sceneName == "Waterways_04b") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardFungalCore: if (sceneName == "Fungus2_25") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardHive: if (sceneName == "Hive_04") { goto case SplitName.OnObtainMaskShard; } break;
                case SplitName.MaskShardFlower: if (sceneName == "Room_Mansion") { goto case SplitName.OnObtainMaskShard; } break;

                case SplitName.VesselFragGreenpath: if (sceneName == "Fungus1_13") { goto case SplitName.OnObtainVesselFragment; } break;
                case SplitName.VesselFragCrossroadsLift: if (sceneName == "Crossroads_37") { goto case SplitName.OnObtainVesselFragment; } break;
                case SplitName.VesselFragKingsStation: if (sceneName == "Ruins2_09") { goto case SplitName.OnObtainVesselFragment; } break;
                case SplitName.VesselFragGarpedes: if (sceneName == "Deepnest_38") { goto case SplitName.OnObtainVesselFragment; } break;
                case SplitName.VesselFragStagNest: if (sceneName == "Cliffs_03") { goto case SplitName.OnObtainVesselFragment; } break;
                case SplitName.VesselFragSeer: if (sceneName == "RestingGrounds_07") { goto case SplitName.OnObtainVesselFragment; } break;
                case SplitName.VesselFragFountain: if (sceneName == "Abyss_04") { goto case SplitName.OnObtainVesselFragment; } break;

                case SplitName.OnObtainMaskShard:
                    shouldSplit = store.CheckIncremented(Offset.maxHealthBase) || (store.CheckIncremented(Offset.heartPieces) && mem.PlayerData<int>(Offset.heartPieces) < 4);
                    break;
                case SplitName.OnObtainVesselFragment:
                    shouldSplit = store.CheckIncreasedBy(Offset.MPReserveMax, 33) || (store.CheckIncremented(Offset.vesselFragments) && mem.PlayerData<int>(Offset.vesselFragments) < 3);
                    break;

                case SplitName.ColosseumBronzeUnlocked: shouldSplit = mem.PlayerData<bool>(Offset.colosseumBronzeOpened); break;
                case SplitName.ColosseumSilverUnlocked: shouldSplit = mem.PlayerData<bool>(Offset.colosseumSilverOpened); break;
                case SplitName.ColosseumGoldUnlocked: shouldSplit = mem.PlayerData<bool>(Offset.colosseumGoldOpened); break;
                case SplitName.ColosseumBronzeEntry: shouldSplit = sceneName == "Room_Colosseum_01" && nextScene == "Room_Colosseum_Bronze"; break;
                case SplitName.ColosseumSilverEntry: shouldSplit = sceneName == "Room_Colosseum_01" && nextScene == "Room_Colosseum_Silver"; break;
                case SplitName.ColosseumGoldEntry: shouldSplit = sceneName == "Room_Colosseum_01" && nextScene == "Room_Colosseum_Gold"; break;
                case SplitName.ColosseumBronzeExit: shouldSplit = mem.PlayerData<bool>(Offset.colosseumBronzeCompleted) && !nextScene.StartsWith("Room_Colosseum_Bronze") && nextScene != sceneName; break;
                case SplitName.ColosseumSilverExit: shouldSplit = mem.PlayerData<bool>(Offset.colosseumSilverCompleted) && !nextScene.StartsWith("Room_Colosseum_Silver") && nextScene != sceneName; break;
                case SplitName.ColosseumGoldExit: shouldSplit = mem.PlayerData<bool>(Offset.colosseumGoldCompleted) && !nextScene.StartsWith("Room_Colosseum_Gold") && nextScene != sceneName; break;

                #region Trial of the Warrior
                case SplitName.Bronze1a: // 1 × Shielded Fool
                    shouldSplit = store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) == 1;
                    shouldSkip = mem.PlayerData<int>(Offset.killsColShield) == 0 || store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 1;
                    break;
                case SplitName.Bronze1b: // 2 × Shielded Fool
                    shouldSplit = store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) == 3;
                    shouldSkip = mem.PlayerData<int>(Offset.killsColShield) == 0 || store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 3;
                    break;
                case SplitName.Bronze1c: // 2 × Baldur
                    shouldSplit = store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) == 2;
                    shouldSkip = mem.PlayerData<int>(Offset.killsColRoller) == 0 || store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) > 2;
                    break;
                case SplitName.Bronze2: // 5 × Baldur
                    shouldSplit = store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) == 7;
                    shouldSkip = mem.PlayerData<int>(Offset.killsColRoller) == 0 || store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) > 7;
                    break;
                case SplitName.Bronze3a: // 1 × Sturdy Fool
                    shouldSplit = store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) == 1;
                    shouldSkip = mem.PlayerData<int>(Offset.killsColMiner) == 0 || store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) > 1;
                    break;
                case SplitName.Bronze3b: // 2 × Sturdy Fool
                    shouldSplit = store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) == 3;
                    shouldSkip = mem.PlayerData<int>(Offset.killsColMiner) == 0 || store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) > 3;
                    break;
                case SplitName.Bronze4: // 2 × Aspid
                    shouldSplit = store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) == 2;
                    shouldSkip = mem.PlayerData<int>(Offset.killsSpitter) == 0 || store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) > 2;
                    break;
                case SplitName.Bronze5: // 2 × Aspid
                    shouldSplit = store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) == 4;
                    shouldSkip = mem.PlayerData<int>(Offset.killsSpitter) == 0 || store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) > 4;
                    break;
                case SplitName.Bronze6: // 3 × Sturdy Fool
                    shouldSplit = store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) == 6;
                    shouldSkip = mem.PlayerData<int>(Offset.killsColMiner) == 0 || store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) > 6;
                    break;
                case SplitName.Bronze7: // 2 × Aspid, 2 × Baldur
                    shouldSplit =
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) == 6 &&
                        store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) == 9;
                    shouldSkip = mem.PlayerData<int>(Offset.killsSpitter) == 0 || mem.PlayerData<int>(Offset.killsColRoller) == 0 ||
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) > 6 ||
                        store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) > 9;
                    break;
                case SplitName.Bronze8a: // 4 × Vengefly
                    shouldSplit = store.killsBuzzerStart - mem.PlayerData<int>(Offset.killsBuzzer) == 4;
                    shouldSkip = mem.PlayerData<int>(Offset.killsBuzzer) == 0 || store.killsBuzzerStart - mem.PlayerData<int>(Offset.killsBuzzer) > 4;
                    break;
                case SplitName.Bronze8b: // 1 × Vengefly King
                    shouldSplit = store.killsBigBuzzerStart - mem.PlayerData<int>(Offset.killsBigBuzzer) == 1;
                    shouldSkip = mem.PlayerData<int>(Offset.killsBigBuzzer) == 0 || store.killsBigBuzzerStart - mem.PlayerData<int>(Offset.killsBigBuzzer) > 1;
                    break;
                case SplitName.Bronze9: // 3 × Sturdy Fool, 2 × Shielded Fool, 2 × Aspid, 2 × Baldur
                    shouldSplit =
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) == 8 &&
                        store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) == 10 &&
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) == 9 &&
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) == 5;
                    shouldSkip = mem.PlayerData<int>(Offset.killsSpitter) == 0 ||
                        mem.PlayerData<int>(Offset.killsColRoller) == 0 ||
                        mem.PlayerData<int>(Offset.killsColMiner) == 0 ||
                        mem.PlayerData<int>(Offset.killsColShield) == 0 ||
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) > 8 ||
                        store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) > 10 ||
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) > 9 ||
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 5;
                    break;
                case SplitName.Bronze10: // 3 × Baldur
                    shouldSplit = store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) == 13;
                    shouldSkip = mem.PlayerData<int>(Offset.killsColRoller) == 0 || store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) > 13;
                    break;
                case SplitName.Bronze11a: // 2 × Infected Gruzzer
                    shouldSplit = store.killsBurstingBouncerStart - mem.PlayerData<int>(Offset.killsBurstingBouncer) == 2;
                    shouldSkip = mem.PlayerData<int>(Offset.killsBurstingBouncer) == 0 || store.killsBurstingBouncerStart - mem.PlayerData<int>(Offset.killsBurstingBouncer) > 2;
                    break;
                case SplitName.Bronze11b: // 3 × Infected Gruzzer
                    shouldSplit = store.killsBurstingBouncerStart - mem.PlayerData<int>(Offset.killsBurstingBouncer) == 5;
                    shouldSkip = mem.PlayerData<int>(Offset.killsBurstingBouncer) == 0 || store.killsBurstingBouncerStart - mem.PlayerData<int>(Offset.killsBurstingBouncer) > 5;
                    break;
                case SplitName.BronzeEnd: // 2 × Gruz Mom
                    shouldSplit = store.killsBigFlyStart - mem.PlayerData<int>(Offset.killsBigFly) == 2;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsBigFly) == 0 &&
                        sceneName.StartsWith("Room_Colosseum_Bronze") &&
                        nextScene != sceneName;
                    break;
                #endregion

                #region Trial of the Conqueror
                case SplitName.Silver1: // 2 × Heavy Fool, 3 × Winged Fool
                    shouldSplit =
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 2 &&
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 3;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 2 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 3;
                    break;
                case SplitName.Silver2: // 2 × Squit
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 2;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 2;
                    break;
                case SplitName.Silver3: // 2 × Squit
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 4;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 4;
                    break;
                case SplitName.Silver4: // 1 × Squit, 1 × Winged Fool
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 5 &&
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 4;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 5 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 4;
                    break;
                case SplitName.Silver5: // 2 × Aspid, 2 × Squit, 5 × Infected Gruzzer, aspid kills in Colo 2 use killsSuperSpitter, even though Colo 1 and 3 use killsSpitter
                    shouldSplit =
                        store.killsBurstingBouncerStart - mem.PlayerData<int>(Offset.killsBurstingBouncer) == 5 &&
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 7 &&
                        store.killsSuperSpitterStart - mem.PlayerData<int>(Offset.killsSuperSpitter) == 2;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsBurstingBouncer) == 0 ||
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsSuperSpitter) == 0 ||
                        store.killsBurstingBouncerStart - mem.PlayerData<int>(Offset.killsBurstingBouncer) > 5 &&
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 7 &&
                        store.killsSuperSpitterStart - mem.PlayerData<int>(Offset.killsSuperSpitter) > 2;
                    break;
                case SplitName.Silver6: // 1 × Heavy Fool, 3 × Belfly
                    shouldSplit =
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 3 &&
                        store.killsCeilingDropperStart - mem.PlayerData<int>(Offset.killsCeilingDropper) == 3;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsCeilingDropper) == 0 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 3 ||
                        store.killsCeilingDropperStart - mem.PlayerData<int>(Offset.killsCeilingDropper) > 3;
                    break;
                case SplitName.Silver7: // 1 × Belfly
                    shouldSplit =
                        store.killsCeilingDropperStart - mem.PlayerData<int>(Offset.killsCeilingDropper) == 4;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsCeilingDropper) == 0 ||
                        store.killsCeilingDropperStart - mem.PlayerData<int>(Offset.killsCeilingDropper) > 4;
                    break;
                case SplitName.Silver8: // 8 × Hopper, 1 × Great Hopper
                    shouldSplit =
                        store.killsGiantHopperStart - mem.PlayerData<int>(Offset.killsGiantHopper) == 1; // only checking great hopper
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsGiantHopper) == 0 ||
                        store.killsGiantHopperStart - mem.PlayerData<int>(Offset.killsGiantHopper) > 1;
                    break;
                case SplitName.Silver9: // 1 × Great Hopper
                    shouldSplit =
                        store.killsGiantHopperStart - mem.PlayerData<int>(Offset.killsGiantHopper) == 2;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsGiantHopper) == 0 ||
                        store.killsGiantHopperStart - mem.PlayerData<int>(Offset.killsGiantHopper) > 2;
                    break;
                case SplitName.Silver10: // 1 × Mimic
                    shouldSplit =
                        store.killsGrubMimicStart - mem.PlayerData<int>(Offset.killsGrubMimic) == 1;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsGrubMimic) == 0 ||
                        store.killsGrubMimicStart - mem.PlayerData<int>(Offset.killsGrubMimic) > 1;
                    break;
                case SplitName.Silver11: // 2 × Shielded fool, 2 × Winged Fool, 1 × Heavy Fool, 2 × Squit
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 9 &&
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 6 &&
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 4;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 9 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 6 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 4;
                    break;
                case SplitName.Silver12: // 1 × Heavy Fool, 1 × Winged Fool
                    shouldSplit =
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 7 &&
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 5;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 7 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 5;
                    break;
                case SplitName.Silver13: // 1 × Winged Fool, 3 × Squit
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 12 &&
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 8;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 12 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 8;
                    break;
                case SplitName.Silver14: // 3 × Winged Fool, 2 × Squit
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 14 &&
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 11;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 14 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 11;
                    break;
                case SplitName.Silver15: // 9 × Obbles
                    shouldSplit =
                        store.killsBlobbleStart - mem.PlayerData<int>(Offset.killsBlobble) == 9;
                    shouldSkip =
                        store.killsBlobbleStart - mem.PlayerData<int>(Offset.killsBlobble) > 9 ||
                        mem.PlayerData<int>(Offset.killsBlobble) == 0;
                    break;
                case SplitName.Silver16: // 4 × Obbles
                    shouldSplit =
                        store.killsBlobbleStart - mem.PlayerData<int>(Offset.killsBlobble) == 13;
                    shouldSkip =
                        store.killsBlobbleStart - mem.PlayerData<int>(Offset.killsBlobble) > 13 ||
                        mem.PlayerData<int>(Offset.killsBlobble) == 0;
                    break;
                case SplitName.SilverEnd: // 2 × Oblobbles
                    shouldSplit =
                        store.killsOblobbleStart - mem.PlayerData<int>(Offset.killsOblobble) == 2;
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsOblobble) == 0 &&
                        sceneName.StartsWith("Room_Colosseum_Silver") &&
                        nextScene != sceneName;
                    break;
                #endregion

                #region Trial of the Fool
                case SplitName.Gold1:
                    #region
                    shouldSplit =
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 1 &&  // 1 Heavy Fool
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) == 1 &&  // 1 Sturdy Fool
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 2 &&  // 2 Squit
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) == 2 &&  // 2 Shielded Fool
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) == 1 &&  // 1 Aspid
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 2 &&  // 2 Winged Fool
                        store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) == 2;    // 2 Baldurs
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsColMiner) == 0 ||
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsColShield) == 0 ||
                        mem.PlayerData<int>(Offset.killsSpitter) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        mem.PlayerData<int>(Offset.killsColRoller) == 0 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 1 ||
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) > 1 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 2 ||
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 2 ||
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) > 1 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 2 ||
                        store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) > 2;
                    break;
                #endregion
                // Wave 2 splits inconsistently since the enemies are killed by the spikes on the floor automatically
                case SplitName.Gold3:
                    #region
                    shouldSplit =
                        store.killsBlobbleStart - mem.PlayerData<int>(Offset.killsBlobble) == 3 &&  // 3 Obble
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 3 &&  // 1 Winged Fool
                        store.killsAngryBuzzerStart - mem.PlayerData<int>(Offset.killsAngryBuzzer) == 2;    // 2 Infected Vengefly
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsBlobble) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        mem.PlayerData<int>(Offset.killsAngryBuzzer) == 0 ||
                        store.killsBlobbleStart - mem.PlayerData<int>(Offset.killsBlobble) > 3 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 3 ||
                        store.killsAngryBuzzerStart - mem.PlayerData<int>(Offset.killsAngryBuzzer) > 2;
                    break;
                #endregion
                case SplitName.Gold4:
                    #region
                    shouldSplit =
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 3 &&  // 2 Heavy Fool
                        store.killsCeilingDropperStart - mem.PlayerData<int>(Offset.killsCeilingDropper) == 6;    // 6 Belflies
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsCeilingDropper) == 0 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 3 ||
                        store.killsCeilingDropperStart - mem.PlayerData<int>(Offset.killsCeilingDropper) > 6;
                    break;
                #endregion
                case SplitName.Gold5:
                    #region
                    shouldSplit =
                        store.killsColHopperStart - mem.PlayerData<int>(Offset.killsColHopper) == 3;    // 3 Loodle
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColHopper) == 0 ||
                        store.killsColHopperStart - mem.PlayerData<int>(Offset.killsColHopper) > 3;
                    break;
                #endregion
                case SplitName.Gold6:
                    #region
                    shouldSplit =
                        store.killsColHopperStart - mem.PlayerData<int>(Offset.killsColHopper) == 8;    // 5 Loodle
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColHopper) == 0 ||
                        store.killsColHopperStart - mem.PlayerData<int>(Offset.killsColHopper) > 8;
                    break;
                #endregion
                case SplitName.Gold7:
                    #region
                    shouldSplit =
                        store.killsColHopperStart - mem.PlayerData<int>(Offset.killsColHopper) == 11;   // 3 Loodle
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColHopper) == 0 ||
                        store.killsColHopperStart - mem.PlayerData<int>(Offset.killsColHopper) > 11;
                    break;
                #endregion
                case SplitName.Gold8a:
                    #region
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 4 &&  // 2 Squit
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) == 5 &&  // 3 Aspid
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 4;    // 1 Winged Fool
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsSpitter) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 4 ||
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) > 5 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 4;
                    break;
                #endregion
                case SplitName.Gold8:
                    #region
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 6 &&  // 2 Squit
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 5;    // 1 Winged Fool
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 6 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 5;
                    break;
                #endregion
                case SplitName.Gold9a:
                    #region
                    shouldSplit =
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) == 3 &&  // 1 Shielded Fool
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 5 &&  // 2 Heavy Fool
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) == 6 &&  // 1 Aspid
                        store.killsHeavyMantisStart - mem.PlayerData<int>(Offset.killsHeavyMantis) == 2 &&  // 2 Mantis Traitor
                        store.killsHeavyMantisFlyerStart - mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) == 4;    // 4 Mantis Petra
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColShield) == 0 ||
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsSpitter) == 0 ||
                        mem.PlayerData<int>(Offset.killsHeavyMantis) == 0 ||
                        mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) == 0 ||
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 3 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 5 ||
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) > 6 ||
                        store.killsHeavyMantisStart - mem.PlayerData<int>(Offset.killsHeavyMantis) > 2 ||
                        store.killsHeavyMantisFlyerStart - mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) > 4;
                    break;
                #endregion
                case SplitName.Gold9b:
                    #region
                    shouldSplit =
                        store.killsMageKnightStart - mem.PlayerData<int>(Offset.killsMageKnight) == 1;    // 1 Soul Warrior
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsMageKnight) == 0 ||
                        store.killsMageKnightStart - mem.PlayerData<int>(Offset.killsMageKnight) > 1;
                    break;
                #endregion
                case SplitName.Gold10:
                    #region
                    shouldSplit =
                        store.killsElectricMageStart - mem.PlayerData<int>(Offset.killsElectricMage) == 3 &&  // 3 Volt Twister
                        store.killsMageStart - mem.PlayerData<int>(Offset.killsMage) == 4;    // 2 Soul Twister
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsElectricMage) == 0 ||
                        mem.PlayerData<int>(Offset.killsMage) == 0 ||
                        store.killsElectricMageStart - mem.PlayerData<int>(Offset.killsElectricMage) > 3 ||
                        store.killsMageStart - mem.PlayerData<int>(Offset.killsMage) > 4;
                    break;
                #endregion
                case SplitName.Gold11:
                    #region
                    shouldSplit =
                        store.killsMageKnightStart - mem.PlayerData<int>(Offset.killsMageKnight) == 2 &&  // 1 Soul Warrior
                        store.killsMageStart - mem.PlayerData<int>(Offset.killsMage) == 5;    // 1 Soul Twister
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsMageKnight) == 0 ||
                        store.killsMageKnightStart - mem.PlayerData<int>(Offset.killsMageKnight) > 2 ||
                        store.killsMageStart - mem.PlayerData<int>(Offset.killsMage) > 5;
                    break;
                #endregion
                case SplitName.Gold12a:
                    #region
                    shouldSplit =
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 7 &&  // 2 Winged Fool
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) == 4 &&  // 1 Sturdy Fool
                        store.killsLesserMawlekStart - mem.PlayerData<int>(Offset.killsLesserMawlek) == 4;    // 4 Lesser Mawlek
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        mem.PlayerData<int>(Offset.killsColMiner) == 0 ||
                        mem.PlayerData<int>(Offset.killsLesserMawlek) == 0 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 7 ||
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) > 4 ||
                        store.killsLesserMawlekStart - mem.PlayerData<int>(Offset.killsLesserMawlek) > 4;
                    break;
                #endregion
                case SplitName.Gold12b:
                    #region
                    shouldSplit =
                        store.killsMawlekStart - mem.PlayerData<int>(Offset.killsMawlek) == 1;    // 1 Brooding Mawlek
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsMawlek) == 0 ||
                        store.killsMawlekStart - mem.PlayerData<int>(Offset.killsMawlek) > 1;
                    break;
                #endregion
                // Wave 13 doesn't really exist, it's just vertical Garpedes so there's nothing to Split on
                case SplitName.Gold14a:
                    #region
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 10 && // 1 Squit
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) == 7 &&  // 1 Aspid
                        store.killsHeavyMantisFlyerStart - mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) == 5;    // 1 Mantis Petra
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) == 0 ||
                        mem.PlayerData<int>(Offset.killsSpitter) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 10 ||
                        store.killsHeavyMantisFlyerStart - mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) > 5 ||
                        store.killsSpitterStart - mem.PlayerData<int>(Offset.killsSpitter) - 1 > 7;
                    break;
                #endregion
                case SplitName.Gold14b:
                    #region
                    shouldSplit =
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 10 && // 2 Winged Fool
                        store.killsBlobbleStart - mem.PlayerData<int>(Offset.killsBlobble) == 7;    // 4 Obble
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        mem.PlayerData<int>(Offset.killsBlobble) == 0 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 10 ||
                        store.killsBlobbleStart - mem.PlayerData<int>(Offset.killsBlobble) > 7;
                    break;
                #endregion
                case SplitName.Gold15:
                    #region
                    shouldSplit =
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 12;    // 2 Squit
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColMosquito) == 0 ||
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) > 12;
                    break;
                #endregion
                case SplitName.Gold16:
                    #region
                    shouldSplit =
                        store.killsColHopperStart - mem.PlayerData<int>(Offset.killsColHopper) == 25;    // 14 Loodle elderC
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColHopper) == 0 ||
                        store.killsColHopperStart - mem.PlayerData<int>(Offset.killsColHopper) > 25;
                    break;
                #endregion
                case SplitName.Gold17a:
                    #region
                    shouldSplit =
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 6 &&  // 1 Heavy Fool
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) == 5 &&  // 1 Sturdy Fool
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) == 4 &&  // 1 Shielded Fool
                        store.killsHeavyMantisFlyerStart - mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) == 6 &&  // 1 Mantis Petra
                        store.killsHeavyMantisStart - mem.PlayerData<int>(Offset.killsHeavyMantis) == 3 &&  // 1 Mantis Traitor
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 11;   // 1 Winged Fool
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsColMiner) == 0 ||
                        mem.PlayerData<int>(Offset.killsColShield) == 0 ||
                        mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) == 0 ||
                        mem.PlayerData<int>(Offset.killsHeavyMantis) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 6 ||
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) > 5 ||
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 4 ||
                        store.killsHeavyMantisFlyerStart - mem.PlayerData<int>(Offset.killsMantisHeavyFlyer) - 1 > 6 ||
                        store.killsHeavyMantisStart - mem.PlayerData<int>(Offset.killsHeavyMantis) > 3 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) - 1 > 11;
                    break;
                #endregion
                case SplitName.Gold17b:
                    #region
                    shouldSplit =
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 7 &&  // 1 Heavy Fool
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) == 5 &&  // 1 Shielded Fool
                        store.killsMageStart - mem.PlayerData<int>(Offset.killsMage) == 6 &&  // 1 Soul Twister
                        store.killsElectricMageStart - mem.PlayerData<int>(Offset.killsElectricMage) == 4;    // 1 Volt Twister
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsColShield) == 0 ||
                        mem.PlayerData<int>(Offset.killsMage) == 0 ||
                        mem.PlayerData<int>(Offset.killsElectricMage) == 0 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 7 ||
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 5 ||
                        store.killsMageStart - mem.PlayerData<int>(Offset.killsMage) > 6 ||
                        store.killsElectricMageStart - mem.PlayerData<int>(Offset.killsElectricMage) > 4;
                    break;
                #endregion
                case SplitName.Gold17c:
                    #region
                    shouldSplit =
                        store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) == 4 &&  // 2 Baldur
                        store.killsColMosquitoStart - mem.PlayerData<int>(Offset.killsColMosquito) == 14 && // 2 Squit
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) == 8 &&  // 1 Heavy Fool
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) == 6 &&  // 1 Shielded Fool
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) == 6 &&  // 1 Sturdy Fool
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) == 12;   // 1 Winged Fool
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsColWorm) == 0 ||
                        mem.PlayerData<int>(Offset.killsColShield) == 0 ||
                        mem.PlayerData<int>(Offset.killsColMiner) == 0 ||
                        mem.PlayerData<int>(Offset.killsColFlyingSentry) == 0 ||
                        mem.PlayerData<int>(Offset.killsColRoller) == 0 ||
                        mem.PlayerData<int>(Offset.killsColShield) == 0 ||
                        store.killsColRollerStart - mem.PlayerData<int>(Offset.killsColRoller) > 4 ||
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 14 ||
                        store.killsColWormStart - mem.PlayerData<int>(Offset.killsColWorm) > 8 ||
                        store.killsColShieldStart - mem.PlayerData<int>(Offset.killsColShield) > 6 ||
                        store.killsColMinerStart - mem.PlayerData<int>(Offset.killsColMiner) > 6 ||
                        store.killsColFlyingSentryStart - mem.PlayerData<int>(Offset.killsColFlyingSentry) > 12;
                    break;
                #endregion
                case SplitName.GoldEnd:
                    #region
                    shouldSplit =
                        store.killsLobsterLancerStart - mem.PlayerData<int>(Offset.killsLobsterLancer) == 1;    // God Tamer
                    shouldSkip =
                        mem.PlayerData<int>(Offset.killsLobsterLancer) == 0 &&
                        sceneName.StartsWith("Room_Colosseum_Gold") &&
                        nextScene != sceneName;
                    break;
                #endregion
                #endregion

                default:
                    //throw new Exception(split + " does not have a defined shouldsplit value");
                    if (!failedValues.Contains(split)) {
                        failedValues.Add(split);
                    }
                    break;
            }

            if (shouldReset) {
                action = SplitterAction.Reset;
                store.ResetKills();
            } else if (shouldSkip) {
                action = SplitterAction.Skip;
            } else if (shouldSplit) {
                action = SplitterAction.Split;
            } else {
                action = SplitterAction.Pass;
            }

            return action;
        }

        private void HandleSplit(SplitterAction action, bool shouldReset = false) {
            bool splitAdvanced = false;

            if (action == SplitterAction.Reset || shouldReset) {
                if (currentSplit >= 0) {
                    Model.Reset();
                }
            } else if (action == SplitterAction.Skip) {
                if (currentSplit >= 0) {
                    Model.SkipSplit();
                }
                splitAdvanced = true;
            } else if (action == SplitterAction.Split) {
                if (currentSplit < 0) {
                    Model.Start();
                } else {
                    Model.Split();
                }
                splitAdvanced = true;
            }

            if (splitAdvanced) {
                store.SplitThisTransition = true;
                store.Update();
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
            store.Reset();
            if (failedValues.Count > 0) {
                WriteLog("---------Splits without match-------------------");
                foreach (var value in failedValues) {
                    WriteLogWithTime(value.ToString() + " - does not have a defined shouldsplit value");
                }
                failedValues.Clear();
            }
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
            store.Reset();
            failedValues.Clear();
            store.SplitThisTransition = true;
            store.Update();
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
            store.SplitThisTransition = true;
            store.Update();

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
