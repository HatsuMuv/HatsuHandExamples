from lib.HatsuHandAPI import RobotHandAPI

import time
import tkinter as tk

def OpenHand():
	for i in reversed(range(5)):
		hand.set_motor_position(i, 4000)
		if i==4:
			time.sleep(0.5)

def CloseHand():
	for i in range(5):
		hand.set_motor_position(i, 8000)
		if i==3:
			time.sleep(0.5)
		

def FingerWave():
	fingers = [0, 1, 2, 3] 
	close_position = 8000  
	open_position = 4000   
	delay = 0.3 
	
	for i in range(3):
		for finger in fingers:
			hand.set_motor_position(finger, close_position)
			time.sleep(delay)
	
		for finger in fingers:
			hand.set_motor_position(finger, open_position)
			time.sleep(delay) 
		time.sleep(delay)

def CountBinaryNumber():
	binary_number_list = []
	for i in range(1,16):
		binary_number_list.append("".join(reversed(f'{i:0=4b}')))
	
	for bin in binary_number_list:
		for i in range(4):
			hand.set_motor_position(i, 4000 if bin[i] == '1' else 8000)
		time.sleep(1.5)    

def ExitButton():
	print("Exiting the application")
	root.quit()



# Create the window
root = tk.Tk()
root.title("HatsuHandControlPanel")
root.geometry("400x300")  # Set the window size
root.config(bg="darkgray")

root.grid_columnconfigure(0, weight=1)  # Set the first column to expand
root.grid_columnconfigure(1, weight=1)  # Set the second column to expand
root.grid_rowconfigure(0, weight=1)     # Set the first row to expand
root.grid_rowconfigure(1, weight=1)     # Set the second row to expand
root.grid_rowconfigure(2, weight=1)     # Set the third row to expand


# Create buttons and assign corresponding functions
button1 = tk.Button(root, text="Open Hand", command=OpenHand, width=20, height=2)
button2 = tk.Button(root, text="Close Hand", command=CloseHand, width=20, height=2)
button3 = tk.Button(root, text="Finger Wave", command=FingerWave, width=20, height=2)
button4 = tk.Button(root, text="Count Binary Number", command=CountBinaryNumber, width=20, height=2)
button5 = tk.Button(root, text="Exit", command=ExitButton, width=10, height=2)

# Arrange buttons using grid layout (2x2 grid, and the last button centered on the last row)
button1.grid(row=0, column=0, padx=10, pady=10)
button2.grid(row=0, column=1, padx=10, pady=10)
button3.grid(row=1, column=0, padx=10, pady=10)
button4.grid(row=1, column=1, padx=10, pady=10)
button5.grid(row=2, column=0, columnspan=2, padx=10, pady=10)  # Center the last button

# Run the GUI
if __name__ == "__main__":
	try:
		with RobotHandAPI() as hand:
			for chan in hand.channels:
				hand.set_motor_range(chan,4000, 8000)
				hand.set_motor_speed(chan, 60)       # Set speed for each motor
				hand.set_motor_acceleration(chan, 10) # Set acceleration for each motor

			root.mainloop()

	except Exception as e:
		print(f"An error occurred: {e}")
