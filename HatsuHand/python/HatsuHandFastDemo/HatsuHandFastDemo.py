import sys
sys.path.append("..")

from lib.HatsuHandAPI import RobotHandAPI
from GUI import HatsuHandGUI


if __name__ == "__main__":
    try:
        root = HatsuHandGUI().root
        root.mainloop()
        #with RobotHandAPI() as hand:
        #    #for chan in hand.channels:
        #    #    hand.set_motor_range(chan, 4000, 8000)
        #    #    hand.set_motor_speed(chan, 60)
        #    #    hand.set_motor_acceleration(chan, 10)
        #
        #    root = CreateHandGUI()
        #    root.mainloop()

    except Exception as e:
        print(f"An error occurred: {e}")


