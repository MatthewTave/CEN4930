using Robocode;
using System;

namespace CAP4053.Student
{
    public class Megatron : TeamRobot
    {
        private class botInfo
        {
            public bool aquired;
            public double bearing;
            public double absoluteBearing;
            public double heading;
            public double distance;
            public double velocity;
            public long infoTime;
            public botInfo()
            {
                aquired = false;
                bearing = 0;
                heading = 0;
                distance = 0;
                velocity = 0;
                infoTime = 0;
            }
        }

        private botInfo targetInfo;

        public Megatron() {
            targetInfo = new botInfo();
        }

        public void BeatRobot()
        {
            BeatRobot();
        }
        public override void Run()
        {
            MaxVelocity = 8;
            IsAdjustRadarForRobotTurn = true;

            while (true)
            {
                // Radar
                if(this.Time - targetInfo.infoTime > 5) 
                {
                    targetInfo.aquired = false;
                }
                ManageRadarLock();

                // Gunnery

                // Movement



                SetTurnLeft(100);
                SetAhead(100);

                

                Execute();
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            Console.WriteLine("Target Aquired B) Bearing: " + evnt.Bearing);

            targetInfo.aquired = true;
            targetInfo.bearing = evnt.Bearing;
            targetInfo.heading = evnt.Heading;
            targetInfo.distance = evnt.Distance;
            targetInfo.velocity = evnt.Velocity;
            targetInfo.infoTime = evnt.Time;

            targetInfo.absoluteBearing = Math.IEEERemainder(this.Heading + targetInfo.bearing, 360);
            if (targetInfo.absoluteBearing < 0)
            {
                targetInfo.absoluteBearing += 360;
            }
        }

        public void ManageRadarLock()
        {
            if(!targetInfo.aquired)
            {
                SetTurnRadarRight(100);
            } else
            {
                if (Math.Abs(this.RadarHeading - targetInfo.absoluteBearing) >= 180)
                {
                    SetTurnRadarRight(this.RadarHeading - targetInfo.absoluteBearing);
                }
                else
                {
                    SetTurnRadarLeft(this.RadarHeading - targetInfo.absoluteBearing);
                }
            }
        }

        private void SetTurnTo(double angleDegrees)
        {
            if(Math.Abs(this.Heading -  angleDegrees) >= 180)
            {
                SetTurnRight(this.Heading - angleDegrees);
            } else
            {
                SetTurnLeft(this.Heading - angleDegrees);
            }
        }

    }
}
