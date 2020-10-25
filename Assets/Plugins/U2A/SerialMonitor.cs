using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.Events;

public class SerialMonitor : MonoBehaviour
{
    #region Fields
    [Header("Serial Monitor")]
    [Tooltip("Use COM Port also used in Arduino IDE")]
    public string comPort = "COM3";
    [Tooltip("Use Same Baud Rate used in Arduino Project")]
    public int baudRate = 9600;
    [Tooltip("Use Same Time out choosen in Arduino Project")]
    public int timeout = 5;
    [HideInInspector] public bool IsConnected => serialManager.IsOpen;

    private Dictionary<string, SerialEvent> serialEventDictionary = new Dictionary<string, SerialEvent>();

    private SerialManager serialManager;
    #endregion Fields

    #region Unity Methods
    void Start()
    {
        // serialManager = new SerialManager(baudRate, comPort, timeout);
    }

    void Update()
    {
        CheckForDataRecieved();
    }
    void OnDisable()
    {
        serialManager.StopThread();
    }
    private void OnDestroy()
    {
        serialManager.StopThread();
    }
    #endregion Unity Methods

    #region Public Methods
    public string[] GetComPorts() => SerialPort.GetPortNames();

    public void SetComPort(string port)
    {
        comPort = port;
        StartCoroutine(SetComPortInternal());
    }
    
    public void AddSerialEvent(string eventName, UnityAction<string> action)
    {
        if (!serialEventDictionary.ContainsKey(eventName))
        {
            serialEventDictionary.Add(eventName, new SerialEvent());
        }
        serialEventDictionary[eventName].AddListener(action);
    }

    public void WriteToArduino(string message) => serialManager.WriteToArduino(message);
    #endregion Public Methods

    #region Internal Methods
    private IEnumerator SetComPortInternal()
    {
        if (serialManager != null)
        {
            serialManager.StopThread();
            while (serialManager.IsOpen) { yield return null; }
        }
        serialManager = new SerialManager(baudRate, comPort, timeout);
    }
    private void CheckForDataRecieved()
    {
        string data = serialManager?.ReadFromArduino();
        if (data == null || data == "") return;
        Debug.Log("CheckForDataRecieved: " + data);
        foreach (KeyValuePair<string, SerialEvent> ev in serialEventDictionary)
        {
            if (data.StartsWith(ev.Key))
            {
                ev.Value.Invoke(data);
                return;
            }
        }
    }
    #endregion Internal Methods

    public class SerialEvent : UnityEvent<string> { };
}
