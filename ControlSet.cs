namespace USBLauncher
{
    public class ControlSet
        {
            public int VendorId { get; set; }
            public int DeviceId { get; set; }
            public byte[] Left { get; set; }
            public byte[] Right { get; set; }
            public byte[] Up { get; set; }
            public byte[] Down { get; set; }
            public byte[] Fire { get; set; }
            public byte[] Stop { get; set; }

            public int FireDelay { get; set; }
            public bool CeaseFireIsMove { get; set; }
            public byte[] CeaseFire { get; set; }
            public bool MoveMustIssueStop { get; set; }
            public int TraverseRangeMax { get; set; }
            public int MinimumElevation { get; set; }
            public int MaximumElevation { get; set; }
            public int TraverseMillisecondsPerDegree { get; set; }
            public int ElevationMillisecondsPerDegree { get; set; }
            public bool ElevationRaiseIsSlower { get; set; }
            public int SlopThreshhold { get; set; }

            public ControlSet()
            {
                SetDefaults();
            }
            public ControlSet(bool WithDefaults)
            {
                if (WithDefaults)
                {
                    SetDefaults();
                }
            }

            /// <summary>
            /// These defaults are for the Dreamcheeky USB launcher that isn't the "thunder" but a lot like it.
            /// </summary>
            private void SetDefaults()
            {
                this.VendorId = 0x0a81;
                this.DeviceId = 0x0701;

                this.Left = new byte[] { 2, 4 };
                this.Right = new byte[] { 2, 8 };
                this.Up = new byte[] { 2, 2 };
                this.Down = new byte[] { 2, 1 };
                this.Fire = new byte[] { 2, 16 };
                this.Stop = new byte[] { 2, 32 };
                this.MoveMustIssueStop = true;
                this.TraverseRangeMax = 345;
                this.MinimumElevation = -4;
                this.MaximumElevation = 23;
                this.TraverseMillisecondsPerDegree = 59;
                this.ElevationMillisecondsPerDegree = 100;
                this.ElevationRaiseIsSlower = true;
                this.CeaseFireIsMove = true;
                this.CeaseFire = this.Right;
                this.SlopThreshhold = 5000;
            }

            public int MaxToHomeTraverseMilleseconds
            {
                get
                {
                    if (TraverseMillisecondsPerDegree != 0)
                        return (TraverseRangeMax * TraverseMillisecondsPerDegree);
                    else
                        return 0;
                }
            }

            public int MaxToHomeElevationMilleseconds
            {
                get
                {
                    if (ElevationMillisecondsPerDegree != 0)
                        return (MaximumElevation - MinimumElevation) * ElevationMillisecondsPerDegree;
                    else
                        return 0;
                }
            }

        }
    }

