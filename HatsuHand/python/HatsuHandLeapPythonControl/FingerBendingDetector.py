import leap
import math
import time

_TRACKING_MODES = {
    leap.TrackingMode.Desktop: "Desktop",
    leap.TrackingMode.HMD: "HMD",
    leap.TrackingMode.ScreenTop: "ScreenTop",
}

class HandTracker:
    def __init__(self):
        self.tracking_mode = None
        # Baseline distances for each finger when fully extended
        self.extended_distances = {
            "Thumb": 100.0,  # Placeholder values; calibrate based on actual measurements
            "Index": 110.0,
            "Middle": 120.0,
            "Ring": 100.0,
            "Pinky": 100.0
        }
        self.bent_distances = {
            "Thumb": 85.0,  # Placeholder values; calibrate based on actual measurements
            "Index": 70.0,
            "Middle": 50.0,
            "Ring": 50.0,
            "Pinky": 50.0
        }

    def set_fully_extended(self, digit_name, distance):
        """ Set the fully extended distance for a given finger. """
        self.extended_distances[digit_name] = distance

    def set_fully_bent(self, digit_name, distance):
        """ Set the fully bent distance for a given finger. """
        self.bent_distances[digit_name] = distance

    def calculate_bend_percentage(self, distance, extended, bent):
        """ Calculate the bend percentage based on the distance, fully extended, and fully bent values. """
        if distance >= extended:
            return 0.0  # Fully extended
        elif distance <= bent:
            return 100.0  # Fully bent
        else:
            return ((extended - distance) / (extended - bent)) * 100

    
    def set_tracking_mode(self, tracking_mode):
        self.tracking_mode = tracking_mode

    def process_hand_data(self, event):
        if len(event.hands) == 0:
            return

        for hand in event.hands:
            hand_type = "Left" if str(hand.type) == "HandType.Left" else "Right"

            # Digits
            for digit_index, digit in enumerate(hand.digits):
                digit_name = ["Thumb", "Index", "Middle", "Ring", "Pinky"][digit_index]

                # Calculate the distance between the first and last joint
                first_joint = digit.bones[0].prev_joint
                last_joint = digit.bones[3].next_joint

                if first_joint and last_joint:
                    distance = math.sqrt(
                        (last_joint.x - first_joint.x) ** 2 +
                        (last_joint.y - first_joint.y) ** 2 +
                        (last_joint.z - first_joint.z) ** 2
                    )
                    print(f"{hand_type} {digit_name} distance between first and last joint: {distance:.2f}")
                    # Calculate the bending percentage
                    extended_distance = self.extended_distances[digit_name]
                    bent_distance = self.bent_distances[digit_name]
                    bend_percentage = self.calculate_bend_percentage(distance, extended_distance, bent_distance)
                    print(f"{hand_type} {digit_name} bend: {bend_percentage:.2f}%")
            

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
            time.sleep(1)


if __name__ == "__main__":
    main()