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

    def set_tracking_mode(self, tracking_mode):
        self.tracking_mode = tracking_mode

    def process_hand_data(self, event):
        if len(event.hands) == 0:
            return

        for hand in event.hands:
            hand_type = "Left" if str(hand.type) == "HandType.Left" else "Right"
            

            # Wrist 
            wrist = hand.arm.next_joint
            wrist_pos = (wrist.x,wrist.y,wrist.z)
            print(f"{hand_type} wrist joint {wrist_pos[0]:.2f}, {wrist_pos[1]:.2f}, {wrist_pos[2]:.2f}")


            # Elbow
            elbow = hand.arm.prev_joint
            elbow_pos = (elbow.x,elbow.y,elbow.z)
            print(f"{hand_type} elbow joint {elbow_pos[0]:.2f}, {elbow_pos[1]:.2f}, {elbow_pos[2]:.2f}")


            # Digits
            for digit_index, digit in enumerate(hand.digits):
                digit_name = ["Thumb", "Index", "Middle", "Ring", "Pinky"][digit_index]

                for joint_index, bone in enumerate(digit.bones):
                    start_pos = (bone.prev_joint.x, bone.prev_joint.y, bone.prev_joint.z) if bone.prev_joint else None
                    end_pos = (bone.next_joint.x, bone.next_joint.y, bone.next_joint.z) if bone.next_joint else None

                    if start_pos:
                        print(f"{hand_type} {digit_name} joint {joint_index} Start: {start_pos[0]:.2f}, {start_pos[1]:.2f}, {start_pos[2]:.2f}")
                    if end_pos:
                        print(f"{hand_type} {digit_name} joint {joint_index} End: {end_pos[0]:.2f}, {end_pos[1]:.2f}, {end_pos[2]:.2f}")
                
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