using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coopetition
{
    public class User
    {
        private int id;
        private bool activated;

        public int Id 
        {
            get { return id; }
            set { id = value; } 
        }

        public bool Activated 
        {
            get { return activated; }
            set { activated = value; } 
        }

        public User()
        { }

        //private Task GenerateTask(int _id)
        //{
        //    Task task = new Task(_id);
        //    return task;
        //}

        //public void SendRequestToCommunity(Community community)
        //{
        //    Task task = GenerateTask();
        //    community.TaskPool.Add(task);
        //}

        public void SendFeedback()
        { 
        }
    }
}
