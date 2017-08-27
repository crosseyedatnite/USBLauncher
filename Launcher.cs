using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;

namespace USBLauncher
{
    public class Launcher
    {
        private HidDevice currentDevice;
        private int currentTraverse = 0;
        private int currentElevation = 0;
        private int locationSlop = 0;
        private bool locationKnown = false;
        private ControlSet controlSet;
        private List<LaunchTarget> knownTargets;

        #region Constructors
        public Launcher()
        {
            controlSet = new ControlSet(true);

            var deviceList = HidDevices.Enumerate(controlSet.VendorId, controlSet.DeviceId);
            currentDevice = deviceList.First();
            currentDevice.OpenDevice();
            ResetToBaseLocation();
            knownTargets = new List<LaunchTarget>();
        }
        #endregion

        public void AddTarget(LaunchTarget target)
        {
            var existingTarget = knownTargets.FirstOrDefault(t => t.Name == target.Name);

            if (existingTarget != null)
            {
                knownTargets.Remove(existingTarget);
            }

            knownTargets.Add(target);

        }
        public void AddTarget(IEnumerable<LaunchTarget> targetList)
        {
            knownTargets.AddRange(targetList);
        }
        
        public IEnumerable<LaunchTarget> GetTargetList()
        {
            return knownTargets;
        }

        public void ResetToBaseLocation()
        {
            var elevationBump = controlSet.ElevationRaiseIsSlower ? controlSet.Up : controlSet.Down;

            if (locationKnown)
            {
                MoveToPosition(0, controlSet.ElevationRaiseIsSlower ? controlSet.MaximumElevation : controlSet.MinimumElevation);

                // Small Bump Left to make sure
                DoAction(controlSet.Left, 1000, controlSet.Stop);

                // Small elevation bump to make sure
                DoAction(elevationBump, 500, controlSet.Stop);
            }
            else
            {
                var traverseTime = controlSet.MaxToHomeTraverseMilleseconds;
                var elevationTime = controlSet.MaxToHomeElevationMilleseconds + (controlSet.ElevationRaiseIsSlower? controlSet.MaxToHomeElevationMilleseconds : 0);
                DoAction(controlSet.Left, traverseTime, controlSet.Stop);
                DoAction(elevationBump, elevationTime, controlSet.Stop);
            }

            currentTraverse = 0;
            currentElevation = controlSet.ElevationRaiseIsSlower ? controlSet.MaximumElevation : controlSet.MinimumElevation;
            locationKnown = true;
            locationSlop = 0;
        }

        public void MoveToPosition(int TraverseLocation, int Elevation)
        {
            // Often, the time to go to a position from a non-default position introduces enough uncertainty that we need to revert to base location first.
            if (locationSlop > controlSet.SlopThreshhold)
            {
                ResetToBaseLocation();
            }

            var traverseDelay = Math.Abs(TraverseLocation - currentTraverse) * controlSet.TraverseMillisecondsPerDegree;

            byte[] action = TraverseLocation > currentTraverse ? controlSet.Right : controlSet.Left;

            DoAction(action, traverseDelay, controlSet.Stop);

            var elevationDelay = Math.Abs(Elevation - currentElevation) * controlSet.ElevationMillisecondsPerDegree;

            action = Elevation < currentElevation ? controlSet.Down : controlSet.Up;

            DoAction(action, elevationDelay, controlSet.Stop);

            currentTraverse = TraverseLocation;
            currentElevation = Elevation;
            locationSlop += traverseDelay;
        }

        public void MoveToTarget(string targetName)
        {
            var target = knownTargets.First(t => t.Name == targetName);
            MoveToPosition(target.Traverse, target.Elevation);
        }

        public void FireMissile()
        {
            DoAction(controlSet.Fire, controlSet.FireDelay, controlSet.CeaseFire);
            if (controlSet.CeaseFireIsMove)
            {
                Thread.Sleep(100);
                SimpleAction(controlSet.Stop);
            }
                
        }

        private void DoAction(byte[] action, int delay, byte[] delayAction)
        {
            SimpleAction(action);
            Thread.Sleep(delay);
            currentDevice.Write(delayAction);
        }

        private void SimpleAction(byte[] action)
        {
            currentDevice.Write(action);
        }
    }

}
