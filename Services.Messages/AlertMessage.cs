using System;

namespace Services.Messages
{
    public class AlertMessage
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return $"Alert='{Text}'";
        }

    }
}
