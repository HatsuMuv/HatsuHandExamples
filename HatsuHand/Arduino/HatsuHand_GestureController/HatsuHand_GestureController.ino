#include <Servo.h>

// Define pins
const int potPin = A0;            // Potentiometer for sensitivity
const int xPin = A4;              // Joystick X-axis
const int yPin = A3;              // Joystick Y-axis
const int buttonPin = 12;         // Joystick button for toggle

// Define finger servo pins
const int thumbPinServo = 10;
const int indexPinServo = 3;
const int middlePinServo = 5;
const int ringPinServo = 6;
const int pinkyPinServo = 9;



const int servoPins[] = {indexPinServo, middlePinServo, ringPinServo, pinkyPinServo, thumbPinServo}; // Servo pins


const int calibrationSamples = 100;
const int scaledownparameter = 10;


int centerX = 512;
int centerY = 512;
int deadzoneCenter = 50; // Deadzone near the center
int deadzoneEdges = 200;  // Deadzone near the edges

// Create Servo objects
Servo servos[5];

// Setup constants
const int maxJoystickValue = 1023; // Max joystick value

// State variables
bool isGestureLocked = false; // Lock state for gestures
int lockedGestureIndex = -1;  // Locked gesture direction
int lockedIntensity = 0;     // Locked servo angle intensity
bool lastButtonState = HIGH;  // Track previous button state

void setup() {
  Serial.begin(57600);
  pinMode(buttonPin, INPUT_PULLUP); // Set button pin with pull-up

  // Attach each servo to its respective pin
  for (int i = 0; i < 5; i++) {
    servos[i].attach(servoPins[i]);
  }

  delay(500); // For stablize the read value

  long sumX = 0, sumY = 0;
  for (int i = 0; i < calibrationSamples; i++) {
    sumX += analogRead(xPin);
    sumY += analogRead(yPin);
    delay(5); // Allow time between readings for stability
  }
  centerX = sumX / calibrationSamples;
  centerY = sumY / calibrationSamples;

  Serial.print("Calibrated Center X: ");
  Serial.print(centerX);
  Serial.print(" | Calibrated Center Y: ");
  Serial.println(centerY);

}

void loop() {
  // Read potentiometer for sensitivity control
  int potValue = analogRead(potPin);
  delay(1); // Allow ADC to stabilize
  int rawMaxControlDistance = map(potValue, 0, 1023, 200, maxJoystickValue / 2); // Raw sensitivity
  int effectiveRange = rawMaxControlDistance - (deadzoneCenter + deadzoneEdges); // Adjust for both deadzones
  int scaledMaxControlDistance = effectiveRange / scaledownparameter; // Scale down effective range




  // Read joystick X and Y values
  int xValue = analogRead(xPin);
  int yValue = analogRead(yPin);

  // Apply center deadzone
  if (abs(xValue - centerX) < deadzoneCenter) xValue = centerX;
  if (abs(yValue - centerY) < deadzoneCenter) yValue = centerY;

  // Apply edge deadzone
  if (xValue < deadzoneEdges) xValue = 0;
  if (xValue > maxJoystickValue - deadzoneEdges) xValue = maxJoystickValue;

  if (yValue < deadzoneEdges) yValue = 0;
  if (yValue > maxJoystickValue - deadzoneEdges) yValue = maxJoystickValue;

  // Center joystick values to 0
  xValue -= centerX;
  yValue -= centerY;

  // Scale down the joystick values
  xValue = xValue/10;
  yValue = yValue/10;

  // Fix intensity calculation
  int adjustedX = constrain(xValue, -scaledMaxControlDistance, scaledMaxControlDistance); // Clip to max range
  int adjustedY = constrain(yValue, -scaledMaxControlDistance, scaledMaxControlDistance);

  float distance = sqrt(sq(adjustedX) + sq(adjustedY));
  float intensity = constrain(map(distance, 0, scaledMaxControlDistance, 0, 100), 0, 100); // Normalize to 0-100%



  // Determine the direction
  float angle = atan2(yValue, xValue) * 180 / PI; // Angle in degrees
  if (angle < 0) angle += 360; // Normalize angle

  // Determine gesture based on angle
  int gestureIndex;
  if (intensity > 10){
    if (angle >= 337.5 || angle < 22.5) gestureIndex = 0;       // East
    else if (angle >= 22.5 && angle < 67.5) gestureIndex = 1;   // NE
    else if (angle >= 67.5 && angle < 112.5) gestureIndex = 2;  // North
    else if (angle >= 112.5 && angle < 157.5) gestureIndex = 3; // NW
    else if (angle >= 157.5 && angle < 202.5) gestureIndex = 4; // West
    else if (angle >= 202.5 && angle < 247.5) gestureIndex = 5; // SW
    else if (angle >= 247.5 && angle < 292.5) gestureIndex = 6; // South
    else gestureIndex = 7;    
  }
  else{
    gestureIndex = -1;
  }


  // Check button for toggle functionality
  bool currentButtonState = digitalRead(buttonPin) == LOW; // Button pressed if LOW
  if (currentButtonState && !lastButtonState) { // Detect button press
    isGestureLocked = !isGestureLocked; // Toggle lock state

    if (isGestureLocked) {
      // Lock the current gesture
      lockedGestureIndex = gestureIndex;
      //lockedServoAngle = map(intensity, 0, 100, 0, 180);
      lockedIntensity = intensity;
    }
  }
  lastButtonState = currentButtonState; // Update button state

  // Apply gesture based on lock state
  if (isGestureLocked) {
    // Locked gesture - apply locked servo angle
    performGesture(lockedGestureIndex, lockedIntensity); // Locked gesture with intensity
    Serial.print("Locked Gesture: ");
    Serial.print(lockedGestureIndex);
    Serial.print(" | Locked Intensity: ");
    Serial.print(lockedIntensity);
  } else {
    // Dynamic gesture - apply based on joystick position
    performGesture(gestureIndex, intensity);
    Serial.print("Direction: ");
    Serial.print(gestureIndex);
    Serial.print(" | Intensity: ");
    Serial.print(intensity);
  }

  // Print joystick values, button state, and sensitivity information
  Serial.print(" | X: ");
  Serial.print(xValue);
  Serial.print(" | Y: ");
  Serial.print(yValue);
  Serial.print(" | Button: ");
  Serial.print(currentButtonState ? "Pressed" : "Not Pressed");
  Serial.print(" | Sensitivity (Max Control Distance): ");
  Serial.println(potValue);

  delay(5); // Delay for readability
}

