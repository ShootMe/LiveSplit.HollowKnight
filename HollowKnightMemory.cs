using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
namespace LiveSplit.HollowKnight {
    public partial class HollowKnightMemory {
        private ProgramPointer gameManager;
        public Process Program { get; set; }
        public bool IsHooked { get; set; }
        private DateTime lastHooked;
        private int uiManager, inputHandler, cameraCtrl, gameState, heroController, camTarget, camMode, camTMode, camDest, menuState, uiState, achievementHandler;
        private int heroAccepting, actorState, transistionState, camTeleport, playerData, debugInfo, tilemapDirty, cState, sceneName, nextSceneName, entryGateName, hazardRespawning, onGround, spellquake;
        //private int sceneData, awardAchievementEvent;
        private Version lastVersion;

        public HollowKnightMemory() {
            lastHooked = DateTime.MinValue;
            gameManager = new ProgramPointer(
                new FindPointerSignature(PointerVersion.Normal64, AutoDeref.Single, "41FFD3E96300000048B8????????????????488B10488BCE488D6424009049BB", 10),
                new FindPointerSignature(PointerVersion.Normal64, AutoDeref.Single32, "488BCE49BB????????????????41FFD3E9??000000488B1425", 25),
                new FindPointerSignature(PointerVersion.Normal, AutoDeref.Single, "83C41083EC0C57E8????????83C410EB3D8B05", 19),
                new FindPointerSignature(PointerVersion.API, AutoDeref.Single, "83C41083EC0C57393FE8????????83C410EB3F8B05", 21)) { UpdatedPointer = UpdatedPointer };
        }

