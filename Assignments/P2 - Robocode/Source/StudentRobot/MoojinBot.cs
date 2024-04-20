using System;
using System.Drawing;
using Robocode;
using Robocode.Util;

namespace CAP4053.Student
{
    public class MoojinBot : TeamRobot
    {
        public override void Run()
        {
            // Set colors
            this.BodyColor = System.Drawing.ColorTranslator.FromHtml("#602537");
            this.GunColor = System.Drawing.ColorTranslator.FromHtml("#0F0F0F");
            this.RadarColor = System.Drawing.ColorTranslator.FromHtml("#464F51");
            this.BulletColor = System.Drawing.ColorTranslator.FromHtml("#1B2C3C");

            // Loop forever
            while (true)
            {
                TurnGunRight(10); // Scans automatically

            }
        }

        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            if(IsTeammate(e.Name))
            {
                return;
            }

            // Calculate exact location of the robot
            double absoluteBearing = Heading + e.Bearing;
            double bearingFromGun = Utils.NormalRelativeAngleDegrees(absoluteBearing - GunHeading);

            // Calculate the angle for aiming based on the formula
            double bearingToEnemy = Utils.NormalRelativeAngleDegrees(e.Bearing);
            double bulletAngle = Math.Asin(8 * Math.Sin(bearingToEnemy) / 11);
            // If it's close enough, fire!
            if (Math.Abs(bulletAngle) <= 3)
            {
                TurnGunRight(bearingFromGun);
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

                // Otherwise just set the gun to turn.
                // Note:  This will have no effect until we call scan()
                TurnGunRight(bearingFromGun);
            }

            // Generates another scan event if we see a robot.
            // We only need to call this if the gun (and therefore radar)
            // are not turning.  Otherwise, scan is called automatically.
            if (bearingFromGun == 0)
            {
                Scan();
            }
        }
    }
}
