using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NDepend.Path;
using NUnit.Framework;
using ZSync;

namespace ZsyncTests
{
    [TestFixture]
    public class ControlFileTests
    {
        [Test]
        public void CanOpenFile()
        {
            var path = @"".ToAbsoluteFilePath();
            var cf = ControlFile.Create(path);

            cf.Version.Should().Be("0.6.2");
        }
    }
}
