﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Console.Wpf.Tests.VSTS.DevTests.given_shell_service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Console.Wpf.ViewModel.Services;
using Microsoft.Win32;
using System.IO;
using System.Configuration;


namespace Console.Wpf.Tests.VSTS.DevTests.given_application_model
{
    [DeploymentItem("configuration_error.config")]
    [TestClass]
    public class when_opening_configuration_source_with_errors : given_clean_appllication_model
    {
        protected override void Arrange()
        {
            base.Arrange();
            string errorSectionConfigurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configuration_error.config");
            
            UIServiceMock.Setup(x => x.ShowFileDialog(It.IsAny<OpenFileDialog>()))
                         .Returns(new FileDialogResult { DialogResult = true, FileName = errorSectionConfigurationPath });
            
            UIServiceMock.Setup(x => x.ShowError(It.IsAny<ConfigurationErrorsException>(), It.IsAny<string>()))
                .Verifiable();
        }

        protected override void Act()
        {
            ApplicationModel.OpenConfigurationSource();
        }

        [TestMethod]
        public void then_error_was_shown()
        {
            UIServiceMock.Verify();
        }

    }
}
