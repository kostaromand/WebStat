using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStat
{
    //Класс для ввода данных из консоли, используется в приложении
    public class ConsoleReader : IConsoleReader 
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
