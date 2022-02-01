using Moq;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using WarGrapher.Common;
using WarGrapher.Models;
using WarGrapher.Models.Equipment;

namespace Tests.CalculationTests.Helpers
{
    internal class CalculationTestHelper
    {
        public CalculationSourceData DefaultSourceData { get; }

        public CalculationTestHelper()
        {
            DefaultSourceData = new CalculationSourceData()
            {
                Weapons = new[]
                {
                    new Weapon("TestWeapon1", new BitmapImage(), new Dictionary<string, double>()
                        { ["damage"] = 80, ["mindamage"] = 30, ["dist"] = 9.5, ["drop"] = 5, ["rpm"] = 600,
                        ["headX"] = 4.7, ["bodyX"] = 1, ["handX"] = 0.8, ["legX"] = 0.8, }),
                    new Weapon("TestWeapon2", new BitmapImage(), new Dictionary<string, double>()
                        { ["damage"] = 90, ["mindamage"] = 40, ["dist"] = 11, ["drop"] = 2, ["rpm"] = 500,
                        ["headX"] = 5, ["bodyX"] = 1, ["handX"] = 1.2, ["legX"] = 1, }),
                },

                BodyArmor = new BodyArmor("TestVest", new BitmapImage(), new Dictionary<string, double>()
                { ["armor bonus"] = 25, ["damage absorb"] = 0, ["damage factor sub"] = 0 }),

                FocusedBodyPartArmor = new ArmsArmor("TestGloves", new BitmapImage(), new Dictionary<string, double>()
                { ["armor bonus"] = 0, ["damage absorb"] = 0, ["damage factor sub"] = 0.2 }),

                FocusedBodyPart = BodyPart.Arms,
            };
        }

        /// <summary>
        /// Initializes a new Mock object of the application model 
        /// so that it will return the passed calculation source data.
        /// </summary>
        public Mock<ISelectedDataConsumer> GetModelMock(CalculationSourceData sourceData)
        {
            var appModelMock = new Mock<ISelectedDataConsumer>();

            appModelMock
                .SetupGet(x => x.FocusedBodyPart)
                .Returns(sourceData.FocusedBodyPart)
                .Verifiable();

            appModelMock
                .Setup(x => x.GetFocusedBodyPartData())
                .Returns(new[] { sourceData.FocusedBodyPartArmor })
                .Verifiable();

            appModelMock
                .Setup(x => x.GetSelectedDataOfType(It.Is<EquipType>(y => y == EquipType.Weapon)))
                .Returns(sourceData.Weapons)
                .Verifiable();

            appModelMock
                .Setup(x => x.GetSelectedDataOfType(It.Is<EquipType>(y => y == EquipType.BodyArmor)))
                .Returns(new[] { sourceData.BodyArmor })
                .Verifiable();

            return appModelMock;
        }
    }
}
