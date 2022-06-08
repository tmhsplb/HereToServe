using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OPIDDaily
{
    public class Config
    {
        public static int CaseManagerExpiryDuration
        {
            get
            {
                return Convert.ToInt32(ConfigurationManager.AppSettings["CaseManagerExpiryDuration"]);
            }
        }
        public static string ConnectionString
        {
            get
            {
                // The value of OpidDailyConnectionString configured on Web.config is overwritten at AppHarbor deployment time.
                return ConfigurationManager.ConnectionStrings["HereToServeConnectionString"].ToString();
            }
        }

        public static string TrainingClients
        {
            get
            {
                return ConfigurationManager.AppSettings["TrainingClients"];
            }
        }

        public static string WorkingDesktopConnectionString
        {
            get
            {
               // return ConfigurationManager.AppSettings["SQLSERVER_CONNECTION_STRING"];
                return "data source=DESKTOP-0U83VML\\SqlExpress;initial catalog=OpidDailyDB;Integrated Security=True";
            }
        }

        public static string WorkingTrainingConnectionString
        {
            get
            {
                return "Server=47fcd2d4-5aa2-492b-b84a-aeae0105f2b1.sqlserver.sequelizer.com;Database=db47fcd2d45aa2492bb84aaeae0105f2b1;User ID=ockbzcabmgsiyhzi;Password=eZspkKKWRiSE5jyNRSHFy5khu5Hv64n7Qi8aWni6BJvWvBguZdKoRx5hPj3ocoYV;";
            }
        }

        public static string WorkingStagingConnectionString
        {
            get
            {
                return "Server=729e1dc9-4b8c-486e-9d65-aabe015accd3.sqlserver.sequelizer.com;Database=db729e1dc94b8c486e9d65aabe015accd3;User ID=yadsgyjugioocuyw;Password=yTxBdAhtthhVyucQw58T8rRGSwqNetdrznZyhTHF2VyhYGy4rZmiWm2gcXRPjBoa;";
            }
        }

        public static string WorkingProductionConnectionString
        {
            get
            {
                // return "Server=57d3c1fb-7e27-4b81-a9f2-aa8f0182310e.sqlserver.sequelizer.com;Database=db57d3c1fb7e274b81a9f2aa8f0182310e;User ID=fnalfcnlfsgdzgys;Password=LBwjPwsBZk577fNWh25JqJh3AKBaQiQzibLomTwYoGJckFBgdz8dswBcuCRd3sTQ;";
                // Database for application HereToServe
                return "Server=38cef54f-3aaf-47b7-bdaa-ad8d01019adf.sqlserver.sequelizer.com;Database=db38cef54f3aaf47b7bdaaad8d01019adf;User ID=hprerfhehwozwstf;Password=TXmDfSAAwUP4qLUB7GR5ysFTfcyk7KZM5MZ5Z2iGioV3Gw2Nhe7ubXWzqAxctqCU;";
                // Database for application HTS:
               // return "Server=da70aead-a3bf-4b8c-ac97-ad8f01120a2d.sqlserver.sequelizer.com;Database=dbda70aeada3bf4b8cac97ad8f01120a2d;User ID=madtfqqdwodmmodg;Password=oK6ijz8HuaUCkuXMXFXicXgqprJeFWxnKiF6V2cccHAmYi6ESBP5T7nxCP32X3bE;";
            }
        }

        public static string SuperadminEmail
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SuperadminEmail"];
            }
        }

        public static string SuperadminPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["SuperadminPwd"];
            }
        }
 
        public static string Release
        {
            get
            {
                return ConfigurationManager.AppSettings["Release"];
            }
        }

        public static string RecentYears
        {
            get
            {
                return ConfigurationManager.AppSettings["RecentYears"];
            }
        }

        public static string AncientYears
        {
            get
            {
                return ConfigurationManager.AppSettings["AncientYears"];
            }
        }
    }
}