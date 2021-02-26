using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlayAdv.Client;



namespace PlayAdv
{
    class Program
    {
        private static GameMoveResult gmr;
        private static IConfiguration configuration;
        private static string ApiUrl = "";


        public static Boolean PlayAdventure(string WelcomeTitle, string WelcomeText, string ApiUrl)
        {
            string instanceID;
            string move;
            bool error;
            string errormsg = "";
            
            var _gmr = new GameMoveResult();

            var _httpClient = new System.Net.Http.HttpClient();
            var _client = new swaggerClient(_httpClient);
            _client.BaseUrl = ApiUrl;
            
            
    

            move = ""; 

            try
            {
                Console.WriteLine();
                Console.WriteLine(WelcomeTitle);
                Console.WriteLine(WelcomeText);
                Console.WriteLine();
                Console.WriteLine("cquit = quit game");
                Console.WriteLine("");

                // default to game 1 until I we have selection system
                // Gets the first game and sets up the game 
                gmr = _client.Adventure3Async(1).GetAwaiter().GetResult();
                instanceID = gmr.InstanceID;
                error = false; 

            }
            catch (Exception e)
            {
                // oops! Looks like we had a problem starting the game. 
                errormsg = "Error: Can not create new game (" +ApiUrl+")";
                Console.WriteLine(errormsg);
                Console.WriteLine(e.ToString());
                return false;
            }

            while (move != "cquit")
            {

                switch (move)
                {
                    case "capi":
                        Console.WriteLine();
                        Console.WriteLine("Api:" + ApiUrl);
                        Console.WriteLine();
                        move = "";
                        break;
                    default:
               
                    Console.WriteLine(gmr.RoomName);
                    Console.WriteLine(gmr.RoomMessage);
                    Console.WriteLine(gmr.ItemsMessage);
                    Console.Write(">");

                        if (error == true)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Client Error:");
                            Console.WriteLine(errormsg);
                            Console.WriteLine();
                        }

                    move = Console.ReadLine();

                        try
                        { 
                            // once the game is setup by calling the get that returns the first gmr you then
                            // just pass the game move with the instance id 
                            gmr = _client.Adventure2Async(instanceID,move).GetAwaiter().GetResult();
                        }
                        catch (Exception e)
                        {
                            error = true;
                            errormsg = "Error: Can not Process Move - Possible Timeout. Try move again or LOOK.";
                            gmr.RoomMessage = errormsg; // report the error ro the user; 

                        }
                        break; 


                }

            }
            
            if (error)
            {
                Console.WriteLine(errormsg); 
            }

            return error; 
        }

        public static bool HelpCheck(string[] args)
        {
            if (args.Where(i => i.Contains("help")).Count() > 0) return true;
            if (args.Count() == 0) return true;
            return false; 
        }

        public static bool ArgCheck(string key, string[] args)
        {
            if (args.Where(i => i.Contains(key)).Count() > 0) return true;
            return false;
        }

        public static string GetArgValue(string key, string[] args)
        {
            int index = 0;
            foreach (string arg in args)
            {
                if (arg == null) return "";
                
                if (arg.Contains(key))
                {
                    if (args.Count() <= index) return "";
                    var value = args[index+1];
                    return value; 
                }

                index++;
            }

            return "";

        }

        public static void DisplayHelp(string title, string welcome, string url)
        {
            Console.WriteLine(title);
            Console.WriteLine(welcome);
            Console.WriteLine();
            Console.WriteLine("Play Adventure Help");
            Console.WriteLine("--help                         This message.");
            Console.WriteLine("--url [url]                    URL for Adventure API.");
            Console.WriteLine("--play                         Atempts to play game at default url: " + url);
            Console.WriteLine("--timeout [value in seconds]   Sets value for the API call timeout. Min is 1 & Max is 60" );
            Console.WriteLine();

        }

        static void Main(string[] args)
        {
            string WelcomeTitle;
            string WelcomeText;
            string ClientTimeOut;

            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                var _configuration = builder.Build();
                configuration = _configuration;

                WelcomeTitle = configuration["WelcomeTitle"];
                WelcomeText = configuration["WelcomeText"];
                ApiUrl = configuration["ApiUrl"];
                ClientTimeOut = configuration["ClientTimeOut"];


            }
            catch (Exception)
            {
                Console.WriteLine("appsettings.json missing items using default values");

                WelcomeTitle = "API Adventure";
                WelcomeText = "Client for the API Adventure api use playadv.exe --help";
                ApiUrl = "https://adventureserver.azurewebsites.net";
                ClientTimeOut = "10";

            }

            try
            {

             
                if (HelpCheck(args))
                {
                    DisplayHelp(WelcomeTitle, WelcomeText, ApiUrl);
                    return;
                }

                if (GetArgValue("--timeout", args) == "")
                {
                    // do nothing use default
                }
                else
                {
                    var tov = GetArgValue("--timeout", args);
                    var tv = Convert.ToInt32(tov);
                    if (tv > 60) tv = 60;
                    if (tv < 1) tv = 10;

                }

                if (ArgCheck("--play", args))
                {
                    List<string> items = new List<string>
                    {
                        "--url",
                        ApiUrl
                    };

                    args = items.ToArray();
                }


                if (GetArgValue("--url", args) == "")
                {
                    DisplayHelp(WelcomeTitle, WelcomeText, ApiUrl);
                    return;
                }
                else
                {
                    ApiUrl = GetArgValue("--url", args);

                  
                    bool urlcheckresult = Uri.TryCreate(ApiUrl, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if (urlcheckresult == false)
                    {
                        Console.WriteLine("Error: " + ApiUrl + " is not a valid url.");
                        Console.WriteLine();
                        return;
                    }
                }

                bool x = PlayAdventure(WelcomeTitle, WelcomeText, ApiUrl);

            }
            catch (Exception)
            {
                Console.WriteLine("Error Starting Game at " + ApiUrl);
            }


        }
    }
}
