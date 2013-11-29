using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace вывод_данных_из_бд
{
    class FactTable
    {
        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
       
        private String primaryKey;
        public String PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }

        private Dictionary<String, Tuple<int, String>> fields;
        public Dictionary<String, Tuple<int, String>> Fields
        {
            get { return fields; }
        }

        public FactTable()
        {
            this.fields = new Dictionary<String, Tuple<int, String>>();
        }
    }
}
