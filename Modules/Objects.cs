using System;
using System.Collections.Generic;
using Discord;
using Discord.Net;
using Discord.Interactions;
using Discord.WebSocket;

namespace csharpi
{
    class BotSettings
    {
        private string fielda = "Default";
        private int fieldb = 0;
        private bool fieldc = false;
        public string FieldA
        {
            get { return fielda; }
            set { fielda = value;}
        }
        public int FieldB
        {
            get { return fieldb; }
            set { fieldb = value;}
        }
        public bool FieldC
        {
            get { return fieldc; }
            set { fieldc = value;}
        }
    }
    class RockPaperScissorsObject 
    {
        private SocketUser users;
        private string answers;
        public SocketUser user
        {
            get { return users; }
            set { users = value;}
        }
        public string answer
        {
            get { return answers; }
            set { answers = value;}
        }
    }
    class RPSUserInput
    {
        
    }
}