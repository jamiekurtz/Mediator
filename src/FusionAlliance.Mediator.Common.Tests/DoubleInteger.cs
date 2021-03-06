﻿namespace FusionAlliance.Mediator.Common.Tests
{
    public class DoubleInteger : IRequest<DoubleIntegerReply>
    {
        private readonly int _value;

        public DoubleInteger(int value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return string.Format("Value: {0}", _value);
        }

        public int Value
        {
            get { return _value; }
        }
    }
}