        public string VersionNumber() {
            return lastVersion.ToString();
        }
        private void UpdatedPointer(ProgramPointer pointer) {
            //GameManager
            playerData = 0x30;
            //sceneData = 0x34;
            sceneName = 0xc;
            nextSceneName = 0x10;
            entryGateName = 0x14;
            uiManager = 0x84;
            inputHandler = 0x68;
            cameraCtrl = 0x74;
            gameState = 0x98;
            heroController = 0x78;
            debugInfo = 0x2c;
            tilemapDirty = 0xcf;
            achievementHandler = 0x78;

            //AchievementHandler
            //awardAchievementEvent = 0x20;

            //CameraController
            camTarget = 0x28;
            camMode = 0x40;
            camTeleport = 0x4b;
            camDest = 0x24;
            camTMode = 0x20;

            //HeroController
            cState = 0x108;
            heroAccepting = 0x457;
            actorState = 0x374;
            transistionState = 0x37c;
            hazardRespawning = 0x26;
            onGround = 0x9;
            spellquake = 0x37;

            int versionString = 0x1c;
            string version;
            int len = 12;

            //1568
            if (MemoryReader.is64Bit) {
                sceneName = 0x18;
                nextSceneName = 0x20;
                entryGateName = 0x28;
                inputHandler = 0x40;
                achievementHandler = 0x50;
                cameraCtrl = 0x80;
                heroController = 0x88;
                uiManager = 0xa0;
                playerData = 0xc8;
                //sceneData = 0xd0;
                gameState = 0x174;
                tilemapDirty = 0x1ab;

                //InputHandler
                debugInfo = 0x60;

                //CameraController
                camTarget = 0x48;
                camMode = 0x6c;
                camTeleport = 0x77;

                //CameraTarget
                camTMode = 0x3c;
                camDest = 0x40;

                //HeroController
                cState = 0x210;
                actorState = 0x600;
                transistionState = 0x608;
                heroAccepting = 0x6e7;

                //HeroControllerStates
                hazardRespawning = 0x2e;
                onGround = 0x11;
                spellquake = 0x3f;

                versionString = 0x38;

                do {
                    version = gameManager.Read(Program, 0x0, inputHandler, debugInfo, versionString);
                    if (string.IsNullOrEmpty(version)) {
                        Thread.Sleep(50);
                    }
                } while (string.IsNullOrEmpty(version) && len-- > 0);

                lastVersion = new Version(version);

                if (lastVersion.Build == 68) {
                    //UIManager
                    uiState = 0x29c;
                    menuState = 0x2a0;
                } else {
                    //UIManager
                    uiState = 0x2a4;
                    menuState = 0x2a8;

                    // Modding API 1.5.75.11827-63
                    // MAPI keeps the same ui property, but changes it to get/set from a different backing field.
                    // The original backing field stays at the same address, but has no setters any more.
                    if (gameManager.Read<IntPtr>(Program, 0x0, uiManager) == IntPtr.Zero) {
                        gameState = 0x184;
                        tilemapDirty = 0x1bb;
                        uiState = 0x2ac;
                        menuState = 0x2b0;
                        uiManager = 0x178;
                    }
                }
            } else {
                len = gameManager.Read<int>(Program, 0x0, inputHandler, debugInfo, versionString, 0x8);

                if (len != 7) {
                    // before 1221 but after 1118
                    inputHandler = 0x6c;
                    uiManager = 0x88;
                    cameraCtrl = 0x78;
                    gameState = 0x9c;
                    heroController = 0x7c;
                    camTarget = 0x24;
                    camMode = 0x3c;
                    camTeleport = 0x47;
                    tilemapDirty = 0xd3;

                    len = gameManager.Read<int>(Program, 0x0, inputHandler, debugInfo, versionString, 0x8);
                    if (len != 7) {
                        // 1432/1315
                        playerData = 0x60;
                        uiManager = 0x4c;
                        inputHandler = 0x20;
                        cameraCtrl = 0x3c;
                        gameState = 0xb4;
                        heroController = 0x40;
                        tilemapDirty = 0xef;
                        debugInfo = 0x30;

                        camMode = 0x38;
                        camTeleport = 0x43;

                        heroAccepting = 0x497;
                        actorState = 0x3b0;
                        transistionState = 0x3b8;

                        uiState = 0x148;
                        menuState = 0x14c;

                        do {
                            version = gameManager.Read(Program, 0x0, inputHandler, debugInfo, versionString);
                            if (string.IsNullOrEmpty(version)) {
                                Thread.Sleep(50);
                            }
                        } while (string.IsNullOrEmpty(version) && len-- > 0);

                        lastVersion = new Version(version);

                        if (lastVersion.Minor > 3) {
                            // 1432
                            gameState = 0xb8;

                            heroAccepting = 0x4b3;
                            actorState = 0x3cc;
                            transistionState = 0x3d4;

                            uiState = 0x154;
                            menuState = 0x158;
                        }
                    } else {
                        do {
                            version = gameManager.Read(Program, 0x0, inputHandler, debugInfo, versionString);
                            if (string.IsNullOrEmpty(version)) {
                                Thread.Sleep(50);
                            }
                        } while (string.IsNullOrEmpty(version) && len-- > 0);

                        lastVersion = new Version(version);

                        if (lastVersion.Minor == 0 && (lastVersion.Build < 3 || lastVersion.Revision < 4)) {
                            uiState = 0x128;
                            menuState = 0x12c;
                            tilemapDirty = 0xcf;
                            if (gameManager.Read<IntPtr>(Program, 0x0, uiManager) == IntPtr.Zero) {
                                uiManager = 0x9c;
                                gameState = 0xa4;
                                tilemapDirty = 0xd7;
                                uiState = 0x12c;
                                menuState = 0x130;
                            }
                        } else if (lastVersion.Minor == 0) {
                            // 10??
                            uiState = 0x12c;
                            menuState = 0x130;
                            tilemapDirty = 0xcf;
                        } else if (lastVersion.Minor == 1) {
                            // 1118?
                            uiState = 0x130;
                            menuState = 0x134;
                            heroAccepting = 0x45b;
                            actorState = 0x378;
                            transistionState = 0x380;
                        } else {
                            // 1221
                            uiState = 0x130;
                            menuState = 0x134;
                            uiManager = 0x8c;
                            cameraCtrl = 0x7c;
                            gameState = 0xa0;
                            heroController = 0x80;
                            cState = 0x10C;
                            heroAccepting = 0x46b;
                            actorState = 0x388;
                            transistionState = 0x390;
                        }
                    }
                } else {
                    len = 40;
                    do {
                        version = gameManager.Read(Program, 0x0, inputHandler, debugInfo, versionString);
                        if (string.IsNullOrEmpty(version)) {
                            Thread.Sleep(50);
                        }
                    } while (string.IsNullOrEmpty(version) && len-- > 0);

                    lastVersion = new Version(version);

                    // 1006 yes/1118???
                    menuState = 0x128;
                    uiState = 0x124;

                    if (lastVersion.Major == 1 &&
                        lastVersion.Minor == 0 &&
                        lastVersion.Build == 0 &&
                        lastVersion.Revision == 6) {
                        cState = 0x104;
                        transistionState = 0x36c;
                        tilemapDirty = 0xcb;
                    }
                }
            }

            HollowKnight.PlayerData.InitializeData(lastVersion);
        }
        public byte[] GetPlayerData(int length) {
            //GameManger._instance.playerData
            return gameManager.ReadBytes(Program, length, 0x0, playerData, 0x0);
        }
        public void SetCameraZoom(float zoom) {
            //GameManger._instance.gameCams.tk2dCam.zoomFactor
            if (lastVersion?.Minor >= 3) {
                if (lastVersion?.Minor >= 5) {
                    gameManager.Write<float>(Program, zoom, 0x0, 0x48, 0x90, 0x68);
                } else {
                    gameManager.Write<float>(Program, zoom, 0x0, 0x24, 0x48, 0x48);
                }
            } else {
                gameManager.Write<float>(Program, zoom, 0x0, 0x20, 0x40, 0x48);
            }
        }
        public bool CameraTeleporting() {
            //GameManger._instance.cameraCtrl.teleporting
            return gameManager.Read<bool>(Program, 0x0, cameraCtrl, camTeleport);
        }
        public PointF GetCameraTarget() {
            //GameManger._instance.cameraCtrl.camTarget.destination
            float x = gameManager.Read<float>(Program, 0x0, cameraCtrl, camTarget, camDest);
            float y = gameManager.Read<float>(Program, 0x0, cameraCtrl, camTarget, camDest + 0x4);
            return new PointF(x, y);
        }
        public TargetMode GetCameraTargetMode() {
            //GameManger._instance.cameraCtrl.camTarget.mode
            return (TargetMode)gameManager.Read<int>(Program, 0x0, cameraCtrl, camTarget, camTMode);
        }
        public void SetCameraTargetMode(TargetMode mode) {
            //GameManger._instance.cameraCtrl.camTarget.mode
            gameManager.Write(Program, (int)mode, 0x0, cameraCtrl, camTarget, camTMode);
        }
        public CameraMode CameraMode() {
            //GameManager._instance.cameraCtrl.mode
            return (CameraMode)gameManager.Read<int>(Program, 0x0, cameraCtrl, camMode);
        }
        public void EnableDebug(bool enable) {
            //inputHandler.onScreenDebugInfo.showFPS
            gameManager.Write(Program, enable, 0x0, inputHandler, debugInfo, lastVersion?.Minor >= 5 ? 0x9c : 0x7c);
            //inputHandler.onScreenDebugInfo.showInfo
            gameManager.Write(Program, enable, 0x0, inputHandler, debugInfo, lastVersion?.Minor >= 5 ? 0x9d : 0x7d);
            //inputHandler.onScreenDebugInfo.showInput
            gameManager.Write(Program, enable, 0x0, inputHandler, debugInfo, lastVersion?.Minor >= 5 ? 0x9e : 0x7e);
            //inputHandler.onScreenDebugInfo.showLoadingTime
            gameManager.Write(Program, enable, 0x0, inputHandler, debugInfo, lastVersion?.Minor >= 5 ? 0x9f : 0x7f);
            //inputHandler.onScreenDebugInfo.showTFR
            gameManager.Write(Program, enable, 0x0, inputHandler, debugInfo, lastVersion?.Minor >= 5 ? 0xa0 : 0x80);
        }
        public void SetPlayerData(Offset offset, int value) {
            //GameManger._instance.playerData.(offset)
            gameManager.Write(Program, value, 0x0, playerData, HollowKnight.PlayerData.GetOffset(offset));
        }
        public void SetPlayerData(Offset offset, bool value) {
            //GameManger._instance.playerData.(offset)
            gameManager.Write(Program, value, 0x0, playerData, HollowKnight.PlayerData.GetOffset(offset));
        }
        public T PlayerData<T>(Offset offset) where T : unmanaged {
            //GameManger._instance.playerData.(offset)
            switch (offset) {
                case Offset.bossDoorStateTier1:
                case Offset.bossDoorStateTier2:
                case Offset.bossDoorStateTier3:
                case Offset.bossDoorStateTier4:
                case Offset.bossDoorStateTier5:
                    return gameManager.Read<T>(Program, 0x0, playerData, HollowKnight.PlayerData.GetOffset(offset) + 0x2);
                default:
                    return gameManager.Read<T>(Program, 0x0, playerData, HollowKnight.PlayerData.GetOffset(offset));
            }
        }
        public string PlayerDataString<String>(Offset offset) {
            //GameManger._instance.playerData.(offset)
            switch (offset) {
                case Offset.bossDoorStateTier1:
                case Offset.bossDoorStateTier2:
                case Offset.bossDoorStateTier3:
                case Offset.bossDoorStateTier4:
                case Offset.bossDoorStateTier5:
                    return gameManager.Read(Program, 0x0, playerData, HollowKnight.PlayerData.GetOffset(offset) + 0x2);
                default:
                    return gameManager.Read(Program, 0x0, playerData, HollowKnight.PlayerData.GetOffset(offset));
            }
        }

