from HandControl import HatsuHandControl as HandControl
from lib.HatsuHandAPI import RobotHandAPI
import tkinter as tk
import time

class HatsuHandGUI:
	def __init__(self):
	
		self.connectingSerialIndex = 0
		self.hand = RobotHandAPI(self.connectingSerialIndex)
		for chan in self.hand.channels:
			self.hand.set_motor_range(chan, 4000, 8000)
			self.hand.set_motor_speed(chan, 60)
			self.hand.set_motor_acceleration(chan, 10)

		self.handControl = HandControl(self.hand)
		_, self.PololuSerials = self.hand.find_pololu_port()
		self.connectingSerial = self.PololuSerials[self.connectingSerialIndex]

		self.root = self.CreateGUI()

		self.Update()


	def CreateGUI(self):
		root = tk.Tk()
		root.title("HatsuHandControlPanel")
		root.geometry("750x260")  # Set the window size (width increased for better layout)
		root.config(bg="darkgray")



		# Static GUI (buttons)
		button_frame = tk.Frame(root, bg="darkgray")  # Frame for buttons
		button_frame.grid(row=0, column=1, columnspan=2, padx=10, pady=10)  # Place it on the right side

		self.button1 = tk.Button(button_frame, text="Open Hand", command=self.handControl.OpenHand, width=20, height=2, font=("Arial", 12, "bold"))
		self.button2 = tk.Button(button_frame, text="Close Hand", command=self.handControl.CloseHand, width=20, height=2, font=("Arial", 12, "bold"))
		self.button3 = tk.Button(button_frame, text="Finger Wave", command=self.handControl.FingerWave, width=20, height=2, font=("Arial", 12, "bold"))
		self.button4 = tk.Button(button_frame, text="Count Binary Number", command=self.handControl.CountBinaryNumber, width=20, height=2, font=("Arial", 12, "bold"))
		self.button5 = tk.Button(button_frame, text="Exit", command=lambda: self.ExitButton(), width=10, height=2, font=("Arial", 12, "bold"))

		# Arrange buttons using grid layout
		self.button1.grid(row=0, column=0, padx=10, pady=10)
		self.button2.grid(row=0, column=1, padx=10, pady=10)
		self.button3.grid(row=1, column=0, padx=10, pady=10)
		self.button4.grid(row=1, column=1, padx=10, pady=10)
		self.button5.grid(row=2, column=0, columnspan=2, padx=10, pady=10)  # Center the last button
		


		# Create a frame for connection info
		connection_frame = tk.Frame(root, bg="lightgray", bd=2, relief="groove")
		connection_frame.grid(row=0, column=0, rowspan=3, padx=10, pady=10, sticky="nsew")  # Place it on the left side

		# Connection information section
		connection_title = tk.Label(connection_frame, text="Current Connection Info", font=("Helvetica", 16, "bold"), bg="lightgray")
		connection_title.pack(pady=(10, 0))  # Title at the top

		deviceText = "Device: \n"
		for serial in self.PololuSerials:
			deviceText += serial + "\n"
		self.device_label = tk.Label(connection_frame, text=deviceText, font=("Arial", 12), bg="lightgray")
		self.device_label.pack(pady=(5, 0))  # Device label below title

		statusText = "Status: "
		statusText += "Connected to " +self.connectingSerial
		self.status_label = tk.Label(connection_frame, text=statusText, font=("Arial", 12), bg="lightgray")
		self.status_label.pack(pady=(5, 10))  # Status label below device

		refresh_button = tk.Button(connection_frame, text="Change \n Connecting Port", font=("Arial", 12, "bold"), command=self.ChangeConnectingPort)
		refresh_button.pack(pady=(10, 10))  # Button below the status label

		return root

	def ExitButton(self):
		print("Exiting the application")
		self.root.quit()

	def ChangeConnectingPort(self):
		time.sleep(0.5)
		if self.hand:
			self.hand.close()
			self.connectingSerialIndex+= 1
			if self.connectingSerialIndex >= len(self.PololuSerials):
				self.connectingSerialIndex = 0

			self.hand = None
			self.hand = RobotHandAPI(self.connectingSerialIndex)

			self.handControl = HandControl(self.hand)
			self.connectingSerial = self.PololuSerials[self.connectingSerialIndex]

			self.UpdateText()
			self.UpdateButton()

		

	def Update(self):
		_, currentPololuSerials = RobotHandAPI.__new__(RobotHandAPI).find_pololu_port()

		if(self.PololuSerials != currentPololuSerials):
			if self.hand is not None:
				self.hand.close()

			time.sleep(0.5)
			self.PololuSerials = currentPololuSerials

			if len(self.PololuSerials) == 0:
				self.hand = None
				self.handControl = None
				self.connectingSerial = None
			else:
				self.connectingSerialIndex = 0
				self.hand = RobotHandAPI(self.connectingSerialIndex)

				self.handControl = HandControl(self.hand)
				self.connectingSerial = self.PololuSerials[self.connectingSerialIndex]

			self.UpdateText()
			self.UpdateButton()

		self.root.after(20, self.Update)

	def UpdateText(self):
		if len(self.PololuSerials) == 0:
			self.device_label.config(text="Device: None")
			self.status_label.config(text="Status: Disconnected")
		else:
			deviceText = "Device: \n"
			for serial in self.PololuSerials:
				deviceText += serial + "\n"
			self.device_label.config(text=deviceText)

			statusText = "Status: "
			statusText += "Connected to " +self.connectingSerial
			self.status_label.config(text=statusText)

	def UpdateButton(self):
		if self.hand is None:
				self.button1.config(state=tk.DISABLED) 
				self.button2.config(state=tk.DISABLED)
				self.button3.config(state=tk.DISABLED)
				self.button4.config(state=tk.DISABLED)
		else:
			self.button1.config(state=tk.NORMAL)  
			self.button2.config(state=tk.NORMAL)
			self.button3.config(state=tk.NORMAL)
			self.button4.config(state=tk.NORMAL)
			self.button1.config(command=self.handControl.OpenHand)
			self.button2.config(command=self.handControl.CloseHand)
			self.button3.config(command=self.handControl.FingerWave)
			self.button4.config(command=self.handControl.CountBinaryNumber)

		

