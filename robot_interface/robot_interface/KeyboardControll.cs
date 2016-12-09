using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace robot_interface
{
    class KeyboardControll
    {

//setting the movement parameters
int MotorMax 		= 130;							
int MaxMultiSteer 	= int(round(MotorMax/float(40)));	
int MotorMulti 		= int(round(MotorMax/float(30)));	//multiplier per step of acceleration/deceleration
int MotorMultiSteer = int(round(MotorMax/float(12))); 	//maximum nr. of steps for acc/dec
//calculate the maximum nr. of steps for acc/dec
int MaxMulti 		= int(round(MotorMax/float(MotorMulti)));
double Correction 		= 1.04;									//correction factor if motors are uneven
//content: nr. of UP,LEFT,RIGHT,DOWN, states of UP,LEFT,RIGHT,DOWN
int[] Times 			= [0,0,0,0,0,0,0,0];
//initialize motor speeds to zero
int Motor1Speed 	= 0;
int Motor2Speed 	= 0;

public void run()
{

	bool app_running = true;
	while app_running:				//#main loop
		time.sleep(0.05)			//#time pitch for key inputs (in seconds)
		events = pygame.event.get()	#capture mouse and keyboard inputs
		for e in events:			#look for interesting events
			if e.type == pygame.QUIT:			#"dummy" window has been closed
				app_running = False				#forces exit from main loop on next loop
				break
			elif e.type == pygame.KEYDOWN:		#at least one key is pressed
				#NothingHappened = 0		
				if e.key == pygame.K_UP:		#<Up Arrow> is pressed
					Times[4] = 1				#set the corresponding flag
				elif e.key == pygame.K_DOWN:	#(and so on)
					Times[7] = 1
				elif e.key == pygame.K_LEFT:
					Times[5] = 1
				elif e.key == pygame.K_RIGHT:
					Times[6] = 1
				if e.key == pygame.K_ESCAPE:	#<Escape> is pressed
					print "EXITING"				
					app_running = False			
			elif e.type == pygame.KEYUP:		#at least one key was released
				case(e.KeyUp)
				if e.key == pygame.K_UP:		#<Up Arrow> was released
					Times[4] = 0				#clears corresponding flag
				elif e.key == pygame.K_DOWN:	#(repeat)
					Times[7] = 0
				elif e.key == pygame.K_LEFT:
					Times[5] = 0
				elif e.key == pygame.K_RIGHT:
					Times[6] = 0
			else:								#no change in keyboard state and no key pressed
				Times[4] = 0					#clear all key flags
				Times[5] = 0					#(just for security reason
				Times[6] = 0					# - flags _should_ get cleared if their
				Times[7] = 0					#corresponding key was released)
				#NothingHappened = 1
		if Times[4] == 0 and Times[0] > 0:		#if flag for <Up Arrow> is cleared,
			Times[0] -= 1  						#decrement counter for <Up Arrow> if not already zero	
		if Times[5] == 0 and Times[1] > 0:		#(like above)
			Times[1] -= 1
		if Times[6] == 0 and Times[2] > 0:
			Times[2] -= 1
		if Times[7] == 0 and Times[3] > 0:
			Times[3] -= 1
		if Times[4] == 1 and Times[0] < MaxMulti:		#flag of <Up Arrow> is set -> counter increment
			Times[0] += 1  								#if not already at maximum
		if Times[5] == 1 and Times[1] < MaxMultiSteer:	#(repeat for each key)
			Times[1] += 1
		if Times[6] == 1 and Times[2] < MaxMultiSteer:
			Times[2] += 1
		if Times[7] == 1 and Times[3] < MaxMulti:
			Times[3] += 1
		#calculating the motor speed for linear movement
		Motor1Speed = (Times[0]-Times[3])*MotorMulti
		#calculating a temporary variable
		MotorPercentage = Motor1Speed/float(MotorMax)
		#calculating the current steering movement
		SteerSpeed = (Times[2]-Times[1])*MotorMultiSteer*MotorPercentage
		#limit the accumulated motor speeds so that steering can take effect
		if Motor1Speed+abs(SteerSpeed) >= MotorMax:
			Motor1Speed = MotorMax-abs(SteerSpeed)
		if Motor1Speed-abs(SteerSpeed) <= -MotorMax:
			Motor1Speed = -MotorMax+abs(SteerSpeed)
		#linear movement --> both motors running at the same speed
		Motor2Speed = Motor1Speed
		#adding the steering speed to the both motors
		Motor1Speed += SteerSpeed
		Motor2Speed += -SteerSpeed		#invert the steering at one motor for more effect
		#3pi can't handle negative motor speeds; instead, it uses different
		#control codes for forward/backward movement
		if Motor1Speed < 0:
			TransmitData[0] = Motor1BWD
			#use the Correction value on one motor to equalize both motors
			#float() has to be used if fractal part is necessary for the calulation
			TransmitData[1] = -int(round(Motor1Speed*float(Correction)))				
		else:
			TransmitData[0] = Motor1FWD
			TransmitData[1] = int(round(Motor1Speed*float(Correction)))
    	if Motor2Speed < 0:
		TransmitData[2] = Motor2BWD
		TransmitData[3] = -int(round(Motor2Speed))
	else:
		TransmitData[2] = Motor2FWD
		TransmitData[3] = int(round(Motor2Speed))
	
	#Now all values are calculated and can be sent to the 3pi! chr() converts the integer values
	#to single bytes that can be understood by the 3pi
	port.write(chr(TransmitData[0])+chr(TransmitData[1])+chr(TransmitData[2])+chr(TransmitData[3]))

#these commands are executed after the main loop was left
pygame.quit()		#close "dummy" windows
port.close()		#close serial communication
if __name__ == '__main__':
    }
}
