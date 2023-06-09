﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_Modul._8_dars_MultiTreading.Lessontasks
{
    public static class MultiThreadExample
    {
        public static void Start1()
        {
            Using_Mutex();
        }
        static void ThreadProperties()
        {
            Thread currentThread = Thread.CurrentThread;

            Console.WriteLine($"Имя потока: {currentThread.Name}");
            currentThread.Name = "Метод Main";
            Console.WriteLine($"Имя потока: {currentThread.Name}");
            Console.WriteLine($"Запущен ли поток: {currentThread.IsAlive}");
            Console.WriteLine($"Id потока: {currentThread.ManagedThreadId}");
            Console.WriteLine($"Приоритет потока: {currentThread.Priority}");
            Console.WriteLine($"Статус потока: {currentThread.ThreadState}");
            Console.WriteLine(Thread.GetDomain());
            Console.WriteLine(Thread.GetDomainID());
            currentThread.Interrupt();
            Console.WriteLine($"Статус потока: {currentThread.ThreadState}");
        }
        static void CreateThread()
        {
            ThreadStart mydelegate = Print;
            // ParameterizedThreadStart
            Thread myThread1 = new Thread(mydelegate);
            Thread myThread4 = new Thread(mydelegate, 1);
            Thread myThread2 = new Thread(new ThreadStart(Print));
            Thread myThread3 = new Thread((object? obj) => Console.WriteLine(obj + " Hello Threads"));

            myThread4.Start();
            myThread1.Start();  // запускаем поток myThread1
            myThread2.Start();  // запускаем поток myThread2
            myThread3.Start("salom");  // запускаем поток myThread3
        }
        static void MultiThreadWork()
        {
            // создаем новый поток
            Thread myThread = new Thread(Print);
            //myThread.Priority = ThreadPriority.Lowest;
            // запускаем поток myThread
            myThread.Start();

            // действия, выполняемые в главном потоке
            for (int i = 5; i < 1000; i++)
            {
                Console.WriteLine($"Главный поток: {i}");
                //Thread.Sleep(300);
            }

            // действия, выполняемые во втором потокке
            void Print()
            {
                for (int i = 0; i < 1000; i++)
                {
                    Console.WriteLine($"Второй поток: {i}");
                    // Thread.Sleep(400);
                }
            }
        }
        static void SyncThreads()
        {
            int x = 0;

            // запускаем пять потоков
            for (int i = 1; i < 3; i++)
            {
                Thread myThread = new(Print);
                myThread.Name = $"Поток {i}";   // устанавливаем имя для каждого потока
                myThread.Start();
            }

            void Print()
            {
                x = 1;
                for (int i = 1; i < 6; i++)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name}: {x}");
                    x++;
                    //Thread.Sleep(100);
                }
            }
        }
        static void LockSyncThreads()
        {
            int x = 0;

            object locker = new();  // объект-заглушка
            object locker2 = new();  // объект-заглушка
                                     // запускаем пять потоков
            for (int i = 1; i < 6; i++)
            {
                Thread myThread = new(Print);
                if (i % 2 == 0)
                {
                    myThread = new(Print);
                }
                else
                {
                    myThread = new(Print2);
                }
                myThread.Name = $"Поток {i}";
                myThread.Start();
            }


            void Print()
            {
                lock (locker)
                {
                    x = 1;
                    for (int i = 1; i < 6; i++)
                    {

                        Console.WriteLine($"Print: {Thread.CurrentThread.Name}: {x}");
                        x++;
                        Thread.Sleep(100);

                    }
                }
            }
            void Print2()
            {
                lock (locker2)
                {
                    x = 1;
                    for (int i = 1; i < 6; i++)
                    {
                        Console.WriteLine($"Print2: {Thread.CurrentThread.Name}: {x}");
                        x++;
                        Thread.Sleep(100);
                    }
                }
            }
        }
        static void UsingMonitor()
        {
            int x = 0;
            object locker = new();  // объект-заглушка
                                    // запускаем пять потоков
            for (int i = 1; i < 6; i++)
            {
                Thread myThread = new(Print);
                myThread.Name = $"Поток {i}";
                myThread.Start();
            }

            void Print()
            {
                bool acquiredLock = false;
                try
                {
                    Monitor.Enter(locker, ref acquiredLock);
                    x = 1;
                    for (int i = 1; i < 6; i++)
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name}: {x}");
                        x++;
                        Thread.Sleep(100);
                        throw new Exception();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    if (acquiredLock) Monitor.Exit(locker);
                }
            }
        }
        static void UsingMonitor1()
        {
            MyQueue<string> myQueue = new MyQueue<string>();

            Thread DequeueThread = new Thread(() => Console.WriteLine(myQueue.Dequeue()));
            DequeueThread.Start();
            Console.WriteLine("Before start Enqueu");
            myQueue.Enqueue("hello");
        }
        static void InterlockedIncrement()
        {
            int count = 0;

            for (int i = 0; i < 100; i++)
            {
                Interlocked.Increment(ref count);
            }

            Console.WriteLine("Count = {0}", count);

        }
        static void TimeWaiting()
        {
            object myLock = new object();
            bool success = false;

            lock (myLock)
            {
                success = Monitor.Wait(myLock, TimeSpan.FromSeconds(5));
            }

            if (success)
            {
                // The lock was acquired within the allotted time
            }
            else
            {
                // The wait timed out before the lock was acquired
            }
            Console.WriteLine(success);
        }
        static void MultipleLocks()
        {
            object lock1 = new object();
            object lock2 = new object();

            lock (lock1)
            {
                Console.WriteLine("Acquired lock1");
                object lock3 = new object();

                lock (lock2)
                {
                    Console.WriteLine("Acquired lock2");
                }

                Console.WriteLine("Released lock2");
            }

            Console.WriteLine("Released lock1");

        }
        static void Using_AutoResetEvent()
        {
            AutoResetEvent resetEvent = new AutoResetEvent(false);


            Thread thread2 = new Thread(() =>
            {
                resetEvent.WaitOne();
                Console.WriteLine("thread2 worked");
                // do some other work
            });
            Thread thread1 = new Thread(() =>
            {
                // do some work
                Thread.Sleep(5000);
                Console.WriteLine("Sleep(500000) finished");
                resetEvent.Set();
                Thread.Sleep(10000);
                Console.WriteLine("Hello Thread");
            });
            thread1.Start();
            thread2.Start();
        }
        static void Using_Join()
        {
            Console.WriteLine("Main Thread Started");
            //Main Thread creating three child threads
            Thread thread1 = new Thread(Method1);
            Thread thread2 = new Thread(Method2);
            Thread thread3 = new Thread(Method3);
            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread1.Join(); //Block Main Thread until thread1 completes its execution
            Console.WriteLine("Method tugatib ishladi");
            thread2.Join(); //Block Main Thread until thread2 completes its execution
            thread3.Join(); //Block Main Thread until thread3 completes its execution
            Console.WriteLine("Main Thread Ended");
            Console.Read();
            Console.ReadKey();
            static void Method1()
            {
                Console.WriteLine("Method1 - Thread1 Started");
                Thread.Sleep(5000);
                Console.WriteLine("Method1 - Thread 1 Ended");
            }
            static void Method2()
            {
                Console.WriteLine("Method2 - Thread2 Started");
                Thread.Sleep(2000);
                Console.WriteLine("Method2 - Thread2 Ended");
            }
            static void Method3()
            {
                Console.WriteLine("Method3 - Thread3 Started");
                Thread.Sleep(5000);
                Console.WriteLine("Method3 - Thread3 Ended");
            }
        }
        static void Using_Mutex()
        {
            int x = 0;
            Mutex mutexObj = new(false);

            // запускаем пять потоков
            for (int i = 1; i < 6; i++)
            {
                Thread myThread = new(Print);
                myThread.Name = $"Potok -> {i}";
                
                myThread.Start();
            }

            void Print()
            {
                mutexObj.WaitOne();     // приостанавливаем поток до получения мьютекса
                Console.WriteLine("Current Thread -> "+Thread.CurrentThread.Name);
                Thread.Sleep(1000);
                x = 1;
                for (int i = 1; i < 6; i++)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name}: {x}");
                    x++;
                    Thread.Sleep(100);
                }
                mutexObj.ReleaseMutex();    // освобождаем мьютекс
            }
        }
        static void Using_Semaphore()
        {
            for (int i = 1; i < 6; i++)
            {
                Reader reader = new Reader(i);
            }
        }
        static void Using_Semaphore2()
        {
            Semaphore _pool = new Semaphore(2, 2);
            for (int i = 1; i <= 5; i++)
            {
                Thread t = new Thread(new ParameterizedThreadStart(Worker));
                t.Start(i);
            }
            void Worker(object num)
            {
                Console.WriteLine("Thread {0} is waiting to enter the semaphore.", num);
                _pool.WaitOne();

                Console.WriteLine("Thread {0} has entered the semaphore.", num);
                Thread.Sleep(2000);

                Console.WriteLine("Thread {0} is releasing the semaphore.", num);
                _pool.Release();
            }
        }
        static void Print() { Console.WriteLine("Hello Threads"); }

    }

    //Example Monitor
    public class MyQueue<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();
        private readonly object _lock = new object();

        public void Enqueue(T item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item);
                Monitor.Pulse(_lock);
            }
        }

        public T Dequeue()
        {
            lock (_lock)
            {
                while (_queue.Count == 0)
                {
                    Monitor.Wait(_lock);
                    Console.WriteLine("Dequeu");
                }
                return _queue.Dequeue();
            }
        }

        public void CheckLocker()
        {
            if (Monitor.TryEnter(_lock))
            {
                try
                {
                    // Do something while holding the lock
                }
                finally
                {
                    Monitor.Exit(_lock);
                }
            }
            else
            {
                // The lock is currently held by another thread, so we can't enter it
            }
        }
    }

    //Example Semphore
    class Reader
    {
        // создаем семафор
        static Semaphore sem = new Semaphore(3, 3);
        Thread myThread;
        int count = 3;// счетчик чтения

        public Reader(int i)
        {
            myThread = new Thread(Read);
            myThread.Name = $"Читатель {i}";
            myThread.Start();
        }

        public void Read()
        {
            while (count > 0)
            {
                sem.WaitOne();  // ожидаем, когда освободиться место

                Console.WriteLine($"{Thread.CurrentThread.Name} входит в библиотеку");

                Console.WriteLine($"{Thread.CurrentThread.Name} читает");
                Thread.Sleep(1000);

                Console.WriteLine($"{Thread.CurrentThread.Name} покидает библиотеку");

                sem.Release();  // освобождаем место

                count--;
                Thread.Sleep(1000);
            }
        }
    }
}
    