        public List<string> PlayerDataStringList(Offset offset) {
            IntPtr listPtr = gameManager.Read<IntPtr>(Program, 0x0, playerData, HollowKnight.PlayerData.GetOffset(offset));
            IntPtr arrayPtr = Program.Read<IntPtr>(listPtr, MemoryReader.is64Bit ? 0x10 : 0x8);
            int arraySize = Program.Read<int>(arrayPtr, MemoryReader.is64Bit ? 0x18 : 0xc);

            List<string> list = new List<string>();
            for (int i = 0; i < arraySize; i++) {
                int itemOffset = MemoryReader.is64Bit ? 0x18 + 8 * (i + 1) : 0xc + 4 * (i + 1);
                IntPtr itemPtr = Program.Read<IntPtr>(arrayPtr + itemOffset);
                if (itemPtr == IntPtr.Zero) {
                    continue;
                }

                list.Add(Program.ReadString(itemPtr));
            }

            return list;
        }
        public GameState GameState() {
            //GameManager._instance.gameState
            return (GameState)gameManager.Read<int>(Program, 0x0, gameState);
        }
        public MainMenuState MenuState() {
            //GameManager._instance.uiManager.menuState
            return (MainMenuState)gameManager.Read<int>(Program, 0x0, uiManager, menuState);
        }
        public UIState UIState() {
            //GameManager._instance.uiManager.uiState
            int ui = gameManager.Read<int>(Program, 0x0, uiManager, uiState);
            if (uiState != 0x124 && ui >= 2) {
                ui += 2;
            }
            return (UIState)ui;
        }
        public bool AcceptingInput() {
            //GameManager._instance.InputHandler.acceptingInput
            if (lastVersion?.Minor >= 3) {
                if (lastVersion?.Minor >= 5) {
                    return gameManager.Read<bool>(Program, 0x0, inputHandler, 0xa4);
                }
                return gameManager.Read<bool>(Program, 0x0, inputHandler, 0x5c);
            }
            return gameManager.Read<bool>(Program, 0x0, inputHandler, 0x58);
        }
        public bool AcceptingInputHero() {
            //GameManager._instance.heroCtrl.acceptingInput
            return gameManager.Read<bool>(Program, 0x0, heroController, heroAccepting);
        }
        public ActorStates HeroActorState() {
            //GameManager._instance.heroCtrl.actor_state
            return (ActorStates)gameManager.Read<int>(Program, 0x0, heroController, actorState);
        }
        public HeroTransitionState HeroTransitionState() {
            //GameManager._instance.heroCtrl.transitionState
            return (HeroTransitionState)gameManager.Read<int>(Program, 0x0, heroController, transistionState);
        }

