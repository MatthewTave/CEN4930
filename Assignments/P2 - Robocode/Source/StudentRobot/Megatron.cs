using Robocode;
using Robocode.Util;
using System;

namespace CAP4053.Student
{
    public class Megatron : TeamRobot
    {
        // Configuration Constants
        public const int TARGET_DECAY_TIME = 5;
        public const int WALL_AVOIDANCE_DELAY = 10;

        public const double BULLET_POWER_INCREMENT = 0.1;

        public const double MIN_ENGAGE_SCALING_DISTANCE = 200.0;
        public const double MAX_ENGAGE_SCALING_DISTANCE = 1200.0;

        public const double MIN_ANGLE_OF_ATTACK = 45.0;
        public const double MAX_ANGLE_OF_ATTACK = 90.0;

        public const double MIN_WALL_SCALING_DISTANCE = 100.0;
        public const double MAX_WALL_SCALING_DISTANCE = 200.0;

        public const double TARGET_ENGANGEMENT_DISTANCE = 400.0;

        public double MovementDirection;
        public double LastWallAvoidance;

        private class botInfo
        {
            public bool aquired;
            public double bearing;
            public double absoluteBearing;
            public double heading;
            public double distance;
            public double velocity;
            public double energy;
            public long infoTime;
            public bool energyDropFlag;


            public botInfo()
            {
                aquired = false;
                bearing = 0;
                heading = 0;
                distance = 0;
                velocity = 0;
                energy = 0;
                energyDropFlag = false;
                infoTime = 0;
            }
        }

        private botInfo targetInfo;

        public Megatron() {
            targetInfo = new botInfo();
            MovementDirection = 1;
            LastWallAvoidance = 0;
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
            if (IsTeammate(evnt.Name))
            {
                return;
            }

            targetInfo.aquired = true;
            targetInfo.bearing = evnt.Bearing;
            targetInfo.heading = evnt.Heading;
            targetInfo.distance = evnt.Distance;
            targetInfo.velocity = evnt.Velocity;
            targetInfo.infoTime = evnt.Time;
            targetInfo.energyDropFlag = evnt.Energy < targetInfo.energy;
            targetInfo.energy = evnt.Energy;

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
            // If we don't have a target, point the gun at the center of the screen
            // This makes it more likely that we will already be pointing roughly at an enemy
            if (!targetInfo.aquired)
            {
                SetGunTurnTo(CenterBearing());
                return;
            }
            
            // If we have a target, see if there is a valid firing solution with the current gun angle
            double bulletPower = firingSolutionBulletPower();

            // If there is a valid firing solution, shoot a bullet with the valid bullet power
            if (bulletPower != 0)
            {
                SetFire(bulletPower);
            }

            // Turn the gun to try to get a firing solution with the max bullet power
            SetGunTurnTo(targetGunAngle(maxAcceptableBulletPower(targetInfo.distance)));
        }

        // Utility function to find the angle the gun needs to turn to to shoot a moving target
        private double targetGunAngle(double bulletPower)
        {
            return asin(targetInfo.velocity * sin(normalizeAngle(targetInfo.heading - targetInfo.absoluteBearing)) / Rules.GetBulletSpeed(bulletPower)) + targetInfo.absoluteBearing;
        }

        // Finds the acceptable error for firing the gun based on the distance to the target
        private double acceptableGunneryError() 
        {
            return atan(this.Width * 0.5 / targetInfo.distance);
        }

        // Finds the highest bullet power that will hit the target with the current gun angle, if at all possible
        // If it is not possible, it returns 0;
        private double firingSolutionBulletPower()
        {
            for(double bp = maxAcceptableBulletPower(targetInfo.distance); bp >= 0.1; bp -= BULLET_POWER_INCREMENT)
            {
                if(normalizeAngle(this.GunHeading - targetGunAngle(bp)) <= acceptableGunneryError())
                {
                    return bp;
                }
            }

            return 0;
        }

        // Scales bullet power to be higher for closer enemies, and lower for further enemies
        private double maxAcceptableBulletPower(double distance)
        {
            if(targetInfo.distance > MAX_ENGAGE_SCALING_DISTANCE)
            {
                return Rules.MIN_BULLET_POWER;
            } else if(targetInfo.distance < MIN_ENGAGE_SCALING_DISTANCE)
            {
                return Rules.MAX_BULLET_POWER;
            } else
            {
                double powerMultiplier = (MAX_ENGAGE_SCALING_DISTANCE - targetInfo.distance) / (MAX_ENGAGE_SCALING_DISTANCE - MIN_ENGAGE_SCALING_DISTANCE);
                return (powerMultiplier * (Rules.MAX_BULLET_POWER - Rules.MIN_BULLET_POWER)) + Rules.MIN_BULLET_POWER;
            }
        }

