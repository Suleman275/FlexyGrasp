using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour {
    [SerializeField] List<AudioClip> sounds;
    
    SerialPort serialPort;

    AudioSource audioSource;

    GameObject[] index;
    GameObject[] middle;
    GameObject[] ring;
    GameObject[] pinky;
    GameObject[] thumb;

    GameObject hand;
    GameObject ball;


    float indexRot;
    float middleRot;
    float ringRot;
    float pinkyRot;
    float thumbRot;
    float thumbRotCorrection = 20;

    float handXRot;
    float handYRot;
    float handZRot;

    float handXPos;
    float handYPos;
    float handZPos;

    public float rotationSpeed; //assigned in inspector
    public float moveSpeed; //assigned in inspector

    bool isGun;
    bool isCall;
    bool isPoint;
    bool isThumbsUp;
    bool isYackOff;
    bool isPee;
    bool isSpidey;
    bool isFist;
    bool isPeace;

    bool handHasBall;

    private void Start() {
        serialPort = new SerialPort("COM8", 9600);

        index = GameObject.FindGameObjectsWithTag("Right_Index");
        middle = GameObject.FindGameObjectsWithTag("Right_Middle");
        ring = GameObject.FindGameObjectsWithTag("Right_Ring");
        pinky = GameObject.FindGameObjectsWithTag("Right_Pinky");
        thumb = GameObject.FindGameObjectsWithTag("Right_Thumb");

        hand = GameObject.FindGameObjectWithTag("Right_Hand");

        serialPort.Open();
        serialPort.ReadTimeout = 5000;

        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        ReadAndProcessSerial();

        CheckFist();
        CheckPeace();
        CheckGun();
        CheckCall();
        CheckPoint();
        CheckThumbsUp();
        CheckYackOff();
        CheckPee();
        CheckSpidey();

        if (Input.GetKeyDown(KeyCode.R)) {
            hand.transform.eulerAngles = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            hand.transform.position = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            hand.transform.position += Vector3.forward * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            hand.transform.position += Vector3.back * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            hand.transform.position += Vector3.left * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            hand.transform.position += Vector3.right * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            hand.transform.position += Vector3.up * moveSpeed;
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            hand.transform.position += Vector3.down * moveSpeed;
        }
    }

    private void ReadAndProcessSerial() {
        var serialOutput = serialPort.ReadLine();
        var processedOut = serialOutput.Split(':');
        var val = float.Parse(processedOut[2]);

        switch (processedOut[1]) {
            case "Index":
                RotateFinger(index, val - indexRot);
                indexRot = val;

                if (indexRot > 30) {
                    audioSource.clip = sounds[1];
                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                }
                break;
            case "Middle":
                RotateFinger(middle, val - middleRot);
                middleRot = val;

                if (middleRot > 30) {
                    audioSource.clip = sounds[2];
                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                }
                break;
            case "Ring":
                RotateFinger(ring, val - ringRot);
                ringRot = val;

                if (ringRot > 30) {
                    audioSource.clip = sounds[3];
                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                }
                break;
            case "Pinky":
                RotateFinger(pinky, val - pinkyRot);
                pinkyRot = val;

                if (pinkyRot > 30) {
                    audioSource.clip = sounds[4];
                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                }
                break;
            case "Thumb":
                val -= thumbRotCorrection;
                RotateFinger(thumb, val - thumbRot);
                thumbRot = val;

                if (thumbRot > 20) {
                    audioSource.clip = sounds[0];
                    if (!audioSource.isPlaying) {
                        audioSource.Play();
                    }
                }
                break;
            case "Rotate":
                var x = val / 1000f;
                var y = float.Parse(processedOut[3]) / 1000f;
                var z = float.Parse(processedOut[4]) / -1000f;

                hand.transform.Rotate(x, z, y);
                break;
            //case "Translate":
            //    var x_trans = val / 10000f;
            //    var y_trans = float.Parse(processedOut[3]) / 10000f;
            //    var z_trans = float.Parse(processedOut[4]) / 10000f;

            //    hand.transform.Translate(x_trans-handXPos, 0, 0);
            //    handXPos = x_trans;
            //    break;
            default:
                break;
        }
    }

    private void RotateFinger(GameObject[] finger, float val) {
        foreach (var bone in finger) {
            bone.transform.localEulerAngles = new Vector3(bone.transform.localEulerAngles.x, bone.transform.localEulerAngles.y, bone.transform.localEulerAngles.z - val);
        }
    }

    private void CheckFist() {
        var rotThreshold = 20f;

        if (indexRot > rotThreshold && middleRot > rotThreshold && ringRot > rotThreshold && pinkyRot > rotThreshold && thumbRot > rotThreshold - thumbRotCorrection) {
            isFist = true;
            print("FIST");

            // Shoot a ray directly down
            RaycastHit hit;
            if (Physics.Raycast(hand.transform.position, Vector3.down, out hit, 1f)) {
                if (hit.collider.gameObject.name == "Sphere") {

                    ball = hit.collider.gameObject;
                    ball.GetComponent<Rigidbody>().useGravity = false;
                    ball.transform.SetParent(hand.transform);
                    handHasBall = true;
                }
            }
        }
        else {
            isFist = false;

            if (handHasBall) {
                ball.GetComponent<Rigidbody>().useGravity = true;
                ball.transform.SetParent(null);
                handHasBall = false;
                ball = null;
            }
        }
    }

    private void CheckPeace() {
        var rotTreshold = 30f;

        if (indexRot < rotTreshold && middleRot < rotTreshold && ringRot > rotTreshold && pinkyRot > rotTreshold && thumbRot > rotTreshold - thumbRotCorrection) {
            isPeace = true;
            print("PEACE");
        }
        else {
            isPeace = false;
        }
    }
    private void CheckGun() {
        var rotTreshold = 30f;

        if (indexRot < rotTreshold && middleRot < rotTreshold && ringRot > rotTreshold && pinkyRot > rotTreshold && thumbRot < rotTreshold - thumbRotCorrection) {
            isGun = true;
            print("GUN");
        }
        else {
            isGun = false;
        }
    }
    private void CheckCall() {
        var rotTreshold = 30f;

        if (indexRot > rotTreshold && middleRot > rotTreshold && ringRot > rotTreshold && pinkyRot < rotTreshold && thumbRot < rotTreshold - thumbRotCorrection) {
            isCall = true;
            print("CALL");
        }
        else {
            isCall = false;
        }
    }
    private void CheckPoint() {
        var rotTreshold = 30f;

        if (indexRot < rotTreshold && middleRot > rotTreshold && ringRot > rotTreshold && pinkyRot > rotTreshold && thumbRot > rotTreshold - thumbRotCorrection) {
            isPoint = true;
            print("POINT");
        }
        else {
            isPoint = false;
        }
    }
    private void CheckThumbsUp() {
        var rotTreshold = 30f;

        if (indexRot > rotTreshold && middleRot > rotTreshold && ringRot > rotTreshold && pinkyRot > rotTreshold && thumbRot < rotTreshold - thumbRotCorrection) {
            isThumbsUp = true;
            print("THUMBS UP");
        }
        else {
            isThumbsUp = false;
        }
    }

    private void CheckYackOff() {
        var rotTreshold = 30f;

        if (indexRot > rotTreshold && middleRot < rotTreshold && ringRot > rotTreshold && pinkyRot > rotTreshold && thumbRot > rotTreshold - thumbRotCorrection) {
            isYackOff = true;
            print("YACK OFF");
        }
        else {
            isYackOff = false;
        }
    }
    private void CheckPee() {
        var rotTreshold = 30f;

        if (indexRot > rotTreshold && middleRot > rotTreshold && ringRot > rotTreshold && pinkyRot < rotTreshold && thumbRot > rotTreshold - thumbRotCorrection) {
            isPee = true;
            print("I NEED TO PEE");
        }
        else {
            isPee = false;
        }
    }
    private void CheckSpidey() {
        var rotTreshold = 30f;

        if (indexRot < rotTreshold && middleRot > rotTreshold && ringRot > rotTreshold && pinkyRot < rotTreshold && thumbRot < rotTreshold - thumbRotCorrection) {
            isSpidey = true;
            print("SPIDEY WEB");
        }
        else {
            isSpidey = false;
        }
    }
}
