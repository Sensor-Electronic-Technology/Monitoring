using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitoringData.Infrastructure.Utilities {
    public static class Converters {
        public static int ToInt32(ushort first, ushort second) {
            return BitConverter.ToInt32(BitConverter.GetBytes(second).Concat(BitConverter.GetBytes(first)).ToArray(), 0);
        }

        public static double ToDouble(ushort first, ushort second) {
            return (double)BitConverter.ToSingle(BitConverter.GetBytes(first).Concat(BitConverter.GetBytes(second)).ToArray(), 0);
        }

        public static ushort[] ToUshortArray(int value) {
            ushort[] s = new ushort[2];
            byte[] fBytes = BitConverter.GetBytes(value);
            s[0] = BitConverter.ToUInt16(fBytes, 2);
            s[1] = BitConverter.ToUInt16(fBytes, 0);
            return s;
        }

        public static float[] GetFloatArray(object fltArray) {
            if (fltArray.GetType() == typeof(float[])) {
                object[] t = ((Array)fltArray).Cast<object>().ToArray();

                return Array.ConvertAll(t, item => (float)item);
            }
            return null;
        }

        public static ushort[] GetUshortArray(object array) {
            if (array.GetType() == typeof(ushort[])) {
                object[] t = ((Array)array).Cast<object>().ToArray();

                return Array.ConvertAll(t, item => (ushort)item);
            }
            return null;
        }
    }
}
