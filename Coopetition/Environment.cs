using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Collections;

namespace Coopetition
{
    public class Environment
    {
        public static List<Task> TaskPool = new List<Task>(Constants.NumberOfTasks);
        public static List<Community> Communities = new List<Community>(1);
        public static List<WebService> WebServices = new List<WebService>(Constants.NumberOfWebservices);
        public static ExcelManipulation excel = new ExcelManipulation();
        public static int row = 1;
        public static int col = 1;

        public static DataTable dtWsInitialValues = new DataTable();
       // public static DataRow drWsInitialValues;
        public static DataColumn colCmId = new DataColumn("cmId");
        public static DataColumn colWsId = new DataColumn("wsId");
        public static DataColumn colWsNetwork = new DataColumn("wsNetwork");
        public static DataColumn colWsQoS = new DataColumn("wsQoS");
        public static DataColumn colWsType = new DataColumn("wsType");
        public static DataColumn colWsGrowthFactor = new DataColumn("wsGrowthFactor");
        public static DataColumn colWsReputation = new DataColumn("wsReputation");
       // public static DataColumn colWsNTD = new DataColumn("wsNTD");
        public static DataColumn colWsBudget = new DataColumn("wsBudget");
        public static DataColumn colWsTotalIncome = new DataColumn("wsTotalIncome");
        public static DataColumn colWsCompeted = new DataColumn("wsCompeted");
       // public static DataColumn colWsHasCollaborated = new DataColumn("wsHasCollaborated");
       // public static DataColumn colWsIsCollaborated = new DataColumn("wsIsCollaborated");
       // public static DataColumn colWsTaskQoS = new DataColumn("wsTaskQoS");
       // public static DataColumn colWsTaskFee = new DataColumn("wsTaskFee");
      //  public static DataColumn colWsProvidedQoS = new DataColumn("wsProvidedQoS");

        public static DataTable dtTaskInitialValues = new DataTable();
       // public static DataRow drTaskInitialValues;
        public static DataColumn colTaskId = new DataColumn("taskId");
        public static DataColumn colTaskQoS = new DataColumn("taskQoS");
        public static DataColumn colTaskFee = new DataColumn("taskFee");
        public static DataColumn colTaskAssigned = new DataColumn("taskAssigned");

        public static List<DataSetWebService> dataSet;

        public static RichTextBox outputLog;

        public void CreateInitializationTables()
        {
           // dtWsInitialValues = new DataTable();
          //  DataRow drWsInitialValues = dtWsInitialValues.NewRow();
            dtWsInitialValues.Columns.Add(colCmId);
            dtWsInitialValues.Columns.Add(colWsId);
            dtWsInitialValues.Columns.Add(colWsNetwork);
            dtWsInitialValues.Columns.Add(colWsQoS);
            dtWsInitialValues.Columns.Add(colWsType);
            dtWsInitialValues.Columns.Add(colWsGrowthFactor);
            dtWsInitialValues.Columns.Add(colWsReputation);
           // dtWsInitialValues.Columns.Add(colWsNTD);
            dtWsInitialValues.Columns.Add(colWsBudget);
            dtWsInitialValues.Columns.Add(colWsTotalIncome);
            dtWsInitialValues.Columns.Add(colWsCompeted);
           // dtWsInitialValues.Columns.Add(colWsHasCollaborated);
           // dtWsInitialValues.Columns.Add(colWsIsCollaborated);
           //  dtWsInitialValues.Columns.Add(colWsTaskQoS);
           // dtWsInitialValues.Columns.Add(colWsTaskFee);
           // dtWsInitialValues.Columns.Add(colWsProvidedQoS);

           // dtTaskInitialValues = new DataTable();
           // DataRow drTaskInitialValues = dtTaskInitialValues.NewRow();
            dtTaskInitialValues.Columns.Add(colTaskId);
            dtTaskInitialValues.Columns.Add(colTaskQoS);
            dtTaskInitialValues.Columns.Add(colTaskFee);
            dtTaskInitialValues.Columns.Add(colTaskAssigned);
        }

        public void FillTaskTable(int i, Task task)
        {
            DataRow drTaskInitialValues = dtTaskInitialValues.NewRow();
            drTaskInitialValues[colTaskId] = task.Id;
            drTaskInitialValues[colTaskQoS] = task.QoS;
            drTaskInitialValues[colTaskFee] = task.Fee;
            drTaskInitialValues[colTaskAssigned] = task.Assigned;
            dtTaskInitialValues.Rows.Add(drTaskInitialValues);
        }

