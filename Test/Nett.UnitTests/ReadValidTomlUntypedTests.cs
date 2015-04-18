﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nett.UnitTests
{
    public class ReadValidTomlUntypedTests
    {
        [Fact, ]
        public void ReadValidToml_EmptyArray()
        {
            // Arrange
            var toml = TomlStrings.Valid.EmptyArray;

            // Act
            var read = Toml.Read(toml);

            // Assert
            Assert.NotNull(read);
            Assert.Equal(1, read.Rows.Count);
            Assert.Equal(typeof(TomlArray), read.Rows["thevoid"].GetType());
            var rootArray = read.Rows["thevoid"].Get<TomlArray>();
            Assert.Equal(1, rootArray.Count);
            var subArray = rootArray.Get<TomlArray>(0);
            Assert.Equal(0, subArray.Count);
        }

        [Fact]
        public void ReadValidToml_ArrayNoSpaces()
        {
            // Arrange
            var toml = TomlStrings.Valid.ArrayNoSpaces;

            // Act
            var read = Toml.Read(toml);

            // Assert
            Assert.Equal(1, read.Get<TomlArray>("ints").Get<int>(0));
            Assert.Equal(2, read.Get<TomlArray>("ints").Get<int>(1));
            Assert.Equal(3, read.Get<TomlArray>("ints").Get<int>(2));
        }

        [Fact]
        public void ReadValidToml_HetArray()
        {
            // Arrange
            var toml = TomlStrings.Valid.HetArray;

            // Act
            var read = Toml.Read(toml);

            // Assert
            var a = read.Get<TomlArray>("mixed");
            Assert.NotNull(a);
            Assert.Equal(3, a.Count);

            var intArray = a.Get<TomlArray>(0);
            Assert.Equal(1, intArray.Get<int>(0));
            Assert.Equal(2, intArray.Get<int>(1));

            var stringArray = a.Get<TomlArray>(1);
            Assert.Equal("a", stringArray.Get<string>(0));
            Assert.Equal("b", stringArray.Get<string>(1));

            var doubleArray = a.Get<TomlArray>(2);
            Assert.Equal(1.1, doubleArray.Get<double>(0));
            Assert.Equal(2.1, doubleArray.Get<double>(1));
        }

        [Fact]
        public void RealValidToml_NestedArrays()
        {
            // Arrange
            var toml = TomlStrings.Valid.ArraysNested;

            // Act
            var read = Toml.Read(toml);

            // Assert
            var a = read.Get<TomlArray>("nest");
            Assert.Equal("a", a.Get<TomlArray>(0).Get<string>(0));
            Assert.Equal("b", a.Get<TomlArray>(1).Get<string>(0));
        }

        [Fact]
        public void ReadValidToml_Arrays()
        {
            // Arrange
            var toml = TomlStrings.Valid.Arrays;

            // Act
            var read = Toml.Read(toml);

            // Assert
            Assert.Equal(3, read.Get<TomlArray>("ints").Get<TomlArray>().Count);
            Assert.Equal(3, read.Get<TomlArray>("floats").Get<TomlArray>().Count);
            Assert.Equal(3, read.Get<TomlArray>("strings").Get<TomlArray>().Count);
            Assert.Equal(3, read.Get<TomlArray>("dates").Get<TomlArray>().Count);
        }
    }
}
