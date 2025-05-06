# HatsuHand Unity Demo

This repository provide demos for using Unity3D game engine to control HatsuHand made by HatsuMuv Corporation

## Requirements

- Unity 2020.3 or later
- [DOTween](http://dotween.demigiant.com/)
- HatsuHand with Maestro USB control interface [Windows](https://www.pololu.com/docs/0J40/3.a) / [Linux](https://www.pololu.com/docs/0J40/3.b)

## Project Setup Prerequirements

**Enabling the .NET 4.x scripting runtime in Unity**

You will need to setup .NET 4.X support in order to start. To enable the .NET 4.x scripting runtime, take the following steps:
- Open PlayerSettings in the Unity Inspector by selecting Edit > Project Settings > Player > Other Settings.
- Under the Configuration heading, click the Api compatibility Level dropdown and select .NET Framework. You'll be prompted to restart Unity.

**Setup your Maestro USB control interface before starting control**

You will need to make sure that your HatsuHand can be controlled through Maestro USB control interface.
- Download and install Maestro USB control interface
- Enable servo in Maestro USB control interface, it will setup your control interface automatically

## Demo Scenes
 - [FastDemo](./Assets/Demo/1_FastDemo/) , Download built version [here](./Build/1_FastDemo_Build.7z)
 - [LeapMotionDemo](./Assets/Demo/2_LeapMotionDemo/) , Download built version [here](./Build/2_LeapMotionDemo_Build.7z)


## License

 This project is released under the MIT License.

 **Third-Party Licenses**

 - **Pololu USB SDK**
   - Some code is used from the [Pololu USB SDK](https://github.com/pololu/pololu-usb-sdk/tree/master/Maestro/MaestroEasyExample).
   - The MIT License applies.

 ## Contact

 If you have any questions or need support, please reach out via the [HatsuMuv Discord](https://discord.gg/JbysAbJWCN).
