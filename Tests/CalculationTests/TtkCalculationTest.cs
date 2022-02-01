using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Tests.CalculationTests.Helpers;
using WarGrapher.Models.Calculation;
using WarGrapher.Models.Calculation.Utility;
using WarGrapher.Models.Equipment;
using P = System.Windows.Point;

namespace Tests.CalculationTests
{
    [TestFixture]
    class TtkCalculationTest
    {
        private CalculationTestHelper _helper;
        private CalculationResultComparer _resultComparer;

        [SetUp]
        public void SetUp()
        {
            _helper = new CalculationTestHelper();
            _resultComparer = new CalculationResultComparer();
        }

        [Test]
        public void GetPlotData_DefaultSourceData_ShouldReturnTrue()
        {
            //arrange
            var appModelMock = _helper.GetModelMock(_helper.DefaultSourceData);
            var testedCalculation = new TtkCalculation(appModelMock.Object);

            var expectedResultForFirstDefaulWeaponWithTruncatedValues = new KeyValuePair<string, List<Point>>(
                key: _helper.DefaultSourceData.Weapons.ElementAt(0).Name,
                value: new List<Point>()
                {
                    new P(0,0), new P(0,600), new P(10,600), new P(10,700), new P(11,700), new P(11,800),
                    new P(12,800), new P(12,900), new P(13,900), new P(13,1000), new P(14,1000), new P(14,1100),
                    new P(14,1100), new P(14,1200), new P(14,1200), new P(14,1300), new P(15,1300), new P(30,1300),
                });
            var expectedResultForSecondDefaulWeaponWithTruncatedValues = new KeyValuePair<string, List<Point>>(
                key: _helper.DefaultSourceData.Weapons.ElementAt(1).Name,
                value: new List<Point>()
                {
                    new P(0,0), new P(0,360), new P(14,360), new P(14,480), new P(24,480), new P(30,480),
                });

            //act
            var truncatedResult = testedCalculation.GetPlotData()
                .Select(kvp => new KeyValuePair<string, List<Point>>(kvp.Key, kvp.Value
                .Select(p => new Point(Math.Truncate(p.X), Math.Truncate(p.Y))).ToList()));

            //assert
            appModelMock.VerifyAll();

            Assert.IsTrue(
                truncatedResult.Contains(expectedResultForFirstDefaulWeaponWithTruncatedValues, _resultComparer));
            Assert.IsTrue(
                truncatedResult.Contains(expectedResultForSecondDefaulWeaponWithTruncatedValues, _resultComparer));
        }

        [Test]
        public void GetPlotData_NoSourceData_ShouldTrownCustomException()
        {
            //arrange
            var sourceData = new CalculationSourceData()
            {
                BodyArmor = null,
                FocusedBodyPartArmor = null,
                Weapons = null,
            };

            var appModelMock = _helper.GetModelMock(sourceData);
            var testedCalculation = new TtkCalculation(appModelMock.Object);
            Exception trownException = null;

            //act            
            try
            {
                testedCalculation.GetPlotData();
            }
            catch (Exception ex)         //catch by yourself more convenient than to use Assert.Thrown
            {
                trownException = ex;
            }

            //assert
            appModelMock.VerifyAll();
            Assert.IsTrue(trownException is NoEquipmentDataException);
        }

        [Test]
        public void GetPlotData_EmptyWeaponsList_ShouldReturnEmptyCollection()
        {
            //arrange
            var sourceData = new CalculationSourceData()
            {
                BodyArmor = new BodyArmor("SomeVest", new BitmapImage(), new Dictionary<string, double>()),
                FocusedBodyPartArmor = new BodyArmor("SomeVest", new BitmapImage(), new Dictionary<string, double>()),
                Weapons = new Weapon[0],
            };

            var appModelMock = _helper.GetModelMock(sourceData);
            var testedCalculation = new TtkCalculation(appModelMock.Object);

            //act
            var result = testedCalculation.GetPlotData();

            //assert
            appModelMock.VerifyAll();
            Assert.IsEmpty(result);
        }
    }
}
