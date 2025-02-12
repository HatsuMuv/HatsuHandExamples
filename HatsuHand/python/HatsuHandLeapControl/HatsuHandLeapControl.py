import sys
sys.path.append("..")

import leap
import time
import numpy as np
from FingerBendingFingerBendBasedDetector import HandTracker
from Lib.HatsuHandAPI import RobotHandAPI

close_position = 8000
open_position = 4300


class HandControl:
    def __init__(self, hand):
        self.hand = hand

    def move_finger_to_bend_percentage(self, motor_index, percentage):
        # Calculate target position based on bend percentage
        min_pos, max_pos = open_position, close_position
        target_position = int(min_pos - (percentage / 100.0) * (min_pos - max_pos))
        #print(target_position)
        self.hand.set_motor_position(motor_index, target_position)


class HandTrackingListener(leap.Listener):
    def __init__(self, hand_tracker, hand_control):
        super().__init__()
        self.hand_tracker = hand_tracker
        self.hand_control = hand_control

    def on_tracking_event(self, event):
        self.hand_tracker.process_hand_data(event)

        # Update the robot hand's position based on the bending percentage for each finger
        for i, digit_name in enumerate([ "Index", "Middle", "Ring", "Pinky","Thumb"]):
            percentage = self.hand_tracker.bending_percentage["Right"][digit_name]
            self.hand_control.move_finger_to_bend_percentage(i, percentage)

def main():
    # Initialize Robot Hand API
    connectingSerialIndex = 0


    hand = RobotHandAPI(connectingSerialIndex)

    # Configure motor range, speed, and acceleration for each channel
    for chan in hand.channels:
        hand.set_motor_range(chan, open_position, close_position)
        hand.set_motor_speed(chan, 120)
        hand.set_motor_acceleration(chan, 20)

    handControl = HandControl(hand)
    _, PololuSerials = hand.find_pololu_port()
    connectingSerial = PololuSerials[connectingSerialIndex]

    # Initialize Leap Motion and Listener
    hand_tracker = HandTracker()
    hand_listener = HandTrackingListener(hand_tracker, handControl)
    connection = leap.Connection()
    connection.add_listener(hand_listener)

    # Main loop to keep connection open
    running = True
    with connection.open():
        connection.set_tracking_mode(leap.TrackingMode.Desktop)
        while running:
            time.sleep(0.1)  # Adjust the delay as needed for smooth control

if __name__ == "__main__":
    main()
