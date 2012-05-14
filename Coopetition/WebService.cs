using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Coopetition
{
    public class WebService
    {
        private int id;
        private double reputation;
        private double qos;
        private double growthFactor;
        private int budget;
        private int networkId = -1; // <NetworkId, Network>
        private double taskPortionDone;
        private bool readyToCompete;
        private double providedQoS;
        private bool isCollaborated = false;
        private bool hasCollaborated = false;
        private int communityId;
        private Constants.WebserviceType type = Constants.WebserviceType.Nothing;

        public int Id 
        { 
            get { return id; } 
            set { id = value; } 
        }

        public double Reputation 
        {
            get { return reputation; }
            set { reputation = value; }
        }

        public double QoS 
        {
            get { return qos; }
            set { qos = value; }
        }

        public double GrowthFactor 
        {
            get { return growthFactor; }
            set { growthFactor = value; }
        }

        public int Budget 
        {
            get { return budget; }
            set { budget = value; }
        }

        public int NetworkId
        {
            get { return networkId; }
            set { networkId = value; } 
        }

        public double TaskPortionDone 
        {
            get { return taskPortionDone; }
            set { taskPortionDone = value; }
        }

        public bool ReadyToCompete 
        {
            get { return readyToCompete; }
            set { readyToCompete = value; } 
        }

        public double ProvidedQoS 
        {
            get { return providedQoS; }
            set { providedQoS = value; } 
        }

        public bool IsCollaborated 
        {
            get { return isCollaborated; }
            set { isCollaborated = value; } 
        }

        public bool HasCollaborated 
        {
            get { return hasCollaborated; }
            set { hasCollaborated = value; } 
        }

        public int CommunityId 
        {
            get { return communityId; }
            set { communityId = value; } 
        }

        public Constants.WebserviceType Type 
        {
            get { return type; }
            set { type = value; } 
        }

        public WebService(int _id)
        {
            this.id = _id;

            Random rnd = new Random(DateTime.Now.Millisecond);
            double q = Math.Round(rnd.NextDouble(), 2);
            qos = Constants.WebserviceQoS_LowerBound + (Constants.WebserviceQoS_UpperBound - Constants.WebserviceQoS_LowerBound) * q;

            budget = rnd.Next(Constants.WebserviceBudget_LowerBound, Constants.WebserviceBudget_UpperBound);

            double g = Math.Round(rnd.NextDouble(), 2);
            growthFactor = Constants.WebserviceGrowthFactor_LowerBound + (Constants.WebserviceGrowthFactor_UpperBound - Constants.WebserviceGrowthFactor_LowerBound) * g;

            double rep = Math.Round(rnd.NextDouble(), 2);
            reputation = Constants.WebserviceReputation_LowerBound + (Constants.WebserviceReputation_UpperBound - Constants.WebserviceReputation_LowerBound) * rep;

        }

        public void CoopetitionDecision(int numberOfRun)
        {
            if (Environment.overAllStrategy == Constants.SimulationType.Cooperative)
            {
                growthFactor = this.qos * this.reputation;
                if (this.readyToCompete && numberOfRun > 1)
                {
                    growthFactor = this.providedQoS * this.reputation;
                    // growthFactor *= Math.Abs(((double)(this.bankAccount - Constants.Webservice_DefaultBankAccount) / (numberOfRun * Constants.TaskFee_UpperBound)));
                }
                // Environment.outputLog.AppendText("WSid: " + this.id + " ,GF: " + growthFactor + ", QoS: " + this.qos + ", rep: " + this.reputation + ", numberofRun: " + numberOfRun + ", bankAccount: " + bankAccount + ", readytc: " + readyToCompete + "\n");
                if (growthFactor >= Constants.CompetitionThreshold)
                    readyToCompete = true;
            }
            else if (Environment.overAllStrategy == Constants.SimulationType.AllCompete)
            {
                readyToCompete = true;
            }
            else if (Environment.overAllStrategy == Constants.SimulationType.AllRandom)
            {
                Random rnd = new Random();
                double p = rnd.NextDouble();
                if (p > 0.5)
                    readyToCompete = true;
                else
                    readyToCompete = false;
            }

        }

        public void StartDoingTask(Task task, Community community)
        {
            if (task != null)
            {
              //  Environment.outputLog.AppendText("WSid: " + this.id + ", Running Task\n");

                double resultQoS;
                Random rnd = new Random(DateTime.Now.Millisecond);
                double rndDecision = rnd.NextDouble();
                if ((rndDecision > Constants.DoingAloneProbability) && (rndDecision <= Constants.CollaborateProbability))
                {
                    CollaborationNetwork net = community.IntraNetworks.Find(delegate(CollaborationNetwork nw) { return nw.Id == this.networkId; });
                    if (net != null)
                    {
                        Collaborate(task, net, community);
                    }
                    else
                    {
                        this.taskPortionDone = 1;
                        double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 2) / Constants.QoSVarianceModifier));

                        this.providedQoS = rndQoS;
                        this.hasCollaborated = false;
                        resultQoS = this.providedQoS;

                        community.Members[this.id].NumberOfTasksDone++;
                        this.budget += task.Fee; // Should be changed based on the provided QoS
                    }
                }
                else
                {
                    this.taskPortionDone = 1;
                    double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 2) / Constants.QoSVarianceModifier));

                    this.providedQoS = rndQoS;
                    this.hasCollaborated = false;
                    resultQoS = this.providedQoS;

                    community.Members[this.id].NumberOfTasksDone++;
                    this.budget += task.Fee; // Should be changed based on the provided QoS
                }
            }
        }

        private void Collaborate(Task task, CollaborationNetwork network, Community cm)
        {
            List<Community.WebServiceInfo> webserviceInfos = cm.Members;
            Random rnd = new Random(DateTime.Now.Millisecond);
            double rndPortion = Math.Round(rnd.NextDouble(), 2);
            this.hasCollaborated = true;
            this.taskPortionDone = rndPortion; // main web service task portion
            cm.Members[this.id].NumberOfTasksDone++;
            double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 2) / Constants.QoSVarianceModifier));
            this.providedQoS = rndQoS;
            this.budget += (int)((1 - Constants.CooperationFeePercentage) * task.Fee);
            double networkPortion = 1 - rndPortion;
            int numberOfCollaborators = rnd.Next(1, network.MembersIds.Count);

            double networkMembersPortion = Math.Round((double)networkPortion / numberOfCollaborators, 2);
            double networkQoS = 0;
            
            List<WebService> collaborationNetwork = new List<WebService>();
            List<WebService> networkMembers = new List<WebService>();
            for (int i = 0; i < network.MembersIds.Count; i++)
            {
                networkMembers.Add(webserviceInfos.Find(delegate(Community.WebServiceInfo wsInfo) { return wsInfo.Webservice.Id == network.MembersIds[i]; }).Webservice);
            }
            networkMembers.RemoveAll(delegate(WebService ws) { return ws.Id == this.id; });
            List<WebService> tempNetwork = networkMembers;
            while (numberOfCollaborators > 0)
            {
                int j = rnd.Next(0, tempNetwork.Count);
                numberOfCollaborators--;
                ((WebService)tempNetwork[j]).isCollaborated = true;
                collaborationNetwork.Add((WebService)tempNetwork[j]);
                tempNetwork.RemoveAt(j);
            }

            String collaborativeMemberIds = "";
            collaborationNetwork.ForEach(delegate(WebService ws) 
                                { 
                                    ws.taskPortionDone = networkMembersPortion; 
                                    ws.providedQoS = Math.Max(0, ws.qos - (Math.Round(rnd.NextDouble(), 2) / Constants.QoSVarianceModifier));
                                    cm.Members[ws.id].NumberOfTasksDone++;
                                    ws.budget += (int)((Constants.CooperationFeePercentage / collaborationNetwork.Count) * task.Fee);
                                    networkQoS += ws.providedQoS;
                                    collaborativeMemberIds += ws.id + ", ";
                                });
            char[] charsToRemove = ", ".ToCharArray();
            collaborativeMemberIds = collaborativeMemberIds.TrimEnd(charsToRemove);
            Environment.outputLog.AppendText("Web service " + this.id + " Cooperates with " + collaborativeMemberIds + "\n");
            this.providedQoS = (Constants.ResponsibleWSQoSPortion * this.providedQoS) + ((1 - Constants.ResponsibleWSQoSPortion) * networkQoS); // main web service provided QoS (provided QoS for the task)
        }

    }
}