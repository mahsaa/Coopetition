using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for WebService
/// </summary>
public class WebService
{

    private double _QoS;
    private double _Reputation;


    public double QoS 
    {
        get { return _QoS; }
        set { _QoS = value; } 
    }

    public double Reputation 
    {
        get { return _Reputation; }
        set { _Reputation = value; } 
    }

	public WebService()
	{
	}
}
