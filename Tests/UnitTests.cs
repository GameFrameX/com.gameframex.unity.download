using System;
using GameFrameX.Download.Runtime;
using GameFrameX.Runtime;
using NUnit.Framework;

namespace GameFrameX.Download.Tests
{
    internal class UnitTests
    {
        private DateTime dateTime, dateTime1;

        [SetUp]
        public void Setup()
        {
            dateTime = DateTime.Now;
            dateTime1 = DateTime.Now.AddHours(1);
        }

        [TearDown]
        public void TearDown()
        {
            GameFrameXOptions.Reset(); // 清除 Options 注册，避免跨用例静态泄漏
        }

        // Here is an example of a unit test for the IsUnixSameDay method
        [Test]
        public void TestIsUnixSameDay()
        {
            // Arrange
            // long timestamp1 = 1617842400; // April 7, 2021 12:00:00 AM UTC
            // long timestamp2 = 1617896400; // April 7, 2021 12:00:00 PM UTC

            // Act
        }


        [Test]
        public void Test1()
        {
            Assert.That(dateTime1.Year, Is.EqualTo(dateTime.Year));
            Assert.That(dateTime1.Month, Is.EqualTo(dateTime.Month));
            Assert.That(dateTime1.Day, Is.EqualTo(dateTime.Day));
        }

        [Test]
        public void DownloadOptions_ConfigureAndTryGet_RoundTripsDiffs()
        {
            Func<DownloadAgentHelperBase> downloadAgentHelperFactory = () => null;
            GameFrameXOptions.Reset();
            GameFrameXOptions.Configure(new DownloadOptions { AgentHelperCount = 8, Timeout = 60f, FlushSize = 2048, DownloadAgentHelperFactory = downloadAgentHelperFactory });

            Assert.IsTrue(GameFrameXOptions.TryGet<DownloadOptions>(out var options));
            Assert.That(options.AgentHelperCount, Is.EqualTo(8));
            Assert.That(options.Timeout, Is.EqualTo(60f));
            Assert.That(options.FlushSize, Is.EqualTo(2048));
            Assert.That(options.DownloadAgentHelperFactory, Is.SameAs(downloadAgentHelperFactory));

            GameFrameXOptions.Reset();
            var defaults = new DownloadOptions();
            Assert.That(defaults.AgentHelperCount, Is.EqualTo(3));
            Assert.That(defaults.Timeout, Is.EqualTo(30f));
            Assert.That(defaults.FlushSize, Is.EqualTo(1024 * 1024));
            var downloadAgentHelper = defaults.DownloadAgentHelperFactory();
            Assert.That(downloadAgentHelper, Is.InstanceOf<UnityWebRequestDownloadAgentHelper>());
            UnityEngine.Object.DestroyImmediate(downloadAgentHelper);
        }
    }
}