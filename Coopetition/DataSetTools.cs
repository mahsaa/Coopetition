using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Coopetition
{
    class DataSetTools
    {

        private const int FEATURE_NUMBER = 9;

        List<DataSetWebService> dataSet = null;
        double[] max = new double[FEATURE_NUMBER];
        double[] min = new double[FEATURE_NUMBER];

        public DataSetTools()
        {
            for (int i = 0; i < FEATURE_NUMBER; i++)
                max[i] = -10000000;
            for (int i = 0; i < FEATURE_NUMBER; i++)
                min[i] = 10000000;
        }

        public List<DataSetWebService> parseDateSet(String fileName)
        {

            dataSet = new List<DataSetWebService>();
            int count = 0;

            StreamReader FileStreamReader;
            FileStreamReader = File.OpenText(fileName);

            while (FileStreamReader.Peek() != -1)
            {
                String line = FileStreamReader.ReadLine();
                if (line.StartsWith("#")) continue;                

                string[] words = line.Split(',');

                if (words.Length > 10)
                {
                    count++;
                    DataSetWebService dsWebService = new DataSetWebService();

                    dsWebService.ResponseTime = double.Parse(words[0]);
                    dsWebService.Availability = double.Parse(words[1]);
                    dsWebService.Throughput = double.Parse(words[2]);
                    dsWebService.Successability = double.Parse(words[3]);
                    dsWebService.Reliability = double.Parse(words[4]);
                    dsWebService.Compliance = double.Parse(words[5]);
                    dsWebService.BestPractices = double.Parse(words[6]);
                    dsWebService.Latency = double.Parse(words[7]);
                    dsWebService.Documentation = double.Parse(words[8]);
                    dsWebService.ServiceName = words[9];
                    dsWebService.WSDLAddress = words[10];

                    if (dsWebService.ResponseTime > max[0])
                        max[0] = dsWebService.ResponseTime;
                    if (dsWebService.ResponseTime < min[0])
                        min[0] = dsWebService.ResponseTime;
                    if (dsWebService.Availability > max[1])
                        max[1] = dsWebService.Availability;
                    if (dsWebService.Availability < min[1])
                        min[1] = dsWebService.Availability;
                    if (dsWebService.Throughput > max[2])
                        max[2] = dsWebService.Throughput;
                    if (dsWebService.Throughput < min[2])
                        min[2] = dsWebService.Throughput;
                    if (dsWebService.Successability > max[3])
                        max[3] = dsWebService.Successability;
                    if (dsWebService.Successability < min[3])
                        min[3] = dsWebService.Successability;
                    if (dsWebService.Reliability > max[4])
                        max[4] = dsWebService.Reliability;
                    if (dsWebService.Reliability < min[4])
                        min[4] = dsWebService.Reliability;
                    if (dsWebService.Compliance > max[5])
                        max[5] = dsWebService.Compliance;
                    if (dsWebService.Compliance < min[5])
                        min[5] = dsWebService.Compliance;
                    if (dsWebService.BestPractices > max[6])
                        max[6] = dsWebService.BestPractices;
                    if (dsWebService.BestPractices < min[6])
                        min[6] = dsWebService.BestPractices;
                    if (dsWebService.Latency > max[7])
                        max[7] = dsWebService.Latency;
                    if (dsWebService.Latency < min[7])
                        min[7] = dsWebService.Latency;
                    if (dsWebService.Documentation > max[8])
                        max[8] = dsWebService.Documentation;
                    if (dsWebService.Documentation < min[8])
                        min[8] = dsWebService.Documentation;

                    dataSet.Add(dsWebService);
                }
            }

            Environment.outputLog.AppendText("Parsed " + count + " Web Services from file " + fileName + "\n");

            FileStreamReader.Close();
            return getAllWebServicesWithQoS();       
        }

        public double calculateQoSofOneWebService(DataSetWebService dsWebService)
        {
            double[] qos = new double[FEATURE_NUMBER];

            qos[0] = (dsWebService.ResponseTime - min[0]) / (max[0] - min[0]);
            qos[0] = 1 - qos[0];
            qos[1] = (dsWebService.Availability - min[1]) / (max[1] - min[1]);
            qos[2] = (dsWebService.Throughput - min[2]) / (max[2] - min[2]);
            qos[3] = (dsWebService.Successability - min[3]) / (max[3] - min[3]);
            qos[4] = (dsWebService.Reliability - min[4]) / (max[4] - min[4]);
            qos[5] = (dsWebService.Compliance - min[5]) / (max[5] - min[5]);
            //qos[5] = 1 - qos[5];
            qos[6] = (dsWebService.BestPractices - min[6]) / (max[6] - min[6]);
            qos[7] = (dsWebService.Latency - min[7]) / (max[7] - min[7]);
            qos[7] = 1 - qos[7];
            qos[8] = (dsWebService.Documentation - min[8]) / (max[8] - min[8]);

            double average = Math.Round((qos[0] + qos[1] + qos[2] + qos[3] + qos[4] + qos[5] + qos[6] + qos[7] + qos[8]) / 9, 4);
            return average;
        }

        public List<DataSetWebService> getAllWebServicesWithQoS () 
        {
            List<DataSetWebService> dataSetQoS = new List<DataSetWebService>();
            foreach (DataSetWebService dsWebService in dataSet) {
                dsWebService.QoS = calculateQoSofOneWebService(dsWebService);
                dataSetQoS.Add(dsWebService);
            }
            return dataSetQoS;
        }
    }   

}
