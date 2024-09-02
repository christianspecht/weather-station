using Tinkerforge;

namespace WeatherStation
{
    internal class Program
    {
        static BrickletLCD20x4 lcd;
        static BrickletBarometer barometer;

        static void Main(string[] args)
        {
            var ipcon = new IPConnection();
            ipcon.Connect(Settings.Host, Settings.Port);

            ipcon.EnumerateCallback += Con_EnumerateCallback;
            ipcon.Connected += Con_ConnectedCallback;

            ipcon.Enumerate();
            Console.ReadLine();
            lcd.BacklightOff();
        }

        static void Con_ConnectedCallback(IPConnection sender, short connectReason)
        {
            if (connectReason == IPConnection.CONNECT_REASON_AUTO_RECONNECT)
            {
                sender.Enumerate();
            }
        }

        static void Con_EnumerateCallback(IPConnection sender, string uid, string connectedUid, char position, short[] hardwareVersion, short[] firmwareVersion, int deviceIdentifier, short enumerationType)
        {
            if (enumerationType == IPConnection.ENUMERATION_TYPE_CONNECTED || enumerationType == IPConnection.ENUMERATION_TYPE_AVAILABLE)
            {
                if (deviceIdentifier == BrickletLCD20x4.DEVICE_IDENTIFIER)
                {
                    lcd = new BrickletLCD20x4(uid, sender);
                    lcd.ClearDisplay();
                    lcd.BacklightOn();
                    lcd.WriteLine(0, 0, "HELLO " + DateTime.Now.Year.ToString());
                }
                else if (deviceIdentifier == BrickletBarometer.DEVICE_IDENTIFIER)
                {
                    barometer = new BrickletBarometer(uid, sender);
                    barometer.SetAirPressureCallbackPeriod(Settings.CallbackPeriod);
                    barometer.AirPressure += Barometer_AirPressure;
                }
            }
        }

        private static void Barometer_AirPressure(BrickletBarometer sender, int airPressure)
        {
            // TODO: air pressure

            // temperature
            var temp = sender.GetChipTemperature() / 100.0;
            string text = string.Format("Temperature {0:##.00} {1}C", temp, (char)0xDF);
            lcd.WriteLine(2, 0, text);
        }
    }
}
