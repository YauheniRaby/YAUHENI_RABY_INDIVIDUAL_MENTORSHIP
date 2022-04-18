using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BusinessLayer.Infrastructure
{
    public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            string format = "yyyy-MM-dd HH:mm:ss";


            var str = reader.GetString();
            Console.WriteLine($"str - {str}");
            
            var V1 = DateTime.Parse(reader.GetString());
            Console.WriteLine($"TestParseV1 - {V1.ToString("1 -dd, 2 -MM, 3 -yyyy HH:mm:ss")}");

            var V2 = DateTime.ParseExact(str, format, provider);
            Console.WriteLine($"TestParseV2 - {V2.ToString("1 -dd, 2 -MM, 3 -yyyy HH:mm:ss")}");


            ////var V2 = DateTime.ParseExact(reader.GetString(), "yyyy-MM-dd HH:mm:ss", null);

            //Console.WriteLine($"TestParseV1 - {V1.ToString("1 -dd, 2 -MM, 3 -yyyy HH:mm:ss")}");
            //Console.WriteLine($"TestParseV2 - {V2.ToString("1 -dd, 2 -MM, 3 -yyyy HH:mm:ss")}");

            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("dd-MM-yyyy HH:mm:ss"));
        }
    }
}
