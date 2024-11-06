# HatsuHandLeapControl

This project provides a Python-based control system for a robotic hand using Leap Motion for real-time hand tracking. The code leverages Leap Motion’s tracking data to control the finger positions of a robotic hand, with each finger's position based on its bending percentage. The system requires a specific setup for both Leap Motion and the robotic hand controller.

## Requirements

### Hardware
- [Leap Motion Controller](https://www.ultraleap.com/product/leap-motion-controller/) for hand tracking.
- Pololu Maestro Controller for controlling servo motors in the robotic hand.
- HatsuHand configured for finger movements.

### Software Dependencies
1. **Ultraleap Environment Setup**: You need to setup your Leap Environment to allow your PC to access Leap hardware first.
 - For Windows: Please download and install Driver from [Official Site](https://www.ultraleap.com/)
 - For Linux: Please follow [here](https://docs.ultraleap.com/linux/)



2. **Leap Motion SDK**: You need to have Leap Motion's SDK set up with Python bindings.
   - Install the Python bindings for LeapC as described in the [LeapC Python Bindings Repository](https://github.com/ultraleap/leapc-python-bindings).

2. **Python Libraries**:
   - `numpy` for numerical calculations.
   - `pyserial` for serial communication with the Pololu Maestro controller.


## Installation and Setup

1. **Clone this Repository** and install required Python packages:
   ```bash
   git clone https://github.com/HatsuMuv/HatsuHandExamples.git
   cd HatsuHand/python/HatsuHandLeapControl
   pip install -r requirements.txt
   ```

2. **Set Up Leap Motion Python Bindings**:
   - Follow the instructions provided in the [LeapC Python Bindings repository](https://github.com/ultraleap/leapc-python-bindings) to install and configure the Leap Motion SDK with Python support.


## Usage

1. **Connect the Leap Motion Controller and Pololu Maestro Controller which connected to HatsuHand** to your computer.
2. **Run the Script**:
   ```bash
   python HatsuHandLeapControl.py
   ```

   This script initializes the robotic hand, configures motor ranges and speeds, and starts tracking **Right** hand movements via Leap Motion.

3. **Control Flow**:
   - The code continuously listens for hand tracking data from the Leap Motion Controller.
   - Finger bending percentages for each finger are calculated based on the angle between finger bones.
   - These percentages are then translated to motor positions on the robotic hand, allowing it to mirror the user’s hand gestures.

## Code Structure

- `HandTracker`: Tracks finger bending percentages for both left and right hands.
- `HandControl`: Converts bending percentages into servo motor positions based on predefined open/close ranges.
- `HandTrackingListener`: Processes Leap Motion events, updating the robotic hand’s position based on real-time data.

## Configuration Notes

- **Motor Range**: The servo motor positions for the robotic hand are configured with a minimum (open) of `4000` and a maximum (closed) of `8000`. Adjust these values if needed based on your specific servo motor requirements.
- **Tracking Mode**: The Leap Motion is set to `Desktop` mode in this example. Change the tracking mode if necessary.

## Troubleshooting

- Ensure that the Leap Motion SDK and Python bindings are installed correctly and that the device is recognized.
- Verify that the Pololu Maestro Controller is correctly connected to your computer and robotic hand.
- Check serial connections and device ports if the hand is not responding to Leap Motion data.

## Contact

If you have any questions or need support, please reach out via the [HatsuMuv Discord](https://discord.gg/JbysAbJWCN).

## License

This project is licensed under the MIT License.
