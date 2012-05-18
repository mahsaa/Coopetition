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
        private bool readyToCompete = false;
        private double providedQoS;
        private bool isCollaborated = false;
        private bool hasCollaborated = false;
        private int communityId;
        private Constants.WebserviceType type = Constants.WebserviceType.Random;
        private int totalIncome;
        private int numberOfCompetitions;
        private double competedProbability;
        private int numberOfRewarded;

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

        public int TotalIncome 
        {
            get { return totalIncome; }
            set { totalIncome = value; } 
        }

        public int NumberOfCompetitions 
        {
            get { return numberOfCompetitions; }
            set { numberOfCompetitions = value; } 
        }

        public double CompetedProbability 
        {
            get { return competedProbability; }
            set { competedProbability = value; } 
        }

        public int NumberOfRewarded
        {
            get { return numberOfRewarded; }
            set { numberOfRewarded = value; }
        }

        public WebService()
        { }

        public WebService(int _id)
        {
            this.id = _id;

            Random rnd = new Random(DateTime.Now.Millisecond);
           // Thread.Sleep(5);
           // double q = Math.Round(rnd.NextDouble(), 4);
           // qos = Constants.WebserviceQoS_LowerBound + (Constants.WebserviceQoS_UpperBound - Constants.WebserviceQoS_LowerBound) * q;
            qos = Environment.dataSet[_id].QoS;

           // budget = rnd.Next(Constants.WebserviceBudget_LowerBound, Constants.WebserviceBudget_UpperBound);
            budget = Constants.WebserviceDefaultBudget;

            Thread.Sleep(5);
            double g = Math.Round(rnd.NextDouble(), 4);
            growthFactor = Constants.WebserviceGrowthFactor_LowerBound + (Constants.WebserviceGrowthFactor_UpperBound - Constants.WebserviceGrowthFactor_LowerBound) * g;

            Thread.Sleep(5);
            double rep = Math.Round(rnd.NextDouble(), 4);
            reputation = Constants.WebserviceReputation_LowerBound + (Constants.WebserviceReputation_UpperBound - Constants.WebserviceReputation_LowerBound) * rep;

        }

        public void CoopetitionDecision(int numberOfTasksDone, int numberOfRun)
        {
            int totalMembershipFeesPaid = numberOfRun * Constants.MembershipFee;
            if (numberOfTasksDone > 0)
            {
                //this.growthFactor = (double)(this.reputation + this.qos + ((double)this.totalIncome / totalMembershipFeesPaid * numberOfTasksDone)) / 3;
                this.growthFactor = (double)(this.reputation + this.qos + ((double)numberOfTasksDone / numberOfRun)) / 3;
            }
            else if (numberOfTasksDone == 0)
            {
                this.growthFactor = (double)(this.reputation + this.qos) / 3;
            }

            if ((this.type == Constants.WebserviceType.Coopetitive) || (this.type == Constants.WebserviceType.Random))
            {
                if (this.growthFactor >= Constants.CompetitionThreshold)
                {
                    this.readyToCompete = true;
                    this.numberOfCompetitions++;
                   // this.competedProbability = (double)this.numberOfCompetitions / Constants.NumberOfRuns;
                } 
            }
            else if (this.type == Constants.WebserviceType.JustCompetitive)
            {
                this.readyToCompete = true;
                this.numberOfCompetitions++;
                //this.competedProbability = (double)numberOfCompetitions / Constants.NumberOfRuns;
            }

            this.competedProbability = (double)numberOfCompetitions / numberOfRun;
            Environment.outputLog.AppendText("Web service " + this.id + "'s NTD: " + numberOfTasksDone + " GrowthFactor: " + this.growthFactor + " TotalIncome: " + totalIncome + " Competition Probability: " + competedProbability + "\n");
        }

        public void StartDoingTask(Task task, Community community)
        {
            if (task != null)
            {
                double resultQoS;
                Random rnd = new Random(DateTime.Now.Millisecond);

                if ((this.type == Constants.WebserviceType.Coopetitive) || (this.type == Constants.WebserviceType.Random))
                {
                    double rndDecision = rnd.NextDouble();
                    if ((rndDecision > Constants.DoingAloneProbability) && (rndDecision <= Constants.CollaborateProbability))
                    {
                        CollaborationNetwork net = community.IntraNetworks.Find(delegate(CollaborationNetwork nw) { return nw.Id == this.networkId; });
                        if (net != null)
                        {
                            int cooperativeMembersInNetwork = net.MembersIds.FindAll(delegate(int wsId) { return community.Members[wsId].Webservice.readyToCompete == false; }).Count;
                            if (cooperativeMembersInNetwork > 1)
                            {
                                Collaborate(task, net, community);
                            }
                            else
                            {
                                this.taskPortionDone = 1;
                                Thread.Sleep(5);
                                double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 4) / Constants.QoSVarianceModifier));

                                this.providedQoS = rndQoS;
                                this.hasCollaborated = false;
                                resultQoS = this.providedQoS;

                                community.Members[this.id].NumberOfTasksDone++;
                                this.budget += task.Fee; // Should be changed based on the provided QoS
                                this.totalIncome += task.Fee;
                            }
                        }
                        else
                        {
                            this.taskPortionDone = 1;
                            Thread.Sleep(5);
                            double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 4) / Constants.QoSVarianceModifier));

                            this.providedQoS = rndQoS;
                            this.hasCollaborated = false;
                            resultQoS = this.providedQoS;

                            community.Members[this.id].NumberOfTasksDone++;
                            this.budget += task.Fee; // Should be changed based on the provided QoS
                            this.totalIncome += task.Fee;
                        }
                    }
                    else
                    {
                        this.taskPortionDone = 1;
                        Thread.Sleep(5);
                        double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 4) / Constants.QoSVarianceModifier));

                        this.providedQoS = rndQoS;
                        this.hasCollaborated = false;
                        resultQoS = this.providedQoS;

                        community.Members[this.id].NumberOfTasksDone++;
                        this.budget += task.Fee; // Should be changed based on the provided QoS
                        this.totalIncome += task.Fee;
                    } 
                }
                else if (this.type == Constants.WebserviceType.JustCompetitive)
                {
                    this.taskPortionDone = 1;
                    Thread.Sleep(5);
                    double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 4) / Constants.QoSVarianceModifier));

                    this.providedQoS = rndQoS;
                    this.hasCollaborated = false;
                    resultQoS = this.providedQoS;

                    community.Members[this.id].NumberOfTasksDone++;
                    this.budget += task.Fee; // Should be changed based on the provided QoS
                    this.totalIncome += task.Fee;
                }
            }
        }

        private void Collaborate(Task task, CollaborationNetwork network, Community cm)
        {
            List<Community.WebServiceInfo> webserviceInfos = cm.Members;
            Random rnd = new Random(DateTime.Now.Millisecond);
            Thread.Sleep(5);
            double rndPortion = Math.Round(rnd.NextDouble(), 4);
            this.hasCollaborated = true;
            this.taskPortionDone = rndPortion; // main web service task portion
            cm.Members[this.id].NumberOfTasksDone++;
            Thread.Sleep(5);
            double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 4) / Constants.QoSVarianceModifier));
            this.providedQoS = rndQoS;
            this.budget += (int)((1 - Constants.CooperationFeePercentage) * task.Fee);
            this.totalIncome += (int)((1 - Constants.CooperationFeePercentage) * task.Fee);
            double networkPortion = 1 - rndPortion;
            //int numberOfCollaborators = rnd.Next(1, network.MembersIds.Count);

            //double networkMembersPortion = Math.Round((double)networkPortion / numberOfCollaborators, 2);
            double networkQoS = 0;
            
            List<WebService> collaborationNetwork = new List<WebService>();
            List<WebService> networkMembers = new List<WebService>();
            for (int i = 0; i < network.MembersIds.Count; i++)
            {
                networkMembers.Add(webserviceInfos.Find(delegate(Community.WebServiceInfo wsInfo) { return wsInfo.Webservice.Id == network.MembersIds[i]; }).Webservice);
            }
            networkMembers.RemoveAll(delegate(WebService ws) { return (ws.Id == this.id) || (ws.readyToCompete == true); });
            List<WebService> tempNetwork = networkMembers;
            int numberOfCollaborators = rnd.Next(1, networkMembers.Count);
            double networkMembersPortion = Math.Round((double)networkPortion / numberOfCollaborators, 2);
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
                                    Thread.Sleep(5);
                                    ws.providedQoS = Math.Max(0, ws.qos - (Math.Round(rnd.NextDouble(), 4) / Constants.QoSVarianceModifier));
                                    cm.Members[ws.id].NumberOfTasksDone++;
                                    ws.budget += (int)((Constants.CooperationFeePercentage / collaborationNetwork.Count) * task.Fee);
                                    ws.totalIncome += (int)((Constants.CooperationFeePercentage / collaborationNetwork.Count) * task.Fee);
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