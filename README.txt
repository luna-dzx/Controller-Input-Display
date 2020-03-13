Based on the Sonic Input display by jelly https://github.com/TurtleMan64/SADX-SA2-Input-Display
You can edit the deadzone and the behaviour of RT LT, RB LB in the settings.ini file

You can switch between skins by using left/right arrow
keys on your keyboard. New skins can be added by simply
creating a new folder with the same filenames in them.

The program also counts how many times you have pressed
each button. You can view the button counts be pressing
the B key on the keyboard. Pressing R will reset the
button counts to zero.

Modified by Refrag (https://www.speedrun.com/user/Refrag)

You can compile the source the same way as the Sonic Input Display however, you need to add the reference for the SharpDX dlls :

csc.exe /target:library /out:joydisp.DLL /reference:SharpDX.XInput.DLL /reference:SharpDX.DLL JoystickDisplay.cs
csc.exe /out:SonicInputDisplay.exe /reference:joydisp.DLL SonicInputDisplay.cs