        public string SceneName() {
            //GameManager._instance.sceneName
            return gameManager.Read(Program, 0x0, sceneName);
        }
        public string NextSceneName() {
            //GameManager._instance.nextSceneName
            return gameManager.Read(Program, 0x0, nextSceneName);
        }

        public string EntryGateName() {
            //GameManager._instance.entryGateName
            return gameManager.Read(Program, 0x0, entryGateName);
        }

        public bool UsesSceneTransitionRoutine() {
            /*
             * 1.3.1.5 and above swap from using LoadSceneAdditive to a SceneTransitionRoutine triggered
             * by BeginSceneTransitionRoutine, which doesn't set tilemapDirty back to false when you enter dnail
             * However, the early control glitch can only be performed on early patches so we can avoid this check entirely
             * Also used as Hive Knight exists, in OnObtainGhostVespa
             */
            return lastVersion?.Minor >= 3;
        }
        public bool TileMapDirty() {
            //GameManager._instance.tileMapDirty
            return gameManager.Read<bool>(Program, 0x0, tilemapDirty);
        }
        public bool HazardRespawning() {
            //GameManager._instance.hero_ctrl.cState.hazardRespawning
            return gameManager.Read<bool>(Program, 0x0, heroController, cState, hazardRespawning);
        }
        public bool OnGround() {
            //GameManager._instance.hero_ctrl.cState.onGround
            return gameManager.Read<bool>(Program, 0x0, heroController, cState, onGround);
        }
        public bool Spellquake() {
            //GameManager._instance.hero_ctrl.cState.spellquake
            return gameManager.Read<bool>(Program, 0x0, heroController, cState, spellquake);
        }
        public string AchievementKey() {
            IntPtr basePointer = gameManager.Read<IntPtr>(Program, 0x0, achievementHandler);
            gameManager.Read<int>(Program, 0x0, achievementHandler);

            return string.Empty;
        }
        public bool HookProcess() {
            IsHooked = Program != null && !Program.HasExited;
            if (!IsHooked && DateTime.Now > lastHooked.AddSeconds(1)) {
                lastHooked = DateTime.Now;

                Process[] processes = Process.GetProcessesByName("Hollow_Knight");
                Program = processes != null && processes.Length > 0 ? processes[0] : null;

                if (Program == null) {
                    processes = Process.GetProcessesByName("Hollow Knight");
                    Program = processes != null && processes.Length > 0 ? processes[0] : null;
                }

                if (Program != null && !Program.HasExited) {
                    MemoryReader.Update64Bit(Program);
                    IsHooked = true;
                }
            }

            if (!IsHooked) {
                lastVersion = null;
            }

            return IsHooked;
        }
        public void Dispose() {
            if (Program != null) {
                Program.Dispose();
            }
        }
    }
    public class PlayerData {
        public static Dictionary<string, PlayerKey> Data = new Dictionary<string, PlayerKey>(StringComparer.OrdinalIgnoreCase);
        public static int DataLength;