void performGesture(int gestureIndex, float intensity) {
  // Array to hold the target pulse widths for each linear motor
  int neutralPulseWidth = 1300; // Neutral position for all motors
  int targetPulseWidths[5] = {neutralPulseWidth, neutralPulseWidth, neutralPulseWidth, neutralPulseWidth, neutralPulseWidth}; // Neutral position
  

  // Define full gesture poses (fully executed at 100% intensity)
  switch (gestureIndex) {
    case -1:
      targetPulseWidths[0] = neutralPulseWidth; // Index: Neutral
      targetPulseWidths[1] = neutralPulseWidth; // Middle: Neutral
      targetPulseWidths[2] = neutralPulseWidth; // Ring: Neutral
      targetPulseWidths[3] = neutralPulseWidth; // Pinky: Neutral
      targetPulseWidths[4] = neutralPulseWidth; // Thumb: Neutral
      break;
    case 0: // East: RockNRoll
      targetPulseWidths[0] = 1000; // Index: Fully extend
      targetPulseWidths[1] = 2000; // Middle: Fully retract
      targetPulseWidths[2] = 2000; // Ring: Fully retract
      targetPulseWidths[3] = 1000; // Pinky: Fully extend
      targetPulseWidths[4] = 1500; // Thumb: Neutral
      break;
    case 1: // NorthEast: ThmbUp
      targetPulseWidths[0] = 1500; // Index: Neutral
      targetPulseWidths[1] = 2000; // Middle: Fully retract
      targetPulseWidths[2] = 2000; // Ring: Fully retract
      targetPulseWidths[3] = 2000; // Pinky: Fully retract
      targetPulseWidths[4] = 1000; // Thumb: Fully extend
      break;
    case 2: // North: Fully extend
      targetPulseWidths[0] = 1000; // Index: Fully extend
      targetPulseWidths[1] = 1000; // Middle: Fully extend
      targetPulseWidths[2] = 1000; // Ring: Fully extend
      targetPulseWidths[3] = 1000; // Pinky: Fully extend
      targetPulseWidths[4] = 1000; // Thumb: Fully extend
      break;
    case 3: // NorthWest: PeacePose
      targetPulseWidths[0] = 1000; // Index: Fully extend
      targetPulseWidths[1] = 1000; // Middle: Fully extend
      targetPulseWidths[2] = 2000; // Ring: Fully retract
      targetPulseWidths[3] = 2000; // Pinky: Fully retract
      targetPulseWidths[4] = 2000; // Thumb: retract
      break;
    case 4: // West: IndexPointing
      targetPulseWidths[0] = 1000; // Index: Fully extend
      targetPulseWidths[1] = 2000; // Middle: Fully retract
      targetPulseWidths[2] = 2000; // Ring: Fully retract
      targetPulseWidths[3] = 2000; // Pinky: Fully retract
      targetPulseWidths[4] = 2000; // Thumb: retract
      break;
    case 5: // SouthWest: PinkyFingerExtendOnly
      targetPulseWidths[0] = 2000; // Index: Fully retract
      targetPulseWidths[1] = 2000; // Middle: Fully retract
      targetPulseWidths[2] = 2000; // Ring: Fully retract
      targetPulseWidths[3] = 1000; // Pinky: Fully extend
      targetPulseWidths[4] = 2000; // Thumb: retract
      break;
    case 6: // South: Fully Retract
      targetPulseWidths[0] = 2000; // Index: Fully retract
      targetPulseWidths[1] = 2000; // Middle: Fully retract
      targetPulseWidths[2] = 2000; // Ring: Fully retract
      targetPulseWidths[3] = 2000; // Pinky: Fully retract
      targetPulseWidths[4] = 2000; // Thumb: Fully retract
      break;
    case 7: // EastSouth: HandGunPose
      targetPulseWidths[0] = 1000; // Index: Fully extend
      targetPulseWidths[1] = 2000; // Middle: Fully retract
      targetPulseWidths[2] = 2000; // Ring: Fully retract
      targetPulseWidths[3] = 2000; // Pinky: Fully retract
      targetPulseWidths[4] = 1000; // Thumb: Fully extend
      break;
    default: // Neutral Position
      for (int i = 0; i < 5; i++) {
        targetPulseWidths[i] = neutralPulseWidth; // Neutral position
      }
      break;
  }

  // Apply intensity scaling to interpolate between neutral and target positions
  for (int i = 0; i < 5; i++) {
    int scaledPulseWidth = neutralPulseWidth + ((targetPulseWidths[i] - neutralPulseWidth) * intensity / 100.0);
    servos[i].writeMicroseconds(scaledPulseWidth);
  }
}
