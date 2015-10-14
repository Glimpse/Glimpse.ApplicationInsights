using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace Glimpse.ApplicationInsights.Test
{
    [TestClass]
    public class SendData
    {
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
            TestTelemetryChanel.Send(DTitem);
        }
    }
}
