using System;
using System.Diagnostics;
namespace LiveSplit.HollowKnight {
    public enum PointerVersion {
        Normal,
        Normal64,
        API
    }
    public enum AutoDeref {
        None,
        Single,
        Double,
        Single32,
        Double32
    }
    public class ProgramPointer {
        private int lastID;
        private DateTime lastTry;
        private IFindPointer currentFinder;
        public IntPtr Pointer { get; private set; }
        public IFindPointer[] Finders { get; private set; }
        public string AsmName { get; private set; }
        public Action<ProgramPointer> UpdatedPointer { get; set; }

        public ProgramPointer(params IFindPointer[] finders) : this(string.Empty, finders) { }
        public ProgramPointer(string asmName, params IFindPointer[] finders) {
            AsmName = asmName;
            Finders = finders;
            lastID = -1;
            lastTry = DateTime.MinValue;
        }

        public T Read<T>(Process program, params int[] offsets) where T : unmanaged {
            GetPointer(program);
            return program.Read<T>(Pointer, offsets);
        }
        public string Read(Process program, params int[] offsets) {
            GetPointer(program);
            return program.ReadString(program.Read<IntPtr>(Pointer, offsets));
        }
        public byte[] ReadBytes(Process program, int length, params int[] offsets) {
            GetPointer(program);
            return program.Read(Pointer, length, offsets);
        }
        public void Write<T>(Process program, T value, params int[] offsets) where T : unmanaged {
            GetPointer(program);
            program.Write<T>(Pointer, value, offsets);
        }
        public void Write(Process program, byte[] value, params int[] offsets) {
            GetPointer(program);
            program.Write(Pointer, value, offsets);
        }
        public IntPtr GetPointer(Process program) {
            if (program == null) {
                Pointer = IntPtr.Zero;
                lastID = -1;
                return Pointer;
            } else if (program.Id != lastID) {
                Pointer = IntPtr.Zero;
                lastID = program.Id;
            } else if (Pointer != IntPtr.Zero) {
                IntPtr pointer = Pointer;
                currentFinder.VerifyPointer(program, ref pointer);
                Pointer = pointer;
            }

            if (Pointer == IntPtr.Zero && DateTime.Now > lastTry) {
                lastTry = DateTime.Now.AddSeconds(1);

                for (int i = 0; i < Finders.Length; i++) {
                    IFindPointer finder = Finders[i];
                    try {
                        Pointer = finder.FindPointer(program, AsmName);
                        if (Pointer != IntPtr.Zero || finder.FoundBaseAddress()) {
                            currentFinder = finder;
                            UpdatedPointer?.Invoke(this);
                            break;
                        }
                    } catch { }
                }
            }
            return Pointer;
        }
        public static IntPtr DerefPointer(Process program, IntPtr pointer, AutoDeref autoDeref) {
            if (pointer != IntPtr.Zero) {
                switch (autoDeref) {
                    case AutoDeref.Single: pointer = program.Read<IntPtr>(pointer); break;
                    case AutoDeref.Single32: pointer = (IntPtr)program.Read<uint>(pointer); break;
                    case AutoDeref.Double:
                        pointer = program.Read<IntPtr>(pointer);
                        pointer = program.Read<IntPtr>(pointer);
                        break;
                    case AutoDeref.Double32:
                        pointer = (IntPtr)program.Read<uint>(pointer);
                        pointer = program.Read<IntPtr>(pointer);
                        break;
                }
            }
            return pointer;
        }
        public static Tuple<IntPtr, IntPtr> GetAddressRange(Process program, string asmName) {
            Module64 module = program.Module64(asmName);
            if (module != null) {
                return new Tuple<IntPtr, IntPtr>(module.BaseAddress, module.BaseAddress + module.MemorySize);
            }
            return new Tuple<IntPtr, IntPtr>(IntPtr.Zero, IntPtr.Zero);
        }
    }
    public interface IFindPointer {
        IntPtr FindPointer(Process program, string asmName);
        bool FoundBaseAddress();
        void VerifyPointer(Process program, ref IntPtr pointer);
        PointerVersion Version { get; }
    }
    public class FindPointerSignature : IFindPointer {
        public PointerVersion Version { get; private set; }
        private readonly AutoDeref AutoDeref;
        private readonly string Signature;
        private readonly MemorySearcher Searcher;
        private readonly int[] Relative;
        private IntPtr BasePtr;
        private DateTime LastVerified;

