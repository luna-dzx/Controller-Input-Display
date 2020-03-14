# Controller-Input-Display
An input display that reads directly from XInput controllers, with a customizable look. Based on https://github.com/TurtleMan64/SADX-SA2-Input-Display  
    
![Imgur](https://refrag.s-ul.eu/EdMDkU3w)    

### Download the exe:     
https://github.com/R3FR4G/Controller-Input-Display/releases/latest     
     
     
#### How to compile:     
     
Compile JoystickDisplay.cs as a DLL with the command:    
`C:/Windows/Microsoft.NET/Framework/v4.0.30319/csc.exe /target:library /out:joydisp.DLL /reference:SharpDX.XInput.DLL /reference:SharpDX.DLL JoystickDisplay.cs`    
    
Now compile the main SonicInputDisplay.cs while linking the library:    
`C:/Windows/Microsoft.NET/Framework/v4.0.30319/csc.exe /out:SonicInputDisplay.exe /reference:joydisp.DLL SonicInputDisplay.cs`    
    
Now you have the `SonicInputDisplay.exe` that you can run. Make sure that you run it in the same folder as the res folder that contains all of the images.     
