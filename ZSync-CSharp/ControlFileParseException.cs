using System;

namespace ZSync
{
    public class ControlFileParseException : Exception
    {
        internal ControlFileParseException(string keyExpectedNotFound) : base(keyExpectedNotFound){}
    }
}