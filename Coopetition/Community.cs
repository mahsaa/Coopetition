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
            private int numberOfTasksDone;
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
        private int bankAccount;
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

        public int BankAccount 
        {
            get { return bankAccount; }
            set { bankAccount = value; }
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
          //  SortTaskPool(taskPool, "ResponseTime");
            Random rnd = new Random(DateTime.Now.Millisecond);
            int numberOfTasksToBeDone = rnd.Next(1, competitiveMembers.Count);

            int numberOfAcceptingMembers = (int)Math.Ceiling(Constants.AcceptanceProbability * competitiveMembers.Count);
            int numberOfRejectingMembers = competitiveMembers.Count - numberOfAcceptingMembers;

            Environment.outputLog.AppendText("Competetive Members: " + competitiveMembers.Count + "\n");
            Environment.outputLog.AppendText("Number of Tasks to be done: " + numberOfTasksToBeDone + "\n");
            Environment.outputLog.AppendText("Number of Accepting Members: " + numberOfAcceptingMembers + "\n");

            int numberOfTasksToBeAssigned = Math.Min(numberOfAcceptingMembers, numberOfTasksToBeDone);
            List<Task> notAssignedTasks = taskPool.FindAll(delegate(Task task) { return !(task.Assigned); });

            int k = 0;
            for (int i = 0; i < numberOfTasksToBeAssigned; i++)
            {
                // taskPool[i].Assigned = true;
                notAssignedTasks[i].Assigned = true;
                competitiveMembers[i].NumberOfOfferedTasks++;
                competitiveMembers[i].CurrentIfOfferedTask = true;
                competitiveMembers[i].NumberOfAcceptedTasks++;
                competitiveMembers[i].CurrentIfAcceptedTask = true;
                competitiveMembers[i].CurrentAssignedTask = notAssignedTasks[i];
                k = i;
            }

            if (numberOfAcceptingMembers < numberOfTasksToBeAssigned)
            {
                int numberOfRemainedTasks = Math.Max(numberOfAcceptingMembers, numberOfTasksToBeDone) - numberOfTasksToBeAssigned;
                for (int j = k; j < numberOfRemainedTasks; j++)
                {
                    competitiveMembers[j].NumberOfOfferedTasks++;
                    competitiveMembers[j].CurrentIfOfferedTask = true;
                    competitiveMembers[j].CurrentIfAcceptedTask = false;
                    // competitiveMembers[j].NumberOfAcceptedTasks--;
                } 
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
                        if (wsInfo.Webservice.HasCollaborated) // Has collaborated
                        {
                            if (wsInfo.Webservice.ProvidedQoS >= wsInfo.CurrentAssignedTask.QoS)
                            {
                                double totalReward = (Constants.RewardCoefficient * (wsInfo.Webservice.ProvidedQoS - wsInfo.CurrentAssignedTask.QoS)) + Constants.RewardConstant;
                                wsInfo.Webservice.Reputation += (totalReward * wsInfo.Webservice.TaskPortionDone);
                                if (wsInfo.Webservice.Reputation > 1)
                                {
                                    wsInfo.Webservice.Reputation = 1;
                                }
                               // CollaborationNetwork network = this.intraNetworks.Find(delegate(CollaborationNetwork net) { return net.Id == wsInfo.Webservice.NetworkId; });
                                for (int i = 0; i < wsnetwork.MembersIds.Count; i++)
                                {
                                    WebService ws = members.Find(delegate(WebServiceInfo wesInfo) { return wesInfo.Webservice.Id == wsnetwork.MembersIds[i]; }).Webservice;
                                    if (ws.IsCollaborated)
                                    {
                                        ws.Reputation += (totalReward * ws.TaskPortionDone);
                                        if (ws.Reputation > 1)
                                        {
                                            ws.Reputation = 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                double totalPenalty = (Constants.PenaltyCoefficient * (wsInfo.Webservice.ProvidedQoS - wsInfo.CurrentAssignedTask.QoS)) + Constants.PenaltyConstant;
                                wsInfo.Webservice.Reputation -= totalPenalty;
                                if (wsInfo.Webservice.Reputation < 0)
                                {
                                    wsInfo.Webservice.Reputation = 0;
                                }
                                for (int i = 0; i < wsnetwork.MembersIds.Count; i++)
                                {
                                    WebService ws = members.Find(delegate(WebServiceInfo wesInfo) { return wesInfo.Webservice.Id == wsnetwork.MembersIds[i]; }).Webservice;
                                    if (ws.IsCollaborated)
                                    {
                                        ws.Reputation -= (totalPenalty * ws.TaskPortionDone);
                                        if (wsInfo.Webservice.Reputation < 0)
                                        {
                                            wsInfo.Webservice.Reputation = 0;
                                        }
                                    }
                                }
                            }
                        }
                        else // Has done it alone
                        {
                            if (wsInfo.Webservice.ProvidedQoS >= wsInfo.CurrentAssignedTask.QoS)
                            {
                                wsInfo.Webservice.Reputation += (Constants.RewardCoefficient * (wsInfo.Webservice.ProvidedQoS - wsInfo.CurrentAssignedTask.QoS)) + Constants.RewardConstant;
                                if (wsInfo.Webservice.Reputation > 1)
                                {
                                    wsInfo.Webservice.Reputation = 1;
                                }
                            }
                            else
                            {
                                wsInfo.Webservice.Reputation -= (Constants.PenaltyCoefficient * (wsInfo.Webservice.ProvidedQoS - wsInfo.CurrentAssignedTask.QoS)) + Constants.PenaltyConstant;
                                if (wsInfo.Webservice.Reputation < 0)
                                {
                                    wsInfo.Webservice.Reputation = 0;
                                }
                            }
                        }


                        if ((wsInfo.Webservice.ProvidedQoS >= wsInfo.CurrentAssignedTask.QoS) || (wsInfo.Webservice.ProvidedQoS < wsInfo.CurrentAssignedTask.QoS - 0.1))
                        {
                            wsInfo.Webservice.Reputation += (wsInfo.Webservice.Reputation * 0.1);
                            if (wsnetwork != null)
                            {
                                for (int i = 0; i < wsnetwork.MembersIds.Count; i++)
                                {
                                    WebService ws = members.Find(delegate(WebServiceInfo wesInfo) { return wesInfo.Webservice.Id == wsnetwork.MembersIds[i]; }).Webservice;
                                    if (ws.IsCollaborated)
                                    {
                                        ws.Reputation += (ws.Reputation * 0.08);
                                    }
                                }
                            }
                        }
                        else
                        {
                            wsInfo.Webservice.Reputation -= (wsInfo.Webservice.Reputation * Constants.ResponsibleWSPunishmentPercentage);
                            if (wsnetwork != null)
                            {
                                for (int i = 0; i < wsnetwork.MembersIds.Count; i++)
                                {
                                    WebService ws = members.Find(delegate(WebServiceInfo wesInfo) { return wesInfo.Webservice.Id == wsnetwork.MembersIds[i]; }).Webservice;

                                    if (ws.IsCollaborated)
                                    {
                                        ws.Reputation -= (ws.Reputation * (1 - Constants.ResponsibleWSPunishmentPercentage));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {

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
