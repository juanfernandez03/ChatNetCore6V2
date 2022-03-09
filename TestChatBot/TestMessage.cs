using System;
using System.Collections.Generic;
using Castle.Core.Configuration;
using ChatNetCore6.Data;
using ChatNetCore6.Models;
using ChatNetCore6.Services;
using GenFu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace TestChatBot
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }
        static ApplicationDbContext context;
        [Test]
        public void CreateMessages()
        {
            ManageMessage manageMessage = Arrage();
            var messageList = this.GetMessageList();
            foreach (var item in messageList)
            {
                Random rnd = new Random();
                item.Id = rnd.Next(99999999);
                var msg = manageMessage.CreateNewMessage(item);
                Assert.AreEqual(item, msg);
            }
        }
        [Test]
        public void GetMessage()
        {
            ManageMessage manageMessage = Arrage();
            var result = manageMessage.GetMessages();
            Assert.AreEqual(50, result.Count);
            context.Database.EnsureDeleted();
        }

        private static ManageMessage Arrage()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Messages")
            .Options;
            context = new ApplicationDbContext(options);
            ManageMessage manageMessage = new ManageMessage(context);
            return manageMessage;
        }

 
        public IList<Message> GetMessageList()
        {
            A.Configure<Message>()
                .Fill(x => x.Text).AsCity()
                .Fill(x => x.UserID).AsMusicArtistName()
                .Fill(x => x.UserName).AsMusicArtistName();
            return A.ListOf<Message>(55);
        }

        //[Test]
        //public void Test2()
        //{
        //    public RmqProducerService(IServiceProvider serviceProvider, ILogger<RmqConsumerService> logger, IConfiguration config)

        //    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        //    var service = new Mock<IServiceProvider>();
        //    var loggerStub = new Mock<ILogger<RmqProducerService>>();
        //    var configurtion = new Mock<IConfiguration>();

        //    RmqProducerService p = new RmqProducerService(service,loggerStub,configurtion);
        //    p.Produce("dsfasd");
        //    var messageList = this.GetMessageList();
        //    foreach (var item in messageList)
        //    {
        //        var msg = manageMessage.CreateNewMessage(item);
        //        Assert.AreEqual(msg, item);
        //    }
        //}
    }
}