using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coopetition
{
    public class Community
    {
        public class WebServiceInfo
        {
            private WebService webservice;
            private int numberOfOfferedTasks;
            private int numberOfAcceptedTasks;
            private int numberOfTasksDone = 0;
            private int positiveFeedbacks;
            private int negativeFeedbacks;
            private double activenessFactor;
            private bool currentIfOfferedTask = false;
            private bool currentIfAcceptedTask = false;
            private Task currentAssignedTask;

            public WebService Webservice 
            {
                get { return webservice; }
                set { webservice = value; } 
            }

            public int NumberOfOfferedTasks 
            {
                get { return numberOfOfferedTasks; }
                set { numberOfOfferedTasks = value; } 
            }

            public int NumberOfAcceptedTasks 
            {
                get { return numberOfAcceptedTasks; }
                set { numberOfAcceptedTasks = value; }
            }

            public int NumberOfTasksDone 
            {
                get { return numberOfTasksDone; }
                set { numberOfTasksDone = value; } 
            }

            public int PositiveFeedbacks 
            {
                get { return positiveFeedbacks; }
                set { positiveFeedbacks = value; } 
            }

            public int NegativeFeedbacks 
            {
                get { return negativeFeedbacks; }
                set { negativeFeedbacks = value; } 
            }

            public double ActivenessFactor 
            {
                get { return activenessFactor; }
                set { activenessFactor = value; } 
            }

            public bool CurrentIfOfferedTask 
            {
                get { return currentIfOfferedTask; }
                set { currentIfOfferedTask = value; } 
            }

            public bool CurrentIfAcceptedTask
            {
                get { return currentIfAcceptedTask; }
                set { currentIfAcceptedTask = value; } 
            }

            public Task CurrentAssignedTask 
            {
                get { return currentAssignedTask; }
                set { currentAssignedTask = value; } 
            }
        }

        private int id;
        private int marketShare;
        private int budget;
        private double reputation;
        private List<WebServiceInfo> members;
        private List<Community> network;
        private List<Task> taskPool;
        private List<CollaborationNetwork> intraNetworks;

        public int Id 
        {
            get { return id; }
            set { id = value; }
        }

        public int MarketShare 
        {
            get { return marketShare; }
            set { marketShare = value; } 
        }

        public int Budget 
        {
            get { return budget; }
            set { budget = value; }
        }

        public double Reputation 
        {
            get { return reputation; }
            set { reputation = value; }
        }

        public List<WebServiceInfo> Members 
        {
            get { return members; }
            set { members = value; }
        }

        public List<Community> Network 
        {
            get { return network; }
            set { network = value; }
        }

        public List<Task> TaskPool 
        {
            get { return taskPool; }
            set { taskPool = value; }
        }

        public List<CollaborationNetwork> IntraNetworks 
        {
            get { return intraNetworks; }
            set { intraNetworks = value; } 
        }

        public Community(int _id)
        {
            this.id = _id;

            members = new List<WebServiceInfo>();
            network = new List<Community>();
            taskPool = new List<Task>();
            intraNetworks = new List<CollaborationNetwork>();
        }

        public void AddMember(WebService ws)
        {
            WebServiceInfo wsInfo = new WebServiceInfo();
            wsInfo.ActivenessFactor = 0;
            wsInfo.NegativeFeedbacks = 0;
            wsInfo.NumberOfAcceptedTasks = 0;
            wsInfo.NumberOfOfferedTasks = 0;
            wsInfo.NumberOfTasksDone = 0;
            wsInfo.PositiveFeedbacks = 0;
            wsInfo.Webservice = ws;
            this.members.Add(wsInfo);
        }

        /// <summary>
        /// Task allocation
        /// </summary>
        public void OfferTaskToWebservice()
        {
           // UpdateMemberlist();
            List<WebServiceInfo> competitiveMembers = members.FindAll(delegate(WebServiceInfo wsInfo) { return wsInfo.Webservice.ReadyToCompete == true; });

            SortTaskPool(taskPool, "QoS");
            if (competitiveMembers.Count < 1)
            {
                Environment.outputLog.AppendText("There are no competitive web services.\n");
                return;
            }
            Random rnd = new Random(DateTime.Now.Millisecond);
            //int numberOfTasksToBeDone = rnd.Next(1, competitiveMembers.Count);
            int numberOfTasksToBeDone = rnd.Next(1, Constants.MaxNumberOfTasksPerRun);

            int numberOfAcceptingMembers = (int)Math.Ceiling(Constants.AcceptanceProbability * competitiveMembers.Count);
            int numberOfRejectingMembers = competitiveMembers.Count - numberOfAcceptingMembers;

            Environment.outputLog.AppendText("competitive Members: " + competitiveMembers.Count + "\n");
            Environment.outputLog.AppendText("Number of Tasks to be done: " + numberOfTasksToBeDone + "\n");
            Environment.outputLog.AppendText("Number of Accepting Members: " + numberOfAcceptingMembers + "\n");

            List<Task> notAssignedTasks = taskPool.FindAll(delegate(Task task) { return !(task.Assigned); });
            int numberOfTasksToBeAssigned = Math.Min(numberOfAcceptingMembers, numberOfTasksToBeDone);
            numberOfTasksToBeAssigned = Math.Min(numberOfTasksToBeAssigned, notAssignedTasks.Count);

            Environment.outputLog.AppendText("Number of Tasks to be assigned: " + numberOfTasksToBeAssigned + "\n");

            if (notAssignedTasks.Count < 1)
            {
                Environment.outputLog.AppendText("There are no tasks left.\n");
                return;
            }

            Environment.outputLog.AppendText("Number of Tasks Left: " + notAssignedTasks.Count + "\n");

            Environment.outputLog.AppendText("Web Services ");
            int k = 0;

            // Randomly selects among competitive web services
            int[] indices = new int[numberOfTasksToBeAssigned];
            List<int> competitiveIndices = new List<int>(competitiveMembers.Count);
            for (int i = 0; i < competitiveMembers.Count; i++)
            {
                competitiveIndices.Add(competitiveMembers[i].Webservice.Id);
            }
            for (int i = 0; i < numberOfTasksToBeAssigned; i++)
            {
                int index = rnd.Next(0, competitiveIndices.Count);
                indices[i] = competitiveIndices[index];
                competitiveIndices.RemoveAt(index);
            }

            // Assigns tasks
            for (int i = 0; i < numberOfTasksToBeAssigned; i++)
            {
                notAssignedTasks[i].Assigned = true;
                WebServiceInfo wsInfo = competitiveMembers.Find(delegate(WebServiceInfo winfo) { return winfo.Webservice.Id == indices[i]; });
                if (wsInfo.Webservice.Reputation >= Constants.ReputationThreshold)
                {
                    wsInfo.NumberOfOfferedTasks++;
                    wsInfo.CurrentIfOfferedTask = true;
                    wsInfo.NumberOfAcceptedTasks++;
                    wsInfo.CurrentIfAcceptedTask = true;
                    wsInfo.CurrentAssignedTask = notAssignedTasks[i];
                    Environment.outputLog.AppendText(" " + wsInfo.Webservice.Id + " ");
                    k = i;
                }
            }

            Environment.outputLog.AppendText(" got the job\n");

            if (numberOfAcceptingMembers < numberOfTasksToBeAssigned)
            {
                int numberOfRemainedTasks = Math.Max(numberOfAcceptingMembers, numberOfTasksToBeDone) - numberOfTasksToBeAssigned;
                int[] refusedIndices = new int[numberOfRemainedTasks];
                for (int i = 0; i < numberOfRemainedTasks; i++)
                {
                    int index = rnd.Next(0, competitiveIndices.Count);
                    refusedIndices[i] = competitiveIndices[index];
                    competitiveIndices.RemoveAt(index);
                }

                for (int j = k; j < numberOfRemainedTasks; j++)
                {
                    WebServiceInfo wsInfo = competitiveMembers.Find(delegate(WebServiceInfo winfo) { return winfo.Webservice.Id == refusedIndices[j]; });
                    wsInfo.NumberOfOfferedTasks++;
                    wsInfo.CurrentIfOfferedTask = true;
                    wsInfo.CurrentIfAcceptedTask = false;
                    Environment.outputLog.AppendText(" " + wsInfo.Webservice.Id + " ");
                }

                Environment.outputLog.AppendText(" refused the offer\n");
            }


            // Check for the case that number of tasks are greater than number of accepting web services

        }

        public void UpdateMemberReputation()
        {
            foreach (WebServiceInfo wsInfo in members)
            {
                
                CollaborationNetwork wsnetwork = this.intraNetworks.Find(delegate(CollaborationNetwork net) { return net.Id == wsInfo.Webservice.NetworkId; });
                
                if (wsInfo.CurrentIfOfferedTask)
                {
                    if (wsInfo.CurrentIfAcceptedTask)
                    {
                        if (wsInfo.Webservice.ProvidedQoS >= wsInfo.CurrentAssignedTask.QoS)
                        {
                            double totalReward = (Constants.RewardCoefficient * (wsInfo.Webservice.ProvidedQoS - wsInfo.CurrentAssignedTask.QoS)) + Constants.RewardConstant;
                            wsInfo.Webservice.Reputation += (totalReward * wsInfo.Webservice.TaskPortionDone);
                            if (wsInfo.Webservice.HasCollaborated) // Has collaborated
                            {
                                for (int i = 0; i < wsnetwork.MembersIds.Count; i++)
                                {
                                    WebService ws = members.Find(delegate(WebServiceInfo wesInfo) { return wesInfo.Webservice.Id == wsnetwork.MembersIds[i]; }).Webservice;
                                    if (ws.IsCollaborated)
                                    {
                                        ws.Reputation += (totalReward * ws.TaskPortionDone);
                                        ws.NumberOfRewarded++;
                                        Environment.outputLog.AppendText("web service " + ws.Id + " got reward!\n");
                                    }
                                }
                            }
                            wsInfo.Webservice.NumberOfRewarded++;
                            Environment.outputLog.AppendText("web service " + wsInfo.Webservice.Id + " got reward!\n");
                        }
                        else
                        {
                            double totalPenalty = (Constants.PenaltyCoefficient * (wsInfo.Webservice.ProvidedQoS - wsInfo.CurrentAssignedTask.QoS)) + Constants.PenaltyConstant;
                            wsInfo.Webservice.Reputation -= totalPenalty; // Penalized badly by MWS, so we don't consider task portion done by the main web service here
                            if (wsInfo.Webservice.HasCollaborated) // Has collaborated
                            {
                                for (int i = 0; i < wsnetwork.MembersIds.Count; i++)
                                {
                                    WebService ws = members.Find(delegate(WebServiceInfo wesInfo) { return wesInfo.Webservice.Id == wsnetwork.MembersIds[i]; }).Webservice;
                                    if (ws.IsCollaborated)
                                    {
                                        ws.Reputation -= (totalPenalty * ws.TaskPortionDone);
                                        Environment.outputLog.AppendText("web service " + ws.Id + " got penalized!\n");
                                    }
                                }
                            }
                            Environment.outputLog.AppendText("web service " + wsInfo.Webservice.Id + " got penalized!\n");
                        }

                        // Maintains the reputation constraints
                        if (wsInfo.Webservice.Reputation > Constants.WebserviceReputation_UpperBound)
                        {
                            wsInfo.Webservice.Reputation = Constants.WebserviceReputation_UpperBound;
                        }
                        else if (wsInfo.Webservice.Reputation < Constants.WebserviceReputation_LowerBound)
                        {
                            wsInfo.Webservice.Reputation = Constants.WebserviceReputation_LowerBound;
                        }

                        // I don't understand this code section!
                        //if ((wsInfo.Webservice.ProvidedQoS >= wsInfo.CurrentAssignedTask.QoS) || (wsInfo.Webservice.ProvidedQoS < wsInfo.CurrentAssignedTask.QoS - 0.1))
                        //{
                        //    wsInfo.Webservice.Reputation += (wsInfo.Webservice.Reputation * 0.1);
                        //    if (wsnetwork != null)
                        //    {
                        //        for (int i = 0; i < wsnetwork.MembersIds.Count; i++)
                        //        {
                        //            WebService ws = members.Find(delegate(WebServiceInfo wesInfo) { return wesInfo.Webservice.Id == wsnetwork.MembersIds[i]; }).Webservice;
                        //            if (ws.IsCollaborated)
                        //            {
                        //                ws.Reputation += (ws.Reputation * 0.08);
                        //            }
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    wsInfo.Webservice.Reputation -= (wsInfo.Webservice.Reputation * Constants.ResponsibleWSPunishmentPercentage);
                        //    if (wsnetwork != null)
                        //    {
                        //        for (int i = 0; i < wsnetwork.MembersIds.Count; i++)
                        //        {
                        //            WebService ws = members.Find(delegate(WebServiceInfo wesInfo) { return wesInfo.Webservice.Id == wsnetwork.MembersIds[i]; }).Webservice;

                        //            if (ws.IsCollaborated)
                        //            {
                        //                ws.Reputation -= (ws.Reputation * (1 - Constants.ResponsibleWSPunishmentPercentage));
                        //            }
                        //        }
                        //    }
                        //}
                    }
                }

            }
        }

        // Sort and maintain information of members
        private void UpdateMemberlist()
        {
           // SortMembers(members, "QoS");
           // SortMembers(members, "ResponseTime");
            SortMembers(members, "Reputation");

            foreach (WebServiceInfo wsInfo in members)
            {
                wsInfo.ActivenessFactor = Math.Round((double)wsInfo.NumberOfAcceptedTasks/wsInfo.NumberOfOfferedTasks, 2);
            }
        }

        public void SortMembers(List<WebServiceInfo> webservices, String sortBy)
        {
            HeapifyWebservices(webservices, sortBy);
            int end = webservices.Count - 1;
            while (end > 0)
            {
                SwapWebservices(webservices, end, 0);
                if (sortBy == "QoS")
                    ShiftDownByQoS(webservices, 0, end - 1);
                else if (sortBy == "Reputation")
                    ShiftDownByReputation(webservices, 0, end - 1);
                end--;
            }
        }

        private void HeapifyWebservices(List<WebServiceInfo> webservices, String sortBy)
        {
            int start = webservices.Count / 2;
            while (start >= 0)
            {
                if (sortBy == "QoS")
                    ShiftDownByQoS(webservices, start, webservices.Count - 1);
                else if (sortBy == "Reputation")
                    ShiftDownByReputation(webservices, start, webservices.Count - 1);
                
                start--;
            }
        }

        private void ShiftDownByQoS(List<WebServiceInfo> webservices, int start, int end)
        {
            int root = start;
            while (root * 2 <= end)
            {
                int child = root * 2;
                if ((child < end) && (webservices[child].Webservice.QoS < webservices[child + 1].Webservice.QoS))
                {
                    child++;
                }
                if (webservices[root].Webservice.QoS < webservices[child].Webservice.QoS)
                {
                    SwapWebservices(webservices, root, child);
                    root = child;
                }
                else
                {
                    return;
                }
            }
        }

        private void ShiftDownByReputation(List<WebServiceInfo> webservices, int start, int end)
        {
            int root = start;
            while (root * 2 <= end)
            {
                int child = root * 2;
                if ((child < end) && (webservices[child].Webservice.Reputation < webservices[child + 1].Webservice.Reputation))
                {
                    child++;
                }
                if (webservices[root].Webservice.Reputation < webservices[child].Webservice.Reputation)
                {
                    SwapWebservices(webservices, root, child);
                    root = child;
                }
                else
                {
                    return;
                }
            }
        }

        private void SwapWebservices(List<WebServiceInfo> webservices, int index1, int index2)
        {
            WebServiceInfo temp = webservices[index1];
            webservices[index1] = webservices[index2];
            webservices[index2] = temp;
        }

        public void SortTaskPool(List<Task> taskpool, String sortBy)
        {
            HeapifyTasks(taskpool, sortBy);
            int end = taskpool.Count - 1;
            while (end > 0)
            {
                SwapTasks(taskpool, end, 0);
                if (sortBy == "QoS")
                    ShiftDownTasksByQoS(taskpool, 0, end - 1);

                end--;
            }
        }

        private void HeapifyTasks(List<Task> taskpool, String sortBy)
        {
            int start = taskpool.Count / 2;
            while (start >= 0)
            {
                if (sortBy == "QoS")
                    ShiftDownTasksByQoS(taskpool, start, taskpool.Count - 1);

                start--;
            }
        }

        private void ShiftDownTasksByQoS(List<Task> taskpool, int start, int end)
        {
            int root = start;
            while (root * 2 <= end)
            {
                int child = root * 2;
                if ((child < end) && (taskpool[child].QoS < taskpool[child + 1].QoS))
                {
                    child++;
                }
                if (taskpool[root].QoS < taskpool[child].QoS)
                {
                    SwapTasks(taskpool, root, child);
                    root = child;
                }
                else
                {
                    return;
                }
            }
        }

        private void SwapTasks(List<Task> taskpool, int index1, int index2)
        {
            Task temp = taskpool[index1];
            taskpool[index1] = taskpool[index2];
            taskpool[index2] = temp;
        }


        // Sort and maintain list of collaborative communities
        private void UpdateNetwork()
        { 
        }



    }
}
