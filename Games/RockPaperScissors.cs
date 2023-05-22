using Discord;
using Discord.Net;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using csharpi.Connection;

namespace csharpi.rockpaperscissors 
{
    public class rockgame
    {
        
        List<Tuple<SocketUser, string>> UserAnswers = new List<Tuple<SocketUser, string>>();
        List<string> TooSlow = new List<string>();
        DBConnection connection = new DBConnection();
        private int answerCount = 0;
        public int AnswerCount
        {
            get { return answerCount; }
            set { answerCount = value; }
        }
        private int statCount = 0;
        public int StatCount
        {
            get { return statCount; }
            set { statCount = value; }
        }
        private int rematchCount = 0;
        public int RematchCount
        {
            get { return rematchCount; }
            set { rematchCount = value; }
        }
        HashSet<string> validAnswerIds = new HashSet<string> { "rock-id", "paper-id", "scissors-id" };
        private bool gameActive = false;
        public bool GameActive
        {
            get { return gameActive; }
            set { gameActive = value; }
        }
        private SocketMessageComponent cp;
        public SocketMessageComponent CP
        {
            get { return cp; }
            set { cp = value; }
        }
        private string answerRPS;
        public string AnswerRPS
        {
            get { return answerRPS; }
            set { answerRPS = value; }
        }
        
        
        
        
        public async Task ButtonChooseRPS(SocketMessageComponent component)
        {
            cp = component;
            var message = "";
            // Since the custom id of each button corresponds to the answer value, 
            // we can simply use that value directly instead of using a switch statement.
            if (cp.Data.CustomId.StartsWith("rock")) {
                answerRPS = "rock";
            } else if (cp.Data.CustomId.StartsWith("paper")) {
                answerRPS = "paper";
            } else if (cp.Data.CustomId.StartsWith("scissors")) {
                answerRPS = "scissors";
            } else if (cp.Data.CustomId == "stats-id" && StatCount < 1 && RematchCount < 1) {
                StatCount++;
                Task<string> statsTask = showStats();
                string statsMessage = await statsTask;
                
                var statbuilder = new ComponentBuilder()
                    .WithButton("Rematch!", "rematch-id");
                
                await cp.UpdateAsync(x => { x.Content = statsMessage; x.Components = statbuilder.Build(); });
            } else if (cp.Data.CustomId == "rematch-id" && StatCount < 1 && RematchCount < 1) {
                RematchCount++;
            
                GameActive = true;
                
                var rockbuilder = new ComponentBuilder()
                    .WithButton("Rock", "rock-id")
                    .WithButton("Paper", "paper-id")
                    .WithButton("Scissors", "scissors-id");
                
                message = "Choose to play!";
                await cp.UpdateAsync(x => { x.Content = message; x.Components = rockbuilder.Build(); });
            
                await InitializeGame();
            }
            

            if (validAnswerIds.Contains(cp.Data.CustomId)) {
                await addEntryRPS();

                if (UserAnswers.Count() >= 2)
                {
                    await checkWinState();
                } else 
                {
                    await cp.UpdateAsync(x => { x.Content = $"{component.User.Username} has chosen!\n"; });
                }
            }
        }
        public async Task<string> showStats()
        {
            DataTable results = DBConnection.GetRPSWinStats();
            
            // Use string interpolation to simplify message construction
            var message = $"WIN STATS: \n{(results.Rows.Count == 0 ? "No one has won yet... this is awkward.\n" : "")}";
        
            foreach (DataRow row in results.Rows)
            {
                string username = row["username"].ToString();
                int wins = Convert.ToInt32(row["wins"]);
        
                // Replace if statement with ternary operator
                var plural = (wins > 1) ? "s" : "";
        
                message += ($"[**{username}**] has [**{wins}**] win{plural} \n");
            }
                    
            return message;
        }
        
        public async Task checkWinState()
        {
            var message = "";
        
            // Use a dictionary to avoid multiple if statements
            var moves = new Dictionary<string, string> {
                { "rock", "scissors" },
                { "paper", "rock" },
                { "scissors", "paper" }
            };
        
            // Check if it's a tie
            if (UserAnswers[0].Item2 == UserAnswers[1].Item2) {
                message = $"[**{UserAnswers[0].Item1.Username}**] >> [**{UserAnswers[0].Item2}**]\n";
                message += $"[**{UserAnswers[1].Item1.Username}**] >> [**{UserAnswers[1].Item2}**]\n";
                message += $"Its a tie!";
            } else {
                // Check if user 1 wins
                if (moves[UserAnswers[0].Item2] == UserAnswers[1].Item2) {
                    message = $"Winner: [**{UserAnswers[0].Item1.Username}**] >> [**{UserAnswers[0].Item2}**]\n";
                    message += $"Loser:   [**{UserAnswers[1].Item1.Username}**] >> [**{UserAnswers[1].Item2}**]\n";
                    connection.AddWinnerRPS(UserAnswers[0].Item1.Username);
                } else {
                    // If not, user 2 wins
                    message = $"Winner: [**{UserAnswers[1].Item1.Username}**] >> [**{UserAnswers[1].Item2}**]\n";
                    message += $"Loser:   [**{UserAnswers[0].Item1.Username}**] >> [**{UserAnswers[0].Item2}**]\n";
                    connection.AddWinnerRPS(UserAnswers[1].Item1.Username);
                }
            }
        
            if (TooSlow.Any()) {
                foreach(var user in TooSlow) {
                    message += $"{user} was too slow!\n";
                }
            }
        
            GameActive = false;
            await EndGame(message);
        }
        
        public async Task InitializeGame() 
        {
            cp = null;
            answerRPS = null;
            UserAnswers.Clear();
            TooSlow.Clear();
            RematchCount = 0;
            AnswerCount = 0;
            StatCount = 0;
        }
        public async Task EndGame(string message)
        {
            GameActive = false;
        
            var rockbuilder = new ComponentBuilder()
                .WithButton("Rematch!", "rematch-id")
                .WithButton("Stats", "stats-id");
        
            // Update message and components in the existing message
            await cp.UpdateAsync(x =>
            { 
                x.Content = $"{message}";
                x.Components = rockbuilder.Build();
            });
        }
        
        public async Task addEntryRPS()
        {
            // Set up object to add to list
            Tuple<SocketUser, string> useranswer = Tuple.Create(cp.User,answerRPS);
            string slowuser = cp.User.Username;

            
            bool uniquePlayer = true;
            foreach (var user in UserAnswers){
                if(user.Item1.Username.Equals(cp.User.Username)){
                    uniquePlayer = false;
                }
            }

            
            if(!UserAnswers.Any())
            {
                AnswerCount += 1;
                UserAnswers.Add(useranswer);
            } else if (UserAnswers.Count() < 2 && uniquePlayer == true)
            {
                AnswerCount += 1;
                UserAnswers.Add(useranswer);
            } else if (uniquePlayer == true)
            {
                TooSlow.Add(slowuser);
            }
        }
    }
}