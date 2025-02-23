using static System.Console;
namespace MagicVilla_VillaAPI.Logging
{
    public class Logging : ILogging
    {
        public void Log(string message, string? type)
        {
            if(type == "error")
            {
                WriteLine("ERROR - "+message);
            }
            else
            {
                WriteLine(message);
            }
        }
    }
}