        public FindPointerSignature(PointerVersion version, AutoDeref autoDeref, string signature, params int[] relative) {
            Version = version;
            AutoDeref = autoDeref;
            Signature = signature;
            BasePtr = IntPtr.Zero;
            Searcher = new MemorySearcher();
            LastVerified = DateTime.MaxValue;
            Relative = relative;
        }

        public bool FoundBaseAddress() {
            return BasePtr != IntPtr.Zero;
        }
        public void VerifyPointer(Process program, ref IntPtr pointer) {
            DateTime now = DateTime.Now;
            if (now <= LastVerified) { return; }

            bool isValid = Searcher.VerifySignature(program, BasePtr, Signature);
            LastVerified = now.AddSeconds(1);
            if (isValid) {
                int offset = CalculateRelative(program);
                IntPtr verify = ProgramPointer.DerefPointer(program, BasePtr + offset, AutoDeref);
                if (verify != pointer) {
                    pointer = verify;
                }
                return;
            }

            BasePtr = IntPtr.Zero;
            pointer = IntPtr.Zero;
        }
        public IntPtr FindPointer(Process program, string asmName) {
            return ProgramPointer.DerefPointer(program, GetPointer(program, asmName), AutoDeref);
        }
        private IntPtr GetPointer(Process program, string asmName) {
            if (string.IsNullOrEmpty(asmName)) {
                Searcher.MemoryFilter = delegate (MemInfo info) {
                    return (info.State & 0x1000) != 0 && (info.Type & 0x20000) != 0 && (info.Protect & 0xe0) != 0 && (info.Protect & 0x101) == 0;
                };
            } else {
                Tuple<IntPtr, IntPtr> range = ProgramPointer.GetAddressRange(program, asmName);
                Searcher.MemoryFilter = delegate (MemInfo info) {
                    return (ulong)info.BaseAddress >= (ulong)range.Item1 && (ulong)info.BaseAddress <= (ulong)range.Item2 && (info.State & 0x1000) != 0 && (info.Protect & 0x20) != 0 && (info.Protect & 0x100) == 0;
                };
            }

            BasePtr = Searcher.FindSignature(program, Signature);
            if (BasePtr != IntPtr.Zero) {
                LastVerified = DateTime.Now.AddSeconds(5);
                int offset = CalculateRelative(program);
                return BasePtr + offset;
            }
            return BasePtr;
        }
        private int CalculateRelative(Process program) {
            int maxIndex = Relative.Length - 1;
            if (Relative == null || maxIndex < 0) { return 0; }

            int offset = 0;
            for (int i = 0; i < maxIndex; i++) {
                offset += Relative[i];
                offset += program.Read<int>(BasePtr + offset) + 4;
            }
            return offset + Relative[maxIndex];
        }
    }
    public class FindOffset : IFindPointer {
        public PointerVersion Version { get; private set; }
        private readonly AutoDeref AutoDeref;
        private readonly int[] Offsets;
        private IntPtr BasePtr;
        private DateTime LastVerified;

        public FindOffset(PointerVersion version, AutoDeref autoDeref, params int[] offsets) {
            Version = version;
            AutoDeref = autoDeref;
            Offsets = offsets;
            LastVerified = DateTime.MaxValue;
        }

        public bool FoundBaseAddress() {
            return BasePtr != IntPtr.Zero;
        }
        public void VerifyPointer(Process program, ref IntPtr pointer) {
            if (DateTime.Now > LastVerified) {
                pointer = IntPtr.Zero;
            }
        }
        public IntPtr FindPointer(Process program, string asmName) {
            if (string.IsNullOrEmpty(asmName)) {
                BasePtr = program.MainModule.BaseAddress;
            } else {
                Tuple<IntPtr, IntPtr> range = ProgramPointer.GetAddressRange(program, asmName);
                BasePtr = range.Item1;
            }

            if (Offsets.Length > 1) {
                LastVerified = DateTime.Now.AddSeconds(5);
                return ProgramPointer.DerefPointer(program, program.Read<IntPtr>(BasePtr, Offsets), AutoDeref);
            } else {
                LastVerified = DateTime.MaxValue;
                BasePtr += Offsets[0];
                return ProgramPointer.DerefPointer(program, BasePtr, AutoDeref);
            }
        }
    }
}