        //---------- Movement Management Functions ----------//
        private void ManageMovement()
        {
            // Calculate Desired Angle of Attack based on wall distance
            double angleOfAttack = AngleOfAttack();

            // If we don't have a target, just circle the center of the board
            if (!targetInfo.aquired)
            {
                SetBodyTurnTo(normalizeAngle(CenterBearing() + angleOfAttack));
                SetAhead(Double.MaxValue * MovementDirection);
                return;
            }

            // Check if we need to switch direction due to enemy fire or we are too close to a wall
            if(targetInfo.energyDropFlag)
            {
                MovementDirection *= -1;
                targetInfo.energyDropFlag = false;
            } else if (IsGoingToHitWall())
            {
                Console.WriteLine("Going to Hit Wall, Switching Direction");
                this.LastWallAvoidance = this.Time;
                MovementDirection *= -1;
            }

            // Otherwise, try to circle around them and dodge their bullets and walls
            SetBodyTurnTo(normalizeAngle(targetInfo.absoluteBearing + angleOfAttack));
            SetAhead(Double.MaxValue * MovementDirection);
        }

        // When we are close to the wall, we want to move more towards the target
        // When we are far from the wall, we want to move in wider sweeps to evade better
        private double AngleOfAttack()
        {
            double closestWallDistance = ClosestWallDistance();
            if (closestWallDistance > MAX_WALL_SCALING_DISTANCE)
            {
                return MAX_ANGLE_OF_ATTACK;
            }
            else if (closestWallDistance < MIN_WALL_SCALING_DISTANCE)
            {
                return MIN_ANGLE_OF_ATTACK;
            }
            else
            {
                double aoaMultiplier = (MAX_WALL_SCALING_DISTANCE - closestWallDistance) / (MAX_WALL_SCALING_DISTANCE - MIN_WALL_SCALING_DISTANCE);
                return (aoaMultiplier * (MAX_ANGLE_OF_ATTACK - MIN_ANGLE_OF_ATTACK)) + MIN_ANGLE_OF_ATTACK;
            }
        }

        private bool IsGoingToHitWall()
        {
            Console.WriteLine("Heading: " + this.Heading + " Vel: " + this.Velocity);
            return (this.Y < MIN_WALL_SCALING_DISTANCE && ((this.Velocity >= 0 && (this.Heading > 90 && this.Heading < 270)) || (this.Velocity <= 0 && (this.Heading < 90 || this.Heading > 270)))) ||
                   (this.BattleFieldHeight - this.Y < MIN_WALL_SCALING_DISTANCE && ((this.Velocity <= 0 && (this.Heading > 90 && this.Heading < 270)) || (this.Velocity >= 0 && (this.Heading < 90 || this.Heading > 270)))) ||
                   (this.X < MIN_WALL_SCALING_DISTANCE && ((this.Velocity >= 0 && (this.Heading > 180)) || (this.Velocity <= 0 && (this.Heading < 180)))) ||
                   (this.BattleFieldWidth - this.X < MIN_WALL_SCALING_DISTANCE && ((this.Velocity >= 0 && (this.Heading < 180)) || (this.Velocity <= 0 && (this.Heading > 180))));
        }

        //---------- Utility Functions ----------//

        private double CenterBearing()
        {
            return atan2(this.BattleFieldWidth / 2 - this.X, this.BattleFieldHeight / 2 - this.Y);
        }

        private double ClosestWallDistance()
        {
            return Math.Min(Math.Min(this.X, this.BattleFieldWidth - this.X - 1), Math.Min(this.Y, this.BattleFieldHeight - this.Y - 1));
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

        public static double sin(double degrees)
        {
            return ToDegrees(Math.Sin(ToRadians(degrees)));
        }

        public static double cos(double degrees)
        {
            return ToDegrees(Math.Cos(ToRadians(degrees)));
        }

        public static double tan(double degrees)
        {
            return ToDegrees(Math.Tan(ToRadians(degrees)));
        }

        public static double asin(double degrees)
        {
            return ToDegrees(Math.Asin(ToRadians(degrees)));
        }

        public static double acos(double degrees)
        {
            return ToDegrees(Math.Acos(ToRadians(degrees)));
        }

        public static double atan(double ratio)
        {
            return ToDegrees(Math.Atan(ratio));
        }

        public static double atan2(double y, double x)
        {
            return ToDegrees(Math.Atan2(y, x));
        }
    }
}
