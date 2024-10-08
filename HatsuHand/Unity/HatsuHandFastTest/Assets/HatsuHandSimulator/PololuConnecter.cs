/*  MaestroEasyExample:
 *    Simple example GUI for the Maestro USB Servo Controller, written in
 *    Visual C#.
 *    
 *    Features:
 *       Temporary native USB connection using Usc class
 *       Button for disabling channel 0.
 *       Button for setting target of channel 0 to 1000 us.
 *       Button for setting target of channel 0 to 2000 us.
 * 
 *  NOTE: Channel 0 should be configured as a servo channel for this program
 *  to work.  You must also connect USB and servo power, and connect a servo
 *  to channel 0.  If this program does not work, use the Maestro Control
 *  Center to check what errors are occurring.
 */

using Pololu.Usc;
using Pololu.UsbWrapper;

using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PololuConnecter : MonoBehaviour
{
    private const int MIN_LIMIT = 4000;

    private const int MAX_LIMIT = 8000;

    public void Test()
    {
        StartCoroutine(TestCoroutine());
    }

    private IEnumerator TestCoroutine()
    {
        TrySetTarget((Byte)0, 8000);

        yield return new WaitForSeconds(1);

        TrySetTarget((Byte)0, 6000);
    }

    public void MoveFingerWithParam(int fingerNum, float param)
    {
        TrySetTarget((Byte)fingerNum, (UInt16)Remap(param, 0, 1, MIN_LIMIT, MAX_LIMIT));
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    /// <summary>
    /// Attempts to set the target (width of pulses sent) of a channel.
    /// </summary>
    /// <param name="channel">Channel number from 0 to 23.</param>
    /// <param name="target">
    ///   Target, in units of quarter microseconds.  For typical servos,
    ///   6000 is neutral and the acceptable range is 4000-8000.
    /// </param>
    public void TrySetTarget(Byte channel, UInt16 target)
    {
        try
        {
            using (Usc device = ConnectToDevice())  // Find a device and temporarily connect.
            {
                device.setTarget(channel, target);

                // device.Dispose() is called automatically when the "using" block ends,
                // allowing other functions and processes to use the device.
            }
        }
        catch
        {
            //Debug.Log("catch");
        }
    }

    /// <summary>
    /// Connects to a Maestro using native USB and returns the Usc object
    /// representing that connection.  When you are done with the
    /// connection, you should close it using the Dispose() method so that
    /// other processes or functions can connect to the device later.  The
    /// "using" statement can do this automatically for you.
    /// </summary>
    Usc ConnectToDevice()
    {
        // Get a list of all connected devices of this type.
        List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();

        foreach (DeviceListItem dli in connectedDevices)
        {
            Usc device = new Usc(dli); // Connect to the device.
            return device;             // Return the device.
        }
        throw new Exception("Could not find device.  Make sure it is plugged in to USB " +
            "and check your Device Manager (Windows) or run lsusb (Linux).");
    }

}
