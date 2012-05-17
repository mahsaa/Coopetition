using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coopetition
{
    public class DataSetWebService
    {

        private double responseTime;
        private double availability;
        private double throughput;
        private double successability;
        private double reliability;
        private double compliance;
        private double bestPractices;
        private double latency;
        private double documentation;
        private String serviceName;
        private String wSDLAddress;
        private double qos;


        public double ResponseTime
        {
            get { return responseTime; }
            set { responseTime = value; }
        }

        public double Availability
        {
            get { return availability; }
            set { availability = value; }
        }

        public double Throughput
        {
            get { return throughput; }
            set { throughput = value; }
        }

        public double Successability
        {
            get { return successability; }
            set { successability = value; }
        }

        public double Reliability
        {
            get { return reliability; }
            set { reliability = value; }
        }

        public double Compliance
        {
            get { return compliance; }
            set { compliance = value; }
        }

        public double BestPractices
        {
            get { return bestPractices; }
            set { bestPractices = value; }
        }

        public double Latency
        {
            get { return latency; }
            set { latency = value; }
        }

        public double Documentation
        {
            get { return documentation; }
            set { documentation = value; }
        }

        public String ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }

        public String WSDLAddress
        {
            get { return wSDLAddress; }
            set { wSDLAddress = value; }
        }

        public double QoS
        {
            get { return qos; }
            set { qos = value; }
        }


    }
}
