namespace SearchInFileCSVLibrary.Resource
{
    using System.Collections.Generic;
    using System.Text;

    public static class DictionaryLibrary
    {
        public static Dictionary<string, Encoding> EncodingDict
        {
            get
            {
                var t = new Dictionary<string, Encoding>();
                t.Add("UTF8", Encoding.UTF8);
                t.Add("UTF7", Encoding.UTF7);
                t.Add("ASCII", Encoding.ASCII);
                t.Add("UTF32", Encoding.UTF32);
                return t;
            }
        }

        public static Dictionary<string, byte> TypeExpressionDict
        {
            get
            {
                var t = new Dictionary<string, byte>();
                t.Add("string", 1);
                t.Add("DateTime", 2);
                t.Add("int", 3);
                t.Add("float", 4);
                return t;
            }
        }
    }
}
