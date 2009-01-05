using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace AdamRoderick.HeatMap
{
    [Serializable]
    [DataContract(Namespace = "http://www.dennydotnet.com/", Name = "Person")]
    public class Person
    {
        private string _name = string.Empty;
        private int _age = 0;

        [DataMember(IsRequired = true, Name = "Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        [DataMember(IsRequired = true, Name = "Age")]
        public int Age
        {
            get { return _age; }
            set { _age = value; }

        }

        [DataMember(IsRequired = true, Name = "Shoes")] public List<String> Shoes;
    }
}
