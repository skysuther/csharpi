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
using Newtonsoft.Json;
using csharpi.Connection;

namespace csharpi.rockpaperscissors 
{
    public class rockgame
    {
        
        List<Tuple<SocketUser, string>> UserAnswers = new List<Tuple<SocketUser, string>>();

        HashSet<string> validAnswerIds = new HashSet<string> { "rock-id", "paper-id", "scissors-id" };
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
            switch(cp.Data.CustomId)
            {
                // Since we set our buttons custom id as 'custom-id', we can check for it like this:
                case "rock-id":
                    answerRPS = "rock";
                    break;
                case "paper-id":
                    answerRPS = "paper";
                    break;
                case "scissors-id":
                    answerRPS = "scissors";
                    break;
                case "rematch-id":
                    var rockbuilder = new ComponentBuilder()
                        .WithButton("Rock", "rock-id")
                        .WithButton("Paper", "paper-id")
                        .WithButton("Scissors", "scissors-id");
                    
                    var message = "Choose to play!";

                    await cp.UpdateAsync(x =>
                        { 
                            x.Content = $"{message}";
                            x.Components = rockbuilder.Build();
                        });
                    break;
            }

            if (validAnswerIds.Contains(cp.Data.CustomId)) {
                await addEntryRPS();

                if (UserAnswers.Count() >= 2)
                {
                    await checkWinState();
                    UserAnswers.Clear();
                } else 
                {
                    await cp.UpdateAsync(x =>
                    { 
                        x.Content = $"{component.User.Username} has chosen!\n";
                    });
                }
            }
            await clearRPSClass();
        }
        public async Task checkWinState()
        {
            var message = "";
            var WinStatus = "win";
            switch(UserAnswers[0].Item2)
            {
                case "rock":
                if (UserAnswers[1].Item2 == "paper") {WinStatus = "lost";}
                if (UserAnswers[1].Item2 == "rock") {WinStatus = "tie";}
                break;
                case "paper":
                if (UserAnswers[1].Item2 == "scissors") {WinStatus = "lost";}
                if (UserAnswers[1].Item2 == "paper") {WinStatus = "tie";}
                break;
                case "scissors":
                if (UserAnswers[1].Item2 == "rock") {WinStatus = "lost";}
                if (UserAnswers[1].Item2 == "scissors") {WinStatus = "tie";}
                break;
            }
            if (WinStatus == "win")
            {
                message =  $"Winner: [**{UserAnswers[0].Item1.Username}**] >> [**{UserAnswers[0].Item2}**]\n";
                message += $"Loser:   [**{UserAnswers[1].Item1.Username}**] >> [**{UserAnswers[1].Item2}**]\n";
                await cp.UpdateAsync(x =>
                { 
                    x.Content = $"{message}";
                    x.Components = null;
                });
            } else if (WinStatus == "tie")
            {
                message = $"[**{UserAnswers[0].Item1.Username}**] >> [**{UserAnswers[0].Item2}**]\n";
                message += $"[**{UserAnswers[1].Item1.Username}**] >> [**{UserAnswers[1].Item2}**]\n";
                message += $"Its a tie!";
                var rockbuilder = new ComponentBuilder()
                    .WithButton("Rematch!", "rematch-id");
                await cp.UpdateAsync(x =>
                { 
                    x.Content = $"{message}";
                    x.Components = rockbuilder.Build();
                });
            } else
            {
                message =  $"Winner: [**{UserAnswers[1].Item1.Username}**] >> [**{UserAnswers[1].Item2}**]\n";
                message += $"Loser:   [**{UserAnswers[0].Item1.Username}**] >> [**{UserAnswers[0].Item2}**]\n";
                await cp.UpdateAsync(x =>
                { 
                    x.Content = $"{message}";
                    x.Components = null;
                });
            }
        }
        public async Task clearRPSClass() 
        {
            cp = null;
            answerRPS = null;
        }
        public async Task addEntryRPS()
        {
            // Set up object to add to list
            Tuple<SocketUser, string> useranswer = Tuple.Create(cp.User,answerRPS);
            // Only one answer per user, add object useranser to list
            if(!UserAnswers.Any())
            {
                UserAnswers.Add(useranswer);
            } else
            {
                bool uniquePlayer = true;
                foreach (var user in UserAnswers){
                    if(user.Item1.Username.Equals(cp.User.Username)){
                        uniquePlayer = false;
                    }
                }
                
                if(uniquePlayer == true)
                {
                    UserAnswers.Add(useranswer);
                } 
                UserAnswers.Add(useranswer);
            }
        }
    }
}