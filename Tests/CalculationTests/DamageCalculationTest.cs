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
    class DamageCalculationTest
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
            var testedCalculation = new DamageCalculation(appModelMock.Object);

            var expectedResultForFirstDefaulWeapon = new KeyValuePair<string, List<Point>>(
                key: _helper.DefaultSourceData.Weapons.ElementAt(0).Name,
                value: new List<Point>()
                {
                    new P(0,80), new P(9.5,80), new P(19.5,30), new P(30,30)
                });
            var expectedResultForSecondDefaulWeapon = new KeyValuePair<string, List<Point>>(
                key: _helper.DefaultSourceData.Weapons.ElementAt(1).Name,
                value: new List<Point>()
                {
                    new P(0,90), new P(11,90), new P(30,52)
                });

            //act
            var result = testedCalculation.GetPlotData();

            //assert
            appModelMock.VerifyAll();

            Assert.IsTrue(
                result.Contains(expectedResultForFirstDefaulWeapon, _resultComparer));
            Assert.IsTrue(
                result.Contains(expectedResultForSecondDefaulWeapon, _resultComparer));
        }

        [Test]
        public void GetPlotData_NoSourceData_ShouldTrownCustomException()
        {
            //arrange
            var sourceData = new CalculationSourceData()
            {
                BodyArmor = new BodyArmor("SomeVest", new BitmapImage(), new Dictionary<string, double>()),
                FocusedBodyPartArmor = null,
                Weapons = null,
            };

            var appModelMock = _helper.GetModelMock(sourceData);
            var testedCalculation = new DamageCalculation(appModelMock.Object);
            Exception trownException = null;

            //act            
            try
            {
                testedCalculation.GetPlotData();
            }
            catch (Exception ex)
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
                FocusedBodyPartArmor = new HeadArmor("SomeHelmet", new BitmapImage(), new Dictionary<string, double>()),
                Weapons = new Weapon[0],
            };

            var appModelMock = _helper.GetModelMock(sourceData);
            var testedCalculation = new DamageCalculation(appModelMock.Object);

            //act
            var result = testedCalculation.GetPlotData();

            //assert
            appModelMock.VerifyAll();
            Assert.IsEmpty(result);
        }
    }
}
