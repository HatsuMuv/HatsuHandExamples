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
using System.ComponentModel;
using System.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class MainWindow : MonoBehaviour
{
    private int MIN_LIMIT = 4096;
    private int MIN_FOURFINGERS_LIMIT = 5000;
    private int MIN_LEFTSUM_LIMIT = 6600;
    private int MAX_SUM_LIMIT = 8000;
    private int MAX_LIMIT = 8384;

    public enum Target
    {
        LeftHand,
        RightHand,
    }

    public void MoveRightFinger(int i, int goalint)
    {
        Byte Fnum = (Byte)i;
        float goalfloat = MIN_LIMIT;
        if (i != 4) goalfloat = Remap((float)goalint, 0f, 60f, (float)MIN_FOURFINGERS_LIMIT, (float)MAX_LIMIT);
        else goalfloat = Remap((float)goalint, 0f, 60f, (float)MIN_FOURFINGERS_LIMIT, (float)MAX_SUM_LIMIT);
        UInt16 goal = (ushort)goalfloat;
        //Debug.Log(Fnum + "+," + goal);
        TrySetTarget(Fnum, goal, Target.RightHand);
    }

    public void MoveLeftFinger(int i, int goalint)
    {
        Byte Fnum = (Byte)i;
        float goalfloat = MIN_LIMIT;
        if (i != 4) goalfloat = Remap((float)goalint, 0f, 60f, (float)MIN_LIMIT, (float)MAX_LIMIT);
        else goalfloat = Remap((float)goalint, 0f, 60f, (float)MIN_LEFTSUM_LIMIT, (float)MAX_SUM_LIMIT);
        UInt16 goal = (ushort)goalfloat;
        //Debug.Log(Fnum + "+," + goal);
        TrySetTarget(Fnum, goal, Target.LeftHand);
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
    public void TrySetTarget(Byte channel, UInt16 target, Target t)
    {
        try
        {
            using (Usc device = connectToDevice(t))  // Find a device and temporarily connect.
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
    Usc connectToDevice(Target t)
    {
        // Get a list of all connected devices of this type.
        List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();

        foreach (DeviceListItem dli in connectedDevices)
        {
            // If you have multiple devices connected and want to select a particular
            // device by serial number, you could simply add a line like this:
            if (t == Target.LeftHand)
            {
                if (dli.serialNumber != "00400211") { continue; }
            }
            else if (t == Target.RightHand)
            {
                if (dli.serialNumber != "00400209") { continue; }
            }
            else
            {
                Debug.Log("[Pololu] Undefinded Target!");
                continue;
            }

            Usc device = new Usc(dli); // Connect to the device.
            return device;             // Return the device.
        }
        throw new Exception("Could not find device.  Make sure it is plugged in to USB " +
            "and check your Device Manager (Windows) or run lsusb (Linux).");
    }

}
