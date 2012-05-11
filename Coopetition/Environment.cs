using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

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

        public static RichTextBox outputLog;

        public void Initialization()
        {
            // Generate tasks
            outputLog.AppendText("Generating Tasks...\n");
            for (int i = 0; i < Constants.NumberOfTasks; i++)
            {
                TaskPool.Add(new Task(i));
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
                community.AddMember(ws);
                ws.CommunityId = community.Id;
            }

            //for (int i = 0; i < Constants.NumberOfWebservices; i++)
            //{
            //    community.Members[i].Webservice.SetNetwork(community);
            //}

            Communities.Add(community);

            // Collaboration Network Initialization
            outputLog.AppendText("Initializating Collaboration Network...\n");
            InitializeNetworks();

            // Computing thresholds and probabilities

        }

        private void InitializeNetworks()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            foreach (Community community in Communities)
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
                        community.Members.Find(delegate(Community.WebServiceInfo wsInfo) { return wsInfo.Webservice.Id == wsId; }).Webservice.NetworkId = network.Id;
                    }
                    community.IntraNetworks.Add(network);
                }
            }

        }

        public void Simulation(int numberOfRun)
        {
            excel.createHeaders(row, col, "cmId", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsId", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsNetwork", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsQoS", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsGrowthFactor", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsReputation", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsNTD", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsBankAccount", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsCompeted", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsHasCollaborated", "A", "B", 2, true, 10, "n");
            excel.createHeaders(row, ++col, "wsIsCollaborated", "A", "B", 2, true, 10, "n");
           // excel.createHeaders(row, ++col, "Webservice_Network", "A", "B", 2, true, 10, "n");
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
                    wsInfo.Webservice.BankAccount -= Constants.MembershipFee;
                    // Checking growth factor by web services
                    wsInfo.Webservice.CoopetitionDecision(numberOfRun);
                    // Insert CommunityId to the excel file 
                    excel.InsertData(row + 1, col, cm.Id.ToString(), "", "", "");
                    // Insert Webservice data to the excel file
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.Id.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.NetworkId.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.QoS.ToString(), "", "", "");
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
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.BankAccount.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.ReadyToCompete.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.HasCollaborated.ToString(), "", "", "");
                    excel.InsertData(row + 1, ++col, wsInfo.Webservice.IsCollaborated.ToString(), "", "", "");
                    // excel.InsertData(row + 1, ++col, wsInfo.Webservice.Network.ToString(), "", "", "");
                    if (wsInfo.CurrentAssignedTask != null)
                    {
                        excel.InsertData(row + 1, ++col, wsInfo.CurrentAssignedTask.QoS.ToString(), "", "", "");
                        excel.InsertData(row + 1, ++col, wsInfo.CurrentAssignedTask.Fee.ToString(), "", "", "");
                        excel.InsertData(row + 1, ++col, wsInfo.Webservice.ProvidedQoS.ToString(), "", "", ""); 
                    }
                    else
                    {
                        excel.InsertData(row + 1, ++col, "", "", "", "");
                        excel.InsertData(row + 1, ++col, "", "", "", "");
                        excel.InsertData(row + 1, ++col, "", "", "", ""); 
                    }
                    row++;
                    col = currentcol;
                }

                
                                    
                // Service evaluation and reputation update by Master web service
                cm.UpdateMemberReputation();
            }
        }

        public void Run()
        {
            Initialization();
            long start = DateTime.Now.Ticks;
            for (int i = 0; i < Constants.NumberOfRuns; i++)
            {
                outputLog.AppendText("Iteration #" + i + "...\n");
                outputLog.ScrollToCaret();
                excel.CreateExcelFile();
                row = 1;
                col = 1;
                Simulation(i);
                long ticks = DateTime.Now.Ticks;
                excel.SaveDocument("coopetition_" + i.ToString() + "_" + ticks + ".xls");
            }
            long end = DateTime.Now.Ticks;
            outputLog.AppendText("Simulation took: " + (int)((end-start)/10000) + " ms\n");
            outputLog.AppendText("Done!\n");
            outputLog.ScrollToCaret();
            MessageBox.Show("Done!");
        }

    }
}
