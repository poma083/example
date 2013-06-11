using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            Manager m = Manager.Instance;// находим (создаём) менеджера

            int i = 0;
            for (i = 0; i < 10; i++)
            {
                Reader r = new Reader(m, i);// Создаём читателя
                m.AddReader(r);// и добавляем его в менеджер
            }

            i = 0;
            while (true)
            {
                System.Threading.Thread.Sleep(0);// Даём читателям возможность почитать
                m.Write(i.ToString());//пишем
                i++;
                
            }
        }
    }
}
