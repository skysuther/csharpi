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
        
        
        
        
        public async Task ButtonChooseRPS(SocketMessageComponent component)
        {
            // We can now check for our custom id
            var answer = "";
            switch(component.Data.CustomId)
            {
                // Since we set our buttons custom id as 'custom-id', we can check for it like this:
                case "rock-id":
                    answer = "rock";
                    break;
                case "paper-id":
                    answer = "paper";
                    break;
                case "scissors-id":
                    answer = "scissors";
                    break;
                case "rematch-id":
                    var rockbuilder = new ComponentBuilder()
                        .WithButton("Rock", "rock-id")
                        .WithButton("Paper", "paper-id")
                        .WithButton("Scissors", "scissors-id");
                    
                    var message = "Choose to play!";

                    await component.UpdateAsync(x =>
                        { 
                            x.Content = $"{message}";
                            x.Components = rockbuilder.Build();
                        });
                    break;
            }

            if (validAnswerIds.Contains(component.Data.CustomId)) {
                // Set up object to add to list
                Tuple<SocketUser, string> useranswer = Tuple.Create(component.User,answer);
                // Only one answer per user, add object useranser to list
                if(UserAnswers.Count() == 0)
                {
                    UserAnswers.Add(useranswer);
                } else
                {
                    if(UserAnswers[0].Item1.Username.Equals(component.User.Username))
                    {
                    } else
                    {
                        UserAnswers.Add(useranswer);
                    }
                }
                // UserAnswers.Add(useranswer);
                var message = "";
                var WinStatus = "win";

                if (UserAnswers.Count() >= 2)
                {
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
                        await component.UpdateAsync(x =>
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
                        await component.UpdateAsync(x =>
                        { 
                            x.Content = $"{message}";
                            x.Components = rockbuilder.Build();
                        });
                    } else
                    {
                        message =  $"Winner: [**{UserAnswers[1].Item1.Username}**] >> [**{UserAnswers[1].Item2}**]\n";
                        message += $"Loser:   [**{UserAnswers[0].Item1.Username}**] >> [**{UserAnswers[0].Item2}**]\n";
                        await component.UpdateAsync(x =>
                        { 
                            x.Content = $"{message}";
                            x.Components = null;
                        });
                    }

                    UserAnswers.Clear();
                } else 
                {
                    await component.UpdateAsync(x =>
                    { 
                        x.Content = $"{component.User.Username} has chosen!\n";
                    });
                }
            }
            
        }
    }
}