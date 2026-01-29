using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baseapp.Helper
{
    public class KeyValuePair
    {
        private string _key = "";
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
            }
        }
        private string _label = "";
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value;
            }
        }
        private string _val = "";
        public string Value
        {
            get { return _val; }
            set
            {
                _val = value;
            }
        }
        public KeyValuePair(string _key, string _value, string _label)
        {
            Key = _key;
            Value = _value;
            Label = _label;
        }
    }
}
