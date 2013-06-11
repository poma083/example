using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication2
{
    class Manager : IDisposable
    {
        private static Manager instance = null;
        private static Object singletoneLocker = new Object();
        private System.Threading.EventWaitHandle LockerForReaders = null;
        private IList<Reader> readers = null;
        private String data;

        private Manager()
        {
            data = "";
            LockerForReaders = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset);
            readers = new List<Reader>();
        }

        public static Manager Instance
        {
            get
            {
                lock (singletoneLocker)// создавать менеджера может только один поток в данный момент времени
                {
                    if (instance == null)
                    {
                        instance = new Manager();
                    }
                }
                return instance;
            }
        }

        public void Write(String Data)
        {
            try
            {
                LockerForReaders.Reset();
                System.Threading.EventWaitHandle[] handles = null;
                lock (singletoneLocker)// Запрещаем другим потокам добавлять(удалять) читателей пока мы с ними работаем
                {
                    handles = new System.Threading.EventWaitHandle[readers.Count()];
                    int i = 0;
                    foreach (Reader r in readers)
                    {
                        handles[i] = r.LockerForWriter;
                        i++;
                    }
                }
                if (handles.Length == 0)
                {
                    return;
                }
                System.Threading.EventWaitHandle.WaitAll(handles);
                Console.WriteLine("Что-то пишем далее что-то делаем в течении секунды (в это время никто ничего не имеет права ни читать ни писать)");
                data = "--";
                for (int i = 0; i < 10; i++)
                {
                    data += i.ToString();
                    System.Threading.Thread.Sleep(50);
                }
                data += "--";
            }
            finally
            {
                LockerForReaders.Set();
            }
        }

        public void AddReader(Reader r)
        {
            lock (singletoneLocker)// Запрещаем другим потокам добавлять(удалять) читателей пока мы с ними работаем
            {
                readers.Add(r);
            }
        }

        public void WaitOne()
        {
            LockerForReaders.WaitOne();
        }

        public String GetData()
        {
            return data;
        }

        public void Dispose()
        {
            LockerForReaders.Close();
            readers.Clear();
        }
    }
}