        public void FillWsTable(int i, WebService ws)
        {
            DataRow drWsInitialValues = dtWsInitialValues.NewRow();
            drWsInitialValues[colCmId] = ws.CommunityId;
            drWsInitialValues[colWsId] = ws.Id;
            drWsInitialValues[colWsNetwork] = ws.NetworkId;
            drWsInitialValues[colWsQoS] = ws.QoS;
            drWsInitialValues[colWsType] = ws.Type.ToString();
            drWsInitialValues[colWsGrowthFactor] = ws.GrowthFactor;
            drWsInitialValues[colWsReputation] = ws.Reputation;
           // dtWsInitialValues.Rows[i][colWsNTD] = 0;
            drWsInitialValues[colWsBudget] = ws.Budget;
            drWsInitialValues[colWsTotalIncome] = ws.TotalIncome;
            drWsInitialValues[colWsCompeted] = ws.ReadyToCompete;
            drWsInitialValues[colCmId] = ws.CommunityId;
            dtWsInitialValues.Rows.Add(drWsInitialValues);
        }

        public void Initialization(Constants.SimulationType simulationType)
        {
            CreateInitializationTables();

            DataSetTools dsTools = new DataSetTools();
            dataSet = dsTools.parseDateSet("c:\\projects\\QWS_Dataset_v2.txt");

            outputLog.AppendText("/*********************** Strategy: " + simulationType.ToString() + " ***********************/\n");

            // Generate tasks
            outputLog.AppendText("Generating Tasks...\n");
            for (int i = 0; i < Constants.NumberOfTasks; i++)
            {
                Task task = new Task(i);
                TaskPool.Add(task);
                FillTaskTable(i, task);
            }

            // Generate communities
            outputLog.AppendText("Generating Communities...\n");
            Community community = new Community(1);
            community.TaskPool = TaskPool;

            // Generate web services
            outputLog.AppendText("Generating " + Constants.NumberOfWebservices + " of Web Services...\n");

            for (int i = 0; i < Constants.NumberOfWebservices; i++)
            {
                WebService ws = new WebService(i);

                switch (simulationType)
	            {
		            case Constants.SimulationType.AllCompetitive:
                        ws.Type = Constants.WebserviceType.JustCompetitive;
                     break;
                    case Constants.SimulationType.AllRandom:
                        ws.Type = Constants.WebserviceType.Random;
                     break;
                    case Constants.SimulationType.Coopetitive:
                        ws.Type = Constants.WebserviceType.Coopetitive;
                     break;
	            }

                community.AddMember(ws);
                ws.CommunityId = community.Id;
                FillWsTable(i, ws);
            }

            Communities.Add(community);

            // Collaboration Network Initialization
            outputLog.AppendText("Initializating Collaboration Network...\n");
            InitializeNetworks(simulationType);

            // Computing thresholds and probabilities

        }

        private void InitializeUsingDataTables(Constants.SimulationType simulationType)
        {
            outputLog.AppendText("/*********************** Strategy: " + simulationType.ToString() + " ***********************/\n");

            // Generate tasks
            TaskPool.Clear();
            outputLog.AppendText("Generating Tasks...\n");
            foreach (DataRow drTask in dtTaskInitialValues.Rows)
            {
                Task task = new Task();
                task.Id = Int32.Parse(drTask[colTaskId].ToString());
                task.QoS = double.Parse(drTask[colTaskQoS].ToString());
                task.Fee = Int32.Parse(drTask[colTaskFee].ToString());
                task.Assigned = Boolean.Parse(drTask[colTaskAssigned].ToString());
                TaskPool.Add(task);
            }

            // Generate communities
            outputLog.AppendText("Generating Communities...\n");
            Communities.RemoveAt(0);
            Community community = new Community(1);
            community.TaskPool = TaskPool;

            // Generate web services
            outputLog.AppendText("Generating " + Constants.NumberOfWebservices + " of Web Services...\n");

            community.Members.Clear();
            community.IntraNetworks.Clear();
            foreach (DataRow drWs in dtWsInitialValues.Rows)
            {
                WebService ws = new WebService();
                ws.Id = Int32.Parse(drWs[colWsId].ToString());
                ws.QoS = double.Parse(drWs[colWsQoS].ToString());
                ws.GrowthFactor = double.Parse(drWs[colWsGrowthFactor].ToString());
                ws.Reputation = double.Parse(drWs[colWsReputation].ToString());
                ws.Budget = Int32.Parse(drWs[colWsBudget].ToString());
                switch (simulationType)
                {
                    case Constants.SimulationType.AllCompetitive:
                        ws.Type = Constants.WebserviceType.JustCompetitive;
                        break;
                    case Constants.SimulationType.AllRandom:
                        ws.Type = Constants.WebserviceType.Random;
                        break;
                    case Constants.SimulationType.Coopetitive:
                        ws.Type = Constants.WebserviceType.Coopetitive;
                        break;
                }
                ws.CommunityId = Int32.Parse(drWs[colCmId].ToString());

                ws.NetworkId = Int32.Parse(drWs[colWsNetwork].ToString());

                community.AddMember(ws);
            }

            Communities.Add(community);

            // Collaboration Network Initialization
            outputLog.AppendText("Initializating Collaboration Network...\n");
            InitializeNetworksUsingDataTable();
        }

