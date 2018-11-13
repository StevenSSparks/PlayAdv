using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;



namespace PlayAdv
{
    class Program
    {
        private static PlayAdv.GameMoveResult gmr;
        private static IConfiguration configuration;
        private static string ApiUrl = "";
        private static int TaskTimeOut = 10 * 1000; 


        static private async Task NewGame(AdventureClient client)
        {
            PlayAdv.GameMoveResult _gmr; 
            _gmr = await client.NewGameAsync();
            gmr = _gmr;
        }

        private static async Task GameMove(AdventureClient client, GameMove gm)
        {
            PlayAdv.GameMoveResult _gmr;
            _gmr = await client.GameMoveAsync(gm);
            gmr = _gmr;
        }



        public static Boolean PlayAdventure(string ApiUrl)
        {
            Int64 InstanceID = new Int64(); 
            bool error = false;
            string errormsg = "";

            AdventureClient client = new AdventureClient();

            GameMove gm = new GameMove();
            client.BaseUrl = ApiUrl;
            Task T;

            string move = ""; 

            try
            {
                T = NewGame(client); // create a new instance of the game. 
                T.Wait(TaskTimeOut); // wait up to the length of the timeout for the instance to be created. 
                InstanceID = gmr.InstanceID; 
                error = false; 
            }
            catch (Exception)
            {
                // oops! Looks like we had a problem starting the game. 
                error = true;
                errormsg = "Error: Can not create new game; Try RESTART ";
                gmr.RoomMessage = errormsg; // report the error ro the user; 
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
                        gm.Move = move;
                        gm.InstanceID = InstanceID;
                        T = GameMove(client, gm);
                        T.Wait(TaskTimeOut);
                        error = false;
                    }
                    catch (Exception)
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

            // delete the instance of game. 
            T = client.QuitGameAsync(gm);
            T.Wait(TaskTimeOut);

            return error; 
        }


        static void Main(string[] args)
        {
            string WelcomeTitle = "";
            string WelcomeText = ""; 
            

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
                
                string _timeout = configuration["ClientTimeOut"];
                if (Convert.ToInt32(_timeout) > 0)
                {
                    TaskTimeOut = Convert.ToInt32(_timeout); 
                    TaskTimeOut = TaskTimeOut * 1000;
                };

                Console.WriteLine();
                Console.WriteLine(WelcomeTitle);
                Console.WriteLine(WelcomeText);
                Console.WriteLine();

            }
            catch (Exception)
            {
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("Error: appsettings.json error.");
                Console.WriteLine("Using Defaults");
                WelcomeTitle = "API Adventure";
                WelcomeText = "";
                ApiUrl = "http://adventure.eastus.azurecontainer.io";
                Console.WriteLine();
                Console.WriteLine(WelcomeTitle);
                Console.WriteLine(WelcomeText);
                Console.WriteLine("");
                Console.WriteLine("-----------------------------------------------------------------");
            }

            Console.WriteLine("cquit = quit game");
            Console.WriteLine("");

            bool x = PlayAdventure(ApiUrl);


        }
    }
}
