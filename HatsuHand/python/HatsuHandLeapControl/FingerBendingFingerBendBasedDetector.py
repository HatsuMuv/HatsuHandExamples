import leap
import math
import time
import numpy as np

_TRACKING_MODES = {
    leap.TrackingMode.Desktop: "Desktop",
    leap.TrackingMode.HMD: "HMD",
    leap.TrackingMode.ScreenTop: "ScreenTop",
}

class HandTracker:
    def __init__(self):
        self.tracking_mode = None
        # Baseline angles for each finger when fully extended
        self.extended_angle = {
            "Thumb": 6.0,  # Placeholder values; calibrate based on actual measurements
            "Index": 6.0,
            "Middle": 6.0,
            "Ring": 6.0,
            "Pinky": 6.0
        }
        self.bent_angle = {
            "Thumb": 37.0,  # Placeholder values; calibrate based on actual measurements
            "Index": 40.0,
            "Middle": 40.0,
            "Ring": 40.0,
            "Pinky": 40.0
        }
        self.bending_percentage = {
            "Left": {
                "Thumb": 0.0,
                "Index": 0.0,
                "Middle": 0.0,
                "Ring": 0.0,
                "Pinky": 0.0
            },
            "Right": {
                "Thumb": 0.0,
                "Index": 0.0,
                "Middle": 0.0,
                "Ring": 0.0,
                "Pinky": 0.0
            }
        }

    def set_fully_extended(self, digit_name, angle):
        """ Set the fully extended angle for a given finger. """
        self.extended_angle[digit_name] = angle

    def set_fully_bent(self, digit_name, angle):
        """ Set the fully bent angle for a given finger. """
        self.bent_angle[digit_name] = angle

    def calculate_bend_percentage(self, angle, extended, bent):
        """ Calculate the bend percentage based on the angle, fully extended, and fully bent values. """
        if angle <= extended:
            return 0.0  # Fully extended
        elif angle >= bent:
            return 100.0  # Fully bent

        else:
            return ((angle-extended) / (bent - extended)) * 100
    
    def set_tracking_mode(self, tracking_mode):
        self.tracking_mode = tracking_mode

    def calculate_angle_between_vectors(self, vector1, vector2):
        # Calculate the dot product
        dot_product = np.dot(vector1, vector2)
    
        # Calculate the magnitudes of the vectors
        magnitude1 = np.linalg.norm(vector1)
        magnitude2 = np.linalg.norm(vector2)
    
        # Calculate the cosine of the angle
        cos_theta = dot_product / (magnitude1 * magnitude2)
    
        # Ensure the cosine value is in the range [-1, 1] to avoid domain errors due to rounding
        cos_theta = np.clip(cos_theta, -1.0, 1.0)
    
        # Calculate the angle in radians and then convert to degrees
        angle_rad = np.arccos(cos_theta)
        angle_deg = np.degrees(angle_rad)
    
        return angle_deg

    def process_hand_data(self, event):
        if len(event.hands) == 0:
            return

        for hand in event.hands:
            hand_type = "Left" if str(hand.type) == "HandType.Left" else "Right"

            # Digits
            for digit_index, digit in enumerate(hand.digits):
                digit_name = ["Thumb", "Index", "Middle", "Ring", "Pinky"][digit_index]

                # Calculate the distance between the first and last joint
                proximal = digit.bones[1].next_joint
                intermediate = digit.bones[2].next_joint
                distal = digit.bones[3].next_joint

                vec1 = (intermediate.x - proximal.x, intermediate.y - proximal.y, intermediate.z - proximal.z)
                vec2 = (distal.x - intermediate.x, distal.y - intermediate.y, distal.z - intermediate.z)
                angle = self.calculate_angle_between_vectors(vec1,vec2)
                self.bending_percentage[hand_type][digit_name] = self.calculate_bend_percentage(
                    angle, self.extended_angle[digit_name], self.bent_angle[digit_name]
                )
                print(f"{hand_type} {digit_name} Angle:{angle} Percentage:{self.bending_percentage[hand_type][digit_name]}%")

class HandTrackingListener(leap.Listener):
    def __init__(self, hand_tracker):
        self.hand_tracker = hand_tracker

    def on_tracking_mode_event(self, event):
        self.hand_tracker.set_tracking_mode(event.current_tracking_mode)
        print(f"Tracking mode changed to {_TRACKING_MODES[event.current_tracking_mode]}")

    def on_device_event(self, event):
        try:
            with event.device.open():
                info = event.device.get_info()
        except leap.LeapCannotOpenDeviceError:
            info = event.device.get_info()
        print(f"Found device {info.serial}")

    def on_tracking_event(self, event):
        self.hand_tracker.process_hand_data(event)


def main():
    hand_tracker = HandTracker()
    tracking_listener = HandTrackingListener(hand_tracker)
    connection = leap.Connection()
    connection.add_listener(tracking_listener)

    running = True

    with connection.open():
        connection.set_tracking_mode(leap.TrackingMode.Desktop)
        while running:
            time.sleep(0.5)


if __name__ == "__main__":
    main()