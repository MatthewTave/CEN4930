using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors.StudentAI
{
    public class Flock
    {
        public float AlignmentStrength { get; set; }
        public float CohesionStrength { get; set; }
        public float SeparationStrength { get; set; }
        public List<MovingObject> Boids { get; protected set; }
        public Vector3 AveragePosition { get; set; }
        protected Vector3 AverageForward { get; set; }
        public float FlockRadius { get; set; }

        public virtual void Update(float deltaTime)
        {
            this.AverageForward = get_avg_forward(this.Boids);
            this.AveragePosition = get_avg_position(this.Boids);

            foreach(MovingObject boid in this.Boids)
            {
                Vector3 accel = calc_alignment_accel(boid);
                accel += calc_cohesion_accel(boid);
                accel += calc_separation_accel(boid);

                accel *= boid.MaxSpeed * deltaTime;

                boid.Velocity += accel;

                if(boid.Velocity.Length > boid.MaxSpeed)
                {
                    boid.Velocity.Normalize();
                    boid.Velocity *= boid.MaxSpeed;
                }

                boid.Update(deltaTime);
            }
        }

        private Vector3 get_avg_forward(List<MovingObject> objs)
        {
            Vector3 fwd = get_avg(objs.Select(obj => obj.Velocity).ToList());
            fwd.Normalize();

            return fwd;
        }

        private Vector3 get_avg_position(List<MovingObject> objs)
        {
            return get_avg(objs.Select(obj => obj.Position).ToList());
        }

        private Vector3 get_avg(List<Vector3> vecs)
        {
            Vector3 avg = new Vector3(0, 0, 0);

            foreach (var vecsItem in vecs)
            {
                avg += vecsItem;
            }

            avg.X /= vecs.Count;
            avg.Y /= vecs.Count;
            avg.Z /= vecs.Count;

            return avg;
        }

        private Vector3 calc_alignment_accel(MovingObject boid)
        {
            Vector3 accel = this.AverageForward;

            if(accel.Length > 1)
                accel.Normalize();

            return accel * this.AlignmentStrength;
        }

        private Vector3 calc_cohesion_accel(MovingObject boid)
        {
            Vector3 accel = this.AveragePosition - boid.Position;
            float distance = accel.Length;
            accel.Normalize();

            if(distance < this.FlockRadius) 
                accel = accel * (distance / this.FlockRadius);

            return accel * this.CohesionStrength;
        }

        private Vector3 calc_separation_accel(MovingObject boid)
        {
            Vector3 sum = new Vector3(0, 0, 0);

            foreach(MovingObject sibling in this.Boids)
            {
                if (sibling == boid) continue;

                Vector3 accel = boid.Position - sibling.Position;
                float distance = accel.Length;
                float safeDistance = boid.SafeRadius + sibling.SafeRadius;

                if(distance < safeDistance)
                {
                    accel.Normalize();
                    accel *= (safeDistance - distance)/safeDistance;
                    sum += accel;
                }
            }

            if(sum.Length > 1) sum.Normalize();

            return sum * this.SeparationStrength;
        }
        
    }
}
