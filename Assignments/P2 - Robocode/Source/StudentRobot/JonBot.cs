using Robocode;
using Robocode.Util;
using System.Drawing;
using System;
using System.Runtime.Remoting.Services;

namespace CAP4053.Student
{
    public class JonBot : TeamRobot
    {
        private bool tracking = false;
        public override void Run()
        {
            // Set colors
            this.BodyColor = System.Drawing.ColorTranslator.FromHtml("#602537");
            this.GunColor = System.Drawing.ColorTranslator.FromHtml("#0F0F0F");
            this.RadarColor = System.Drawing.ColorTranslator.FromHtml("#464F51");
            this.BulletColor = System.Drawing.ColorTranslator.FromHtml("#1B2C3C");

            IsAdjustGunForRobotTurn = true;
            IsAdjustRadarForGunTurn = true;
            IsAdjustRadarForRobotTurn = true;

            // Loop forever
            while (true)
            {
                TurnRadarRight(360);
            }
        }

        /// <summary>
        ///   onScannedRobot:  We have a target.  Go get it.
        /// </summary>
        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            if(IsTeammate(e.Name))
            {
                return;
            }

            // Calculate exact location of the robot
            double absoluteBearing = Heading + e.Bearing;
            double bearingFromHead = Utils.NormalRelativeAngleDegrees(e.Bearing);
            double bearingFromGun = Utils.NormalRelativeAngleDegrees(absoluteBearing - GunHeading);
            double bearingFromRadar = Utils.NormalRelativeAngleDegrees(absoluteBearing - RadarHeading);
            double time = e.Distance / (20 - 3 * 4);
            double offset = 45 * Math.Sin(Utils.NormalRelativeAngle(e.HeadingRadians - GunHeadingRadians));
            offset = offset * Math.Atan(e.Velocity * time / e.Distance) / (Math.PI / 2);

            //double BE = Math.PI - e.HeadingRadians - bearingFromGun * Math.PI / 180;
            //double offset = Math.Asin(e.Velocity * Math.Sin(BE) / (20 - 3 * 4)) * 180 / Math.PI;
            //Console.WriteLine($"{e.Velocity} * {time} / {e.Distance} whose angle is {Math.Atan(e.Velocity * time / e.Distance)}");
            Console.WriteLine($"{offset}");

            SetAhead(double.PositiveInfinity);

            //double angleOffset = Math.Asin(e.Velocity * Math.Sin(Math.PI - e.HeadingRadians - absoluteBearing * Math.PI / 180));

            // If it's close enough, fire!
            if (Math.Abs(bearingFromRadar) <= 3 && Math.Abs(e.Distance) < 200)
            {
                Console.WriteLine($"Heat is {GunHeat}");
                SetTurnRight(bearingFromHead + offset);
                SetTurnRadarRight(bearingFromRadar);
                SetTurnGunRight(bearingFromGun + offset);

                // We check gun heat here, because calling Fire()
                // uses a turn, which could cause us to lose track
                // of the other robot.
                if (GunHeat == 0)
                {
                    Fire(3);
                }
            }
            else
            {
                // otherwise just set the gun to turn.
                // Note:  This will have no effect until we call scan()
                SetTurnRight(bearingFromHead);
                SetTurnRadarRight(bearingFromRadar);
                SetTurnGunRight(bearingFromGun - offset);
            }
            // Generates another scan event if we see a robot.
            // We only need to call this if the gun (and therefore radar)
            // are not turning.  Otherwise, scan is called automatically.

            Execute();

            if (bearingFromGun == 0)
            {
                Scan();
            }
        }

        public override void OnWin(WinEvent e)
        {
            // Victory dance
            TurnRight(36000);
        }
    }
}