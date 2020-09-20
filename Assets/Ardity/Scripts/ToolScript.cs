using UnityEngine;


    public class ToolScript : MonoBehaviour
    {
        private UdpReceiver _udpReceiver;

        public static Vector3 SetOffsetRotation { get; set; } = Vector3.one;
        public static Vector3 SetOffsetPosition { get; set; } = Vector3.zero;
        public float speed = 5f;
        private GameObject mouth; 

        private void Awake()
        {
            _udpReceiver = FindObjectOfType<UdpReceiver>();
            mouth = GameObject.Find("Mouth");
        }


        public float Speed
        {
            get => speed;
        }

        void Update()
        {
            SetRotation();
            SetPosition();
          //  SetMouthMovement();
        }

        /*void SetMouthMovement()
        {
            mouth.transform.localRotation = Quaternion.Lerp( 
                mouth.transform.localRotation,
                new Quaternion(
                    1,
                    1,
                    1),
                Time.deltaTime*15f );
        }*/

        private void SetRotation()
        {
            var e = new Quaternion(_udpReceiver.udpData[10], _udpReceiver.udpData[11],
                _udpReceiver.udpData[12], _udpReceiver.udpData[13]).eulerAngles;
            var r = Quaternion.Euler(-e.x, -e.y, e.z);
            r *= Quaternion.Euler(SetOffsetRotation);
            transform.localRotation = r;
        }

        private void SetPosition()
        {
            CheckSpeed();
            var t = new Vector3(
                _udpReceiver.udpData[7],
                _udpReceiver.udpData[8],
                _udpReceiver.udpData[9]);
            t.Set(t.x, t.y, -t.z);
            var localPosition = Vector3.Scale(t, new Vector3(Speed, Speed, Speed));
            localPosition += SetOffsetPosition;
            transform.localPosition = localPosition;
        }

        private void CheckSpeed()
        {
            if (speed < 1)
            {
                speed = 1;
            }
        }
    }