        public PlayerData() { }

        public static void InitializeData(Version ver) {
            Assembly asm = Assembly.GetExecutingAssembly();

            Stream file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1568.txt");
            if (ver.Minor == 0 && (ver.Build < 3 || ver.Revision < 2)) {
                file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.Original.txt");
            } else if (ver.Minor == 0) {
                file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1032.txt");
            } else if (ver.Minor == 1) {
                file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1114.txt");
            } else if (ver.Minor == 2 && ver.Build == 1 && ver.Revision < 4) {
                file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1211.txt");
            } else if (ver.Minor == 2 && ((ver.Build == 1 && ver.Revision >= 4) || ver.Build > 1)) {
                file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1214.txt");
            } else if (ver.Minor == 3) {
                file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1315.txt");
            } else if (ver.Minor == 4) {
                file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1424.txt");
            }

            if (file != null) {
                Data.Clear();

                string line = null;
                DataLength = 0;
                using (StreamReader sr = new StreamReader(file)) {
                    while ((line = sr.ReadLine()) != null) {
                        string[] record = line.Split(':');
                        int offset = int.Parse(record[0], NumberStyles.HexNumber);
                        Data.Add(record[1], new PlayerKey(offset, record[1], record[2]));
                        if (offset > DataLength) { DataLength = offset; }
                    }
                }
                DataLength++;
            }
        }

