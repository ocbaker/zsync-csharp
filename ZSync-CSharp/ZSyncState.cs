using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NDepend.Path;
[assembly: InternalsVisibleTo("ZSyncTests")]
namespace ZSync
{
    
    class ControlFile
    {
        private ControlFile() { }
        public string Version { get; private set; }
        public string Filename { get; private set; }
        public string MTime { get; private set; }
        public int Blocksize { get; private set; }
        public long Length { get; private set; }
        public IReadOnlyList<int> HashLengths { get; private set; }
        public string Url { get; private set; }
        public string Sha1 { get; private set; }

        internal static ControlFile Create(IAbsoluteFilePath controlFile)
        {
            var @this = new ControlFile();

            var lines = new Queue<string>(File.ReadAllLines(controlFile.ToString()));
            
            @this.Version = GetValue("zsync", lines.Dequeue());
            @this.Filename = GetValue("Filename", lines.Dequeue());
            @this.MTime = GetValue("MTime", lines.Dequeue());
            @this.Blocksize = Convert.ToInt32(GetValue("Blocksize", lines.Dequeue()));
            @this.Length = Convert.ToInt64(GetValue("Length", lines.Dequeue()));
            @this.HashLengths = GetValue("Hash-Lengths", lines.Dequeue()).Split(',').Select(int.Parse).ToList();
            @this.Url = GetValue("URL", lines.Dequeue());
            @this.Sha1 = GetValue("SHA-1", lines.Dequeue());

            if(lines.Dequeue() != "")
                throw new ControlFileParseException("Expected End of Headers");



            return @this;
        }

        private static string GetValue(string keyExpected, string line, Expression<Func<string, bool>> checkFunc)
        {
            var val = GetValue(keyExpected, line);
            if(!checkFunc.Compile()(val))
                throw new ControlFileParseException("Value for Key invalid");
            return val;
        }

        private static string GetValue(string keyExpected, string line)
        {
            var kvp = GetLine(line);

            if (kvp.Key != keyExpected)
                throw new ControlFileParseException("Key Expected Not Found");

            return kvp.Value;
        }
        
        private static Regex _lineFormat = new Regex("(?<key>[^:]*):[ ](?<value>.*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
        private static KeyValuePair<string, string> GetLine(string dequeue)
        {
            var match = _lineFormat.Match(dequeue);
            return new KeyValuePair<string, string>(match.Groups["key"].Value, match.Groups["value"].Value);
        }
    }
}
