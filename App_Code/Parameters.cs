using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Keeps parameters and their initial values
/// </summary>
public static class Parameters
{
    /****************** Application ******************/
    
    public static string BaseDir = @"D:\Projects\Coopetition\";
    public const int WebserviceCount = 50;
    public const int CommunityCount = 10;
    public const int UserCount = 1000;
    public const int IterationCount = 100;
    
    /*************************************************/

    /****************** Web Service ******************/

    public const double wsReputation_LowerBound = 0.01;
    public const double wsReputation_UpperBound = 0.99;
    public const double wsQoS_LowerBound = 0.01;
    public const double wsQoS_UpperBound = 0.99;
    public const double wsSatisfaction = 0.5;

    /*************************************************/

    /******************* Community *******************/

    public const double cmReputation_LowerBound = 0.01;
    public const double cmReputation_UpperBound = 0.99;
    public const double cmQoS_LowerBound = 0.01;
    public const double cmQoS_UpperBound = 0.99; 
    public const double cmSatisfaction = 0.5;

    /*************************************************/

    /********************* User **********************/

    public const double usrSatisfaction = 0.5;

    /*************************************************/

}
