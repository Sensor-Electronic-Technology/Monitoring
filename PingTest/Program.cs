// See https://aka.ms/new-console-template for more information
using System;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.ComponentModel;
using System.Threading;


Console.WriteLine("Hello, World!");

AutoResetEvent waiter1 = new AutoResetEvent (false);
AutoResetEvent waiter2 = new AutoResetEvent (false);
AutoResetEvent waiter3 = new AutoResetEvent (false);
AutoResetEvent waiter4 = new AutoResetEvent (false);
Ping pingSender = new Ping ();

// When the PingCompleted event is raised,
// the PingCompletedCallback method is called.
pingSender.PingCompleted += new PingCompletedEventHandler (PingCompletedCallback);

// Create a buffer of 32 bytes of data to be transmitted.
string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
byte[] buffer = Encoding.ASCII.GetBytes (data);

// Wait 12 seconds for a reply.
int timeout = 12000;

// Set options for transmission:
// The data can go through 64 gateways or routers
// before it is destroyed, and the data packet
// cannot be fragmented.
PingOptions options = new PingOptions (64, true);

Console.WriteLine ("Time to live: {0}", options.Ttl);
Console.WriteLine ("Don't fragment: {0}", options.DontFragment);

// Send the ping asynchronously.
// Use the waiter as the user token.
// When the callback completes, it can wake up this thread.
var reply1=await pingSender.SendPingAsync("172.20.5.42", timeout, buffer, options);
var reply2=await pingSender.SendPingAsync("172.20.5.39", timeout, buffer, options);
var reply3=await pingSender.SendPingAsync("172.20.5.201", timeout, buffer, options);
var reply4=await pingSender.SendPingAsync("172.21.100.29", timeout, buffer, options);

Console.WriteLine($"{reply1.Address} Reply: {reply1.Status}");
Console.WriteLine($"{reply2.Address} Reply: {reply2.Status}");
Console.WriteLine($"{reply3.Address} Reply: {reply3.Status}");
Console.WriteLine($"{reply4.Address} Reply: {reply4.Status}");

// Prevent this example application from ending.
// A real application should do something useful
// when possible.
Console.WriteLine ("Ping example completed.");

static void PingCompletedCallback (object sender, PingCompletedEventArgs e)
{
    // If the operation was canceled, display a message to the user.
    if (e.Cancelled)
    {
        Console.WriteLine ("Ping canceled.");

        // Let the main thread resume.
        // UserToken is the AutoResetEvent object that the main thread
        // is waiting for.
        ((AutoResetEvent)e.UserState).Set ();
    }

    // If an error occurred, display the exception to the user.
    if (e.Error != null)
    {
        Console.WriteLine ("Ping failed:");
        Console.WriteLine (e.Error.ToString ());

        // Let the main thread resume.
        ((AutoResetEvent)e.UserState).Set();
    }

    PingReply reply = e.Reply;

    DisplayReply (reply);

    // Let the main thread resume.
    ((AutoResetEvent)e.UserState).Set();
}

static void DisplayReply (PingReply reply)
{
    if (reply == null)
        return;

    Console.WriteLine ("ping status: {0}", reply.Status);
    if (reply.Status == IPStatus.Success)
    {
        Console.WriteLine ("Address: {0}", reply.Address.ToString ());
        Console.WriteLine ("RoundTrip time: {0}", reply.RoundtripTime);
        Console.WriteLine ("Time to live: {0}", reply.Options.Ttl);
        Console.WriteLine ("Don't fragment: {0}", reply.Options.DontFragment);
        Console.WriteLine ("Buffer size: {0}", reply.Buffer.Length);
    }
}