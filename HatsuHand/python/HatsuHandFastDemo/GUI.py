from lib.HandControl import HatsuHandControl as HandControl
from lib.HatsuHandAPI import RobotHandAPI
import tkinter as tk
from IPython.terminal.debugger import set_trace as keyboard

class HatsuHandGUI:
	def __init__(self):
		self.hand = RobotHandAPI()
		if self.hand.controller:
			for chan in self.hand.channels:
				self.hand.set_motor_range(chan, 4000, 8000)
				self.hand.set_motor_speed(chan, 60)
				self.hand.set_motor_acceleration(chan, 10)
			self.handControl = HandControl(self.hand)

			
			_, self.PololuSerials = self.hand.find_pololu_port()
			
			if self.PololuSerials:
				self.connectingSerial = self.PololuSerials[0]

		self.count = 0

		self.root = self.CreateGUI()

		self.root.bind("<space>", self.increment_count)

		self.UpdateGUI()


	def ExitButton(self):
		print("Exiting the application")
		self.root.quit()

	def increment_count(self,event):
		self.count += 1
		print(self.count)
		self.label.config(text=f"Space Key Count: {self.count}")

	def UpdateGUI(self):
		_, currentPololuSerials = self.hand.find_pololu_port()
		print(self.PololuSerials)
		if(self.PololuSerials != currentPololuSerials):
			self.PololuSerials = currentPololuSerials
		self.root.after(20, self.UpdateGUI)

	def CreateGUI(self):
		root = tk.Tk()
		root.title("HatsuHandControlPanel")
		root.geometry("750x250")
		root.config(bg="darkgray")
		
		# Static GUI (buttons)
		button_frame = tk.Frame(root, bg="darkgray")
		button_frame.grid(row=0, column=1, columnspan=2, padx=10, pady=10)
		
		button1 = tk.Button(button_frame, text="Open Hand", command=self.handControl.OpenHand, width=20, height=2, font=("Arial", 12, "bold"))
		button2 = tk.Button(button_frame, text="Close Hand", command=self.handControl.CloseHand, width=20, height=2, font=("Arial", 12, "bold"))
		button3 = tk.Button(button_frame, text="Finger Wave", command=self.handControl.FingerWave, width=20, height=2, font=("Arial", 12, "bold"))
		button4 = tk.Button(button_frame, text="Count Binary Number", command=self.handControl.CountBinaryNumber, width=20, height=2, font=("Arial", 12, "bold"))
		button5 = tk.Button(button_frame, text="Exit", command=lambda: self.ExitButton(), width=10, height=2, font=("Arial", 12, "bold"))
		
		button1.grid(row=0, column=0, padx=10, pady=10)
		button2.grid(row=0, column=1, padx=10, pady=10)
		button3.grid(row=1, column=0, padx=10, pady=10)
		button4.grid(row=1, column=1, padx=10, pady=10)
		button5.grid(row=2, column=0, columnspan=2, padx=10, pady=10)
		
		# Create a frame for connection info
		connection_frame = tk.Frame(root, bg="lightgray", bd=2, relief="groove")
		connection_frame.grid(row=0, column=0, rowspan=3, padx=10, pady=10, sticky="nsew")
		
		# Connection information section
		connection_title = tk.Label(connection_frame, text="Current Connection Info", font=("Helvetica", 16, "bold"), bg="lightgray")
		connection_title.pack(pady=(10, 0))
		
		deviceText = "Device: \n"
		for serial in self.PololuSerials:
		    deviceText += serial + "\n"
		device_label = tk.Label(connection_frame, text=deviceText, font=("Arial", 12), bg="lightgray")
		device_label.pack(pady=(5, 0))
		
		status_label = tk.Label(connection_frame, text="Status: Disconnected", font=("Arial", 12), bg="lightgray")
		status_label.pack(pady=(5, 10))
		
		# Add a label for displaying the space key count
		self.label = tk.Label(root, text="Space Key Count: 0", font=("Arial", 12, "bold"), bg="lightgray")
		self.label.grid(row=3, column=0, columnspan=3, pady=(10, 0))
		
		return root


