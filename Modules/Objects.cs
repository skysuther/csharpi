using System;

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
}