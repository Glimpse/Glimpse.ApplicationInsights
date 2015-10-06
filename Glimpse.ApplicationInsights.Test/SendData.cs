using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace Glimpse.ApplicationInsights.Test
{
    /// <summary>
    /// Descripción resumida de SendData
    /// </summary>
    [TestClass]
    public class SendData
    {
        public SendData()
        {
            //
            // TODO: Agregar aquí la lógica del constructor
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Obtiene o establece el contexto de las pruebas que proporciona
        ///información y funcionalidad para la serie de pruebas actual.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Atributos de prueba adicionales
        //
        // Puede usar los siguientes atributos adicionales conforme escribe las pruebas:
        //
        // Use ClassInitialize para ejecutar el código antes de ejecutar la primera prueba en la clase
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup para ejecutar el código una vez ejecutadas todas las pruebas en una clase
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Usar TestInitialize para ejecutar el código antes de ejecutar cada prueba 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup para ejecutar el código una vez ejecutadas todas las pruebas
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void FilterstToGlimpseHandler()
        {
            //
            // TODO: Agregar aquí la lógica de las pruebas
            //
            GlimpseTelemetryChannel TestTelemetryChanel = new GlimpseTelemetryChannel();
            ITelemetry RTitem = new RequestTelemetry();
            ITelemetry DTitem = new DependencyTelemetry();
            TestTelemetryChanel.Send(RTitem);
            Assert.AreEqual(false, TestTelemetryChanel.IsItemSent);
            TestTelemetryChanel.Send(DTitem);
            Assert.AreEqual(false, TestTelemetryChanel.IsItemSent);
        }
    }
}
