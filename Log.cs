using System;
using System.IO;
using System.Text;
namespace SpacecraftGT
{
    static class Log 
    {
        static private Verbosity verbosityLevel = (Verbosity) Configuration.GetInt("verbosity", -1);

        public enum Verbosity {
           NOTICE,
            WARNING,
            ERROR,
            ALL,
        }

        private static  string filename = Configuration.Get("debug-log", null);

        static Log()
        {
            if (verbosityLevel == (Verbosity)(-1))
            {
                Log.WriteLine("Verbosity not defined, defaulting to ALL");
                verbosityLevel = Verbosity.ALL;
            }
        }

        private static void Write(string message, Verbosity category=Verbosity.NOTICE)
        {
            if (verbosityLevel < category)
            {
                return; 
            }

            StringBuilder S = new StringBuilder();

            S.Append(DateTime.Now.ToString("{0:HH:mm:ss}"));
            S.Append(Enum.GetName(Verbosity.NOTICE.GetType(), category).PadRight(50));
            S.Append(message);

            Console.Write(Enum.GetName(Verbosity.NOTICE.GetType(), category));
            Console.Write("\t");
            Console.WriteLine(message);

            string str = S.ToString();

            if (Configuration.Defined("debug-log"))
            {
                File.AppendAllText(filename, str);
            }
            else
            {
                filename = DateTime.Now.ToString("{0:dd MM YYYY}")+ ".log";
                File.AppendAllText(filename, str);
            }
        }

        public static void WriteLine(string message, Verbosity category = Verbosity.NOTICE)
        {
            Write(message + System.Environment.NewLine, category);
        }

    }
}
