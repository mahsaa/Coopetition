using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Coopetition
{
    public class Task
    {
        private int id;
        private double qos;
        private int fee;
        private bool assigned;

        public int Id 
        {
            get { return id; }
            set { id = value; }
        }

        public double QoS 
        {
            get { return qos; }
            set { qos = value; }
        }

        public int Fee 
        {
            get { return fee; }
            set { fee = value; }
        }

        public bool Assigned 
        {
            get { return assigned; }
            set { assigned = value; } 
        }

        public Task()
        { }

        public Task(int _id)
        {
            id = _id;
            Random rnd = new Random(DateTime.Now.Millisecond);
            Thread.Sleep(5);
            double q = Math.Round(rnd.NextDouble(), 4);
            qos = Constants.TaskQoS_LowerBound + (Constants.TaskQoS_UpperBound - Constants.TaskQoS_LowerBound) * q;        
            fee = rnd.Next(Constants.TaskFee_LowerBound, Constants.TaskFee_UpperBound);
            assigned = false;
        }
    }
}
