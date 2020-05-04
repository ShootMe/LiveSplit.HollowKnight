using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
namespace LiveSplit.HollowKnight {
    public class EnemyInfo {
        public uint Pointer { get; set; }
        public int HP { get; set; }
        public int HPIndex { get; set; }

        public int UpdateHP(Process program, int newHP = -1) {
            int hp = HP;
            if (Pointer != 0) {
                if (newHP > 0) {
                    HP = newHP;
                    program.Write<int>((IntPtr)Pointer, newHP, 0x10 + HPIndex * 4, 0x14);
                } else {
                    HP = program.Read<int>((IntPtr)Pointer, 0x10 + HPIndex * 4, 0x14);
                }
            }
            return hp;
        }
        public override int GetHashCode() {
            return (int)Pointer;
        }
        public override bool Equals(object obj) {
            return obj != null && (obj is EnemyInfo) && ((EnemyInfo)obj).Pointer == this.Pointer;
        }
    }
    public class EntityInfo {
        public string Name { get; set; }
        public uint Pointer { get; set; }
        public List<KeyValuePair<string, float>> FloatVars { get; set; } = new List<KeyValuePair<string, float>>();
        public List<KeyValuePair<string, int>> IntVars { get; set; } = new List<KeyValuePair<string, int>>();
        public List<KeyValuePair<string, bool>> BoolVars { get; set; } = new List<KeyValuePair<string, bool>>();
        public List<KeyValuePair<string, string>> StringVars { get; set; } = new List<KeyValuePair<string, string>>();
        public List<KeyValuePair<string, PointF>> VectorVars { get; set; } = new List<KeyValuePair<string, PointF>>();
        public List<KeyValuePair<string, int>> ObjVars { get; set; } = new List<KeyValuePair<string, int>>();

        public int Count { get { return FloatVars.Count + IntVars.Count + BoolVars.Count + StringVars.Count + VectorVars.Count + ObjVars.Count; } }
        public override int GetHashCode() {
            return (int)Pointer;
        }
        public bool Same(EntityInfo info) {
            return info.Pointer == this.Pointer;
        }
        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Name);
            for (int i = 0; i < FloatVars.Count; i++) {
                sb.Append("    ").Append(FloatVars[i].Key).Append(" = ").AppendLine(FloatVars[i].Value.ToString());
            }
            for (int i = 0; i < VectorVars.Count; i++) {
                sb.Append("    ").Append(VectorVars[i].Key).Append(" = ").AppendLine(VectorVars[i].Value.ToString());
            }
            for (int i = 0; i < IntVars.Count; i++) {
                sb.Append("    ").Append(IntVars[i].Key).Append(" = ").AppendLine(IntVars[i].Value.ToString());
            }
            for (int i = 0; i < BoolVars.Count; i++) {
                sb.Append("    ").Append(BoolVars[i].Key).Append(" = ").AppendLine(BoolVars[i].Value.ToString());
            }
            for (int i = 0; i < StringVars.Count; i++) {
                sb.Append("    ").Append(StringVars[i].Key).Append(" = ").AppendLine(StringVars[i].Value.ToString());
            }
            for (int i = 0; i < ObjVars.Count; i++) {
                sb.Append("    ").Append(ObjVars[i].Key).Append(" = [").Append(ObjVars[i].Value.ToString()).AppendLine("]");
            }
            return sb.ToString();
        }
        public override bool Equals(object obj) {
            EntityInfo info = obj as EntityInfo;
            if (info == null || info.Count != this.Count) { return false; }

            if (this.FloatVars.Count != info.FloatVars.Count) { return false; }
            for (int i = 0; i < FloatVars.Count; i++) {
                if (FloatVars[i].Value != info.FloatVars[i].Value) { return false; }
            }

            if (this.VectorVars.Count != info.VectorVars.Count) { return false; }
            for (int i = 0; i < VectorVars.Count; i++) {
                if (VectorVars[i].Value != info.VectorVars[i].Value) { return false; }
            }

            if (this.IntVars.Count != info.IntVars.Count) { return false; }
            for (int i = 0; i < IntVars.Count; i++) {
                if (IntVars[i].Value != info.IntVars[i].Value) { return false; }
            }

            if (this.BoolVars.Count != info.BoolVars.Count) { return false; }
            for (int i = 0; i < BoolVars.Count; i++) {
                if (BoolVars[i].Value != info.BoolVars[i].Value) { return false; }
            }

            if (this.StringVars.Count != info.StringVars.Count) { return false; }
            for (int i = 0; i < StringVars.Count; i++) {
                if (StringVars[i].Value != info.StringVars[i].Value) { return false; }
            }

            if (this.ObjVars.Count != info.ObjVars.Count) { return false; }
            for (int i = 0; i < ObjVars.Count; i++) {
                if (ObjVars[i].Value != info.ObjVars[i].Value) { return false; }
            }
            return true;
        }
    }
}