        private void InitializeNetworks(Constants.SimulationType simulationType)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            foreach (Community community in Communities)
            {
                if ((simulationType == Constants.SimulationType.Coopetitive || simulationType == Constants.SimulationType.AllRandom))
                {
                    int NetworkSize = (community.Members.Count - 3) / Constants.NumberOfNetworksInCommunity;
                    Environment.outputLog.AppendText("NetworkSize: " + NetworkSize + "\n");
                    Environment.outputLog.AppendText("Member Count: " + community.Members.Count + "\n");
                    for (int i = 0; i < Constants.NumberOfNetworksInCommunity; i++)
                    {
                        CollaborationNetwork network = new CollaborationNetwork(i, community.Id);
                        for (int j = 0; j < NetworkSize; j++)
                        {
                            int wsId = rnd.Next(0, community.Members.Count);
                            while (community.Members[wsId].Webservice.NetworkId != -1)
                            {
                                Thread.Sleep(10);
                                wsId = rnd.Next(0, community.Members.Count);
                            }
                            network.MembersIds.Add(wsId);
                            WebService ws = community.Members.Find(delegate(Community.WebServiceInfo wsInfo) { return wsInfo.Webservice.Id == wsId; }).Webservice;
                            ws.NetworkId = network.Id;
                            dtWsInitialValues.Rows[ws.Id][colWsNetwork] = ws.NetworkId;
                        }
                        community.IntraNetworks.Add(network);
                    } 
                }
                else
                {
                    foreach (Community.WebServiceInfo wsInfo in community.Members)
                    {
                        // For all competitve members, there is no need to have intra networks
                        wsInfo.Webservice.NetworkId = -1; 
                    }
                }
            }

        }

        private void InitializeNetworksUsingDataTable()
        {
            foreach (Community community in Communities)
            {
                int NetworkSize = (community.Members.Count - 3) / Constants.NumberOfNetworksInCommunity;
                Environment.outputLog.AppendText("NetworkSize: " + NetworkSize + "\n");
                Environment.outputLog.AppendText("Member Count: " + community.Members.Count + "\n");

                ArrayList intraNetworkIds = new ArrayList();

                foreach (Community.WebServiceInfo wsInfo in community.Members)
                {
                    WebService wservice = wsInfo.Webservice;
                    CollaborationNetwork network = new CollaborationNetwork(wservice.NetworkId, community.Id);
                    if ((!intraNetworkIds.Contains(network.Id)) && (network.Id != -1))
                    {
                        List<Community.WebServiceInfo> networkMembers = community.Members.FindAll(delegate(Community.WebServiceInfo wInfo) { return wInfo.Webservice.NetworkId == wservice.NetworkId; });
                        networkMembers.ForEach(delegate(Community.WebServiceInfo wInfo) { network.MembersIds.Add(wInfo.Webservice.Id); });
                        community.IntraNetworks.Add(network);
                        intraNetworkIds.Add(network.Id);
                    }

                }
            }

        }

