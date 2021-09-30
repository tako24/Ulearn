using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Incapsulation.Failures
{
    public class Device
    {
        public int ID;
        public string Name;

        public Device(int id, string name)
        {
            this.ID = id;
            this.Name = name;
        }
    }

    public class Failure
    {
        public FailureType FailureType { get; }
        public DateTime DataTime { get; }
        public bool IsFailureSerious { get; set; }
        public int DeviceId { get; }

        public Failure(FailureType failureType, DateTime dataTime, int deviceId)
        {
            FailureType = failureType;
            DataTime = dataTime;
            DeviceId = deviceId;
            IsFailureSerious = failureType == FailureType.UnexpectedShutdown
                || failureType == FailureType.HardwareFailures;
        }
    }

    public enum FailureType
    {
        UnexpectedShutdown,
        ShortNonResponding,
        HardwareFailures,
        ConnectionProblems
    }

    public class ReportMaker
    {
        private static List<string> FindDevicesFailedBeforeDate(DateTime date, Device[] devices, Failure[] failures)
        {
            var problematicDevices = new List<string>();
            for (int i = 0; i < devices.Length; i++)
            {
                if (failures[i].IsFailureSerious && failures[i].DataTime < date)
                    problematicDevices.Add(devices[i].Name);
            }
            return problematicDevices;
        }

        public static List<string> FindDevicesFailedBeforeDateObsolete(
                            int day,
                            int month,
                            int year,
                            int[] failureTypes,
                            int[] deviceId,
                            object[][] times,
                            List<Dictionary<string, object>> devices)
        {
            DateTime d = new DateTime(year, month, day);
            var failures = new Failure[devices.Count];
            Device[] dev = new Device[devices.Count];
            for (int i = 0; i < deviceId.Length; i++)
            {
                var dataTime = new DateTime((int)times[i][2], (int)times[i][1], (int)times[i][0]);
                dev[i] = new Device((int)devices[i]["DeviceId"]
    , (string)devices[i]["Name"]);
                failures[i] = new Failure((FailureType)failureTypes[i], dataTime, deviceId[i]);
            }
            return FindDevicesFailedBeforeDate(d, dev, failures);
        }
    }
}
