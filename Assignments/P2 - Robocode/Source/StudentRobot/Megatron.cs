using Robocode;
using Robocode.Util;
using System;

namespace CAP4053.Student
{
    public class Megatron : TeamRobot
    {
        // Configuration Constants
        public const int TARGET_DECAY_TIME = 5;

        public const double BULLET_POWER_INCREMENT = 0.1;

        public const double MIN_ENGAGE_SCALING_DISTANCE = 100.0;
        public const double MAX_ENGAGE_SCALING_DISTANCE = 1000.0;

        public const double MIN_BULLET_POWER = 0.1;
        public const double MAX_BULLET_POWER = 3.0;

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
            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;

            while (true)
            {
                // Radar
                ManageRadar();

                // Gunnery
                ManageGun();

                // Movement
                ManageMovement();

                Execute();
            }
        }

        public override void OnScannedRobot(ScannedRobotEvent evnt)
        {
            targetInfo.aquired = true;
            targetInfo.bearing = evnt.Bearing;
            targetInfo.heading = evnt.Heading;
            targetInfo.distance = evnt.Distance;
            targetInfo.velocity = evnt.Velocity;
            targetInfo.infoTime = evnt.Time;

            targetInfo.absoluteBearing = normalizeAngle(this.Heading + targetInfo.bearing);
        }

        //---------- Radar Management Functions ----------//
        public void ManageRadar()
        {
            // Make sure we scan no matter what
            Scan();

            // Check if target is still aquired
            ManageRadarLockDecay();

            if (!targetInfo.aquired)
            {
                // If target not aquired, just spin the radar around to try to get a lock
                SetTurnRadarRight(Double.MaxValue);
            }
            else
            {
                // Otherwise, turn radar to target
                SetRadarTurnTo(targetInfo.absoluteBearing);
            }
        }

        // Marks the target as not aquired if it hasn't been scanned in some time
        public void ManageRadarLockDecay()
        {
            if (this.Time - targetInfo.infoTime > TARGET_DECAY_TIME)
            {
                targetInfo.aquired = false;
            }
        }

        //---------- Gunnery Management Functions ----------//
        private void ManageGun()
        {
            if(targetInfo.aquired)
            {
                // If we have a target, see if there is a valid firing solution with the current gun angle
                double bulletPower = firingSolutionBulletPower();

                // If there is a valid firing solution, shoot a bullet with the valid bullet power
                if (bulletPower != 0)
                {
                    SetFire(bulletPower);
                }

                // Turn the gun to try to get a firing solution with the max bullet power
                SetGunTurnTo(targetGunAngle(maxAcceptableBulletPower(targetInfo.distance)));

            } else
            {
                // Otherwise, point the gun towards the center of the screen
                // This results in the highest chance of having the gun already pointed at the target when we find it
                SetGunTurnTo(CenterBearing());
            }
        }

        // Utility function to find the angle the gun needs to turn to to shoot a moving target
        private double targetGunAngle(double bulletPower)
        {
            return asin(targetInfo.velocity * sin(normalizeAngle(targetInfo.heading - targetInfo.absoluteBearing)) / Rules.GetBulletSpeed(bulletPower)) + targetInfo.absoluteBearing;
        }

        // Finds the acceptable error for firing the gun based on the distance to the target's expected location
        private double acceptableGunneryError(double bulletPower)  // TODO: FINISH THIS FUNCTION
        {
            return 1.0;
        }

        // Finds the highest bullet power that will hit the target with the current gun angle, if at all possible
        // If it is not possible, it returns 0;
        private double firingSolutionBulletPower()
        {
            for(double bp = maxAcceptableBulletPower(targetInfo.distance); bp >= 0.1; bp -= BULLET_POWER_INCREMENT)
            {
                if(normalizeAngle(this.GunHeading - targetGunAngle(bp)) <= acceptableGunneryError(bp))
                {
                    Console.WriteLine("Firing Solution Found, bp: " + bp + " angle: " + targetGunAngle(bp));
                    return bp;
                }
            }

            return 0;
        }

        // Scales bullet power to be higher for closer enemies, and lower for further enemies
        // TODO: Experiment with this scaling
        private double maxAcceptableBulletPower(double distance)
        {
            if(targetInfo.distance > MAX_ENGAGE_SCALING_DISTANCE)
            {
                return MIN_BULLET_POWER;
            } else if(targetInfo.distance < MIN_ENGAGE_SCALING_DISTANCE)
            {
                return MAX_BULLET_POWER;
            } else
            {
                double powerMultiplier = (MAX_ENGAGE_SCALING_DISTANCE - targetInfo.distance) / (MAX_ENGAGE_SCALING_DISTANCE - MIN_ENGAGE_SCALING_DISTANCE);
                return powerMultiplier * (MAX_BULLET_POWER - MIN_BULLET_POWER) + MIN_BULLET_POWER;
            }
        }

        //---------- Movement Management Functions ----------//
        private void ManageMovement()
        {
            SetBodyTurnTo(Double.MaxValue);
            SetAhead(Double.MaxValue);
            //if (targetInfo.aquired)
            //{
            //    SetBodyTurnTo(targetInfo.absoluteBearing);
            //    SetAhead(Double.MaxValue);
            //}
        }

        //---------- Utility Functions ----------//

        // Get the bearing to the center of the screen
        private double CenterBearing() // TODO: FINISH THIS FUNCTION
        {
            return 0;
        }

        // Utility functions to turn to a certain angle as fast as possible
        private void SetBodyTurnTo(double angleDegrees)
        {
            double diff = Utils.NormalRelativeAngleDegrees(angleDegrees - this.Heading);
            if (diff >= 0)
            {
                SetTurnRight(diff);
            }
            else
            {
                SetTurnLeft(-diff);
            }
        }

        private void SetGunTurnTo(double angleDegrees)
        {
            double diff = Utils.NormalRelativeAngleDegrees(angleDegrees - this.GunHeading);
            if (diff >= 0)
            {
                SetTurnGunRight(diff);
            }
            else
            {
                SetTurnGunLeft(-diff);
            }
        }

        private void SetRadarTurnTo(double angleDegrees)
        {
            double diff = Utils.NormalRelativeAngleDegrees(angleDegrees - this.RadarHeading);
            if (diff >= 0)
            {
                SetTurnRadarRight(diff);
            }
            else
            {
                SetTurnRadarLeft(-diff);
            }
        }

        //---------- Static Trig Utility Functions ----------//

        // Normalize angles
        private double normalizeAngle(double angle)
        {
            double ans = Math.IEEERemainder(angle, 360);
            if (angle < 0)
            {
                angle += 360;
            }

            return ans;
        }

        // Convert degrees to radians
        public static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        // Convert radians to degrees
        public static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        // Calculates sin in degrees
        public static double sin(double degrees)
        {
            return ToDegrees(Math.Sin(ToRadians(degrees)));
        }

        // Calculate asin in degrees
        public static double asin(double degrees)
        {
            return ToDegrees(Math.Asin(ToRadians(degrees)));
        }
    }
}
