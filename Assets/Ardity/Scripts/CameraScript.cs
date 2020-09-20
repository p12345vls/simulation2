using System;
using UnityEngine;


public class CameraScript : MonoBehaviour
{
    private UdpReceiver receiver;
    public float cameraSpeed = 5f;
    public float lensShiftRadius = .5f;
    public Vector3 cameraSetOffsetPosition = Vector3.zero;
    public Vector3 cameraSetOffsetRotation = Vector3.one;
    public Vector3 setToolOffsetPosition = Vector3.zero;
    public Vector3 setToolOffsetRotation = Vector3.one;
    public Vector3 setShoulderOffsetPosition = Vector3.zero;
    public Vector3 setShoulderOffsetRotation = Vector3.one;
    private ShoulderScript shoulder;
    private Camera cameraMain;
    private SerialRead serialRead;
    public bool isLensShiftEnabled;

    private void Awake()
    {
        receiver = FindObjectOfType<UdpReceiver>();
        shoulder = FindObjectOfType<ShoulderScript>();
        serialRead = FindObjectOfType<SerialRead>();
        cameraMain = Camera.main;
        isLensShiftEnabled = false;
        if (!(cameraMain is null)) cameraMain.usePhysicalProperties = enabled;
    }

    void Update()
    {
        OffSetHandler();
        SetRotation();
        SetPosition();
        if (isLensShiftEnabled)
        {
            SetLensShift();
        }
    }

    private void SetRotation()
    {
        var e = new Quaternion(receiver.udpData[3], receiver.udpData[4],
            receiver.udpData[5], receiver.udpData[6]).eulerAngles;
        var r = Quaternion.Euler(-e.x, -e.y, e.z);
        r *= Quaternion.Euler(cameraSetOffsetRotation);
        transform.localRotation = r;
    }

    private void SetPosition()
    {
        CheckSpeed();
        var t = new Vector3(receiver.udpData[0], receiver.udpData[1], receiver.udpData[2]);
        t.Set(t.x, t.y, -t.z);
        var localPosition = Vector3.Scale(t, new Vector3(Speed, Speed, Speed));
        localPosition += cameraSetOffsetPosition;
        transform.localPosition = localPosition;
    }

    private void SetLensShift()
    {
        var x = serialRead.ImuData[0];
        var y = serialRead.ImuData[1];
        var z = serialRead.ImuData[2];
        var w = serialRead.ImuData[3];
        var imuQuaternion = new Quaternion(x, y, z, w);
        var degreesValues = imuQuaternion.eulerAngles;
        var yAxisDegrees = degreesValues.y;

        cameraMain.lensShift = new Vector2(
            (float) Math.Cos(yAxisDegrees * Mathf.Deg2Rad) * lensShiftRadius,
            (float) Math.Sin(yAxisDegrees * Mathf.Deg2Rad) * lensShiftRadius);
    }

    private void OffSetHandler()
    {
        ToolScript.SetOffsetPosition = setToolOffsetPosition;
        ToolScript.SetOffsetRotation = setToolOffsetRotation;
        shoulder.transform.localPosition = setShoulderOffsetPosition;
        shoulder.transform.localRotation = Quaternion.Euler(setShoulderOffsetRotation);
    }

    private float Speed
    {
        get => cameraSpeed;
    }

    private void CheckSpeed()
    {
        if (cameraSpeed < 1)
        {
            cameraSpeed = 1;
        }
    }
}