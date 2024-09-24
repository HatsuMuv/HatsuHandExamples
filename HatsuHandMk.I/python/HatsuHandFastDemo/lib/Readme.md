## API Overview: `RobotHandAPI`

The `RobotHandAPI` class is an abstraction layer for controlling a 5-DOF robot hand through the Pololu Maestro servo motor controller. It provides several functions to set motor positions, speeds, and accelerations, as well as retrieve motor statuses. Below is a description of the available methods:

### Initialization

#### `__init__(self, device=0x0c)`
- **Description**: Initializes the `RobotHandAPI` instance and connects to the Pololu Maestro controller.
- **Parameters**:
  - `device` (optional): The device number for the Pololu controller (default is `0x0c`).
- **Usage**: Creates a connection to the Pololu Maestro controller and defines motor channels.

#### `__enter__(self)` / `__exit__(self, exc_type, exc_val, exc_tb)`
- **Description**: Allows the `RobotHandAPI` to be used as a context manager in `with` statements. Ensures proper closing of the controller connection.

### Methods

#### `find_pololu_port(self)`
- **Description**: Searches for the Pololu device's port based on its VID and PID. If found, returns the port string.
- **Returns**: The device port string if found, or `None` if no device is detected.

#### `set_motor_range(self, motor_index, min, max)`
- **Description**: Sets the position range for a specified motor.
- **Parameters**:
  - `motor_index`: The index of the motor (0-4).
  - `min`: Minimum position (in quarter-microseconds).
  - `max`: Maximum position (in quarter-microseconds).
- **Example**: 
  ```python
  hand.set_motor_range(0, 4000, 8000)
  ```

#### `set_motor_position(self, motor_index, target)`
- **Description**: Sets the target position for a specified motor.
- **Parameters**:
  - `motor_index`: The index of the motor (0-4).
  - `target`: Target position (in quarter-microseconds).
- **Example**: 
  ```python
  hand.set_motor_position(1, 6000)
  ```

#### `set_motor_speed(self, motor_index, speed)`
- **Description**: Sets the movement speed for a specified motor.
- **Parameters**:
  - `motor_index`: The index of the motor (0-4).
  - `speed`: Speed value (0 for unlimited, or a number between 1-255).
- **Example**: 
  ```python
  hand.set_motor_speed(2, 100)
  ```

#### `set_motor_acceleration(self, motor_index, acceleration)`
- **Description**: Sets the acceleration for a specified motor.
- **Parameters**:
  - `motor_index`: The index of the motor (0-4).
  - `acceleration`: Acceleration value (0 for unlimited, or a number between 1-255).
- **Example**: 
  ```python
  hand.set_motor_acceleration(3, 50)
  ```

#### `get_motor_position(self, motor_index)`
- **Description**: Retrieves the current position of a specified motor.
- **Parameters**:
  - `motor_index`: The index of the motor (0-4).
- **Returns**: The current position of the motor (in quarter-microseconds).
- **Example**: 
  ```python
  position = hand.get_motor_position(0)
  print(f"Motor 0 position: {position}")
  ```

#### `is_motor_moving(self, motor_index)`
- **Description**: Checks whether a specified motor is currently moving.
- **Parameters**:
  - `motor_index`: The index of the motor (0-4).
- **Returns**: `True` if the motor is moving, `False` otherwise.
- **Example**: 
  ```python
  if hand.is_motor_moving(1):
      print("Motor 1 is moving")
  ```

#### `all_motors_reached_target(self)`
- **Description**: Checks whether all motors have reached their target positions.
- **Returns**: `True` if all motors have reached their target positions, `False` otherwise.
- **Example**: 
  ```python
  if hand.all_motors_reached_target():
      print("All motors have reached their target positions")
  ```

#### `stop_all_motors(self)`
- **Description**: Stops all motor movements by halting the script running on the Maestro controller.
- **Example**: 
  ```python
  hand.stop_all_motors()
  ```

### Example Usage

Here is a basic example of how to use the `RobotHandAPI` with a context manager:

```python
from HatsuHandAPI import RobotHandAPI

# Using the context manager to control the robot hand
with RobotHandAPI() as hand:
    # Set motor parameters
    hand.set_motor_speed(0, 60)
    hand.set_motor_acceleration(0, 10)
    
    # Move motor 0 to position 6000
    hand.set_motor_position(0, 6000)

    # Print current position of motor 0
    print(hand.get_motor_position(0))

    # Check if motor is moving
    if hand.is_motor_moving(0):
        print("Motor 0 is still moving")

    # Stop all motors
    hand.stop_all_motors()
```

This API provides a simple and structured way to control a robot hand using Pololu Maestro, with the ability to adjust motor parameters and handle motor states dynamically.
```

This section explains the `RobotHandAPI` class in detail, describing all methods, parameters, and providing usage examples for easy reference. You can include this in your `README.md` to guide users on how to interact with your API.