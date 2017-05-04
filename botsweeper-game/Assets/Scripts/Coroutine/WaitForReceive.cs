using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Yield instruction for waiting on a TCP socket to receive data
/// </summary>
public class WaitForReceive : CustomYieldInstruction
{
    public WaitForReceive(TcpClient client) : this(client, 10f)
    {
    }

    public WaitForReceive(TcpClient client, float timeout)
    {
        mClient = client;
        mTimeout = timeout;
        mStartTime = Time.time;
    }

    public override bool keepWaiting
    {
        get
        {
            if (Time.time - mStartTime >= mTimeout)
            {
                return false;
            }
            return mClient.Available <= 0;
        }
    }

    private TcpClient mClient;
    private float mTimeout;

    private float mStartTime;
}
