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

        const string DevBy = "Developed by";
        readonly static string SteveSparks = "Steve Sparks";
        readonly static string RepoName = "GitHub";
        readonly static string RepoURL = "https://github.com/StevenSSparks";
        readonly static string WelcomeTitle = "Adventure Sever Client";
        static string ApiUrl { get; set; }  = new string("https://advsrv.azurewebsites.net");
        const string UNDERLINE = "\x1B[4m";
        const string RESET = "\x1B[0m";


        public static Boolean PlayAdventure()
        {
            string instanceID;
            string move;
            bool error;
            string errormsg = "";
            
            

            var _httpClient = new System.Net.Http.HttpClient();
            var _client = new swaggerClient(_httpClient)
            {
                BaseUrl = ApiUrl
            };



            move = ""; 

            try
            {
                DisplayIntro();
                Console.WriteLine();
                SetColor(ConsoleColor.Yellow);
                Console.WriteLine("cquit = quit game and capi = Display API endpoint");
                SetColor(ConsoleColor.Green);
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
                SetColor(ConsoleColor.Red);
                errormsg = "Error: Can not create new game (" +ApiUrl+")";
                Console.WriteLine(errormsg);
                Console.WriteLine(e.ToString());
                SetColor(ConsoleColor.Green);
                return false;
            }

            while (move != "cquit")
            {

                switch (move)
                {
                    case "capi":
                        Console.WriteLine();
                        SetColor(ConsoleColor.Yellow);
                        Console.WriteLine("Api:" + ApiUrl);
                        SetColor(ConsoleColor.Green);
                        Console.WriteLine();
                        move = "";
                        break;
                    default:

                        Console.WriteLine();
                        SetColor(ConsoleColor.Yellow);
                        Console.Write(gmr.RoomName);
                        Console.WriteLine();
                        SetColor(ConsoleColor.Green);
                        Console.WriteLine(gmr.RoomMessage);
                        SetColor(ConsoleColor.DarkCyan);
                        Console.WriteLine(gmr.ItemsMessage);
                        Console.WriteLine();
                        SetColor(ConsoleColor.White);
                        Console.Write("What now?"); SetColor(ConsoleColor.Green); Console.Write(" >"); SetColor(ConsoleColor.Green);

                        if (error == true)
                        {
                            Console.WriteLine();
                            SetColor(ConsoleColor.Red);
                            Console.WriteLine("Client Error:");
                            Console.WriteLine(errormsg);
                            SetColor(ConsoleColor.Green);
                            Console.WriteLine();
                        }

                    move = Console.ReadLine();

                        try
                        { 
                            // once the game is setup by calling the get that returns the first gmr you then
                            // just pass the game move with the instance id 
                            gmr = _client.Adventure2Async(instanceID,move).GetAwaiter().GetResult();
                        }
                        catch (Exception)
                        {
                            error = true;
                            SetColor(ConsoleColor.Red);
                            errormsg = "Error: Can not Process Move - Possible Timeout. Try move again or LOOK.";
                            gmr.RoomMessage = errormsg; // report the error ro the user; 
                            SetColor(ConsoleColor.Green);

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


        public static bool ArgCheck(string key, string[] args)
        {
            if (args.Where(i => i.Contains(key)).Any()) return true;
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
                    if (args.Length <= index) return "";
                    var value = args[index+1];
                    return value; 
                }

                index++;
            }

            return "";

        }

        public static void DisplayIntro()
        {
            Console.WriteLine();
            SetColor(ConsoleColor.DarkBlue);
            Console.WriteLine(WelcomeTitle.ToUpper());
            SetColor(ConsoleColor.White); Console.Write(DevBy + " "); SetColor(ConsoleColor.Red); Console.WriteLine(SteveSparks);
            SetColor(ConsoleColor.White);
            Console.Write("Find out more on "); SetColor(ConsoleColor.Green); Console.Write(RepoName); SetColor(ConsoleColor.White); Console.Write(" at "); SetColor(ConsoleColor.Blue); Console.Write(RepoURL);
            Console.WriteLine();
        }



        public static void DisplayHelp()
        {
            Console.WriteLine();
            DisplayIntro();
            Console.WriteLine();
            SetColor(ConsoleColor.Yellow);  
            Console.WriteLine("Play Adventure Help");
            SetColor(ConsoleColor.Green); Console.WriteLine("--url [url]                    URL for Adventure API.");
            SetColor(ConsoleColor.Green); Console.WriteLine("--play                         Atempts to play game at default url: " + ApiUrl);
            Console.WriteLine();
            SetColor(ConsoleColor.Yellow); Console.WriteLine("Example: PlayAdv.exe --url https://advsrv.azurewebsites.net");
            SetColor(ConsoleColor.Yellow); Console.WriteLine("Example: PlayAdv.exe --url http://localhost:5001");
            SetColor(ConsoleColor.Yellow); Console.WriteLine("Example: PlayAdv.exe --play");
            SetColor(ConsoleColor.White);
            Console.WriteLine();
            Console.WriteLine();

        }

        static void SetColor(ConsoleColor consoleWordColor)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = consoleWordColor;
        }
            

        static void Main(string[] args)
        {


            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                var _configuration = builder.Build();
                configuration = _configuration;


                ApiUrl = configuration["ApiUrl"].ToString();


            }
            catch (Exception)
            {
              
                Console.WriteLine();
                SetColor(ConsoleColor.Red);
                Console.WriteLine("Optional: appsettings.json missing items using default values");
                SetColor(ConsoleColor.Yellow);
                Console.WriteLine("Example: appsettings.json");
                Console.WriteLine("{");
                Console.WriteLine("   \"ApiURL\": \"http://https://advsrv.azurewebsites.net/\"");
                Console.WriteLine("}");
                SetColor(ConsoleColor.Green);
            }
  

            try
            {


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
                    DisplayHelp();
                    return;
                }
                else
                {
                    ApiUrl = GetArgValue("--url", args);

                  
                    bool urlcheckresult = Uri.TryCreate(ApiUrl, UriKind.Absolute, out Uri uriResult)
                        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                    if (urlcheckresult == false)
                    {
                        SetColor(ConsoleColor.Red);
                        Console.WriteLine("Error: " + ApiUrl + " is not a valid url.");
                        Console.WriteLine();
                        SetColor(ConsoleColor.Green);
                        return;
                    }
                }

                bool x = PlayAdventure();

            }
            catch (Exception)
            {
                SetColor(ConsoleColor.Yellow);
                Console.WriteLine("Error Starting Game at " + ApiUrl);
                SetColor(ConsoleColor.Green);
            }


        }
    }
}
