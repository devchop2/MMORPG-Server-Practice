using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        public static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            using (XmlReader r = XmlReader.Create("PDL.xml", settings))
            {
                r.MoveToContent(); //move to main content directly
                while (r.Read())
                {
                    Console.WriteLine(r.Name + r["name"]);

                }
            }
        }
    }


}