using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace вывод_данных_из_бд
{
    class Dimension
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private String userName;
        public String UserName
        {
            get { return userName; }
            set { userName = value; }
        }
        private List<Attribute> attributes;
        public List<Attribute> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }
        private String realName;
        public String RealName
        {
            get { return realName; }
            set { realName = value; }
        }
        private String primaryKey;
        public String PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }
        private Dictionary<String, Attribute> fields;
        public Dictionary<String, Attribute> Fields
        {
            get { return fields; }
        }

        public Dimension()
        {
            this.attributes = new List<Attribute>();
            this.fields = new Dictionary<String, Attribute>();

        }
    }
}
