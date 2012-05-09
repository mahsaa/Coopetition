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
        private int bankAccount;
        private int networkId = -1; // <NetworkId, Network>
        private double taskPortionDone;
        private bool readyToCompete;
        private double providedQoS;
        private bool isCollaborated = false;
        private bool hasCollaborated = false;
        private int communityId;

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

        public int BankAccount 
        {
            get { return bankAccount; }
            set { bankAccount = value; }
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

        public WebService(int _id)
        {
            this.id = _id;

            Random rnd = new Random(DateTime.Now.Millisecond);
            double q = Math.Round(rnd.NextDouble(), 2);
            //Thread.Sleep(10);
            //while (!((Constants.WebserviceQoS_LowerBound < q) && (q < Constants.WebserviceQoS_UpperBound)))
            //{
            //    q = Math.Round(rnd.NextDouble(), 2);
            //    Thread.Sleep(10);
            //}
            //qos = q;
            qos = Constants.WebserviceQoS_LowerBound + (Constants.WebserviceQoS_UpperBound - Constants.WebserviceQoS_LowerBound) * q;

            bankAccount = Constants.Webservice_DefaultBankAccount;

            double g = Math.Round(rnd.NextDouble(), 2);
            //Thread.Sleep(10);
            //while (!((Constants.WebserviceGrowthFactor_LowerBound < g) && (g < Constants.WebserviceGrowthFactor_UpperBound)))
            //{
            //    g = Math.Round(rnd.NextDouble(), 2);
            //    Thread.Sleep(10);
            //}
            //growthFactor = g;
            growthFactor = Constants.WebserviceGrowthFactor_LowerBound + (Constants.WebserviceGrowthFactor_UpperBound - Constants.WebserviceGrowthFactor_LowerBound) * g;

            double rep = Math.Round(rnd.NextDouble(), 2);
            Thread.Sleep(10);
            //while (!((Constants.WebserviceReputation_LowerBound < rep) && (rep < Constants.WebserviceReputation_UpperBound)))
            //{
            //    rep = Math.Round(rnd.NextDouble(), 2);
            //    Thread.Sleep(10);
            //}
            //reputation = rep;
            reputation = Constants.WebserviceReputation_LowerBound + (Constants.WebserviceReputation_UpperBound - Constants.WebserviceReputation_LowerBound) * rep;
            //Environment.outputLog.AppendText("rep: " + reputation + "\n");

        }

        //public void SetNetwork(Community community)
        //{
        //    Random rnd = new Random(DateTime.Now.Millisecond);
        //    int networksize = rnd.Next(Constants.CollaborationNetworkSize_LowerBound, Constants.CollaborationNetworkSize_UpperBound);
        //    for (int i = 0; i < networksize; i++)
        //    {
        //        int wsIndex = rnd.Next(0, community.Members.Count - 1);
        //        Thread.Sleep(10);
        //        if (!community.Members[wsIndex].Webservice.ReadyToCompete)
        //        {
        //            this.networkId.Add(community.Members[wsIndex].Webservice);
        //        }
        //    }
        //}

        public void CoopetitionDecision(int numberOfRun)
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
                        this.bankAccount += task.Fee; // Should be changed based on the provided QoS
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
                    this.bankAccount += task.Fee; // Should be changed based on the provided QoS
                }
            }
        }

        private void Collaborate(Task task, CollaborationNetwork network, Community cm)
        {
            List<Community.WebServiceInfo> webserviceInfos = cm.Members;
            Random rnd = new Random(DateTime.Now.Millisecond);
            double rndPortion = Math.Round(rnd.NextDouble(), 2);
            this.hasCollaborated = true;
            //while (rndPortion < Constants.MainTaskPortion)
            //{
            //    rndPortion = Math.Round(rnd.NextDouble(), 2);
            //    Thread.Sleep(10);
            //}
            this.taskPortionDone = rndPortion;
            cm.Members[this.id].NumberOfTasksDone++;
            this.bankAccount += (int)((1 - Constants.CooperationFeePercentage) * task.Fee);
            double networkPortion = 1 - rndPortion;
            int numberOfCollaborators = rnd.Next(1, network.MembersIds.Count); //rnd.Next(1, this.networkId.Count); 

            double rndQoS = Math.Max(0, this.qos - (Math.Round(rnd.NextDouble(), 2) / Constants.QoSVarianceModifier));

            //double rndQoS = Math.Round(rnd.NextDouble(), 2);
            //while (rndQoS > this.qos)
            //{
            //    rndQoS = Math.Round(rnd.NextDouble(), 2);
            //    Thread.Sleep(10);
            //}
            //this.providedQoS = rndQoS;

            double networkMembersPortion = Math.Round((double)networkPortion / numberOfCollaborators, 2);
            double networkQoS = 0;
            
            List<WebService> collaborationNetwork = new List<WebService>();
            List<WebService> networkMembers = new List<WebService>();
            for (int i = 0; i < network.MembersIds.Count; i++)
            {
                networkMembers.Add(webserviceInfos.Find(delegate(Community.WebServiceInfo wsInfo) { return wsInfo.Webservice.Id == network.MembersIds[i]; }).Webservice);
            }
            networkMembers.RemoveAll(delegate(WebService ws) { return ws.Id == this.id; });
            List<WebService> tempNetwork = networkMembers; //this.networkId;
            while (numberOfCollaborators > 0)
            {
                int j = rnd.Next(0, tempNetwork.Count);
                numberOfCollaborators--;
                ((WebService)tempNetwork[j]).isCollaborated = true;
                collaborationNetwork.Add((WebService)tempNetwork[j]);
                tempNetwork.RemoveAt(j);
            }

            collaborationNetwork.ForEach(delegate(WebService ws) 
                                { 
                                    ws.taskPortionDone = networkMembersPortion; 
                                    /* ws.StartCollaborate(); */
                                    //double rndCollabQoS = Math.Round(rnd.NextDouble(), 2);
                                    //while (rndCollabQoS > ws.qos)
                                    //{
                                    //    rndCollabQoS = Math.Round(rnd.NextDouble(), 2);
                                    //    Thread.Sleep(10);
                                    //}
                                    //ws.providedQoS = rndCollabQoS;
                                    ws.providedQoS = Math.Max(0, ws.qos - (Math.Round(rnd.NextDouble(), 2) / Constants.QoSVarianceModifier));
                                    cm.Members[ws.id].NumberOfTasksDone++;
                                    ws.bankAccount += (int)((Constants.CooperationFeePercentage / collaborationNetwork.Count) * task.Fee);
                                    networkQoS += ws.providedQoS;
                                });

            this.providedQoS = (Constants.ResponsibleWSQoSPortion * this.providedQoS) + ((1 - Constants.ResponsibleWSQoSPortion) * networkQoS);
        }

    }
}