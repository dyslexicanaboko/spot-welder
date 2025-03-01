﻿using NUnit.Framework;
using System.Collections.Generic;
using SpotWelder.Lib;
using SpotWelder.Lib.Models;
using SpotWelder.Lib.Services.CodeFactory;
using SpotWelder.Lib.Services.Generators;

namespace SpotWelder.Tests.Lib.Services.Generator
{
    [TestFixture]
    public class ModelGeneratorTests
        : TestBase
    {
        private readonly ClassEntityGenerator _generator;

        public ModelGeneratorTests()
        {
            var instructions = new ClassInstructions();

            _generator = new ClassEntityGenerator(instructions);
        }

        [Test]
        public void Empty_list_creates_no_attributes()
        {
            //Arrange
            var lst = new List<string>();

            //Act
            var actual = InvokePrivateMethod<ClassEntityGenerator, string>(_generator, "FormatClassAttributes", lst);

            //Assert
            Assert.That(string.Empty, Is.EqualTo(actual));
        }

        [Test]
        public void Empty_list_creates_no_namespaces()
        {
            //Arrange
            var lst = new List<string>();

            //Act
            var actual = InvokePrivateMethod<ClassEntityGenerator, string>(_generator, "FormatNamespaces", lst);

            //Assert
            Assert.That(string.Empty, Is.EqualTo(actual));
        }

        [Test]
        public void Empty_list_creates_no_properties()
        {
            //Arrange
            var lst = new List<ClassMemberStrings>();

            //Act
            var actual = InvokePrivateMethod<ClassEntityGenerator, string>(_generator, "FormatProperties", lst);

            //Assert
            Assert.That(string.Empty, Is.EqualTo(actual));
        }

        [Test]
        public void One_attribute_creates_expected_string()
        {
            //Arrange
            var item = "FakeAttribute";

            var lst = new List<string>
            {
                item
            };

            var expected = "[" + item + "]";

            //Act
            var actual = InvokePrivateMethod<ClassEntityGenerator, string>(_generator, "FormatClassAttributes", lst);

            //Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void One_namespace_creates_expected_string()
        {
            //Arrange
            var item = "Fake.NameSpace";

            var lst = new List<string>
            {
                item
            };

            var expected = "using " + item + ";";

            //Act
            var actual = InvokePrivateMethod<ClassEntityGenerator, string>(_generator, "FormatNamespaces", lst);

            //Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Test]
        public void One_ClassMemberString_creates_expected_string()
        {
            //Arrange
            var dc = GetSchemaColumn(typeof(int), false);

            var item = new ClassMemberStrings(dc, CodeType.CSharp);

            var lst = new List<ClassMemberStrings>
            {
                item
            };

            var expected = "        public int DoesNotMatter { get; set; }";

            //Act
            var actual = InvokePrivateMethod<ClassEntityGenerator, string>(_generator, "FormatProperties", lst);

            //Assert
            Assert.That(expected, Is.EqualTo(actual));
        }
    }
}
