using System;
using System.Collections;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Threading;

public class SerialManager : IDisposable
{
    #region Fields
    public string comPort = "COM3";
    public int baudRate = 9600;
    public int timeout = 5;
    public bool IsOpen => serialPort.IsOpen && IsLooping();

    private Queue serialInputQueue;
    private Queue serialOutputQueue;
    private SerialPort serialPort;
    private bool threadIsRunning = true;
    private readonly Thread thread;
    #endregion Fields

    #region De/Con-structor
    public SerialManager(int _baudRate, string _comPort, int _timeout)
    {
        comPort = _comPort;
        baudRate = _baudRate;
        timeout = _timeout;
        serialInputQueue = Queue.Synchronized(new Queue());
        serialOutputQueue = Queue.Synchronized(new Queue());
        thread = new Thread(ThreadWorker);
        thread.Start();
    }
    ~SerialManager()
    {
        StopThread();
    }
    #endregion De/Con-structor

    #region Internal
    private bool IsLooping()
    {
        lock (this)
        {
            return threadIsRunning;
        }
    }
    private void ThreadWorker()
    {
        serialPort = new SerialPort(comPort, baudRate);
        serialPort.ReadTimeout = timeout;
        serialPort.Open();
        while (IsLooping())
        {
            if (serialInputQueue.Count != 0)
            {
                serialPort.WriteLine((string)serialInputQueue.Dequeue());   
            }

            try
            {
                serialOutputQueue.Enqueue(serialPort.ReadLine());
            }
            catch (TimeoutException) { }
        }
        serialPort.Close();
    }
    #endregion Internal

    #region Public
    public void StopThread()
    {
        lock (this)
        {
            threadIsRunning = false;
        }
    }

    public string ReadFromArduino()
    {
        if (serialOutputQueue.Count == 0) return null;
        return (string)serialOutputQueue.Dequeue();
    }
    public void WriteToArduino(string message)
    {
        serialInputQueue.Enqueue(message);
    }

    public void Dispose()
    {
        StopThread();
    }
    #endregion Public
}
