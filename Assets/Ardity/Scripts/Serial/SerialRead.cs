using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class SerialRead : MonoBehaviour
{
    public SerialController serialController;
    public float[] imuData;
    public float magnetData;


    public float[] ImuData
    {
        get => imuData;
        set => imuData = value;
    }

    public float MagnetData
    {
        get => magnetData;
        set => magnetData = value;
    }


    void Start()
    {
        serialController = FindObjectOfType<SerialController>();
        imuData = new float[4];
    }

    // Executed each frame
    void Update()
    {
        var message = serialController.ReadSerialMessage();
        if (message == null)
            return;
        if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
            Debug.Log("Connection attempt failed or disconnection detected");
        else
        {
            var data = message.Split('/');
            if (data.Any(d => !float.TryParse(d, out _)))
            {
                return;
            }

            if (data.Length == 5)
            {
                for (var i = 0; i < data.Length - 1; i++)
                {
                    imuData[i] = float.Parse(data[i]);
                    magnetData = float.Parse(data[4]);
                }
            }
            //for debugging purposes
            transform.localRotation = new Quaternion(
                imuData[0],imuData[1],imuData[2],imuData[3]
                );
        }
    }
}