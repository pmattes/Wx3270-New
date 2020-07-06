// <copyright file="IPAddressConverter.cs" company="Paul Mattes">
//     Copyright (c) Paul Mattes. All rights reserved.
// </copyright>

namespace Wx3270
{
    using System;
    using System.Net;
    using Newtonsoft.Json;

    /// <summary>
    /// JSON converter for an <see cref="IPAddress"/>.
    /// </summary>
    public class IPAddressConverter : JsonConverter
    {
        /// <summary>
        /// Can-convert method.
        /// </summary>
        /// <param name="objectType">Object type to test</param>
        /// <returns>True if it can be converted</returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(IPAddress);
        }

        /// <summary>
        /// JSON write method.
        /// </summary>
        /// <param name="writer">JSON writer</param>
        /// <param name="value">Object to write</param>
        /// <param name="serializer">JSON serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }

        /// <summary>
        /// JSON read method.
        /// </summary>
        /// <param name="reader">JSON reader</param>
        /// <param name="objectType">Object type</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">JSON serializer</param>
        /// <returns>New object</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return IPAddress.Parse((string)reader.Value);
        }
    }
}
