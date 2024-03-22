Setting up
----------
Clone this folder into the Assets folder of an existing Unity project and you should be ready to roll. If you are using Unity 2019 and above, this should work. I suspect it will also work with 2018 and 2017, but it has not been tested.

This repo contains only a folder in the Assets folder instead of the entire Unity project because it is meant to become an Asset Store asset in the future, so doing it this way makes it easier for us to test the files across different versions of Unity.

How to use
----------
To use the Virtual Joystick, pick one of the prefabs from the `VirtualJoystick/Prefabs` folder into your Scene's HUD Canvas and it should be ready to roll.

1. To retrieve input data from the Joystick, use `VirtualJoystick.GetAxis("Horizontal")` to get movement on the x-axis, and `VirtualJoystick.GetAxis("Vertical")` to get movement on the y-axis.
2. You can also use `VirtualJoystick.GetAxis()` to get a `Vector2` containing the horizontal and vertical inputs.
3. You can have multiple joysticks on your Canvas. In the case of multiple joysticks, you will need to specify an index behind `GetAxis()`, like so: `VirtualJoystick.GetAxis("Horizontal", 1)`. In this case, the code will give you the horizontal input of the 2nd joystick on the Scene (the first joystick is index 0).
4. Some mobile games have joysticks that will shift to the player's finger. To enable this feature on your virtual joystick instance, set the **W** and **H** properties in the **Bounds** property and this will denote the area where the joystick can move around in.

Other properties
----------------
To see a comprehensive list of properties, go here: terresquall.com/games/virtual-joystick-pack/#guide