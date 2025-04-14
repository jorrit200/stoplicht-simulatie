using Godot;
using NetMQ;
using NetMQ.Sockets;
using StoplichtSimGodot.interfaces;

namespace StoplichtSimGodot.scripts;

public partial class ZmqPublisher : Node, IMessagePublisher
{
    private PublisherSocket publisher;
    private string _ip = "tcp://10.121.17.6:5559";

    public override void _Ready()
    {
        publisher = new PublisherSocket();
        publisher.Bind(_ip); // Bind naar poort
    }

    public void Send(string topic, string message)
    {
        publisher.SendMoreFrame(topic).SendFrame(message);
        //GD.Print(topic, "", message);
    }

    public override void _ExitTree()
    {
        publisher.Close();
    }
}