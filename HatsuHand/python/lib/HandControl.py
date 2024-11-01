import time
from lib.HatsuHandAPI import RobotHandAPI


class HatsuHandControl():
	def __init__(self,hand : RobotHandAPI):
		self.hand = hand

	def OpenHand(self):
		for i in reversed(range(5)):
			self.hand.set_motor_position(i, 4000)
			if i==4:
				time.sleep(0.5)

	def CloseHand(self):
		for i in range(5):
			self.hand.set_motor_position(i, 8000)
			if i==3:
				time.sleep(0.5)
			
	
	def FingerWave(self):
		fingers = [0, 1, 2, 3] 
		close_position = 8000  
		open_position = 4000   
		delay = 0.3 
		
		for i in range(3):
			for finger in fingers:
				self.hand.set_motor_position(finger, close_position)
				time.sleep(delay)
		
			for finger in fingers:
				self.hand.set_motor_position(finger, open_position)
				time.sleep(delay) 
			time.sleep(delay)
	
	def CountBinaryNumber(self):
		binary_number_list = []
		for i in range(1,16):
			binary_number_list.append("".join(reversed(f'{i:0=4b}')))
		
		for bin in binary_number_list:
			for i in range(4):
				self.hand.set_motor_position(i, 4000 if bin[i] == '1' else 8000)
			time.sleep(1.5)   
	
	
	