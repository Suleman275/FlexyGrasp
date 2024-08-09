#include <Wire.h>
#include "I2Cdev.h"     
#include "MPU6050.h" 

class Finger {
private:
  int pin;
  int minValue;
  int maxValue;
  String hand;
  String name;

public:
  // Constructor: Assign the pin and initialize min/max values
  Finger(int fingerPin, String hand, String name) {
    pin = fingerPin;
    minValue = 1023; // Initialize to maximum value
    maxValue = 0;    // Initialize to minimum value
    this->hand = hand;
    this->name = name;
    pinMode(pin, INPUT);
  }

  void Process() {
    int flexPosition = analogRead(pin);
    
    // Update min/max values
    minValue = min(minValue, flexPosition);
    maxValue = max(maxValue, flexPosition);

    // Map the value to the desired range (0 to 50)
    int mappedValue = map(flexPosition, minValue, maxValue, 0, 100);

    Serial.println(hand + ":" + name + ":" + String(mappedValue));
  }
};

// Create an instance of the Finger class
Finger thumb(A0, "Right", "Thumb");
Finger index(A1, "Right", "Index");
Finger middle(A2, "Right", "Middle");
Finger ring(A3, "Right", "Ring");
Finger pinky(A4, "Right", "Pinky");   

MPU6050 mpu;

int16_t ax, ay, az, ax_min, ay_min, az_min, ax_max, ay_max, az_max;
int16_t gx, gy, gz, gx_min, gy_min, gz_min, gx_max, gy_max, gz_max;

void setup() {
  Serial.begin(9600);
  Wire.begin();
  mpu.initialize();

  // Initialize min/max values
//  ax_max = ay_max = az_max = -32767;
//  gx_max = gy_max = gz_max = -32767;
//  ax_min = ay_min = az_min = 32767;
//  gx_min = gy_min = gz_min = 32767;
}

void loop() {
  thumb.Process();
  index.Process();
  middle.Process();
  ring.Process();
  pinky.Process();

  mpu.getMotion6(&ax, &ay, &az, &gx, &gy, &gz);

  // Update min/max values for accelerometer
//  ax_max = max(ax_max, ax);
//  ay_max = max(ay_max, ay);
//  az_max = max(az_max, az);
//  
//  ax_min = min(ax_min, ax);
//  ay_min = min(ay_min, ay);
//  az_min = min(az_min, az);

  // Update min/max values for gyroscope
//  gx_max = max(gx_max, gx);
//  gy_max = max(gy_max, gy);
//  gz_max = max(gz_max, gz);
//  
//  gx_min = min(gx_min, gx);
//  gy_min = min(gy_min, gy);
//  gz_min = min(gz_min, gz);

//  int ax_mapped = map(ax, ax_min, ax_max, -100, 100);
//  int ay_mapped = map(ay, ay_min, ay_max, -100, 100);
//  int az_mapped = map(az, az_min, az_max, -100, 100);
  
//  int gx_mapped = map(gx, gx_min, gx_max, -100, 100);
//  int gy_mapped = map(gy, gy_min, gy_max, -100, 100);
//  int gz_mapped = map(gz, gz_min, gz_max, -100, 100);

  Serial.println("Right:Translate:"+String(ax)+":"+String(ay)+":"+String(az));
//  Serial.println("Right:Rotate:"+String(gx_mapped)+":"+String(gy_mapped)+":"+String(gz_mapped));
  Serial.println("Right:Rotate:"+String(gx)+":"+String(gy)+":"+String(gz));
  
  //delay(1000);
}
