using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;



namespace PlayAdv
{
    class Program
    {
        private static PlayAdv.GameMoveResult gmr;

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

        public static Boolean PlayAdventure(string URL)
        {
            Int64 InstanceID = new Int64(); 
            bool error = false;
            string errormsg = "";
            AdventureClient client = new AdventureClient();
            GameMove gm = new GameMove();
            client.BaseUrl = URL;
            Task T;

            string move; 

            try
            {
                T = NewGame(client);
                InstanceID = gmr.InstanceID;
                error = false; 
            }
            catch (Exception)
            {
                error = true;
                errormsg = "Error: Can not create new game;";
            }

            string cmd = "";
            while (cmd != "cquit")
            {

                Console.WriteLine(gmr.RoomName);
                Console.WriteLine(gmr.RoomMessage);
                Console.WriteLine(gmr.ItemsMessage);

                move = Console.ReadLine();

                try
                {
                    gm.Move = move;
                    gm.InstanceID = InstanceID;
                    T = GameMove(client, gm);
                    error = false;
                }
                catch (Exception)
                {
                    error = true;
                    errormsg = "Error: Can not Process Move;";
                }


            }
            
            if (error)
            {
                Console.WriteLine(errormsg); 
            }

            return error; 
        }


        static void Main(string[] args)
        {
            string WelcomeTitle = "";
            string WelcomeText = ""; 
            string ApiUrl = "http://localhost:5000";

            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                var configuration = builder.Build();

                WelcomeTitle = configuration["WelcomeTitle"];
                WelcomeText = configuration["WelcomeText"];
                ApiUrl = configuration["ApiUrl"];

                Console.WriteLine(WelcomeTitle);
                Console.WriteLine(WelcomeText);
                Console.WriteLine("Api:" + ApiUrl);
                Console.WriteLine("");

 
            }
            catch
            {
                Console.WriteLine("Error: appsettings.json error.");
            }

            bool x = PlayAdventure(ApiUrl);

            var r = Console.ReadKey();

        }
    }
}
