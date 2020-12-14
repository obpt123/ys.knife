﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Hosting
{
    [TestClass]
    public class InjectConfigurationTest : KnifeHost
    {
        public InjectConfigurationTest():base(new Dictionary<string, object>
        {
            ["prop_a"]="value from command line",
            ["prop_b"]="value from command line"
        })
        {
        }
        [InjectConfiguration("prop_b")]
        private const string ValueWillOverwriteCommand = "value from injection";
        
        [InjectConfiguration("connectionstrings:private_const")]
        private const string ConnectionStringFromPrivateConst = "value_from_private_const";
        
        [InjectConfiguration("connectionstrings:public_const")]
        public const string ConnectionStringFromPublicConst = "value_from_public_const";

        [InjectConfiguration("connectionstrings:private_static_field")]
        private static string ConnectionStringFromPrivateStaticField = "value_from_private_static_field";
        
        [InjectConfiguration("connectionstrings:public_static_field")]
        public static string ConnectionStringFromPublicStaticField = "value_from_public_static_field";

        [InjectConfiguration("connectionstrings:private_instance_field")]
        private readonly string ConnectionStringFromPrivateInstanceField = "value_from_private_instance_field";

        [InjectConfiguration("connectionstrings:public_instance_field")]
        public string ConnectionStringFromPublicInstanceField = "value_from_public_instance_field";

        [InjectConfiguration("connectionstrings:private_static_property")]
        public static string ConnectionStringFromPrivateStaticProperty { get; set; } = "value_from_private_static_property";

        [InjectConfiguration("connectionstrings:public_static_property")]
        public static string ConnectionStringFromPublicStaticProperty { get; set; } = "value_from_public_static_property";
        
        [InjectConfiguration("connectionstrings:private_instance_property")]
        public string ConnectionStringFromPrivateInstanceProperty { get; set; } = "value_from_private_instance_property";

        [InjectConfiguration("connectionstrings:public_instance_property")]
        public string ConnectionStringFromPublicInstanceProperty { get; set; } = "value_from_public_instance_property";

        [InjectConfiguration("connectionstrings")]
        private IDictionary<string, object> Connections = new Dictionary<string, object>
        {
            ["string_value"] = "string_value",
            ["int_value"] = 123456
        };

        
        [DataRow("private_const", ConnectionStringFromPrivateConst)]
        [DataRow("public_const", ConnectionStringFromPublicConst)]
        [DataRow("private_static_field", "value_from_private_static_field")]
        [DataRow("public_static_field", "value_from_public_static_field")]
        [DataRow("private_instance_field", "value_from_private_instance_field")]
        [DataRow("public_instance_field", "value_from_public_instance_field")]
        [DataRow("private_static_property", "value_from_private_static_property")]
        [DataRow("public_static_property", "value_from_public_static_property")]
        [DataRow("private_instance_property", "value_from_private_instance_property")]
        [DataRow("public_instance_property", "value_from_public_instance_property")]
        [DataRow("string_value", "string_value")]
        [DataRow("int_value", "123456")]
        [DataTestMethod]

        public void ShouldGetConnectionStringsFromInjectValue(string key, string expectedValue)
        {
            var configuration = this.GetService<IConfiguration>();
            Assert.AreEqual(expectedValue, configuration.GetConnectionString(key));
        }

        [TestMethod]
        public void ShouldHaveHighestPriority()
        { 
            var configuration = this.GetService<IConfiguration>();
            Assert.AreEqual("value from command line", configuration.GetSection("prop_a").Get<string>());
            Assert.AreEqual("value from injection", configuration.GetSection("prop_b").Get<string>());
        }
    }
}
