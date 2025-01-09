import sys
sys.path.append("..")

from lib.HatsuHandAPI import RobotHandAPI
from GUI import HatsuHandGUI


if __name__ == "__main__":
    try:
        root = HatsuHandGUI().root
        root.mainloop()

    except Exception as e:
        print(f"An error occurred: {e}")
