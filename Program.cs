using Akka.Actor;
using System;

namespace ActorsPlayground
{
    public interface ITrackable
    {
        long RequestId { get; }
    }

    public sealed class ReadTemperature : ITrackable
    {
        public long RequestId { get; private set; }

        public ReadTemperature(long requestId) => RequestId = requestId;
    }

    public sealed class ResponseTemperature : ITrackable
    {
        public long RequestId { get; private set; }
        public long Temperature { get; private set; }

        public ResponseTemperature(long requestId, long temperature)
        {
            RequestId = requestId;
            Temperature = temperature;
        }
    }

    public sealed class TemperatureRecorder : ITrackable
    {
        public long RequestId { get; private set; }

        public TemperatureRecorder(long requestId) => RequestId = requestId;
    }

    public sealed class RecordTemperature : ITrackable
    {
        public long RequestId { get; private set; }
        public long Temperature { get; private set; }

        public RecordTemperature(long requestId, long temperature)
        {
            RequestId = requestId;
            Temperature = temperature;
        }
    }


    public sealed class DeviceActor : UntypedActor
    {
        public long Temperature { get; private set; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RecordTemperature record:
                    Temperature = record.Temperature;
                    Sender.Tell(new TemperatureRecorder(record.RequestId));
                    break;
                case ReadTemperature read:
                    Sender.Tell(new ResponseTemperature(read.RequestId, Temperature));
                    break;
            }
        }

        public static Props Props() => Akka.Actor.Props.Create<DeviceActor>();
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var system = ActorSystem.Create("iot-system"))
            {
                // Create top level supervisor
                var device = system.ActorOf(DeviceActor.Props(), "device");

                device.Tell(new RecordTemperature(1, 37));
                
                var temperature = device.Ask<ResponseTemperature>(new ReadTemperature(1)).Result;
                Console.WriteLine(temperature.Temperature);

                

                // Exit the system after ENTER is pressed
                Console.ReadLine();
            }
        }
    }
}