        public void Simulation(int numberOfRun)
        {
            excel.createHeaders(row, col, "cmId", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsId", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsNetwork", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsQoS", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsType", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsGrowthFactor", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsReputation", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsNTD", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsBudget", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsTotalIncome", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsCompeted", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsHasCollaborated", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsIsCollaborated", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsTaskQoS", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsTaskFee", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsProvidedQoS", "A", "B", 2, true, 10, "n");

            for (int j = 0; j < Communities.Count; j++)
            {
                Community cm = Communities[j];
                col = 1;
                int currentrow = row;
                int currentcol = col;

                for (int i = 0; i < cm.Members.Count; i++)
                {
                    Community.WebServiceInfo wsInfo = cm.Members[i];
                    wsInfo.Webservice.Budget -= Constants.MembershipFee;
                    // Checking growth factor by web services
                    wsInfo.Webservice.CoopetitionDecision(wsInfo.NumberOfTasksDone, numberOfRun);
                    // Insert CommunityId to the excel file 
                    excel.InsertData(row + 1, col, cm.Id.ToString(), "", "", "");
                    // Insert Webservice data to the excel file
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.Id.ToString(), "", "", "");
                    String strNetworkMembers = "";
                    if (wsInfo.Webservice.NetworkId != -1)
                    {
                        CollaborationNetwork net = cm.IntraNetworks.Find(delegate(CollaborationNetwork nw) { return nw.Id == wsInfo.Webservice.NetworkId; });
                        foreach (int memberid in net.MembersIds)
                        {
                            strNetworkMembers += memberid + ", ";
                        }
                    }
                    char[] charsToRemove = ", ".ToCharArray();
                    strNetworkMembers = strNetworkMembers.TrimEnd(charsToRemove);
                    excel.InsertData(row + 1, ++col, strNetworkMembers, "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.QoS.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.Type.ToString(), "", "", ""); 
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.GrowthFactor.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.Reputation.ToString(), "", "", "");
                    row++;
                    currentcol = col;
                    col = 1;
                }
                                    
                // Task allocation to competitive web services
                cm.OfferTaskToWebservice();

                row = currentrow;
                col = currentcol;
                for (int i = 0; i < cm.Members.Count; i++)
                {
                    Community.WebServiceInfo wsInfo = cm.Members[i];
                    // Doing the tasks by web services
                    wsInfo.Webservice.StartDoingTask(wsInfo.CurrentAssignedTask, cm);
                    // Insert Webservice data to the excel file
                    excel.InsertData(row + 1, ++col, wsInfo.NumberOfTasksDone.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.Budget.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.TotalIncome.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.ReadyToCompete.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.HasCollaborated.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.IsCollaborated.ToString(), "", "", "");
                    if (wsInfo.CurrentAssignedTask != null)
                    {
                        excel.InsertData(row + 1, ++col, wsInfo.CurrentAssignedTask.QoS.ToString(), "", "", "");
                        excel.InsertData(row + 1, ++col, wsInfo.CurrentAssignedTask.Fee.ToString(), "", "", "");
                    }
                    else
                    {
                        excel.InsertData(row + 1, ++col, "", "", "", "");
                        excel.InsertData(row + 1, ++col, "", "", "", "");
                    }
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.ProvidedQoS.ToString(), "", "", ""); 
                    row++;
                    col = currentcol;
                }

                // Service evaluation and reputation update by Master web service
                cm.UpdateMemberReputation();
            }
        }

        private void ReleaseTasks()
        {
            for (int j = 0; j < Communities.Count; j++)
            {
                Community cm = Communities[j];
                foreach (Community.WebServiceInfo wsInfo in cm.Members)
                {
                    wsInfo.CurrentAssignedTask = null;
                    wsInfo.CurrentIfAcceptedTask = false;
                    wsInfo.CurrentIfOfferedTask = false;
                }
            }
        }

        public void Run(Constants.SimulationType simulationType)
        {
            if (dtWsInitialValues.Rows.Count == 0)
            {
                Initialization(simulationType);
            }
            else
            {
                InitializeUsingDataTables(simulationType);
            }

            long start = DateTime.Now.Ticks;
            for (int i = 0; i < Constants.NumberOfRuns; i++)
            {
                outputLog.AppendText("Iteration #" + i + "...\n");
                outputLog.ScrollToCaret();
                excel.CreateExcelFile();
                row = 1;
                col = 1;
                Simulation(i + 1);
                long ticks = DateTime.Now.Ticks;
                excel.SaveDocument("coopetition_" + i.ToString() + "_" + ticks + ".xls");
                ReleaseTasks();
            }
            long end = DateTime.Now.Ticks;
            outputLog.AppendText("Simulation took: " + (int)((end-start)/10000) + " ms\n");
            outputLog.AppendText("Done!\n");
            outputLog.ScrollToCaret();
            MessageBox.Show("Done!");
        }

    }
}
