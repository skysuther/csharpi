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
        List<Tuple<SocketUser, string, int>> UserAnswerRounds = new List<Tuple<SocketUser, string, int>>();
        List<Tuple<string, int, string>> UserRoundStatus = new List<Tuple<string, int, string>>();
        List<Tuple<int, string, string>> RoundCompetitors = new List<Tuple<int, string, string>>();
        HashSet<string> validAnswerIds = new HashSet<string> { "rock-id", "paper-id", "scissors-id" };
        List<string> PlayerList = new List<string>();
        List<int> Rounds = new List<int>();
        private string roundMessage;
        public string RoundMessage
        {
            get { return roundMessage; }
            set { roundMessage = value; }
        }
        private SocketMessageComponent staticComponent;
        public SocketMessageComponent StaticComponent
        {
            get { return staticComponent; }
            set { staticComponent = value; }
        }
        private bool playerstandby;
        public bool PlayerStandby
        {
            get { return playerstandby; }
            set { playerstandby = value; }
        }
        private string playerBrackets;
        public string PlayerBrackets
        {
            get { return playerBrackets; }
            set { playerBrackets = value; }
        }
        public async Task ButtonInitGame(SocketMessageComponent component)
        {
            staticComponent = component;
            Task t1 = null;
            string id = component.Data.CustomId;
            var joinedPlayers = "";
            var message = "";
            var answer = "";
            switch(component.Data.CustomId)
            {
                case "join-session-id":
                        if (!PlayerList.Any(p => component.User.Username.Contains(p)))
                        {
                            t1 = Task.Run(() => PlayerList.Add(component.User.Username));
                            await Task.WhenAll(t1);
                        }
                        foreach (string player in PlayerList)
                        {
                            joinedPlayers += $"{player} has joined!\n";
                        }
                        await component.UpdateAsync(x =>
                            { 
                                x.Content = $"Join the session and start the game!\n";
                                x.Content += $"{joinedPlayers}";
                            });
                    break;
                case "start-game-id":
                    await nextRound();
                    roundMessage = playerBrackets;
                    
                    var rockbuilder = new ComponentBuilder()
                        .WithButton("Rock", "rock-id")
                        .WithButton("Paper", "paper-id")
                        .WithButton("Scissors", "scissors-id");
                    await staticComponent.UpdateAsync(x =>
                    { 
                        x.Content = $"{roundMessage}";
                        x.Components = rockbuilder.Build();
                    });
                    break;
                case "rock-id":
                    answer = "rock";
                    break;
                case "paper-id":
                    answer = "paper";
                    break;
                case "scissors-id":
                    answer = "scissors";
                    break;
            }
            if (validAnswerIds.Contains(component.Data.CustomId)) {
                Tuple<SocketUser, string, int> useranswerrounds = Tuple.Create(component.User,answer, Rounds.Last());
                if (UserAnswerRounds.Any()){
                    bool twoAnswers = false;
                    foreach (var user in UserAnswerRounds) {
                        if (user.Item1 == component.User) {
                            twoAnswers = true;
                        }
                    }
                    if (twoAnswers == false) {
                        UserAnswerRounds.Add(useranswerrounds);
                    }
                } else {
                    UserAnswerRounds.Add(useranswerrounds);
                }
                

                if (RoundCompetitors.Count * 2 <= UserAnswerRounds.Count) {
                    await checkWin();
                    roundMessage += $"Competitions: {RoundCompetitors.Count}, User Answers: {UserAnswerRounds.Count}\n";
                    await component.UpdateAsync(x =>
                    { 
                        x.Content = $"{roundMessage}";
                    });

                    if (RoundCompetitors.Any()){
                        foreach(var comp in RoundCompetitors)
                        {
                            foreach(var user in UserAnswerRounds)
                            {
                                if (comp.Item2 == user.Item1.Username)
                                {
                                    UserAnswerRounds.Remove(user);
                                } else if (comp.Item3 == user.Item1.Username)
                                {
                                    UserAnswerRounds.Remove(user);
                                }
                            }
                        }
                        await rematch();
                        var rockbuilder = new ComponentBuilder()
                            .WithButton("Rock", "rock-id")
                            .WithButton("Paper", "paper-id")
                            .WithButton("Scissors", "scissors-id");
                        await component.UpdateAsync(x =>
                        { 
                            x.Content = $"{roundMessage}";
                            x.Components = rockbuilder.Build();
                        });
                    }
                } else {
                    roundMessage += $"{component.User.Username} has chosen!\n";
                    await component.UpdateAsync(x =>
                    { 
                        x.Content = $"{roundMessage}";
                    });
                }
            }
        }
        public async Task endGame() {
            UserAnswerRounds.Clear();
            UserRoundStatus.Clear();
            RoundCompetitors.Clear();
            PlayerList.Clear();
            Rounds.Clear();
            roundMessage = "";
            playerBrackets = "";
            playerstandby = false;
        }
        public async Task nextRound() {
            // Increment rounds
            if (Rounds.Any())
            {
                int lastRound = Rounds.Last();
                Rounds.Add(lastRound + 1);
            }
            else
            {
                Rounds.Add(1);
            }
            var message = "";
            var standby = "";
            Random rand = new Random();
            var shuffledPlayers = PlayerList.OrderBy(_ => rand.Next()).ToList();

            // If odd number of players, select first player as standby
            if (shuffledPlayers.Count % 2 != 0) {standby = shuffledPlayers[0];shuffledPlayers.RemoveAt(0);}


            for (int i = 0; i < (shuffledPlayers.Count); i++)
            {
                Tuple<int, string, string> roundcompetitors = Tuple.Create(Rounds.Last(),shuffledPlayers[0],shuffledPlayers[shuffledPlayers.Count - 1]);
                RoundCompetitors.Add(roundcompetitors);
                message += $"{shuffledPlayers[0]} vs {shuffledPlayers[shuffledPlayers.Count - 1]}\n";
                shuffledPlayers.RemoveAt(0);
                shuffledPlayers.RemoveAt(shuffledPlayers.Count - 1);
            }
            if(standby != ""){message += $"{standby} STANDBY\n";}
            playerBrackets = message;
        }
        public async Task rematch() {
            var message = "";
            for (int i = 0; i < (RoundCompetitors.Count); i++)
            {
                message += $"{RoundCompetitors[i].Item2} and {RoundCompetitors[i].Item3} tied. Rematch!\n";
            }
            roundMessage = $"{playerBrackets}\n{message}";
            
        }
        public async Task checkWin() {
            
                for (int i = 0; i < (RoundCompetitors.Count); i++)
                {
                    var answer1 = "";
                    var user1 = "";
                    var answer2 = "";
                    var user2 = "";
                    var WinStatus = "win";
                    for (int x = 0; x < UserAnswerRounds.Count; x++)
                    {
                        if (UserAnswerRounds[x].Item1.Username == RoundCompetitors[i].Item2 && UserAnswerRounds.Count >1){
                            answer1 = UserAnswerRounds[x].Item2;
                            user1 = UserAnswerRounds[x].Item1.Username;
                        }
                        else if (UserAnswerRounds[x].Item1.Username == RoundCompetitors[i].Item3){
                            answer2 = UserAnswerRounds[x].Item2;
                            user2 = UserAnswerRounds[x].Item1.Username;
                        }
                        
                    }   
                    switch(answer1)
                        {
                            case "rock":
                            if (answer2 == "paper") {WinStatus = "lost";}
                            if (answer2 == "rock") {WinStatus = "tie";}
                            break;
                            case "paper":
                            if (answer2 == "scissors") {WinStatus = "lost";}
                            if (answer2 == "paper") {WinStatus = "tie";}
                            break;
                            case "scissors":
                            if (answer2 == "rock") {WinStatus = "lost";}
                            if (answer2 == "scissors") {WinStatus = "tie";}
                            break;
                        }   
                    
                    roundMessage += $"Win state: {WinStatus}, Answer1: {answer1}, Answer2: {answer2}, Answer count: {UserAnswerRounds.Count}\n";     
                    if (WinStatus == "win"){
                        Tuple<string, int, string> userroundstatus1 = Tuple.Create(user1, Rounds.Last(), answer1);
                        UserRoundStatus.Add(userroundstatus1);
                        PlayerList.Remove(user2);

                    } else if (WinStatus == "lost"){
                        Tuple<string, int, string> userroundstatus2 = Tuple.Create(user2, Rounds.Last(), answer2);
                        UserRoundStatus.Add(userroundstatus2);
                        PlayerList.Remove(user1);
                    }       
                }
            
        }
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
            }
            // Set up object to add to list
            Tuple<SocketUser, string> useranswer = Tuple.Create(component.User,answer);
            // Only one answer per user, add object useranser to list
            // if(UserAnswers.Count() == 0)
            // {
            //     UserAnswers.Add(useranswer);
            // } else
            // {
            //     if(UserAnswers[0].Item1.Username.Equals(component.User.Username))
            //     {
            //     } else
            //     {
            //         UserAnswers.Add(useranswer);
            //     }
            // }
            UserAnswers.Add(useranswer);
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
                    message = $"[**{UserAnswers[0].Item1.Username}**] has won >> [**{UserAnswers[0].Item2}**]\n";
                    message += $"[**{UserAnswers[1].Item1.Username}**] has lost >> [**{UserAnswers[1].Item2}**]\n";
                } else if (WinStatus == "tie")
                {
                    message = $"[**{UserAnswers[0].Item1.Username}**] >> [**{UserAnswers[0].Item2}**]\n";
                    message += $"[**{UserAnswers[1].Item1.Username}**] >> [**{UserAnswers[1].Item2}**]\n";
                    message += $"Its a tie!";
                } else
                {
                    message = $"[**{UserAnswers[1].Item1.Username}**] has won >> [**{UserAnswers[1].Item2}**]\n";
                    message += $"[**{UserAnswers[0].Item1.Username}**] has lost >> [**{UserAnswers[0].Item2}**]\n";
                }
                await component.UpdateAsync(x =>
                { 
                    x.Content = $"{message}";
                    x.Components = null;
                });

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