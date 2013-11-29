using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace вывод_данных_из_бд
{
    class Attribute
    {
        private int id;
        public int Id
        {
            get {return id;}
            set{id=value;}
        }
        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public Attribute(int id, String name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
