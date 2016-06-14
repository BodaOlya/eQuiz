using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eQuiz.Repositories.Abstract;
using Moq;
using AdminQuizzesController = eQuiz.Web.Areas.Admin.Controllers.QuizzesController;

namespace eQuiz.Web.Tests.Areas.Admin.Controllers
{
    [TestClass]
    class QuizzesControllerTest
    {
        private AdminQuizzesController _controller;
        private Mock<IRepository> _mockRepository = new Mock<IRepository>();

        [TestInitialize]
        public void BeforeTestMethod()
        {

            
        }


    }
}