        public static int GetOffset(Offset offset) {
            if (Data.TryGetValue(offset.ToString(), out PlayerKey key)) {
                return key.Index;
            }
            return (int)offset;
        }
        public void UpdateData(HollowKnightMemory mem, Action<string> logWriter) {
            Process program = mem.Program;
            byte[] playerData = mem.GetPlayerData(DataLength);
            foreach (KeyValuePair<string, PlayerKey> pair in Data) {
                PlayerKey key = pair.Value;

                switch (key.Name) {
                    case "scenesVisited":
                    case "scenesMapped":
                    case "scenesEncounteredBench":
                    case "scenesGrubRescued":
                    case "scenesEncounteredCocoon":
                    case "scenesEncounteredDreamPlant":
                    case "scenesEncounteredDreamPlantC":
                    case "playerStory":
                    case "MPCharge":
                    case "MPReserve":
                    case "equippedCharms": continue;
                }

                object oldValue = key.Value;

                switch (key.Type) {
                    case "Vector3": key.Value = new PointF(BitConverter.ToSingle(playerData, key.Index), BitConverter.ToSingle(playerData, key.Index + 4)); break;
                    case "Single": key.Value = BitConverter.ToSingle(playerData, key.Index); break;
                    case "Int16": key.Value = BitConverter.ToInt16(playerData, key.Index); break;
                    case "Int64": key.Value = BitConverter.ToInt64(playerData, key.Index); break;
                    case "String": key.Value = program.ReadString((IntPtr)BitConverter.ToUInt32(playerData, key.Index)); break;
                    case "Completion": key.Value = program.Read<bool>((IntPtr)BitConverter.ToUInt32(playerData, key.Index), 0xa); break;
                    case "Byte": key.Value = playerData[key.Index]; break;
                    case "Boolean": key.Value = playerData[key.Index] == 1; break;
                    default: key.Value = BitConverter.ToInt32(playerData, key.Index); break;
                }

                bool changed = oldValue == null;
                if (!changed) {
                    switch (key.Type) {
                        case "Vector3": changed = (PointF)oldValue != (PointF)key.Value; break;
                        case "Single": changed = (float)oldValue != (float)key.Value; break;
                        case "Int16": changed = (short)oldValue != (short)key.Value; break;
                        case "Int64": changed = (long)oldValue != (long)key.Value; break;
                        case "String": changed = (string)oldValue != (string)key.Value; break;
                        case "Completion": changed = (bool)oldValue != (bool)key.Value; break;
                        case "Byte": changed = (byte)oldValue != (byte)key.Value; break;
                        case "Boolean": changed = (bool)oldValue != (bool)key.Value; break;
                        default: changed = (int)oldValue != (int)key.Value; break;
                    }
                }
                if (changed && oldValue != null && logWriter != null) {
                    logWriter(key.ToString(oldValue));
                }
            }
        }
    }

    public class SceneData {
        public static Dictionary<string, PlayerKey> Data = new Dictionary<string, PlayerKey>(StringComparer.OrdinalIgnoreCase);
        public static int DataLength;

        //georocks
        //persistentintdata
        //persistentbooldata

        public SceneData() { }

        public static void InitializeData(Version ver) { }


    }

    public class PlayerKey {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }

        public PlayerKey(int index, string name, string type) {
            Index = index;
            Name = name;
            Type = type;
        }

        public override string ToString() {
            return Index.ToString("X") + " " + Name + " " + Type + " = " + (Value ?? "").ToString();
        }
        public string ToString(object oldValue) {
            return Name + ": " + (oldValue ?? "").ToString() + " -> " + (Value ?? "").ToString();
        }
    }
}