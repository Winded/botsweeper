using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// Yield instruction for waiting on a TCP socket to connect.
/// Uses the asynchronouse <see cref="TcpClient.BeginConnect(string, int, AsyncCallback, object)"/> method.
/// </summary>
public class WaitForConnect : CustomYieldInstruction
{
    public WaitForConnect(TcpClient client, string host, int port) : this(client, host, port, 10f)
    {
    }

    public WaitForConnect(TcpClient client, string host, int port, float timeout)
    {
        mClient = client;
        mTimeout = timeout;
        mStartTime = Time.time;

        mComplete = false;
        mClient.BeginConnect(host, port, OnConnectResult, mClient);
    }

    public override bool keepWaiting
    {
        get
        {
            if (Time.time - mStartTime >= mTimeout)
            {
                return false;
            }
            return !mComplete;
        }
    }

    private TcpClient mClient;
    private float mTimeout;

    private float mStartTime;
    private bool mComplete;

    private void OnConnectResult(IAsyncResult ar)
    {
        mComplete = true;
    }
}
