using Robocode;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Drawing.Configuration;
using System.Numerics;

namespace CAP4053.Student
{

    public class Dalek : TeamRobot
    {
        private class enemyBot
        {
            //name
            public string name;
            //location
            public Vector2 location;

            //Energy
            public double energy;

            //Bearing (angular location, based on center)
            public double bearing;

            //Heading (where is it pointing)
            public double heading;

            //Distance
            public double distance;

            //Velocity
            public double velocity;

            //lastScan
            public long lastScan;

            //SHOT
            public Boolean shotFlag;
        }

        Dictionary<string, enemyBot> enemies = new Dictionary<string, enemyBot>();
        enemyBot target = null;
        const double turnTime = 0.1;
        int i = 0;
        double xTarget, yTarget;
        Boolean fire = false;
        int direction = 1;
        double bulletSpeed = 3;
        public override void Run()
        {
            GunTurnCompleteCondition doneTurningGun = new GunTurnCompleteCondition(this);
            RadarTurnCompleteCondition doneTurningRadar = new RadarTurnCompleteCondition(this);
            //Set Robot Colors Here
            //asdfasdfdsdafasdafsd
            IsAdjustGunForRobotTurn = true;
            MaxVelocity = 8;

            this.BodyColor = System.Drawing.ColorTranslator.FromHtml("#602537");
            this.GunColor = System.Drawing.ColorTranslator.FromHtml("#0F0F0F");
            this.RadarColor = System.Drawing.ColorTranslator.FromHtml("#464F51");
            this.BulletColor = System.Drawing.ColorTranslator.FromHtml("#1B2C3C");

            while (true)
            {

                //Check Target is valid
                if (target != null && (Time - target.lastScan) > 5)
                {
                    //Out.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~RESET TARGET~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n!\n!\n!");
                    target = null;
                }

                //Radar Scan
                if (target == null)
                {
                    SetTurnRadarRightRadians(10000);
                    //Out.Write("NO TARGET");
                }

                //target = null;    
                //Scan();

                //Tracking
                if (target != null)
                {
                    calculateGunHeading();
                }

                //Turning + Movement
                if (target != null)
                {
                    int distance = 1000;
                    distance *= direction;

                    double enemyAngle = (target.heading % Math.PI) + (target.bearing % Math.PI);

                    //If about to hit wall, change direction
                    detectWallCollision();

                    //Chase
                    if (target.distance > 480)
                    {
                        Out.WriteLine("CHASE " + distance);
                        chaseHeading();

                    }
                    //Circle
                    else if (target.distance > 240 && Math.Abs(Math.PI - enemyAngle) < Math.PI / 10)
                    {
                        Out.WriteLine("CIRCLE " + distance);

                        double relativeAngle = target.bearing - HeadingRadians;
                        double angle = (Math.PI / 2) - relativeAngle;
                        SetTurnLeftRadians(normalizeAngle(angle));
                    }
                    //Reverse
                    else
                    {
                        if (Math.Abs(Math.PI - enemyAngle) < Math.PI / 10)
                            Out.WriteLine("HELP!!! " + enemyAngle);
                        distance *= -1;
                        Out.WriteLine("REVERSE " + distance);

                        if (distance < 0)
                        {
                            double angle = Math.PI - target.heading;
                            SetTurnLeftRadians(angle);
                        }
                        else
                        {
                            double relativeAngle = target.bearing - HeadingRadians;
                            double angle = (Math.PI / 2) - relativeAngle;
                            SetTurnLeftRadians(normalizeAngle(angle));
                        }

                    }

                    SetAhead(distance);
                }

                //Fire
                if (fire)
                    Fire(bulletSpeed);
                fire = false;

                //Perform Actions
                Execute();

            }


        }

        private void detectWallCollision()
        {

        }
        private void chaseHeading()
        {
            double turnHeading = target.bearing - HeadingRadians;
            SetTurnRightRadians(normalizeAngle(turnHeading * direction));
        }
        private double normalizeAngle(double angle)
        {
            if (angle > Math.PI)
            {
                return angle - (2 * Math.PI);
            }
            else if (angle < -Math.PI)
            {
                return angle + (2 * Math.PI);
            }
            return angle;
        }

