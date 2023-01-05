using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharpi.Services
{
    // interation modules must be public and inherit from an IInterationModuleBase
    public class ExampleCommands : InteractionModuleBase<SocketInteractionContext>
    {
        // dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
        public InteractionService Commands { get; set; }
        private CommandHandler _handler;
        private static Random rng = new Random();  

        // constructor injection is also a valid way to access the dependecies
        public ExampleCommands (CommandHandler handler)
        {
            _handler = handler;
        }

        // our first /command!
        [SlashCommand("8ball", "find your answer!")]
        public async Task EightBall(string question)
        {
            // create a list of possible replies
            var replies = new List<string>();

            // add our possible replies
            replies.Add("yes");
            replies.Add("no");
            replies.Add("maybe");
            replies.Add("hazzzzy....");
            replies.Add("Try again later");
            replies.Add("Of course the answer is yes!");
            replies.Add("Why would it be? No!");
            replies.Add("Are you trying to trick me?");
            replies.Add("It is forseen as such");

            // get the answer
            var answer = replies[new Random().Next(replies.Count - 1)];

            // reply with the answer
            await RespondAsync($"You asked: [**{question}**]\nEight ball answer: [**{answer}**]");
        }

        // our second /command!
        [SlashCommand("chooseteam", "let Apple-bot choose your team!")]
        public async Task ChooseTeam(string members, int numberOfTeams = 2)
        {
            // create a list of possible replies
            var teamMembers = new List<string>(members.Split(','));
            var shuffled = teamMembers.OrderBy(_ => rng.Next()).ToList();

            var teamA = new List<string>();
            var teamB = new List<string>();

            var participants = shuffled.Count;
            for (int i = 0; i < participants; i++) 
            {
                if (i%2 == participants%2)
                {
                    teamA.Add(shuffled[i]);
                }
                else {
                    teamB.Add(shuffled[i]);
                }
            }
            string joinedA = String.Join(", ", teamA.ToArray());
            string joinedB = String.Join(", ", teamB.ToArray());

            // reply with the answer
            await RespondAsync($"You entered: [**{members}**]\nThere are [**{participants}**] participants\nYour teams are...\nTeam A: [**{joinedA}**]\nTeam B: [**{joinedB}**]");
        }
    }
}