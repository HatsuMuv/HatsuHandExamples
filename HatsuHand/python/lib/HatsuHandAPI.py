# Importing the maestro.py file
from lib.maestro import Controller
import serial
import serial.tools.list_ports
from IPython.terminal.debugger import set_trace as keyboard

class RobotHandAPI:
	def __init__(self,index,device=0x0c):
		# Initialize the Maestro controller

		ttyStr, _ = self.find_pololu_port()
		if not ttyStr:
			raise Exception("No Pololu device found!")

		self.controller = None
		if ttyStr[index]:
			self.controller = Controller(ttyStr[index], device)
			print("Connected to Pololu device on port:", ttyStr[index])

		# Define channels for each motor (assuming channels 0-4 for your 5 DOF hand)
		self.channels = [0, 1, 2, 3, 4]


	def __enter__(self):
		"""
		Context manager enter function.
		Returns self to allow usage with 'with' statements.
		"""
		return self

	def __exit__(self, exc_type, exc_val, exc_tb):
		"""
		Context manager exit function.
		Ensures that the controller is closed, even if an error occurs.
		"""
		self.close()

	
	def find_pololu_port(self):
		#Auto-detect Pololu device port based on VID and PID
		pololu_vid = '1FFB'  # Pololu VID
		pololu_pids = ['0089', '008A', '008B', '008C']  # PID list for Pololu device
		pololu_devices = []
		pololu_serials = []

		ports = list(serial.tools.list_ports.comports())
		print(ports)
		for port in ports:
			if port.vid is not None and port.pid is not None:
				# Check if the port is connected to Pololu device with PID and VID
				if f"{port.vid:04X}" == pololu_vid:
					#print(port.vid)
					#print(port.description)
					#print(port.serial_number)
					#print(port.pid)
					#print(port.hwid)
					if f"{port.pid:04X}" in pololu_pids: 
						if ("Command" in port.description or "LOCATION" not in port.hwid):
							if not port.serial_number in pololu_serials:
								pololu_devices.append(port.device)
								pololu_serials.append(port.serial_number)

		return pololu_devices, pololu_serials

	def close(self):
		# Close the controller's connection
		print("Closing controller connection...")
		self.controller.close()

	def set_motor_range(self,motor_index,min,max):
		"""
		Set the range for the motor.

		:param motor_index: Index of the motor (0-4)
		:param min: Minimum position (in quarter-microseconds)
		:param max: Maximum position (in quarter-microseconds)
		"""
		if 0 <= motor_index < 5:
			self.controller.setRange(self.channels[motor_index], min, max)
		else:
			raise ValueError("Invalid motor index. Must be between 0 and 4.")

	def set_motor_position(self, motor_index, target):
		"""
		Set the target position for the motor.

		:param motor_index: Index of the motor (0-4)
		:param target: Target position (in quarter-microseconds)
		"""
		if 0 <= motor_index < 5:
			self.controller.setTarget(self.channels[motor_index], target)
		else:
			raise ValueError("Invalid motor index. Must be between 0 and 4.")

	def set_motor_speed(self, motor_index, speed):
		"""
		Set the speed for the motor.

		:param motor_index: Index of the motor (0-4)
		:param speed: Speed value (0 for unlimited, or a number between 1-255)
		"""
		if 0 <= motor_index < 5:
			self.controller.setSpeed(self.channels[motor_index], speed)
		else:
			raise ValueError("Invalid motor index. Must be between 0 and 4.")

	def set_motor_acceleration(self, motor_index, acceleration):
		"""
		Set the acceleration for the motor.

		:param motor_index: Index of the motor (0-4)
		:param acceleration: Acceleration value (0 for unlimited, or a number between 1-255)
		"""
		if 0 <= motor_index < 5:
			self.controller.setAccel(self.channels[motor_index], acceleration)
		else:
			raise ValueError("Invalid motor index. Must be between 0 and 4.")

	def get_motor_position(self, motor_index):
		"""
		Get the current position of the motor.

		:param motor_index: Index of the motor (0-4)
		:return: Current position of the motor (in quarter-microseconds)
		"""
		if 0 <= motor_index < 5:
			return self.controller.getPosition(self.channels[motor_index])
		else:
			raise ValueError("Invalid motor index. Must be between 0 and 4.")

	def is_motor_moving(self, motor_index):
		"""
		Check if the motor is moving.

		:param motor_index: Index of the motor (0-4)
		:return: True if motor is moving, False otherwise
		"""
		if 0 <= motor_index < 5:
			return self.controller.isMoving(self.channels[motor_index])
		else:
			raise ValueError("Invalid motor index. Must be between 0 and 4.")

	def all_motors_reached_target(self):
		"""
		Check if all motors have reached their target positions.

		:return: True if all motors have reached their target, False otherwise
		"""
		return not self.controller.getMovingState()

	def stop_all_motors(self):
		"""
		Stops all motor movements by stopping the script running on the Maestro.
		"""
		self.controller.stopScript()