        private double calculateGunHeading()
        {

            //Bullet speed is hardcoded
            double Be = normalizeAngle(target.heading - target.bearing);
            double relativeHeading = Math.Asin(target.velocity * Math.Sin(Be) / Rules.GetBulletSpeed(bulletSpeed)) + target.bearing;

            //Check to make sure the final distance doesn't go out of bound or hit a wall!!!
            fire = true;
            /*
            double Bt = relativeHeading;
            double Bs = (Math.PI) - Bt - Be;

            double Db = ((Math.Tan(target.heading) * (Y - target.location.Y)) + (target.location.X - X)) / (Math.Sin(HeadingRadians) - Math.Tan(target.heading)*Math.Cos(HeadingRadians));
            
            double dx = (Db * Math.Sin(HeadingRadians)) + X;
            double dy = (Db * Math.Cos(HeadingRadians)) + Y;

            xTarget = dx;
            yTarget = dy;
            Out.WriteLine("Distance: " + Db);
            */
            //Out.Write("Intercept: (" + dx + ", " + dy + ")  ||  ");
            /*
            if (dx > 1200 || dx < 0) {
                Out.Write("Stopped fire b/x of X");
                fire = false;
            }
            else if(dy > 1200 || dy < 0)
            {
                Out.Write("Stopped fire b/x of Y");
                fire = false;
            }*/
            //Out.WriteLine();
            // Out.WriteLine("Current: ("+target.location.X + ", " + target.location.Y + ")");
            //Out.WriteLine("---------------------------------");
            //Turn Gun
            //Out.WriteLine("Relative: " + relativeHeading);
            double gunTurnHeading = normalizeAngle(relativeHeading - GunHeadingRadians);
            SetTurnGunRightRadians(gunTurnHeading);

            return 0;
        }


        public override void OnScannedRobot(ScannedRobotEvent e)
        {
            enemyBot enemy = new enemyBot();

            enemy.bearing = e.BearingRadians;
            enemy.bearing += HeadingRadians;

            float xDisp = (float)(X + e.Distance * Math.Sin(enemy.bearing));
            float yDisp = (float)(Y + e.Distance * Math.Cos(enemy.bearing));

            enemy.name = e.Name;
            enemy.location = new Vector2(xDisp, yDisp);
            enemy.energy = e.Energy;
            enemy.heading = e.HeadingRadians;
            enemy.distance = e.Distance;
            enemy.velocity = e.Velocity;
            enemy.lastScan = e.Time;


            //Radar Tracking
            if (target != null && e.Name == target.name)
            {
                //Out.WriteLine(target.name);
                double radarTurnHeading = normalizeAngle(target.bearing - RadarHeadingRadians);
                SetTurnRadarRightRadians(radarTurnHeading);

                double energyDiff = target.energy - e.Energy;
                //Out.WriteLine(energyDiff);
                if (energyDiff > Rules.MIN_BULLET_POWER && energyDiff < Rules.MAX_BULLET_POWER)
                {
                    enemy.shotFlag = true;
                    direction *= -1;
                    Out.WriteLine("ENEMY FIRE!!");
                }
            }

            if (!IsTeammate(e.Name))
            {
                //Add to dictionary of enemies
                enemies[e.Name] = enemy;

                //Lock onto newest enemy
                target = enemies[e.Name];
            }
            //Out.WriteLine(e.Name + "(" + enemy.location.X + ", " + enemy.location.Y + ") - Bearing: " + enemy.bearing + " | distance: " + enemy.distance + " | velocity: " + enemy.velocity);
            //Out.WriteLine("("+X + ", " + Y + ")");

        }
        public override void OnPaint(IGraphics g)
        {
            Pen pen = new Pen(Color.Red);
            pen.Width = 5.0F;
            g.DrawLine(pen, (int)X, (int)Y, (int)xTarget, (int)yTarget);
        }

        public override void OnHitWall(HitWallEvent e)
        {
            direction *= -1;
        }

        /*
        public override void OnHitByBullet(HitByBulletEvent e)
        {
        }
        */
    }
}



