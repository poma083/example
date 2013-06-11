using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class Reader : IDisposable
    {
        public System.Threading.EventWaitHandle LockerForWriter = null;
        System.Threading.Timer timer = null;
        Manager manager;
        Int32 number = 0;

        public Reader(Manager m, Int32 Number)
        {
            number = Number;
            manager = m;
            LockerForWriter = new System.Threading.EventWaitHandle(true, System.Threading.EventResetMode.ManualReset);
            timer = new System.Threading.Timer(Read, null, 100, 100);
        }
        public void Read(object obj)
        {
            manager.WaitOne();// если данную строку закоментировать, то увидим проблемму несинхронизированных (неверных) данных
            try
            {
                LockerForWriter.Reset();
                string data = manager.GetData();
                Console.WriteLine("Читатель №" + number.ToString() + " читает" + " : " + data);
                //Console.WriteLine("Читатель №" + number.ToString() + " читает" + " : " + manager.GetData());
                System.Threading.Thread.Sleep(0);// Делаем вид что читаем настолько долго
                                                // что управление успевает перехватить другой поток
            }
            finally
            {
                LockerForWriter.Set();
            }
        }

        public void Dispose()
        {
            LockerForWriter.Close();
        }
    }
}
