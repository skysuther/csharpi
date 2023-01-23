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

namespace csharpi.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _commands;
        private readonly IServiceProvider _services;
        private BotSettings bset = new BotSettings();
        private RockPaperScissorsObject rps = new RockPaperScissorsObject();
        DBConnection connection = new DBConnection();
        List<Tuple<SocketUser, string>> UserAnswers = new List<Tuple<SocketUser, string>>();

        public CommandHandler(DiscordSocketClient client, InteractionService commands, IServiceProvider services)
        {
            _client = client;
            _commands = commands;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            // add the public modules that inherit InteractionModuleBase<T> to the InteractionService
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

            // process the InteractionCreated payloads to execute Interactions commands
            _client.InteractionCreated += HandleInteraction;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
            _client.ButtonExecuted += MyButtonHandler;

            // process the command execution results 
            _commands.SlashCommandExecuted += SlashCommandExecuted;
            _commands.ContextCommandExecuted += ContextCommandExecuted;
            _commands.ComponentCommandExecuted += ComponentCommandExecuted;
        }

        private Task ComponentCommandExecuted(ComponentCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }    

            return Task.CompletedTask;
        }

        private Task ContextCommandExecuted(ContextCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private Task SlashCommandExecuted(SlashCommandInfo arg1, Discord.IInteractionContext arg2, IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                switch (arg3.Error)
                {
                    case InteractionCommandError.UnmetPrecondition:
                        // implement
                        break;
                    case InteractionCommandError.UnknownCommand:
                        // implement
                        break;
                    case InteractionCommandError.BadArgs:
                        // implement
                        break;
                    case InteractionCommandError.Exception:
                        // implement
                        break;
                    case InteractionCommandError.Unsuccessful:
                        // implement
                        break;
                    default:
                        break;
                }
            }

            return Task.CompletedTask;
        }

        private async Task HandleInteraction (SocketInteraction arg)
        {
            try
            {
                // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
                var ctx = new SocketInteractionContext(_client, arg);
                await _commands.ExecuteCommandAsync(ctx, _services);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if(arg.Type == InteractionType.ApplicationCommand)
                {
                    await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
                }
            }
        }
        public async Task Client_Ready()
        {
            // Let's build a guild command! We're going to need a guild so lets just put that in a variable.
            ulong guildId = 224340283830042624;
            var guild = _client.GetGuild(guildId);

            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
            var guildCommand = new SlashCommandBuilder()
                .WithName("list-roles")
                .WithDescription("Lists all roles of a user.")
                .AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true);

            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
            var appleFacts = new SlashCommandBuilder()
                .WithName("facts")
                .WithDescription("Shares an apple fact!");
                
            // Next, lets create our slash command builder. This is like the embed builder but for slash commands.
            var play_rockpaperscissors = new SlashCommandBuilder()
                .WithName("play-rockpaperscissors")
                .WithDescription("Initiate a rock, paper, scissors game!");

            // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
            //guildCommand.WithName("first-command");

            // Descriptions can have a max length of 100.
            //guildCommand.WithDescription("This is my first guild slash command!");

            // Let's do our global command
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("first-global-command");
            globalCommand.WithDescription("This is my first global slash command");
            

            // Our settings command
            var settingsCommand = new SlashCommandBuilder()
                .WithName("settings")
                .WithDescription("Changes some settings within the bot.")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("field-a")
                    .WithDescription("Gets or sets the field A")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Sets the field A")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("value", ApplicationCommandOptionType.String, "the value to set the field", isRequired: true)
                    ).AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Gets the value of field A.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                    )
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("field-b")
                    .WithDescription("Gets or sets the field B")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Sets the field B")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("value", ApplicationCommandOptionType.Integer, "the value to set the fie to.", isRequired: true)
                    ).AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Gets the value of field B.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                    )
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("field-c")
                    .WithDescription("Gets or sets the field C")
                    .WithType(ApplicationCommandOptionType.SubCommandGroup)
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("set")
                        .WithDescription("Sets the field C")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                        .AddOption("value", ApplicationCommandOptionType.Boolean, "the value to set the fie to.", isRequired: true)
                    ).AddOption(new SlashCommandOptionBuilder()
                        .WithName("get")
                        .WithDescription("Gets the value of field C.")
                        .WithType(ApplicationCommandOptionType.SubCommand)
                    )
                );


            try
            {
                // Now that we have our builder, we can call the CreateApplicationCommandAsync method to make our slash command.
                //await guild.CreateApplicationCommandAsync(guildCommand.Build());
                await _client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
                await _client.Rest.CreateGuildCommand(play_rockpaperscissors.Build(), guildId);
                await _client.Rest.CreateGuildCommand(settingsCommand.Build(), guildId);
                await _client.Rest.CreateGuildCommand(appleFacts.Build(), guildId);

                // With global commands we don't need the guild.
                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch(HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Message, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            //await command.RespondAsync($"You executed {command.Data.Name}");
            // Let's add a switch statement for the command name so we can handle multiple commands in one event.
            switch(command.Data.Name)
            {
                case "list-roles":
                    await HandleListRoleCommand(command);
                    break;
                case "settings":
                    await HandleSettingsCommand(command);
                    break;
                case "facts":
                    await HandleAppleFactCommand(command);
                    break;
                case "play-rockpaperscissors":
                    await HandleRockPaperScissorsCommand(command);
                    break;
            }
        }
        private async Task HandleListRoleCommand(SocketSlashCommand command)
        {
            // We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
            var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

            // We remove the everyone role and select the mention of each role.
            var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

            var embedBuiler = new EmbedBuilder()
                .WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
                .WithTitle("Roles")
                .WithDescription(roleList)
                .WithColor(Color.Green)
                .WithCurrentTimestamp();


            // Now, Let's respond with the embed.
            await command.RespondAsync(embed: embedBuiler.Build(), ephemeral: true);
        }
        private async Task HandleSettingsCommand(SocketSlashCommand command)
        {
            // First lets extract our variables
            var fieldName = command.Data.Options.First().Name;
            var getOrSet = command.Data.Options.First().Options.First().Name;
            // Since there is no value on a get command, we use the ? operator because "Options" can be null.
            object value = "";
            

            switch (getOrSet)
            {
                case "get":
                    {
                        if(fieldName == "field-a")
                        {
                            await command.RespondAsync($"The value of `field-a` is `{bset.FieldA}`");
                        }
                        else if (fieldName == "field-b")
                        {
                            await command.RespondAsync($"The value of `field-b` is `{bset.FieldB}`");
                        }
                        else if (fieldName == "field-c")
                        {
                            await command.RespondAsync($"The value of `field-c` is `{bset.FieldC}`");
                        }
                    }
                    break;
                case "set":
                    {
                        value = command.Data.Options.First().Options.First().Options?.FirstOrDefault().Value;
                         if(fieldName == "field-a")
                        {
                            bset.FieldA = (string)value;
                            await command.RespondAsync($"`field-a` has been set to `{bset.FieldA}`");
                        }
                        else if (fieldName == "field-b")
                        {
                            bset.FieldB = (int)value;
                            await command.RespondAsync($"`field-b` has been set to `{bset.FieldB}`");
                        }
                        else if (fieldName == "field-c")
                        {
                            bset.FieldC = (bool)value;
                            await command.RespondAsync($"`field-c` has been set to `{bset.FieldC}`");
                        }
                    }
                    break;
            }
        }
        private async Task HandleAppleFactCommand(SocketSlashCommand command)
        {
            // We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
            var applefact = DBConnection.RandomAppleFacts();

            // We remove the everyone role and select the mention of each role.

            var embedBuiler = new EmbedBuilder()
                .WithDescription(applefact.Item1)
                .WithTitle("Apple Facts!")
                .WithAuthor("Category: " + applefact.Item2);


            // Now, Let's respond with the embed.
            await command.RespondAsync(embed: embedBuiler.Build(), ephemeral: false);
        }
        private async Task HandleRockPaperScissorsCommand(SocketSlashCommand command)
        {
            var rockbuilder = new ComponentBuilder()
                .WithButton("Rock", "rock-id")
                .WithButton("Paper", "paper-id")
                .WithButton("Scissors", "scissors-id");

            await command.RespondAsync("Click to play!\n", components: rockbuilder.Build());
        }
        public async Task MyButtonHandler(SocketMessageComponent component)
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
            Tuple<SocketUser, string> useranswer = Tuple.Create(component.User,answer);
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
            var message = "";
            var WinStatus = "win";

            if (UserAnswers.Count() >= 2)
            {
                switch(UserAnswers[0].Item2)
                {
                    case "rock":
                    if (UserAnswers[1].Item2 == "paper") {WinStatus = "lost";}
                    break;
                    case "paper":
                    if (UserAnswers[1].Item2 == "scissors") {WinStatus = "lost";}
                    break;
                    case "scissors":
                    if (UserAnswers[1].Item2 == "rock") {WinStatus = "lost";}
                    break;
                }
                if (WinStatus == "win")
                {
                    message = $"[**{UserAnswers[0].Item1.Username}**] has won >> [**{UserAnswers[0].Item2}**]\n";
                    message += $"[**{UserAnswers[1].Item1.Username}**] has lost >> [**{UserAnswers[1].Item2}**]";
                } else
                {
                    message = $"[**{UserAnswers[1].Item1.Username}**] has won >> [**{UserAnswers[1].Item2}**]\n";
                    message += $"[**{UserAnswers[0].Item1.Username}**] has lost >> [**{UserAnswers[0].Item2}**]";
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